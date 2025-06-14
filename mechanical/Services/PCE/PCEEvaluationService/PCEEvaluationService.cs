using System;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Encodings.Web;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.IO;

using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;

using mechanical.Data;
using mechanical.Hubs;
using mechanical.Utils;
using mechanical.Models;
using mechanical.Models.Entities;
using mechanical.Models.PCE.Entities;
using mechanical.Models.PCE.Enum.PCEEvaluation;
using mechanical.Models.Dto.Correction;
using mechanical.Models.Dto.UploadFileDto;
using mechanical.Models.PCE.Dto.PCEEvaluationDto;
using mechanical.Models.PCE.Dto.PCECaseTimeLineDto;
using mechanical.Models.PCE.Dto.ProductionCapacityDto;
using mechanical.Models.PCE.Dto.PCECaseDto;
using mechanical.Models.PCE.Dto.PCECaseAssignmentDto;
using mechanical.Services.UploadFileService;
using mechanical.Services.NotificationService;
using mechanical.Services.PCE.PCECaseTimeLineService;

namespace mechanical.Services.PCE.PCEEvaluationService
{
    /// <summary>
    /// Service for handling Production Capacity Evaluation (PCE) operations.
    /// </summary>
    public class PCEEvaluationService : IPCEEvaluationService
    {
        private readonly IMapper _mapper;
        private readonly CbeContext _cbeContext;
        private readonly IHubContext<NotificationHub> _hubContext;
        private readonly ILogger<PCEEvaluationService> _logger;
        private readonly IUploadFileService _UploadFileService;
        private readonly IPCECaseTimeLineService _pceCaseTimeLineService;
        private readonly INotificationService _notificationService;

        private const string StatusNew = "New";
        private const string StatusPending = "Pending";
        private const string StatusCompleted = "Completed";
        private const string StatusReturned = "Returned";
        private const string StatusReestimate = "Reestimate";
        private const string RoleMakerOfficer = "Maker Officer";
        private const string RoleMakerTeamLeader = "Maker TeamLeader";
        private const string RoleMakerManager = "Maker Manager";
        private const string RoleRelationManager = "Relation Manager";

        public PCEEvaluationService(CbeContext context, IHubContext<NotificationHub> hubContext, IMapper mapper, ILogger<PCEEvaluationService> logger, INotificationService notificationService, IUploadFileService UploadFileService, IPCECaseTimeLineService PCECaseTimeLineService)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _cbeContext = context ?? throw new ArgumentNullException(nameof(context));
            _hubContext = hubContext ?? throw new ArgumentNullException(nameof(hubContext));
            _UploadFileService = UploadFileService ?? throw new ArgumentNullException(nameof(UploadFileService));
            _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));
            _pceCaseTimeLineService = PCECaseTimeLineService ?? throw new ArgumentNullException(nameof(PCECaseTimeLineService));
        }

        /// <summary>
        /// Creates a new production capacity valuation.
        /// </summary>
        /// <param name="UserId">ID of the user creating the valuation.</param>
        /// <param name="Dto">Valuation data transfer object.</param>
        /// <returns>The created valuation as a DTO.</returns>
        /// <exception cref="ApplicationException">Thrown when creation fails.</exception>
        public async Task<PCEEvaluationReturnDto> CreateValuation(Guid UserId, PCEEvaluationPostDto Dto)
        {
            if (Dto == null) throw new ArgumentNullException(nameof(Dto));
            using var transaction = await _cbeContext.Database.BeginTransactionAsync().ConfigureAwait(false);
            try
            {
                EncodingHelper.EncodeObject(Dto);

                var pceEvaluation = _mapper.Map<PCEEvaluation>(Dto);
                pceEvaluation.Id = Guid.NewGuid();
                pceEvaluation.EvaluatorId = UserId;
                pceEvaluation.CreatedBy = UserId;
                pceEvaluation.CreatedAt = DateTime.UtcNow;

                if (pceEvaluation.ProductionLines != null && pceEvaluation.ProductionLines.Any())
                {
                    foreach (var productionLine in pceEvaluation.ProductionLines)
                    {
                        productionLine.PCEEvaluationId = pceEvaluation.Id;

                        if (productionLine.ProductionLineInputs != null && productionLine.ProductionLineInputs.Any())
                        {
                            foreach (var input in productionLine.ProductionLineInputs)
                            {
                                input.ProductionLineId = productionLine.Id;
                            }
                        }
                    }
                }

                await _cbeContext.PCEEvaluations.AddAsync(pceEvaluation).ConfigureAwait(false);

                var pce = await _cbeContext.ProductionCapacities.FindAsync(pceEvaluation.PCEId).ConfigureAwait(false);

                if (pce == null)
                {
                    _logger.LogError("ProductionCapacity not found for PCEId: {PCEId} in CreateValuation", pceEvaluation.PCEId);
                    throw new KeyNotFoundException("ProductionCapacity not found.");
                }
                pce.MachineName = Dto.MachineName;
                pce.CountryOfOrigin = Dto.CountryOfOrigin;
                _cbeContext.ProductionCapacities.Update(pce);

                await HandleFileUploads(UserId, new List<IFormFile> { Dto.WitnessForm }, "Witness Form", pce.PCECaseId, pceEvaluation.Id).ConfigureAwait(false);
                await HandleFileUploads(UserId, Dto.SupportingEvidences, "Supporting Evidence", pce.PCECaseId, pceEvaluation.Id).ConfigureAwait(false);
                await HandleFileUploads(UserId, Dto.ProductionProcessFlowDiagrams, "Production Process Flow Diagram", pce.PCECaseId, pceEvaluation.Id).ConfigureAwait(false);

                await UpdatePCEStatus(pce, StatusPending, RoleMakerOfficer).ConfigureAwait(false);
                await UpdateCaseAssignmentStatus(pce.Id, pceEvaluation.EvaluatorId, StatusPending).ConfigureAwait(false);
                await LogPCECaseTimeline(pce, $"Production valuation for {Dto.MachineName} is created and pending.").ConfigureAwait(false);

                await _cbeContext.SaveChangesAsync().ConfigureAwait(false);
                await transaction.CommitAsync().ConfigureAwait(false);

                return _mapper.Map<PCEEvaluationReturnDto>(pceEvaluation);
            }
            catch (ApplicationException ex)
            {
                _logger.LogError(ex, "Error creating production capacity valuation. UserId: {UserId}, Dto: {@Dto}", UserId, Dto);
                await transaction.RollbackAsync().ConfigureAwait(false);
                throw new ApplicationException("An error occurred while creating production capacity valuation.", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating production capacity valuation. UserId: {UserId}, Dto: {@Dto}", UserId, Dto);
                await transaction.RollbackAsync().ConfigureAwait(false);
                throw new Exception("Error creating production capacity valuation.", ex);
            }
        }

        /// <summary>
        /// Updates an existing production capacity valuation.
        /// </summary>
        /// <param name="UserId">ID of the user updating the valuation.</param>
        /// <param name="Id">Valuation ID.</param>
        /// <param name="Dto">Update DTO.</param>
        /// <returns>The updated valuation as a DTO.</returns>
        /// <exception cref="ApplicationException">Thrown when update fails.</exception>
        public async Task<PCEEvaluationReturnDto> UpdateValuation(Guid UserId, Guid Id, PCEEvaluationUpdateDto Dto)
        {
            if (Dto == null) throw new ArgumentNullException(nameof(Dto));
            using var transaction = await _cbeContext.Database.BeginTransactionAsync().ConfigureAwait(false);
            try
            {
                EncodingHelper.EncodeObject(Dto);

                var pceEvaluation = await FindValuation(Id).ConfigureAwait(false);
                _mapper.Map(Dto, pceEvaluation);
                pceEvaluation.UpdatedBy = UserId;
                pceEvaluation.UpdatedAt = DateTime.UtcNow;

                if (Dto.NewWitnessForm != null)
                {
                    await HandleFileUploads(UserId, new List<IFormFile> { Dto.NewWitnessForm }, "Witness Form", pceEvaluation.PCE.PCECaseId, pceEvaluation.Id).ConfigureAwait(false);

                    if (Dto.WitnessForm != null)
                    {
                        Dto.DeletedFileIds ??= new List<Guid>();
                        if (!Dto.DeletedFileIds.Contains(Dto.WitnessForm.Id))
                        {
                            Dto.DeletedFileIds.Add(Dto.WitnessForm.Id);
                        }
                    }
                }
                else if (Dto.WitnessForm != null && Dto.DeletedFileIds?.Contains(Dto.WitnessForm.Id) == true)
                {
                    Dto.DeletedFileIds.Remove(Dto.WitnessForm.Id);
                }

                var filesToDelete = await GetFilesToDelete(FileIds: Dto.DeletedFileIds).ConfigureAwait(false);
                var filePathsToDelete = await GetFilePathsToDelete(filesToDelete).ConfigureAwait(false);

                await HandleFileUploads(UserId, Dto.NewSupportingEvidences, "Supporting Evidence", pceEvaluation.PCE.PCECaseId, pceEvaluation.Id).ConfigureAwait(false);
                await HandleFileUploads(UserId, Dto.NewProductionProcessFlowDiagrams, "Production Process Flow Diagram", pceEvaluation.PCE.PCECaseId, pceEvaluation.Id).ConfigureAwait(false);
                await LogPCECaseTimeline(pceEvaluation.PCE, $"Production valuation for {pceEvaluation.MachineName} is updated.").ConfigureAwait(false);

                await _cbeContext.SaveChangesAsync().ConfigureAwait(false);
                await transaction.CommitAsync().ConfigureAwait(false);

                await DeleteFiles(filePathsToDelete).ConfigureAwait(false);

                return _mapper.Map<PCEEvaluationReturnDto>(pceEvaluation);
            }
            catch (ApplicationException ex)
            {
                _logger.LogError(ex, "Error updating production capacity valuation. UserId: {UserId}, Id: {Id}, Dto: {@Dto}", UserId, Id, Dto);
                await transaction.RollbackAsync().ConfigureAwait(false);
                throw new ApplicationException("An error occurred while updating production capacity valuation.", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating production capacity valuation. UserId: {UserId}, Id: {Id}, Dto: {@Dto}", UserId, Id, Dto);
                await transaction.RollbackAsync().ConfigureAwait(false);
                throw new Exception("Error updating production capacity valuation.", ex);
            }
        }

        /// <summary>
        /// Deletes a production capacity valuation.
        /// </summary>
        /// <param name="UserId">ID of the user deleting the valuation.</param>
        /// <param name="Id">Valuation ID.</param>
        /// <returns>True if deleted successfully.</returns>
        /// <exception cref="ApplicationException">Thrown when deletion fails.</exception>
        public async Task<bool> DeleteValuation(Guid UserId, Guid Id)
        {
            using var transaction = await _cbeContext.Database.BeginTransactionAsync().ConfigureAwait(false);
            try
            {
                var pceEvaluation = await FindValuation(Id).ConfigureAwait(false);

                _cbeContext.PCEEvaluations.Remove(pceEvaluation);

                var previousValuation = await _cbeContext.PCEEvaluations
                                                    .Where(res => res.PCEId == pceEvaluation.PCEId && res != pceEvaluation)
                                                    .ToListAsync().ConfigureAwait(false);

                var currentStatus = previousValuation.Any() ? StatusReestimate : StatusNew;

                await UpdatePCEStatus(pceEvaluation.PCE, currentStatus, RoleMakerOfficer).ConfigureAwait(false);
                await UpdateCaseAssignmentStatus(pceEvaluation.PCE.Id, pceEvaluation.EvaluatorId, StatusNew).ConfigureAwait(false);
                await LogPCECaseTimeline(pceEvaluation.PCE, $"The current production valuation for {pceEvaluation.MachineName} is retracted.").ConfigureAwait(false);

                var filesToDelete = await GetFilesToDelete(PCEEvaluationId: pceEvaluation.Id).ConfigureAwait(false);
                var filePathsToDelete = await GetFilePathsToDelete(filesToDelete).ConfigureAwait(false);

                await _cbeContext.SaveChangesAsync().ConfigureAwait(false);
                await transaction.CommitAsync().ConfigureAwait(false);

                await DeleteFiles(filePathsToDelete).ConfigureAwait(false);

                return true;
            }
            catch (ApplicationException ex)
            {
                _logger.LogError(ex, "Error deleting production capacity valuation. UserId: {UserId}, Id: {Id}", UserId, Id);
                await transaction.RollbackAsync().ConfigureAwait(false);
                throw new ApplicationException("An error occurred while deleting production capacity valuation.", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting production capacity valuation. UserId: {UserId}, Id: {Id}", UserId, Id);
                await transaction.RollbackAsync().ConfigureAwait(false);
                throw new Exception("Error deleting production capacity valuation.", ex);
            }
        }

        /// <summary>
        /// Marks a valuation as completed and updates statuses accordingly.
        /// </summary>
        /// <param name="UserId">ID of the user completing the valuation.</param>
        /// <param name="Id">Valuation ID.</param>
        /// <returns>True if completed successfully.</returns>
        /// <exception cref="ApplicationException">Thrown when completion fails.</exception>
        public async Task<bool> CompleteValuation(Guid UserId, Guid Id)
        {
            using var transaction = await _cbeContext.Database.BeginTransactionAsync().ConfigureAwait(false);
            try
            {
                var pceEvaluation = await FindValuation(Id).ConfigureAwait(false);
                pceEvaluation.CompletedAt = DateTime.UtcNow;

                await UpdatePCEStatus(pceEvaluation.PCE, StatusCompleted, RoleRelationManager).ConfigureAwait(false);
                await UpdateCaseAssignmentStatus(pceEvaluation.PCEId, UserId, StatusCompleted, DateTime.UtcNow).ConfigureAwait(false);

                string logNotification = $"Valuation for Production {pceEvaluation.MachineName} is completed and sent to Relation Manager {pceEvaluation.PCE.CreatedBy.Name}.";
                await UpdatePCECaseAssignmentStatusForAll(pceEvaluation.PCE, UserId, StatusCompleted, logNotification).ConfigureAwait(false);
                await LogPCECaseTimeline(pceEvaluation.PCE, logNotification).ConfigureAwait(false);
                await UpdatePCECaseStatusIfAllCompleted(pceEvaluation.PCE).ConfigureAwait(false);

                await _cbeContext.SaveChangesAsync().ConfigureAwait(false);
                await transaction.CommitAsync().ConfigureAwait(false);

                return true;
            }
            catch (ApplicationException ex)
            {
                _logger.LogError(ex, "Error evaluating production capacity. UserId: {UserId}, Id: {Id}", UserId, Id);
                await transaction.RollbackAsync().ConfigureAwait(false);
                throw new ApplicationException("An error occurred while evaluating production capacity.", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error evaluating production capacity. UserId: {UserId}, Id: {Id}", UserId, Id);
                await transaction.RollbackAsync().ConfigureAwait(false);
                throw new Exception("Error evaluating production capacity.", ex);
            }
        }

        /// <summary>
        /// Returns a valuation as inadequate and updates statuses accordingly.
        /// </summary>
        /// <param name="UserId">ID of the user returning the valuation.</param>
        /// <param name="Dto">Return DTO.</param>
        /// <returns>True if returned successfully.</returns>
        /// <exception cref="ApplicationException">Thrown when return fails.</exception>
        public async Task<bool> ReturnValuation(Guid UserId, ReturnedProductionPostDto Dto)
        {
            if (Dto == null) throw new ArgumentNullException(nameof(Dto));
            using var transaction = await _cbeContext.Database.BeginTransactionAsync().ConfigureAwait(false);
            try
            {
                EncodingHelper.EncodeObject(Dto);

                var returnPCE = _mapper.Map<ReturnedProduction>(Dto);
                returnPCE.ReturnedById = UserId;
                returnPCE.ReturnedAt = DateTime.UtcNow;

                await _cbeContext.ReturnedProductions.AddAsync(returnPCE).ConfigureAwait(false);

                var pce = await _cbeContext.ProductionCapacities.Include(p => p.CreatedBy).FirstOrDefaultAsync(p => p.Id == Dto.PCEId).ConfigureAwait(false);

                if (pce == null)
                {
                    _logger.LogError("ProductionCapacity not found for PCEId: {PCEId} in ReturnValuation", Dto.PCEId);
                    throw new KeyNotFoundException("ProductionCapacity not found.");
                }

                await UpdatePCEStatus(pce, StatusReturned, RoleRelationManager).ConfigureAwait(false);
                await UpdateCaseAssignmentStatus(pce.Id, UserId, StatusReturned).ConfigureAwait(false);

                string logNotification = $"The Production {pce.MachineName} is returned as inadequate for valuation and returned to Relation Manager {pce.CreatedBy.Name} for correction.";
                await UpdatePCECaseAssignmentStatusForAll(pce, UserId, StatusReturned, logNotification).ConfigureAwait(false);
                await LogPCECaseTimeline(pce, logNotification).ConfigureAwait(false);

                await _cbeContext.SaveChangesAsync().ConfigureAwait(false);
                await transaction.CommitAsync().ConfigureAwait(false);

                return true;
            }
            catch (ApplicationException ex)
            {
                _logger.LogError(ex, "Error returning production capacity. UserId: {UserId}, Dto: {@Dto}", UserId, Dto);
                await transaction.RollbackAsync().ConfigureAwait(false);
                throw new ApplicationException("An error occurred while returning production capacity.", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error returning production capacity. UserId: {UserId}, Dto: {@Dto}", UserId, Dto);
                await transaction.RollbackAsync().ConfigureAwait(false);
                throw new Exception("Error returning production capacity.", ex);
            }
        }
        /// <summary>
        /// Resends a returned production capacity for valuation.
        /// </summary>
        /// <param name="UserId">ID of the user resending the valuation.</param>
        /// <param name="PCEId">Production capacity ID to resend.</param>
        /// <returns>True if resent successfully.</returns>
        /// <exception cref="ApplicationException">Thrown when resend fails.</exception>
        public async Task<bool> ResendValuation(Guid UserId, Guid PCEId)
        {
            return await ResendValuations(UserId, new List<Guid> { PCEId }).ConfigureAwait(false);
        }

        /// <summary>
        /// Resends one or more returned production capacities for valuation.
        /// </summary>
        /// <param name="UserId">ID of the user resending the valuation(s).</param>
        /// <param name="PCEIds">List of production capacity IDs to resend.</param>
        /// <returns>True if all resent successfully.</returns>
        /// <exception cref="ApplicationException">Thrown when resend fails.</exception>
        public async Task<bool> ResendValuations(Guid UserId, IEnumerable<Guid> PCEIds)
        {
            if (PCEIds == null || !PCEIds.Any())
                throw new ArgumentException("No Production Capacity IDs provided.", nameof(PCEIds));

            using var transaction = await _cbeContext.Database.BeginTransactionAsync().ConfigureAwait(false);
            try
            {
                foreach (var PCEId in PCEIds)
                {
                    var returnedPCE = await _cbeContext.ProductionCapacities.Include(p => p.AssignedEvaluator).FirstOrDefaultAsync(p => p.Id == PCEId).ConfigureAwait(false);

                    if (returnedPCE == null)
                    {
                        _logger.LogError("ProductionCapacity not found for Id: {PCEId} in ResendValuations", PCEId);
                        throw new KeyNotFoundException($"ProductionCapacity not found for Id: {PCEId}.");
                    }

                    returnedPCE.UpdatedById = UserId;
                    returnedPCE.UpdatedAt = DateTime.UtcNow;

                    await UpdatePCEStatus(returnedPCE, StatusNew, RoleMakerOfficer).ConfigureAwait(false);
                    await UpdateCaseAssignmentStatus(returnedPCE.Id, returnedPCE.AssignedEvaluatorId, StatusNew).ConfigureAwait(false);

                    string logNotification = $"The production {returnedPCE.MachineName} is resent to Maker Officer {returnedPCE.AssignedEvaluator?.Name} for valuation.";
                    if (returnedPCE.AssignedEvaluatorId.HasValue)
                    {
                        await SendNotificationAsync(returnedPCE.AssignedEvaluatorId.Value, logNotification, "Valuation", $"/ProductionCapacity/Detail/{returnedPCE.Id}").ConfigureAwait(false);
                    }
                    await UpdatePCECaseAssignmentStatusForAll(returnedPCE, returnedPCE?.AssignedEvaluatorId, StatusPending, logNotification).ConfigureAwait(false);
                    await LogPCECaseTimeline(returnedPCE, logNotification).ConfigureAwait(false);
                }

                await _cbeContext.SaveChangesAsync().ConfigureAwait(false);
                await transaction.CommitAsync().ConfigureAwait(false);

                return true;
            }
            catch (ApplicationException ex)
            {
                _logger.LogError(ex, "Error resending production capacities for valuation. UserId: {UserId}, PCEIds: {@PCEIds}", UserId, PCEIds);
                await transaction.RollbackAsync().ConfigureAwait(false);
                throw new ApplicationException("An error occurred while resending production capacities for valuation.", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error resending production capacities for valuation. UserId: {UserId}, PCEIds: {@PCEIds}", UserId, PCEIds);
                await transaction.RollbackAsync().ConfigureAwait(false);
                throw new Exception("Error resending production capacities for valuation.", ex);
            }
        }

        /// <summary>
        /// Handles a remark for a production capacity valuation.
        /// </summary>
        /// <param name="UserId">ID of the user handling the remark.</param>
        /// <param name="PCEId">Production capacity ID.</param>
        /// <param name="RemarkType">Type of remark.</param>
        /// <param name="FileDto">File DTO for the remark.</param>
        /// <returns>True if handled successfully.</returns>
        /// <exception cref="ApplicationException">Thrown when handling fails.</exception>
        public async Task<bool> HandleRemark(Guid UserId, Guid PCEId, String RemarkType, CreateFileDto FileDto)
        {
            if (FileDto == null) throw new ArgumentNullException(nameof(FileDto));
            using var transaction = await _cbeContext.Database.BeginTransactionAsync().ConfigureAwait(false);
            try
            {
                var currentStatus = RemarkType == "Verfication" ? "Remark Verfication" : "Remark Justfication";

                var pce = await _cbeContext.ProductionCapacities.Include(p => p.AssignedEvaluator).FirstOrDefaultAsync(p => p.Id == PCEId).ConfigureAwait(false);
                if (pce == null)
                {
                    _logger.LogError("ProductionCapacity not found for Id: {PCEId} in HandleRemark", PCEId);
                    throw new KeyNotFoundException("ProductionCapacity not found.");
                }

                if (FileDto.File != null)
                {
                    FileDto.CaseId = pce.PCECaseId;
                    await _UploadFileService.CreateUploadFile(UserId, FileDto).ConfigureAwait(false);
                }

                await UpdatePCEStatus(pce, currentStatus, RoleMakerOfficer).ConfigureAwait(false);
                await UpdateCaseAssignmentStatus(pce.Id, pce.AssignedEvaluatorId, "Remark Handled", DateTime.UtcNow).ConfigureAwait(false);
                await LogPCECaseTimeline(pce, $"The remark for the Production Valuation {pce.MachineName} is handled by Maker Officer {pce.AssignedEvaluator.Name}.").ConfigureAwait(false);

                await _cbeContext.SaveChangesAsync().ConfigureAwait(false);
                await transaction.CommitAsync().ConfigureAwait(false);

                return true;
            }
            catch (ApplicationException ex)
            {
                _logger.LogError(ex, "Error handling production capacity valuation. UserId: {UserId}, PCEId: {PCEId}, RemarkType: {RemarkType}", UserId, PCEId, RemarkType);
                await transaction.RollbackAsync().ConfigureAwait(false);
                throw new ApplicationException("An error occurred while handling production capacity valuation.", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error handling production capacity valuation. UserId: {UserId}, PCEId: {PCEId}, RemarkType: {RemarkType}", UserId, PCEId, RemarkType);
                await transaction.RollbackAsync().ConfigureAwait(false);
                throw new Exception("Error handling production capacity valuation.", ex);
            }
        }

        /// <summary>
        /// Releases a remark for a production capacity valuation.
        /// </summary>
        /// <param name="UserId">ID of the user releasing the remark.</param>
        /// <param name="Id">Valuation ID.</param>
        /// <param name="Remark">Remark text.</param>
        /// <returns>The updated valuation as a DTO.</returns>
        /// <exception cref="ApplicationException">Thrown when release fails.</exception>
        public async Task<PCEEvaluationReturnDto> ReleaseRemark(Guid UserId, Guid Id, String Remark)
        {
            using var transaction = await _cbeContext.Database.BeginTransactionAsync().ConfigureAwait(false);
            try
            {
                var pceEvaluation = await FindValuation(Id).ConfigureAwait(false);
                pceEvaluation.Remark = Remark;
                _cbeContext.Update(pceEvaluation);

                await UpdatePCEStatus(pceEvaluation.PCE, StatusCompleted, RoleRelationManager).ConfigureAwait(false);
                await UpdateCaseAssignmentStatus(pceEvaluation.PCEId, UserId, "Remark Released", DateTime.UtcNow).ConfigureAwait(false);

                string logNotification = $"The Production Valuation for {pceEvaluation.MachineName} is realesed to Relation Manager {pceEvaluation.PCE.CreatedBy.Name}.";
                await UpdatePCECaseAssignmentStatusForAll(pceEvaluation.PCE, UserId, StatusCompleted, logNotification).ConfigureAwait(false);
                await LogPCECaseTimeline(pceEvaluation.PCE, logNotification).ConfigureAwait(false);

                await _cbeContext.SaveChangesAsync().ConfigureAwait(false);
                await transaction.CommitAsync().ConfigureAwait(false);

                return _mapper.Map<PCEEvaluationReturnDto>(pceEvaluation);
            }
            catch (ApplicationException ex)
            {
                _logger.LogError(ex, "Error releasing production capacity valuation. UserId: {UserId}, Id: {Id}", UserId, Id);
                await transaction.RollbackAsync().ConfigureAwait(false);
                throw new ApplicationException("An error occurred while releasing production capacity valuation.", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error releasing production capacity valuation. UserId: {UserId}, Id: {Id}", UserId, Id);
                await transaction.RollbackAsync().ConfigureAwait(false);
                throw new Exception("Error releasing production capacity valuation.", ex);
            }
        }

        private async Task<PCEEvaluation> FindValuation(Guid Id)
        {
            var pceEvaluation = await _cbeContext.PCEEvaluations
                                                .Include(e => e.TimeConsumedToCheck)
                                                .Include(e => e.Justifications)
                                                .Include(e => e.ProductionLines)
                                                    .ThenInclude(pl => pl.ProductionLineInputs)
                                                .Include(e => e.Evaluator)
                                                .Include(e => e.PCE)
                                                    .ThenInclude(pc => pc.CreatedBy)
                                                .Include(e => e.PCE)
                                                    .ThenInclude(pc => pc.AssignedEvaluator)
                                                .Include(e => e.PCE)
                                                    .ThenInclude(pc => pc.PCECase)
                                                .FirstOrDefaultAsync(pce => pce.Id == Id)
                                                .ConfigureAwait(false);

            if (pceEvaluation == null)
            {
                _logger.LogWarning("Production capacity valuation with Id: {Id} not found", Id);
                throw new KeyNotFoundException("Production capacity valuation not found");
            }

            return pceEvaluation;
        }

        private async Task HandleFileUploads(Guid UserId, List<IFormFile> Files, string Category, Guid PCECaseId, Guid PCEEvaluationId)
        {
            if (Files != null && Files.Any())
            {
                foreach (var file in Files)
                {
                    if (file == null)
                    {
                        _logger.LogWarning("Null file encountered in HandleFileUploads for category {Category}, PCECaseId {PCECaseId}, PCEEvaluationId {PCEEvaluationId}", Category, PCECaseId, PCEEvaluationId);
                        throw new ArgumentNullException(nameof(file), "File cannot be null in HandleFileUploads.");
                    }

                    var fileDto = new CreateFileDto
                    {
                        File = file,
                        Category = Category,
                        CaseId = PCECaseId,
                        CollateralId = PCEEvaluationId
                    };

                    await _UploadFileService.CreateUploadFile(UserId, fileDto).ConfigureAwait(false);
                }
            }
        }

        private async Task<List<UploadFile>> GetFilesToDelete(Guid? PCEEvaluationId = null, List<Guid>? FileIds = null)
        {
            List<UploadFile> filesToDelete = null;

            if (PCEEvaluationId != null)
            {
                filesToDelete = await _cbeContext.UploadFiles.Where(file => file.CollateralId == PCEEvaluationId).ToListAsync().ConfigureAwait(false);
            }
            else if (FileIds != null)
            {
                var fileGuids = FileIds.ToList();
                filesToDelete = await _cbeContext.UploadFiles.Where(file => fileGuids.Contains(file.Id)).ToListAsync().ConfigureAwait(false);
            }

            return filesToDelete;
        }

        private async Task<List<string>> GetFilePathsToDelete(List<UploadFile> FilesToDelete)
        {
            var filePaths = new List<string>();

            if (FilesToDelete != null)
            {
                foreach (var file in FilesToDelete)
                {
                    if (File.Exists(file.Path))
                    {
                        filePaths.Add(file.Path);
                    }
                }
                _cbeContext.UploadFiles.RemoveRange(FilesToDelete);
            }

            return filePaths;
        }

        private async Task DeleteFiles(IEnumerable<string> filePaths)
        {
            foreach (var filePath in filePaths)
            {
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
            }
        }

        private async Task UpdatePCEStatus(ProductionCapacity Production, string Status, string Stage)
        {
            if (Production == null) throw new ArgumentNullException(nameof(Production));
            Production.CurrentStage = Stage;
            Production.CurrentStatus = Status;
            _cbeContext.ProductionCapacities.Update(Production);
        }

        /// <summary>
        /// Updates the PCE case status if all production capacities are completed.
        /// </summary>
        /// <param name="Production">Production capacity entity.</param>
        private async Task UpdatePCECaseStatusIfAllCompleted(ProductionCapacity Production)
        {
            if (Production == null) throw new ArgumentNullException(nameof(Production));
            var caseInfo = await _cbeContext.PCECases
                                            .Where(p => p.Id == Production.PCECaseId)
                                            .Select(p => new
                                            {
                                                HasCapacities = p.ProductionCapacities.Any(),
                                                AllOthersCompleted = p.ProductionCapacities
                                                .Where(pc => pc.Id != Production.Id)
                                                .All(pc => pc.CurrentStatus == StatusCompleted)
                                            })
                                            .FirstOrDefaultAsync().ConfigureAwait(false);

            if (caseInfo?.HasCapacities == true && caseInfo.AllOthersCompleted)
            {
                await _cbeContext.PCECases
                    .Where(p => p.Id == Production.PCECaseId)
                    .ExecuteUpdateAsync(s => s
                        .SetProperty(p => p.Status, StatusCompleted)
                        .SetProperty(p => p.CompletedAt, DateTime.UtcNow)).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Updates the assignment status for all relevant users in the PCE case.
        /// </summary>
        /// <param name="Production">Production capacity entity.</param>
        /// <param name="UserId">User ID.</param>
        /// <param name="Status">Status to set.</param>
        /// <param name="notification">Notification message.</param>
        private async Task UpdatePCECaseAssignmentStatusForAll(ProductionCapacity Production, Guid? UserId, string Status = StatusCompleted, string notification = null)
        {
            if (UserId == null || Production == null)
                return;

            // Update assignment for the creator if available
            if (Production.CreatedById != null)
            {
                await UpdateCaseAssignmentStatus(Production.Id, Production.CreatedById, Status).ConfigureAwait(false);
                if (Status != StatusPending && notification != null)
                {
                    await SendNotificationAsync(Production.CreatedById, notification, "Valuation", $"/ProductionCapacity/Detail/{Production.Id}").ConfigureAwait(false);
                }
            }

            // Load user with role and supervisor in a single query
            var user = await _cbeContext.Users.Include(u => u.Role).FirstOrDefaultAsync(u => u.Id == UserId).ConfigureAwait(false);

            if (user?.Role?.Name == RoleMakerTeamLeader || user?.Role?.Name == RoleMakerOfficer)
            {
                // Load supervisor with role
                var supervisor = await _cbeContext.Users.Include(u => u.Role).FirstOrDefaultAsync(u => u.Id == user.SupervisorId).ConfigureAwait(false);

                if (supervisor != null)
                {
                    await UpdateCaseAssignmentStatus(Production.Id, supervisor.Id, Status).ConfigureAwait(false);
                    if (notification != null)
                        await SendNotificationAsync(supervisor.Id, notification, "Valuation", $"/ProductionCapacity/Detail/{Production.Id}").ConfigureAwait(false);

                    if (supervisor.Role?.Name == RoleMakerTeamLeader)
                    {
                        // Load super supervisor with role
                        var superSupervisor = await _cbeContext.Users.Include(u => u.Role).FirstOrDefaultAsync(u => u.Id == supervisor.SupervisorId).ConfigureAwait(false);

                        if (superSupervisor != null)
                        {
                            await UpdateCaseAssignmentStatus(Production.Id, superSupervisor.Id, Status).ConfigureAwait(false);
                            if (notification != null)
                                await SendNotificationAsync(superSupervisor.Id, notification, "Valuation", $"/ProductionCapacity/Detail/{Production.Id}").ConfigureAwait(false);
                        }
                    }
                }
            }
        }

        private async Task UpdateCaseAssignmentStatus(Guid PCEId, Guid? UserId, string Status, DateTime? CompletedAt = null)
        {
            if (UserId == null || PCEId == null)
                return;
            var assignment = await _cbeContext.PCECaseAssignments
                                                .Where(pca => pca.ProductionCapacityId == PCEId && pca.UserId == UserId)
                                                .OrderByDescending(pca => pca.AssignmentDate)
                                                .FirstOrDefaultAsync().ConfigureAwait(false);

            if (assignment != null)
            {
                assignment.Status = Status;
                assignment.CompletedAt = CompletedAt;
                _cbeContext.PCECaseAssignments.Update(assignment);
            }
        }

        private async Task LogPCECaseTimeline(ProductionCapacity Production, string Activity)
        {
            if (Production != null)
            {
                await _pceCaseTimeLineService.PCECaseTimeLine(new PCECaseTimeLinePostDto
                {
                    Activity = $"<strong class=\"text-info\">{HtmlEncoder.Default.Encode(Activity)}</strong><br><i class='text-purple'>Property Owner:</i> {HtmlEncoder.Default.Encode(Production.PropertyOwner)}. &nbsp; <i class='text-purple'>Role:</i> {HtmlEncoder.Default.Encode(Production.Role)}.",
                    CurrentStage = Production.CurrentStage,
                    PCECaseId = Production.PCECaseId
                }).ConfigureAwait(false);
            }
        }

        ///////// PCE Evaluation //////////////

        /// <summary>
        /// Gets a production capacity valuation by its ID.
        /// </summary>
        /// <param name="UserId">User ID.</param>
        /// <param name="Id">Valuation ID.</param>
        /// <returns>The valuation as a DTO.</returns>
        public async Task<PCEEvaluationReturnDto> GetValuation(Guid UserId, Guid Id)
        {
            try
            {
                var pceEvaluation = await _cbeContext.PCEEvaluations
                                                .AsNoTracking()
                                                .Include(e => e.TimeConsumedToCheck)
                                                .Include(e => e.Justifications)
                                                .Include(e => e.ProductionLines)
                                                    .ThenInclude(pl => pl.ProductionLineInputs)
                                                .Include(e => e.Evaluator)
                                                .Include(e => e.PCE)
                                                    .ThenInclude(pc => pc.PCECase)
                                                .FirstOrDefaultAsync(e => e.Id == Id)
                                                .ConfigureAwait(false);

                if (pceEvaluation == null)
                {
                    _logger.LogWarning("Production capacity valuation with Id {Id} not found", Id);
                    throw new KeyNotFoundException("Production capacity valuation not found");
                }

                var uploadedFiles = await _cbeContext.UploadFiles.AsNoTracking().Where(uf => uf.CollateralId.HasValue && uf.CollateralId.Value == pceEvaluation.Id).ToListAsync().ConfigureAwait(false);
                var witnessForm = uploadedFiles.FirstOrDefault(uf => uf.Category == "Witness Form");
                var supportingEvidences = uploadedFiles.Where(uf => uf.Category == "Supporting Evidence").ToList();
                var productionProcessFlowDiagrams = uploadedFiles.Where(uf => uf.Category == "Production Process Flow Diagram").ToList();

                var pceEvaluationDto = _mapper.Map<PCEEvaluationReturnDto>(pceEvaluation);
                pceEvaluationDto.UploadedFiles = _mapper.Map<List<ReturnFileDto>>(uploadedFiles);
                pceEvaluationDto.WitnessForm = _mapper.Map<ReturnFileDto>(witnessForm);
                pceEvaluationDto.SupportingEvidences = _mapper.Map<List<ReturnFileDto>>(supportingEvidences);
                pceEvaluationDto.ProductionProcessFlowDiagrams = _mapper.Map<List<ReturnFileDto>>(productionProcessFlowDiagrams);

                var totalCapacity = pceEvaluationDto.ProductionLines?.Where(pl => pl.ActualCapacity != null && pl.ActualCapacity > 0).Sum(pl => pl.ActualCapacity) ?? 0;
                pceEvaluationDto.TotalCapacity = totalCapacity;

                var bottleneck = pceEvaluationDto.ProductionLines?.FirstOrDefault(pl => pl.IsBottleneck);
                if (bottleneck != null)
                {
                    pceEvaluationDto.BottleneckProductionLine ??= new BottleneckProductionLineDto();
                    pceEvaluationDto.BottleneckProductionLine.LineName = bottleneck.LineName;
                    pceEvaluationDto.BottleneckProductionLine.Capacity = bottleneck.ActualCapacity;
                    pceEvaluationDto.BottleneckProductionLine.Unit = bottleneck.ProductionUnit;
                }

                return pceEvaluationDto;
            }
            catch (ApplicationException ex)
            {
                _logger.LogError(ex, "Error fetching production capacity valuation. UserId: {UserId}, Id: {Id}", UserId, Id);
                throw new ApplicationException("An error occurred while fetching production capacity valuation.", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching production capacity valuation. UserId: {UserId}, Id: {Id}", UserId, Id);
                throw new Exception("Error fetching production capacity valuation.", ex);
            }
        }

        /// <summary>
        /// Gets the latest valuation for a given production capacity ID.
        /// </summary>
        /// <param name="UserId">User ID.</param>
        /// <param name="PCEId">Production capacity ID.</param>
        /// <returns>The latest valuation as a DTO.</returns>
        public async Task<PCEEvaluationReturnDto> GetValuationByPCEId(Guid UserId, Guid PCEId)
        {
            try
            {
                var pceEvaluation = await _cbeContext.PCEEvaluations
                                                    .AsNoTracking()
                                                    .Include(e => e.TimeConsumedToCheck)
                                                    .Include(e => e.Justifications)
                                                    .Include(e => e.ProductionLines)
                                                        .ThenInclude(pl => pl.ProductionLineInputs)
                                                    .Include(e => e.Evaluator)
                                                    .Include(e => e.PCE)
                                                        .ThenInclude(pc => pc.PCECase)
                                                    .OrderByDescending(e => e.UpdatedAt)
                                                    .ThenByDescending(e => e.CreatedAt)
                                                    .FirstOrDefaultAsync(e => e.PCEId == PCEId)
                                                    .ConfigureAwait(false);

                if (pceEvaluation == null)
                {
                    return _mapper.Map<PCEEvaluationReturnDto>(pceEvaluation);
                }
                var uploadedFiles = await _cbeContext.UploadFiles.AsNoTracking().Where(uf => uf.CollateralId.HasValue && uf.CollateralId.Value == pceEvaluation.Id).ToListAsync().ConfigureAwait(false);
                var witnessForm = uploadedFiles.FirstOrDefault(uf => uf.Category == "Witness Form");
                var supportingEvidences = uploadedFiles.Where(uf => uf.Category == "Supporting Evidence").ToList();
                var productionProcessFlowDiagrams = uploadedFiles.Where(uf => uf.Category == "Production Process Flow Diagram").ToList();

                var pceEvaluationDto = _mapper.Map<PCEEvaluationReturnDto>(pceEvaluation);
                pceEvaluationDto.UploadedFiles = _mapper.Map<List<ReturnFileDto>>(uploadedFiles);
                pceEvaluationDto.WitnessForm = _mapper.Map<ReturnFileDto>(witnessForm);
                pceEvaluationDto.SupportingEvidences = _mapper.Map<List<ReturnFileDto>>(supportingEvidences);
                pceEvaluationDto.ProductionProcessFlowDiagrams = _mapper.Map<List<ReturnFileDto>>(productionProcessFlowDiagrams);

                var totalCapacity = pceEvaluationDto.ProductionLines?.Where(pl => pl.ActualCapacity != null && pl.ActualCapacity > 0).Sum(pl => pl.ActualCapacity) ?? 0;
                pceEvaluationDto.TotalCapacity = totalCapacity;

                var bottleneck = pceEvaluationDto.ProductionLines?.FirstOrDefault(pl => pl.IsBottleneck);
                if (bottleneck != null)
                {
                    pceEvaluationDto.BottleneckProductionLine ??= new BottleneckProductionLineDto();
                    pceEvaluationDto.BottleneckProductionLine.LineName = bottleneck.LineName;
                    pceEvaluationDto.BottleneckProductionLine.Capacity = bottleneck.ActualCapacity;
                    pceEvaluationDto.BottleneckProductionLine.Unit = bottleneck.ProductionUnit;
                }

                return pceEvaluationDto;
            }
            catch (ApplicationException ex)
            {
                _logger.LogError(ex, "Error fetching production capacity valuation with ID: {PCEId}", PCEId);
                throw new ApplicationException("An error occurred while fetching production capacity valuation with ID: {PCEId}.", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching production capacity valuation with ID: {PCEId}", PCEId);
                throw new Exception("Error fetching production capacity valuation.", ex);
            }
        }

        /// <summary>
        /// Gets all valuations for a given PCE case ID.
        /// </summary>
        /// <param name="UserId">User ID.</param>
        /// <param name="PCECaseId">PCE case ID.</param>
        /// <returns>Enumerable of valuation DTOs.</returns>
        public async Task<IEnumerable<PCEEvaluationReturnDto>> GetValuationsByPCECaseId(Guid UserId, Guid PCECaseId)
        {
            try
            {
                var pceEntities = await _cbeContext.PCEEvaluations
                                                    .AsNoTracking()
                                                    .Include(e => e.TimeConsumedToCheck)
                                                    .Include(e => e.Justifications)
                                                    .Include(e => e.ProductionLines)
                                                        .ThenInclude(pl => pl.ProductionLineInputs)
                                                    .Include(e => e.Evaluator)
                                                    .Include(e => e.PCE)
                                                        .ThenInclude(pc => pc.PCECase)
                                                    .Where(e => e.PCE.PCECaseId == PCECaseId)
                                                    .OrderByDescending(e => e.UpdatedAt)
                                                    .ThenByDescending(e => e.CreatedAt)
                                                    .ToListAsync().ConfigureAwait(false);

                if (pceEntities == null || !pceEntities.Any())
                {
                    return Enumerable.Empty<PCEEvaluationReturnDto>();
                }

                // Efficiently fetch all files for all evaluations in a single query
                var evalIds = pceEntities.Select(e => e.Id).ToList();
                var files = await _cbeContext.UploadFiles.AsNoTracking()
                    .Where(uf => uf.CollateralId.HasValue && evalIds.Contains(uf.CollateralId.Value))
                    .ToListAsync().ConfigureAwait(false);

                var pceEntitiesDto = _mapper.Map<IEnumerable<PCEEvaluationReturnDto>>(pceEntities).ToList();

                foreach (var dto in pceEntitiesDto)
                {
                    var uploadedFiles = files.Where(uf => uf.CollateralId.HasValue && uf.CollateralId.Value == dto.Id).ToList();
                    dto.UploadedFiles = _mapper.Map<List<ReturnFileDto>>(uploadedFiles);
                    dto.WitnessForm = _mapper.Map<ReturnFileDto>(uploadedFiles.FirstOrDefault(uf => uf.Category == "Witness Form"));
                    dto.SupportingEvidences = _mapper.Map<List<ReturnFileDto>>(uploadedFiles.Where(uf => uf.Category == "Supporting Evidence"));
                    dto.ProductionProcessFlowDiagrams = _mapper.Map<List<ReturnFileDto>>(uploadedFiles.Where(uf => uf.Category == "Production Process Flow Diagram"));

                    var totalCapacity = dto.ProductionLines?.Where(pl => pl.ActualCapacity != null && pl.ActualCapacity > 0).Sum(pl => pl.ActualCapacity) ?? 0;
                    dto.TotalCapacity = totalCapacity;

                    var bottleneck = dto.ProductionLines?.FirstOrDefault(pl => pl.IsBottleneck);
                    if (bottleneck != null)
                    {
                        dto.BottleneckProductionLine ??= new BottleneckProductionLineDto();
                        dto.BottleneckProductionLine.LineName = bottleneck.LineName;
                        dto.BottleneckProductionLine.Capacity = bottleneck.ActualCapacity;
                        dto.BottleneckProductionLine.Unit = bottleneck.ProductionUnit;
                    }
                }

                return pceEntitiesDto;
            }
            catch (ApplicationException ex)
            {
                _logger.LogError(ex, "Error fetching production capacity valuation with PCECaseId: {PCECaseId}", PCECaseId);
                throw new ApplicationException("An error occurred while fetching production capacity valuation with ID: {PCECaseId}.", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching production capacity valuation with PCECaseId: {PCECaseId}", PCECaseId);
                throw new Exception("Error fetching production capacity valuation.", ex);
            }
        }

        /// <summary>
        /// Gets a summary of all valuations for a given PCE case ID.
        /// </summary>
        /// <param name="UserId">User ID.</param>
        /// <param name="PCECaseId">PCE case ID.</param>
        /// <returns>Enumerable of valuation DTOs.</returns>
        public async Task<IEnumerable<PCEEvaluationReturnDto>> GetValuationsSummaryByPCECaseId(Guid UserId, Guid PCECaseId)
        {
            try
            {
                var pceEntities = await _cbeContext.PCEEvaluations
                                    .AsNoTracking()
                                    .Include(e => e.TimeConsumedToCheck)
                                    .Include(e => e.PCE)
                                        .ThenInclude(e => e.PCECase)
                                    .Include(e => e.ProductionLines)
                                        .ThenInclude(e => e.ProductionLineInputs)
                                    .Include(e => e.Justifications)
                                    .Include(e => e.Evaluator)
                                    .Where(e => e.PCE.PCECaseId == PCECaseId)
                                    .ToListAsync().ConfigureAwait(false);

                if (pceEntities == null || !pceEntities.Any())
                {
                    return Enumerable.Empty<PCEEvaluationReturnDto>();
                }
                var pceEntitiesDto = _mapper.Map<IEnumerable<PCEEvaluationReturnDto>>(pceEntities).ToList();

                return pceEntitiesDto;
            }
            catch (ApplicationException ex)
            {
                _logger.LogError(ex, "Error fetching production capacity valuation summary with PCECaseId: {PCECaseId}", PCECaseId);
                throw new ApplicationException("An error occurred while fetching production capacity valuation with ID: {PCECaseId}.", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching production capacity valuation summary with PCECaseId: {PCECaseId}", PCECaseId);
                throw new Exception("Error fetching production capacity valuation.", ex);
            }
        }

        /// <summary>
        /// Gets the valuation history for a given production capacity.
        /// </summary>
        /// <param name="UserId">User ID.</param>
        /// <param name="PCEId">Production capacity ID.</param>
        /// <returns>Valuation history DTO.</returns>
        public async Task<PCEValuationHistoryDto> GetValuationHistory(Guid UserId, Guid PCEId)
        {
            var pce = await _cbeContext.ProductionCapacities
                                        .AsNoTracking()
                                        .FirstOrDefaultAsync(res => res.Id == PCEId)
                                        .ConfigureAwait(false)
                                        ?? throw new KeyNotFoundException("Production Capacity not found.");

            PCEEvaluationReturnDto latestEvaluation = null;

            if (pce != null && pce.CurrentStatus != StatusNew && pce.CurrentStatus != StatusReestimate && pce.CurrentStatus != StatusReturned)
            {
                latestEvaluation = await GetValuationByPCEId(UserId, PCEId).ConfigureAwait(false);
            }

            var previousEvaluations = await _cbeContext.PCEEvaluations
                                                        .AsNoTracking()
                                                        .Include(e => e.TimeConsumedToCheck)
                                                        .Include(e => e.Justifications)
                                                        .Include(e => e.ProductionLines)
                                                            .ThenInclude(pl => pl.ProductionLineInputs)
                                                        .Include(e => e.Evaluator)
                                                        .Include(e => e.PCE)
                                                            .ThenInclude(pc => pc.PCECase)
                                                        .Where(e => e.PCEId == PCEId && (latestEvaluation == null || e.Id != latestEvaluation.Id))
                                                        .ToListAsync().ConfigureAwait(false);

            return new PCEValuationHistoryDto
            {
                LatestEvaluation = latestEvaluation,
                PreviousEvaluations = _mapper.Map<IEnumerable<PCEEvaluationReturnDto>>(previousEvaluations)
            };
        }

        //Ho
        /// <summary>
        /// Gets a head office valuation by its ID.
        /// </summary>
        /// <param name="Id">Valuation ID.</param>
        /// <returns>The valuation as a DTO.</returns>
        public async Task<PCEEvaluationReturnDto> GetHOValuation(Guid Id)
        {
            try
            {
                var pceEvaluation = await _cbeContext.PCEEvaluations
                                                .AsNoTracking()
                                                .Include(e => e.TimeConsumedToCheck)
                                                .Include(e => e.Justifications)
                                                .Include(e => e.ProductionLines)
                                                    .ThenInclude(pl => pl.ProductionLineInputs)
                                                .Include(e => e.Evaluator)
                                                .Include(e => e.PCE)
                                                    .ThenInclude(pc => pc.PCECase)
                                                .FirstOrDefaultAsync(e => e.Id == Id)
                                                .ConfigureAwait(false);

                if (pceEvaluation == null)
                {
                    _logger.LogWarning("Production capacity valuation with Id {Id} not found", Id);
                    throw new KeyNotFoundException("Production capacity valuation not found");
                }

                var uploadedFiles = await _cbeContext.UploadFiles.AsNoTracking().Where(uf => uf.CollateralId.HasValue && uf.CollateralId.Value == pceEvaluation.Id).ToListAsync().ConfigureAwait(false);
                var witnessForm = uploadedFiles.FirstOrDefault(uf => uf.Category == "Witness Form");
                var supportingEvidences = uploadedFiles.Where(uf => uf.Category == "Supporting Evidence").ToList();
                var productionProcessFlowDiagrams = uploadedFiles.Where(uf => uf.Category == "Production Process Flow Diagram").ToList();

                var pceEvaluationDto = _mapper.Map<PCEEvaluationReturnDto>(pceEvaluation);
                pceEvaluationDto.UploadedFiles = _mapper.Map<List<ReturnFileDto>>(uploadedFiles);
                pceEvaluationDto.WitnessForm = _mapper.Map<ReturnFileDto>(witnessForm);
                pceEvaluationDto.SupportingEvidences = _mapper.Map<List<ReturnFileDto>>(supportingEvidences);
                pceEvaluationDto.ProductionProcessFlowDiagrams = _mapper.Map<List<ReturnFileDto>>(productionProcessFlowDiagrams);

                var totalCapacity = pceEvaluationDto.ProductionLines?.Where(pl => pl.ActualCapacity != null && pl.ActualCapacity > 0).Sum(pl => pl.ActualCapacity) ?? 0;
                pceEvaluationDto.TotalCapacity = totalCapacity;

                var bottleneck = pceEvaluationDto.ProductionLines?.FirstOrDefault(pl => pl.IsBottleneck);
                if (bottleneck != null)
                {
                    pceEvaluationDto.BottleneckProductionLine ??= new BottleneckProductionLineDto();
                    pceEvaluationDto.BottleneckProductionLine.LineName = bottleneck.LineName;
                    pceEvaluationDto.BottleneckProductionLine.Capacity = bottleneck.ActualCapacity;
                    pceEvaluationDto.BottleneckProductionLine.Unit = bottleneck.ProductionUnit;
                }

                return pceEvaluationDto;
            }
            catch (ApplicationException ex)
            {
                _logger.LogError(ex, "Error fetching production capacity valuation. Id: {Id}", Id);
                throw new ApplicationException("An error occurred while fetching production capacity valuation.", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching production capacity valuation. Id: {Id}", Id);
                throw new Exception("Error fetching production capacity valuation.", ex);
            }
        }

        /// <summary>
        /// Gets the latest head office valuation for a given production capacity ID.
        /// </summary>
        /// <param name="PCEId">Production capacity ID.</param>
        /// <returns>The latest valuation as a DTO.</returns>
        public async Task<PCEEvaluationReturnDto> GetHOValuationByPCEId(Guid PCEId)
        {
            try
            {
                var pceEvaluation = await _cbeContext.PCEEvaluations
                                                    .AsNoTracking()
                                                    .Include(e => e.TimeConsumedToCheck)
                                                    .Include(e => e.Justifications)
                                                    .Include(e => e.ProductionLines)
                                                        .ThenInclude(pl => pl.ProductionLineInputs)
                                                    .Include(e => e.Evaluator)
                                                    .Include(e => e.PCE)
                                                        .ThenInclude(pc => pc.PCECase)
                                                    .OrderByDescending(e => e.UpdatedAt)
                                                    .ThenByDescending(e => e.CreatedAt)
                                                    .FirstOrDefaultAsync(e => e.PCEId == PCEId)
                                                    .ConfigureAwait(false);

                if (pceEvaluation == null)
                {
                    return _mapper.Map<PCEEvaluationReturnDto>(pceEvaluation);
                }
                var uploadedFiles = await _cbeContext.UploadFiles.AsNoTracking().Where(uf => uf.CollateralId.HasValue && uf.CollateralId.Value == pceEvaluation.Id).ToListAsync().ConfigureAwait(false);
                var witnessForm = uploadedFiles.FirstOrDefault(uf => uf.Category == "Witness Form");
                var supportingEvidences = uploadedFiles.Where(uf => uf.Category == "Supporting Evidence").ToList();
                var productionProcessFlowDiagrams = uploadedFiles.Where(uf => uf.Category == "Production Process Flow Diagram").ToList();

                var pceEvaluationDto = _mapper.Map<PCEEvaluationReturnDto>(pceEvaluation);
                pceEvaluationDto.UploadedFiles = _mapper.Map<List<ReturnFileDto>>(uploadedFiles);
                pceEvaluationDto.WitnessForm = _mapper.Map<ReturnFileDto>(witnessForm);
                pceEvaluationDto.SupportingEvidences = _mapper.Map<List<ReturnFileDto>>(supportingEvidences);
                pceEvaluationDto.ProductionProcessFlowDiagrams = _mapper.Map<List<ReturnFileDto>>(productionProcessFlowDiagrams);

                var totalCapacity = pceEvaluationDto.ProductionLines?.Where(pl => pl.ActualCapacity != null && pl.ActualCapacity > 0).Sum(pl => pl.ActualCapacity) ?? 0;
                pceEvaluationDto.TotalCapacity = totalCapacity;

                var bottleneck = pceEvaluationDto.ProductionLines?.FirstOrDefault(pl => pl.IsBottleneck);
                if (bottleneck != null)
                {
                    pceEvaluationDto.BottleneckProductionLine ??= new BottleneckProductionLineDto();
                    pceEvaluationDto.BottleneckProductionLine.LineName = bottleneck.LineName;
                    pceEvaluationDto.BottleneckProductionLine.Capacity = bottleneck.ActualCapacity;
                    pceEvaluationDto.BottleneckProductionLine.Unit = bottleneck.ProductionUnit;
                }

                return pceEvaluationDto;
            }
            catch (ApplicationException ex)
            {
                _logger.LogError(ex, "Error fetching production capacity valuation with ID: {PCEId}", PCEId);
                throw new ApplicationException("An error occurred while fetching production capacity valuation with ID: {PCEId}.", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching production capacity valuation with ID: {PCEId}", PCEId);
                throw new Exception("Error fetching production capacity valuation.", ex);
            }
        }

        /// <summary>
        /// Gets the head office valuation history for a given production capacity.
        /// </summary>
        /// <param name="PCEId">Production capacity ID.</param>
        /// <returns>Valuation history DTO.</returns>
        public async Task<PCEValuationHistoryDto> GetHOValuationHistory(Guid PCEId)
        {
            var pce = await _cbeContext.ProductionCapacities
                                        .AsNoTracking()
                                        .FirstOrDefaultAsync(res => res.Id == PCEId)
                                        .ConfigureAwait(false)
                                        ?? throw new KeyNotFoundException("Production Capacity not found.");

            PCEEvaluationReturnDto latestEvaluation = null;

            if (pce != null && pce.CurrentStatus != StatusNew && pce.CurrentStatus != StatusReestimate && pce.CurrentStatus != StatusReturned)
            {
                latestEvaluation = await GetHOValuationByPCEId(PCEId).ConfigureAwait(false);
            }

            var previousEvaluations = await _cbeContext.PCEEvaluations
                                                        .AsNoTracking()
                                                        .Include(e => e.TimeConsumedToCheck)
                                                        .Include(e => e.Justifications)
                                                        .Include(e => e.ProductionLines)
                                                            .ThenInclude(pl => pl.ProductionLineInputs)
                                                        .Include(e => e.Evaluator)
                                                        .Include(e => e.PCE)
                                                            .ThenInclude(pc => pc.PCECase)
                                                        .Where(e => e.PCEId == PCEId && (latestEvaluation == null || e.Id != latestEvaluation.Id))
                                                        .ToListAsync().ConfigureAwait(false);

            return new PCEValuationHistoryDto
            {
                LatestEvaluation = latestEvaluation,
                PreviousEvaluations = _mapper.Map<IEnumerable<PCEEvaluationReturnDto>>(previousEvaluations)
            };
        }

        private async Task SendNotificationAsync(Guid recipientId, string message, string type, string url)
        {
            var notification = await _notificationService.AddNotification(recipientId, message, type, url).ConfigureAwait(false);
            await _notificationService.SendNotification(notification).ConfigureAwait(false);
        }
    }
}