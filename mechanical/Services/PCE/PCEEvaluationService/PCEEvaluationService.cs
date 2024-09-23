using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;

using mechanical.Data;
using mechanical.Models;
using mechanical.Models.Entities;
using mechanical.Models.PCE.Entities;
using mechanical.Models.PCE.Dto.PCEEvaluationDto;
using mechanical.Models.Dto.UploadFileDto;
using mechanical.Models.PCE.Dto.PCECaseTimeLineDto;
using mechanical.Models.PCE.Enum.PCEEvaluation;
using mechanical.Services.UploadFileService;
using mechanical.Services.PCE.PCECaseTimeLineService;
using mechanical.Models.Dto.Correction;
using mechanical.Models.PCE.Dto.ProductionCapacityDto;
using mechanical.Models.PCE.Dto.PCECaseDto;
using mechanical.Models.PCE.Dto.PCECaseAssignmentDto;

namespace mechanical.Services.PCE.PCEEvaluationService
{
    public class PCEEvaluationService : IPCEEvaluationService
    {
        private readonly CbeContext _cbeContext;
        private readonly IMapper _mapper;
        private readonly ILogger<PCEEvaluationService> _logger;
        private readonly IUploadFileService _UploadFileService;
        private readonly IPCECaseTimeLineService _pceCaseTimeLineService;
        
        public PCEEvaluationService(CbeContext context, IMapper mapper, ILogger<PCEEvaluationService> logger, IUploadFileService UploadFileService, IPCECaseTimeLineService PCECaseTimeLineService)
        {
            _cbeContext = context;
            _mapper = mapper;
            _logger = logger;
            _UploadFileService = UploadFileService;
            _pceCaseTimeLineService = PCECaseTimeLineService;
        }

        public async Task<PCEEvaluationReturnDto> CreatePCEEvaluation(Guid UserId, PCEEvaluationPostDto Dto)
        {
            using var transaction = await _cbeContext.Database.BeginTransactionAsync();
            try
            {
                var pceEvaluation = _mapper.Map<PCEEvaluation>(Dto);
                InitializePCEEvaluation(pceEvaluation, UserId);

                await _cbeContext.PCEEvaluations.AddAsync(pceEvaluation);
                var pce = await _cbeContext.ProductionCapacities.FindAsync(pceEvaluation.PCEId);

                await HandleFileUploads(UserId, Dto.SupportingEvidences, "Supporting Evidence", pce.PCECaseId, pceEvaluation.Id);
                await HandleFileUploads(UserId, Dto.ProductionProcessFlowDiagrams, "Production Process Flow Diagram", pce.PCECaseId, pceEvaluation.Id);

                UpdatePCEStatus(pce, "Pending", "Maker Officer");
                await UpdateCaseAssignmentStatus(pce.Id, pceEvaluation.EvaluatorId, "Pending");

                await LogPCECaseTimeline(pce, "Production valuation is created and pending.");
                await _cbeContext.SaveChangesAsync();
                await transaction.CommitAsync();

                return _mapper.Map<PCEEvaluationReturnDto>(pceEvaluation);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating PCEEvaluation");
                await transaction.RollbackAsync();
                throw new ApplicationException("An error occurred while creating the PCEEvaluation.");
            }            
        }

        public async Task<PCEEvaluationReturnDto> UpdatePCEEvaluation(Guid UserId, Guid Id, PCEEvaluationUpdateDto Dto)
        {
            using var transaction = await _cbeContext.Database.BeginTransactionAsync();
            try
            {
                var pceEvaluation = await FindPCEEvaluation(Id);
                _mapper.Map(Dto, pceEvaluation);
                UpdatePCEEvaluation(pceEvaluation, UserId);

                var filesToDelete = await HandleDeletedFiles(Dto.DeletedFileIds);
                await HandleFileUploads(UserId, Dto.NewSupportingEvidences, "Supporting Evidence", pceEvaluation.PCE.PCECaseId, pceEvaluation.Id);
                await HandleFileUploads(UserId, Dto.NewProductionProcessFlowDiagrams, "Production Process Flow Diagram", pceEvaluation.PCE.PCECaseId, pceEvaluation.Id);

                await LogPCECaseTimeline(pceEvaluation.PCE, "Production valuation is updated.");

                await _cbeContext.SaveChangesAsync();
                await transaction.CommitAsync();
                DeleteFiles(filesToDelete);

                return _mapper.Map<PCEEvaluationReturnDto>(pceEvaluation);
            }

            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating PCEEvaluation");
                await transaction.RollbackAsync();
                throw new ApplicationException("An error occurred while updating the PCEEvaluation.");
            }
        }

        public async Task<bool> DeletePCEEvaluation(Guid UserId, Guid Id)
        {
            using var transaction = await _cbeContext.Database.BeginTransactionAsync();
            try
            {
                var pceEvaluation = await FindPCEEvaluation(Id);

                _cbeContext.PCEEvaluations.Remove(pceEvaluation);

                // var previousValuation = await _cbeContext.PCEEvaluations.Where(res => res.PCEId == pceEvaluation.PCEId && res != pceEvaluation).ToListAsync();
                // var currentStatus = previousValuation.Any() ? "Reestimate" : "New";
                var currentStatus = "New";
                UpdatePCEStatus(pceEvaluation.PCE, currentStatus, "Maker Officer");

                await UpdateCaseAssignmentStatus(pceEvaluation.PCE.Id, pceEvaluation.EvaluatorId, currentStatus);
                await LogPCECaseTimeline(pceEvaluation.PCE, "The current production valuation is retracted.");
                await _cbeContext.SaveChangesAsync();
                await transaction.CommitAsync();
                await DeleteRelatedFiles(pceEvaluation.Id);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting PCEEvaluation");
                await transaction.RollbackAsync();
                throw new ApplicationException("An error occurred while deleting the PCEEvaluation.");
            }
        }

        public async Task<bool> EvaluatePCEEvaluation(Guid UserId, Guid Id)
        {
            using var transaction = await _cbeContext.Database.BeginTransactionAsync();
            try
            {
                var pceEvaluation = await FindPCEEvaluation(Id);
                UpdatePCEStatus(pceEvaluation.PCE, "Completed", "Relation Manager");

                await UpdateCaseAssignmentStatus(pceEvaluation.PCEId, UserId, "Completed", DateTime.Now);
                await LogPCECaseTimeline(pceEvaluation.PCE, "Production valuation is completed and sent to Relation Manager.");

                await _cbeContext.SaveChangesAsync();
                await transaction.CommitAsync();

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error evaluating PCEEvaluation");
                await transaction.RollbackAsync();
                throw new ApplicationException("An error occurred while evaluating the PCEEvaluation.");
            }
        }

        public async Task<bool> RejectPCEEvaluation(Guid UserId, PCERejectPostDto Dto)
        {
            using var transaction = await _cbeContext.Database.BeginTransactionAsync();
            try
            {
                var returnPCE = _mapper.Map<ProductionReject>(Dto);
                InitializeProductionReject(returnPCE, UserId);
                await _cbeContext.ProductionRejects.AddAsync(returnPCE);

                var pce = await _cbeContext.ProductionCapacities.FindAsync(Dto.PCEId);
                UpdatePCEStatus(pce, "Rejected", "Relation Manager");
                await UpdateCaseAssignmentStatus(Dto.PCEId, UserId, "Rejected");

                await LogPCECaseTimeline(pce, "PCE is rejected by MO as inadequate for evaluation and returned to Relation Manager for correction.");
                await _cbeContext.SaveChangesAsync();
                await transaction.CommitAsync();

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error rejecting PCEEvaluation");
                await transaction.RollbackAsync();
                throw new ApplicationException("An error occurred while rejecting the PCEEvaluation.");
            }
        } 

        public async Task<PCEEvaluationReturnDto> RemarkReleasePCEEvaluation(Guid UserId, Guid Id, String Remark, Guid EvaluatorId)
        {
            using var transaction = await _cbeContext.Database.BeginTransactionAsync();
            try
            {             
                var pceEvaluation = await FindPCEEvaluation(Id);       
                pceEvaluation.Remark = Remark;
                _cbeContext.Update(pceEvaluation);

                UpdatePCEStatus(pceEvaluation.PCE, "Completed", "Relation Manager");
                await UpdateCaseAssignmentStatus(pceEvaluation.PCEId, UserId, "Completed", DateTime.Now);
                await LogPCECaseTimeline(pceEvaluation.PCE, "Production Valuation is realesed to Relation Manager.");

                await _cbeContext.SaveChangesAsync();
                await transaction.CommitAsync();

                return _mapper.Map<PCEEvaluationReturnDto>(pceEvaluation);
                // return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error evaluating PCEEvaluation");
                await transaction.RollbackAsync();
                throw new ApplicationException("An error occurred while evaluating the PCEEvaluation.");
            }
        }

        private void InitializePCEEvaluation(PCEEvaluation pceEntity, Guid UserId)
        {
            pceEntity.Id = Guid.NewGuid();
            pceEntity.EvaluatorId = UserId;
            pceEntity.CreatedBy = UserId;
            pceEntity.CreatedAt = DateTime.Now;
        }

        private void UpdatePCEEvaluation(PCEEvaluation pceEntity, Guid UserId)
        {
            pceEntity.UpdatedBy = UserId;
            pceEntity.UpdatedAt = DateTime.Now;
        }

        private async Task<PCEEvaluation> FindPCEEvaluation(Guid Id)
        {
            var pceEntity = await _cbeContext.PCEEvaluations
                                            .Include(e => e.ShiftHours)
                                            .Include(e => e.TimeConsumedToCheck)
                                            .Include(e => e.PCE)
                                            .ThenInclude(e => e.PCECase)
                                            .FirstOrDefaultAsync(e => e.Id == Id);

            if (pceEntity == null)
            {
                _logger.LogWarning("PCEEvaluation with Id {Id} not found", Id);
                throw new KeyNotFoundException("PCEEvaluation not found");
            }

            return pceEntity;
        }

        private async Task HandleFileUploads(Guid UserId, ICollection<IFormFile> Files, string Category, Guid PCECaseId, Guid PCEEId)
        {
            if (Files != null && Files.Any())
            {
                foreach (var file in Files)
                {
                    var fileDto = new CreateFileDto
                    {
                        File = file,
                        Catagory = Category,
                        CaseId = PCECaseId,
                        CollateralId = PCEEId
                    };

                    await _UploadFileService.CreateUploadFile(UserId, fileDto);
                }
            }
        }

        private async Task<List<string>> HandleDeletedFiles(string DeletedFileIds)
        {
            var filePaths = new List<string>();
            if (!string.IsNullOrEmpty(DeletedFileIds))
            {
                var deletedFileGuids = DeletedFileIds.Split(',').Select(Guid.Parse).ToList();
                var filesToDelete = await _cbeContext.UploadFiles.Where(file => deletedFileGuids.Contains(file.Id)).ToListAsync();

                foreach (var file in filesToDelete)
                {
                    if (File.Exists(file.Path))
                    {
                        // File.Delete(file.Path);
                        filePaths.Add(file.Path);
                    }
                }

                _cbeContext.UploadFiles.RemoveRange(filesToDelete);
            }
            
            return filePaths;
        }

        private void DeleteFiles(IEnumerable<string> filePaths)
        {
            foreach (var filePath in filePaths)
            {
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
            }
        }

        private async Task DeleteRelatedFiles(Guid PCEEId)
        {
            var relatedFiles = await _cbeContext.UploadFiles.Where(file => file.CollateralId == PCEEId).ToListAsync();

            foreach (var file in relatedFiles)
            {
                if (File.Exists(file.Path))
                {
                    File.Delete(file.Path);
                }
            }

            _cbeContext.UploadFiles.RemoveRange(relatedFiles);
        }

        private void UpdatePCEStatus(ProductionCapacity PCE, string Status, string Stage)
        {
            PCE.CurrentStage = Stage;
            PCE.CurrentStatus = Status;
            _cbeContext.ProductionCapacities.Update(PCE);
        }

        private async Task UpdateCaseAssignmentStatus(Guid PCEId, Guid UserId, string Status, DateTime? CompletionDate = null)
        {
            var assignment = await _cbeContext.PCECaseAssignments.FirstOrDefaultAsync(res => res.ProductionCapacityId == PCEId && res.UserId == UserId);

            if (assignment != null)
            {
                assignment.Status = Status;
                assignment.CompletionDate = CompletionDate;
                _cbeContext.PCECaseAssignments.Update(assignment);
            }
        }

        private void InitializeProductionReject(ProductionReject Reject, Guid UserId)
        {
            Reject.CreationDate = DateTime.Now;
            Reject.RejectedBy = UserId;
        }

        private async Task LogPCECaseTimeline(ProductionCapacity PCE, string activity)
        {
            await _pceCaseTimeLineService.PCECaseTimeLine(new PCECaseTimeLinePostDto
            {
                Activity = $"<strong class=\"text-info\">{activity}</strong><br><i class='text-purple'>Property Owner:</i> {PCE.PropertyOwner}. &nbsp; <i class='text-purple'>Role:</i> {PCE.Role}. &nbsp; <i class='text-purple'>Production Type</i>.{PCE.PlantName}",
                CurrentStage = PCE.CurrentStage,
                CaseId = PCE.PCECaseId
            });
        }

        ///////// PCE Evaluation //////////////
        public async Task<PCEEvaluationReturnDto> GetPCEEvaluation(Guid UserId, Guid Id)
        {
            try
            {
                var pceEvaluation = await _cbeContext.PCEEvaluations
                                                .AsNoTracking()
                                                .Include(e => e.ShiftHours)
                                                .Include(e => e.TimeConsumedToCheck)
                                                .Include(e => e.PCE)
                                                .ThenInclude(e => e.PCECase)
                                                .FirstOrDefaultAsync(e => e.Id == Id);

                if (pceEvaluation == null)
                {
                    _logger.LogWarning("PCEEvaluation with Id {Id} not found", Id);
                    throw new KeyNotFoundException("PCEEvaluation not found");
                }

                var uploadFiles = await _cbeContext.UploadFiles.AsNoTracking().Where(uf => uf.CollateralId == pceEvaluation.Id).ToListAsync();
                var supportingEvidences = uploadFiles.Where(uf => uf.Catagory == "Supporting Evidence").ToList();
                var productionProcessFlowDiagrams = uploadFiles.Where(uf => uf.Catagory == "Production Process Flow Diagram").ToList();

                var pceEvaluationDto = _mapper.Map<PCEEvaluationReturnDto>(pceEvaluation);
                pceEvaluationDto.SupportingEvidences = _mapper.Map<ICollection<ReturnFileDto>>(supportingEvidences);
                pceEvaluationDto.ProductionProcessFlowDiagrams = _mapper.Map<ICollection<ReturnFileDto>>(productionProcessFlowDiagrams);

                return pceEvaluationDto;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching PCEEvaluation");
                throw new ApplicationException("An error occurred while fetching the PCEEvaluation.");
            }
        }

        public async Task<PCEEvaluationReturnDto> GetPCEEvaluationByPCEId(Guid UserId, Guid PCEId)
        {
            try
            {
                var pceEvaluation = await _cbeContext.PCEEvaluations
                                                    .AsNoTracking()
                                                    .Include(e => e.ShiftHours)
                                                    .Include(e => e.TimeConsumedToCheck)
                                                    .Include(e => e.PCE)
                                                    .ThenInclude(e => e.PCECase)
                                                    // .OrderByDescending(e => e.UpdatedAt.HasValue ? e.UpdatedAt.Value : e.CreatedAt)
                                                    .OrderByDescending(e => e.UpdatedAt)
                                                    .ThenByDescending(e => e.CreatedAt)
                                                    .FirstOrDefaultAsync(e => e.PCEId == PCEId);

                if (pceEvaluation == null)
                {
                    return _mapper.Map<PCEEvaluationReturnDto>(pceEvaluation);
                }
                var uploadFiles = await _cbeContext.UploadFiles.AsNoTracking().Where(uf => uf.CollateralId == pceEvaluation.Id).ToListAsync();
                var supportingEvidences = uploadFiles.Where(uf => uf.Catagory == "Supporting Evidence").ToList();
                var productionProcessFlowDiagrams = uploadFiles.Where(uf => uf.Catagory == "Production Process Flow Diagram").ToList();

                var pceEvaluationDto = _mapper.Map<PCEEvaluationReturnDto>(pceEvaluation);
                pceEvaluationDto.SupportingEvidences = _mapper.Map<ICollection<ReturnFileDto>>(supportingEvidences);
                pceEvaluationDto.ProductionProcessFlowDiagrams = _mapper.Map<ICollection<ReturnFileDto>>(productionProcessFlowDiagrams);

                return pceEvaluationDto;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching PCEEvaluation with PCEId {PCEId}");
                throw new ApplicationException("An error occurred while fetching the PCEEvaluation with PCEId {PCEId}.");
            }
        }

        public async Task<IEnumerable<PCEEvaluationReturnDto>> GetPCEEvaluationsByPCECaseId(Guid UserId, Guid PCECaseId)
        {
            try
            {
                var pceEntities = await _cbeContext.PCEEvaluations
                                                    .AsNoTracking()
                                                    .Include(e => e.ShiftHours)
                                                    .Include(e => e.TimeConsumedToCheck)
                                                    .Include(e => e.PCE)
                                                    .ThenInclude(e => e.PCECase)
                                                    .Where(e => e.PCE.PCECaseId == PCECaseId)
                                                    .OrderByDescending(e => e.UpdatedAt)
                                                    .ThenByDescending(e => e.CreatedAt)
                                                    .ToListAsync();

                if (pceEntities == null || !pceEntities.Any())
                {
                    return Enumerable.Empty<PCEEvaluationReturnDto>();
                }

                var pceEvaluationIds = pceEntities.Select(e => e.Id).ToList();
                var uploadFiles = await _cbeContext.UploadFiles.AsNoTracking().Where(uf => pceEvaluationIds.Contains(uf.CollateralId.Value)).ToListAsync();          
                var supportingEvidences = uploadFiles.Where(uf => uf.Catagory == "Supporting Evidence").ToList();
                var productionProcessFlowDiagrams = uploadFiles.Where(uf => uf.Catagory == "Production Process Flow Diagram").ToList();

                var pceEntitiesDto = _mapper.Map<IEnumerable<PCEEvaluationReturnDto>>(pceEntities).ToList();

                foreach (var dto in pceEntitiesDto)
                {
                    dto.SupportingEvidences = _mapper.Map<ICollection<ReturnFileDto>>(supportingEvidences);
                    dto.ProductionProcessFlowDiagrams = _mapper.Map<ICollection<ReturnFileDto>>(productionProcessFlowDiagrams);
                }

                return pceEntitiesDto;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching PCEEvaluation with PCEId {PCEId}");
                throw new ApplicationException("An error occurred while fetching the PCEEvaluation with PCEId {PCEId}.");
            }
        }                   
    
        public async Task<PCEValuationHistoryDto> GetValuationHistory(Guid UserId, Guid PCEId)
        {

            var pce = await _cbeContext.ProductionCapacities.AsNoTracking().FirstOrDefaultAsync(res => res.Id == PCEId);                 
            
            PCEEvaluationReturnDto latestEvaluation = null;

            if (pce.CurrentStatus != "New" && pce.CurrentStatus != "Reestimate")
            {  
                latestEvaluation = await GetPCEEvaluationByPCEId(UserId, PCEId);
            }

            var previousEvaluations = await _cbeContext.PCEEvaluations
                                                        .AsNoTracking()
                                                        .Include(p => p.PCE)
                                                        .ThenInclude(pc => pc.PCECase)
                                                        .Where(res => res.PCEId == PCEId && (latestEvaluation == null || res.Id != latestEvaluation.Id))
                                                        .ToListAsync();
            
            return new PCEValuationHistoryDto
            {
                LatestEvaluation = latestEvaluation,
                PreviousEvaluations = _mapper.Map<IEnumerable<PCEEvaluationReturnDto>>(previousEvaluations)
            };
        }           

        public async Task<CreateUser> GetUser(Guid UserId)
        {
            var user = await _cbeContext.CreateUsers.AsNoTracking().Include(res => res.Role).Include(res => res.District).FirstOrDefaultAsync(res => res.Id == UserId);             
            return _mapper.Map<CreateUser>(user);
        }
    }
}