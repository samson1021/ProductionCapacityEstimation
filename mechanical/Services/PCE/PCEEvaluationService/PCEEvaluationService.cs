using System;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Encodings.Web;
using System.Collections.Generic;

using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;

using mechanical.Data;
using mechanical.Utils;
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

        public async Task<PCEEvaluationReturnDto> CreateValuation(Guid UserId, PCEEvaluationPostDto Dto)
        {
            using var transaction = await _cbeContext.Database.BeginTransactionAsync();
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
                        productionLine.EvaluatorId = UserId;

                        if (productionLine.ProductionLineInputs != null && productionLine.ProductionLineInputs.Any())
                        {
                            foreach (var input in productionLine.ProductionLineInputs)
                            {
                                input.ProductionLineId = productionLine.Id;
                            }
                        }
                    }
                }

                await _cbeContext.PCEEvaluations.AddAsync(pceEvaluation);
                // await _cbeContext.ProductionLines.AddRangeAsync(pceEvaluation.ProductionLines);
                // await _cbeContext.ProductionLineInputs.AddRangeAsync(allInputs);
                // await _cbeContext.SaveChangesAsync();

                var pce = await _cbeContext.ProductionCapacities.FindAsync(pceEvaluation.PCEId);
                pce.MachineName = Dto.MachineName;
                pce.CountryOfOrigin = Dto.CountryOfOrigin;
                _cbeContext.ProductionCapacities.Update(pce);

                await HandleFileUploads(UserId, new List<IFormFile> { Dto.WitnessForm }, "Witness Form", pce.PCECaseId, pceEvaluation.Id);
                await HandleFileUploads(UserId, Dto.SupportingEvidences, "Supporting Evidence", pce.PCECaseId, pceEvaluation.Id);
                await HandleFileUploads(UserId, Dto.ProductionProcessFlowDiagrams, "Production Process Flow Diagram", pce.PCECaseId, pceEvaluation.Id);
                await UpdatePCEStatus(pce, "Pending", "Maker Officer");
                await UpdateCaseAssignmentStatus(pce.Id, pceEvaluation.EvaluatorId, "Pending");
                await LogPCECaseTimeline(pce, "Production valuation is created and pending.");

                await _cbeContext.SaveChangesAsync();
                await transaction.CommitAsync();

                return _mapper.Map<PCEEvaluationReturnDto>(pceEvaluation);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating production capacity valuation");
                await transaction.RollbackAsync();
                throw new ApplicationException("An error occurred while creating production capacity valuation.");
            }
        }

        public async Task<PCEEvaluationReturnDto> UpdateValuation(Guid UserId, Guid Id, PCEEvaluationUpdateDto Dto)
        {
            using var transaction = await _cbeContext.Database.BeginTransactionAsync();
            try
            {
                var pceEvaluation = await FindValuation(Id);
                _mapper.Map(Dto, pceEvaluation);
                pceEvaluation.UpdatedBy = UserId;
                pceEvaluation.UpdatedAt = DateTime.UtcNow;

                UpdateJustifications(pceEvaluation, Dto);
                UpdateProductionLines(pceEvaluation, Dto);

                if (Dto.NewWitnessForm != null)
                {
                    await HandleFileUploads(UserId, new List<IFormFile> { Dto.NewWitnessForm }, "Witness Form",  pceEvaluation.PCE.PCECaseId, pceEvaluation.Id);
                    
                    if (Dto.WitnessForm != null)
                    {
                        Dto.DeletedFileIds ??= new List<Guid>();
                        Dto.DeletedFileIds.Add(Dto.WitnessForm.Id);
                    }
                }

                // Handle file uploads
                var filesToDelete = await GetFilesToDelete(FileIds: Dto.DeletedFileIds);
                var filePathsToDelete = await GetFilePathsToDelete(filesToDelete);

                await HandleFileUploads(UserId, Dto.NewSupportingEvidences, "Supporting Evidence", pceEvaluation.PCE.PCECaseId, pceEvaluation.Id);
                await HandleFileUploads(UserId, Dto.NewProductionProcessFlowDiagrams, "Production Process Flow Diagram", pceEvaluation.PCE.PCECaseId, pceEvaluation.Id);
                await LogPCECaseTimeline(pceEvaluation.PCE, "Production valuation is updated.");

                // Save changes
                await _cbeContext.SaveChangesAsync();
                await transaction.CommitAsync();

                // Delete files from storage
                await DeleteFiles(filePathsToDelete);

                return _mapper.Map<PCEEvaluationReturnDto>(pceEvaluation);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating production capacity valuation");
                await transaction.RollbackAsync();
                throw new ApplicationException("An error occurred while updating production capacity valuation.");
            }
        }

        public async Task<bool> DeleteValuation(Guid UserId, Guid Id)
        {
            using var transaction = await _cbeContext.Database.BeginTransactionAsync();
            try
            {
                var pceEvaluation = await FindValuation(Id);

                // Delete the PCEEvaluation
                _cbeContext.PCEEvaluations.Remove(pceEvaluation);

                var previousValuation = await _cbeContext.PCEEvaluations
                                                    .Where(res => res.PCEId == pceEvaluation.PCEId && res != pceEvaluation)
                                                    .ToListAsync();

                var currentStatus = previousValuation.Any() ? "Reestimate" : "New";

                await UpdatePCEStatus(pceEvaluation.PCE, currentStatus, "Maker Officer");
                await UpdateCaseAssignmentStatus(pceEvaluation.PCE.Id, pceEvaluation.EvaluatorId, "New");
                await LogPCECaseTimeline(pceEvaluation.PCE, "The current production valuation is retracted.");

                var filesToDelete = await GetFilesToDelete(PCEEId: pceEvaluation.Id);
                var filePathsToDelete = await GetFilePathsToDelete(filesToDelete);

                await _cbeContext.SaveChangesAsync();
                await transaction.CommitAsync();

                await DeleteFiles(filePathsToDelete);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting production capacity valuation");
                await transaction.RollbackAsync();
                throw new ApplicationException("An error occurred while deleting production capacity valuation.");
            }
        }

        public async Task<bool> CompleteValuation(Guid UserId, Guid Id)
        {
            using var transaction = await _cbeContext.Database.BeginTransactionAsync();
            try
            {
                var pceEvaluation = await FindValuation(Id);

                await UpdatePCEStatus(pceEvaluation.PCE, "Completed", "Relation Manager");
                await UpdateCaseAssignmentStatus(pceEvaluation.PCEId, UserId, "Completed", DateTime.UtcNow);
                await UpdatePCECaseAssignemntStatusForAll(pceEvaluation.PCE, UserId, "Completed");
                await UpdatePCECaseStatusIfAllCompleted(pceEvaluation.PCE.PCECase);
                await LogPCECaseTimeline(pceEvaluation.PCE, "Production valuation is completed and sent to Relation Manager.");

                await _cbeContext.SaveChangesAsync();
                await transaction.CommitAsync();

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error evaluating production capacity.");
                await transaction.RollbackAsync();
                throw new Exception(ex.Message);
                throw new ApplicationException("An error occurred while evaluating production capacity.");
            }
        }

        public async Task<bool> ReturnValuation(Guid UserId, ReturnedProductionPostDto Dto)
        {
            using var transaction = await _cbeContext.Database.BeginTransactionAsync();
            try
            {
                var returnPCE = _mapper.Map<ReturnedProduction>(Dto);
                returnPCE.ReturnedById = UserId;
                returnPCE.ReturnedAt = DateTime.UtcNow;

                await _cbeContext.ReturnedProductions.AddAsync(returnPCE);

                var pce = await _cbeContext.ProductionCapacities.FindAsync(Dto.PCEId);

                await UpdatePCEStatus(pce, "Returned", "Relation Manager");
                await UpdateCaseAssignmentStatus(Dto.PCEId, UserId, "Returned");
                await UpdatePCECaseAssignemntStatusForAll(pce, UserId, "Returned");
                await LogPCECaseTimeline(pce, "PCE is returned as inadequate for evaluation and returned to Relation Manager for correction.");

                await _cbeContext.SaveChangesAsync();
                await transaction.CommitAsync();

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error returning production capacity valuation");
                await transaction.RollbackAsync();
                throw new ApplicationException("An error occurred while returning production capacity valuation.");
            }
        }

        public async Task<bool> HandleRemark(Guid UserId, Guid PCEId, String RemarkType, CreateFileDto FileDto, Guid EvaluatorId)
        {
            using var transaction = await _cbeContext.Database.BeginTransactionAsync();
            try
            {
                var currentStatus = RemarkType == "Verfication" ? "Remark Verfication" : "Remark Justfication";
            
                var pce = await _cbeContext.ProductionCapacities.FindAsync(PCEId);

                if (FileDto.File != null)
                {
                    FileDto.CaseId = pce.PCECaseId;
                    await _UploadFileService.CreateUploadFile(UserId, FileDto);
                }
                await UpdatePCEStatus(pce, currentStatus, "Maker Officer");
                await UpdateCaseAssignmentStatus(pce.Id, EvaluatorId, "Remark Handled", DateTime.UtcNow);
                await LogPCECaseTimeline(pce, "Production Valuation is handled by Maker Officer.");

                await _cbeContext.SaveChangesAsync();
                await transaction.CommitAsync();

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error handling production capacity valuation");
                await transaction.RollbackAsync();
                throw new ApplicationException("An error occurred while handling production capacity valuation.");
            }
        }

        public async Task<PCEEvaluationReturnDto> ReleaseRemark(Guid UserId, Guid Id, String Remark, Guid EvaluatorId)
        {
            using var transaction = await _cbeContext.Database.BeginTransactionAsync();
            try
            {
                var pceEvaluation = await FindValuation(Id);
                pceEvaluation.Remark = Remark;
                _cbeContext.Update(pceEvaluation);

                await UpdatePCEStatus(pceEvaluation.PCE, "Completed", "Relation Manager");
                await UpdateCaseAssignmentStatus(pceEvaluation.PCEId, UserId, "Remark Released", DateTime.UtcNow);
                await UpdatePCECaseAssignemntStatusForAll(pceEvaluation.PCE, UserId, "Completed");
                await LogPCECaseTimeline(pceEvaluation.PCE, "Production Valuation is realesed to Relation Manager.");

                await _cbeContext.SaveChangesAsync();
                await transaction.CommitAsync();

                return _mapper.Map<PCEEvaluationReturnDto>(pceEvaluation);
                // return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error releasing production capacity valuation");
                await transaction.RollbackAsync();
                throw new ApplicationException("An error occurred while releasing production capacity valuation.");
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
                                                    .ThenInclude(pc => pc.PCECase)
                                                        .ThenInclude(p => p.ProductionCapacities)
                                                .FirstOrDefaultAsync(pce => pce.Id == Id);
                                                // ?? throw new KeyNotFoundException("PCE Evaluation not found.");

            if (pceEvaluation == null)
            {
                _logger.LogWarning("Production capacity valuation with Id: {Id} not found", Id);
                throw new KeyNotFoundException("Production capacity valuation not found");
            }

            return pceEvaluation;
        }

        private async Task HandleFileUploads(Guid UserId, List<IFormFile> Files, string Category, Guid PCECaseId, Guid PCEEId)
        {
            if (Files != null && Files.Any())
            {
                foreach (var file in Files)
                {
                    var fileDto = new CreateFileDto
                    {
                        File = file ?? throw new ArgumentNullException(nameof(file)),
                        Catagory = Category,
                        CaseId = PCECaseId,
                        CollateralId = PCEEId
                    };

                    await _UploadFileService.CreateUploadFile(UserId, fileDto);
                }
            }
        }

        private async Task<List<UploadFile>> GetFilesToDelete(Guid? PCEEId = null, List<Guid>? FileIds = null)
        {
            List<UploadFile> filesToDelete = null;
            
            if (PCEEId != null)
            {
                filesToDelete = await _cbeContext.UploadFiles.Where(file => file.CollateralId == PCEEId).ToListAsync();
            }
            else if (FileIds != null)
            {
                var fileGuids = FileIds.ToList();
                filesToDelete = await _cbeContext.UploadFiles.Where(file => fileGuids.Contains(file.Id)).ToListAsync();
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

        private async Task UpdatePCEStatus(ProductionCapacity PCE, string Status, string Stage)
        {
            PCE.CurrentStage = Stage;
            PCE.CurrentStatus = Status;
            _cbeContext.ProductionCapacities.Update(PCE);
        }

        private async Task UpdatePCECaseStatusIfAllCompleted(PCECase PCECase)
        {
            // await _cbeContext.Entry(PCECase).Collection(p => p.ProductionCapacities).LoadAsync();
            var allCompleted = PCECase?.ProductionCapacities?.All(pc => pc.CurrentStatus == "Completed") ?? false;

            if (allCompleted)
            {
                PCECase.Status = "Completed";
                PCECase.CompletedAt = DateTime.UtcNow;
                _cbeContext.PCECases.Update(PCECase);
            }
        }
        
        private async Task UpdatePCECaseAssignemntStatusForAll(ProductionCapacity PCE, Guid UserId, string Status = "Completed")
        {
            if (PCE.CreatedById != null)
            {
                await UpdateCaseAssignmentStatus(PCE.Id, PCE.CreatedById, Status);
            }
            var MOUser = _cbeContext.CreateUsers.Include(res => res.Role).FirstOrDefault(res => res.Id == UserId);
            var MOSupervisor = _cbeContext.CreateUsers.Include(res => res.Role).FirstOrDefault(res => res.Id == MOUser.SupervisorId);
            
            await UpdateCaseAssignmentStatus(PCE.Id, MOSupervisor.Id, Status);
            
            if (MOSupervisor.Role.Name == "Maker TeamLeader")
            {
                var MTLSupervisor = _cbeContext.CreateUsers.Include(res => res.Role).FirstOrDefault(res => res.Id == MOSupervisor.SupervisorId);
                if (MTLSupervisor != null)
                {
                    await UpdateCaseAssignmentStatus(PCE.Id, MTLSupervisor.Id, Status);
                }
            }
        }

        private async Task UpdateCaseAssignmentStatus(Guid PCEId, Guid? UserId, string Status, DateTime? CompletedAt = null)
        {
            var assignment = await _cbeContext.PCECaseAssignments
                                                .Where(pca => pca.ProductionCapacityId == PCEId && pca.UserId == UserId)
                                                .OrderByDescending(pca => pca.AssignmentDate)
                                                .FirstOrDefaultAsync()
                                                ?? throw new KeyNotFoundException("PCECase Assignment not found.");
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
                });
            }
        }

        private void UpdateJustifications(PCEEvaluation evaluation, PCEEvaluationUpdateDto dto)
        {
            var newJustifications = dto.Justifications ?? new List<JustificationUpdateDto>();
            var existingJustifications = evaluation.Justifications ??= new List<Justification>();

            // Map by ID for efficient lookup
            var newJustificationsById = newJustifications.Where(j => j.Id != Guid.Empty).ToDictionary(j => j.Id);
            var existingJustificationsById = existingJustifications.ToDictionary(j => j.Id);

            // 1. Remove deleted justifications
            foreach (var existingJustification in existingJustifications.ToList())
            {
                if (!newJustificationsById.ContainsKey(existingJustification.Id))
                    existingJustifications.Remove(existingJustification);
            }

            // 2. Update or add justifications
            foreach (var newJustification in newJustifications)
            {
                if (newJustification.Id != Guid.Empty && existingJustificationsById.TryGetValue(newJustification.Id, out var existingJustification))
                {
                    _mapper.Map(newJustification, existingJustification);
                }
                else
                {
                    existingJustifications.Add(_mapper.Map<Justification>(newJustification));
                }
            }
        }


        private void UpdateProductionLines(PCEEvaluation evaluation, PCEEvaluationUpdateDto dto)
        {
            var newProductionLines = dto.ProductionLines ?? new List<ProductionLineUpdateDto>();
            var existingProductionLines = evaluation.ProductionLines ??= new List<ProductionLine>();

            var newProductionLinesById = newProductionLines.Where(p => p.Id != Guid.Empty).ToDictionary(p => p.Id);
            var existingProductionLinesById = existingProductionLines.ToDictionary(p => p.Id);

            // 1. Remove deleted lines
            foreach (var existingLine in existingProductionLines.ToList())
            {
                if (!newProductionLinesById.ContainsKey(existingLine.Id))
                    existingProductionLines.Remove(existingLine);
            }

            // 2. Add or update lines
            foreach (var newLine in newProductionLines)
            {
                if (newLine.Id != Guid.Empty && existingProductionLinesById.TryGetValue(newLine.Id, out var existingLine))
                {
                    _mapper.Map(newLine, existingLine);
                    UpdateProductionLineInputs(existingLine, newLine.ProductionLineInputs);
                }
                else
                {
                    existingProductionLines.Add(_mapper.Map<ProductionLine>(newLine));
                }
            }
        }
        private void UpdateProductionLineInputs(ProductionLine line, List<ProductionLineInputUpdateDto> newInputs)
        {
            var newProductionLineInputs = newInputs ?? new List<ProductionLineInputUpdateDto>();
            var existingProductionLineInputs = line.ProductionLineInputs ??= new List<ProductionLineInput>();

            var newProductionLineInputsById = newProductionLineInputs.Where(i => i.Id != Guid.Empty).ToDictionary(i => i.Id);
            var existingProductionLineInputsById = existingProductionLineInputs.ToDictionary(i => i.Id);

            // Remove missing
            foreach (var existingInput in existingProductionLineInputs.ToList())
            {
                if (!newProductionLineInputsById.ContainsKey(existingInput.Id))
                    existingProductionLineInputs.Remove(existingInput);
            }

            // Add or update
            foreach (var newInput in newProductionLineInputs)
            {
                if (newInput.Id != Guid.Empty && existingProductionLineInputsById.TryGetValue(newInput.Id, out var existingInput))
                {
                    _mapper.Map(newInput, existingInput);
                }
                else
                {
                    existingProductionLineInputs.Add(_mapper.Map<ProductionLineInput>(newInput));
                }
            }
        }


        ///////// PCE Evaluation //////////////
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
                                                        .ThenInclude(p => p.ProductionCapacities)
                                                .FirstOrDefaultAsync(e => e.Id == Id);
                                                // ?? throw new KeyNotFoundException("PCE Evaluation not found.");

                if (pceEvaluation == null)
                {
                    _logger.LogWarning("Production capacity valuation with Id {Id} not found", Id);
                    throw new KeyNotFoundException("Production capacity valuation not found");
                }

                var uploadedFiles = await _cbeContext.UploadFiles.AsNoTracking().Where(uf => uf.CollateralId == pceEvaluation.Id).ToListAsync();
                var witnessForm = uploadedFiles.FirstOrDefault(uf => uf.Catagory == "Witness Form");
                var supportingEvidences = uploadedFiles.Where(uf => uf.Catagory == "Supporting Evidence").ToList();
                var productionProcessFlowDiagrams = uploadedFiles.Where(uf => uf.Catagory == "Production Process Flow Diagram").ToList();

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
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching production capacity valuation");
                throw new ApplicationException("An error occurred while fetching production capacity valuation.");
            }
        }

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
                                                            .ThenInclude(p => p.ProductionCapacities)
                                                    // .OrderByDescending(e => e.UpdatedAt.HasValue ? e.UpdatedAt.Value : e.CreatedAt)
                                                    .OrderByDescending(e => e.UpdatedAt)
                                                    .ThenByDescending(e => e.CreatedAt)
                                                    .FirstOrDefaultAsync(e => e.PCEId == PCEId);
                                                    // ?? throw new KeyNotFoundException("PCE Evaluation not found.");

                if (pceEvaluation == null)
                {
                    return _mapper.Map<PCEEvaluationReturnDto>(pceEvaluation);
                }
                var uploadedFiles = await _cbeContext.UploadFiles.AsNoTracking().Where(uf => uf.CollateralId == pceEvaluation.Id).ToListAsync();
                var witnessForm = uploadedFiles.FirstOrDefault(uf => uf.Catagory == "Witness Form");
                var supportingEvidences = uploadedFiles.Where(uf => uf.Catagory == "Supporting Evidence").ToList();
                var productionProcessFlowDiagrams = uploadedFiles.Where(uf => uf.Catagory == "Production Process Flow Diagram").ToList();

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
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching production capacity valuation with ID: {PCEId}");
                throw new ApplicationException("An error occurred while fetching production capacity valuation with ID: {PCEId}.");
            }
        }

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
                                                            .ThenInclude(p => p.ProductionCapacities)
                                                    .Where(e => e.PCE.PCECaseId == PCECaseId) // && e.EvaluatorId == UserId)
                                                    .OrderByDescending(e => e.UpdatedAt)
                                                    .ThenByDescending(e => e.CreatedAt)
                                                    .ToListAsync();

                if (pceEntities == null || !pceEntities.Any())
                {
                    return Enumerable.Empty<PCEEvaluationReturnDto>();
                }

                var pceEvaluationIds = pceEntities.Select(e => e.Id).ToList();
                var uploadedFiles = await _cbeContext.UploadFiles.AsNoTracking().Where(uf => pceEvaluationIds.Contains(uf.CollateralId.Value)).ToListAsync();
                var witnessForm = uploadedFiles.Where(uf => uf.Catagory == "Witness Form");
                var supportingEvidences = uploadedFiles.Where(uf => uf.Catagory == "Supporting Evidence").ToList();
                var productionProcessFlowDiagrams = uploadedFiles.Where(uf => uf.Catagory == "Production Process Flow Diagram").ToList();

                var pceEntitiesDto = _mapper.Map<IEnumerable<PCEEvaluationReturnDto>>(pceEntities).ToList();

                foreach (var dto in pceEntitiesDto)
                {
                    dto.UploadedFiles = _mapper.Map<List<ReturnFileDto>>(uploadedFiles);
                    dto.WitnessForm = _mapper.Map<ReturnFileDto>(witnessForm);
                    dto.SupportingEvidences = _mapper.Map<List<ReturnFileDto>>(supportingEvidences);
                    dto.ProductionProcessFlowDiagrams = _mapper.Map<List<ReturnFileDto>>(productionProcessFlowDiagrams);

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
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching production capacity valuation with ID: {PCEId}");
                throw new ApplicationException("An error occurred while fetching production capacity valuation with ID: {PCEId}.");
            }
        }

        public async Task<PCEValuationHistoryDto> GetValuationHistory(Guid UserId, Guid PCEId)
        {

            var pce = await _cbeContext.ProductionCapacities
                                        .AsNoTracking()
                                        .FirstOrDefaultAsync(res => res.Id == PCEId)
                                        ?? throw new KeyNotFoundException("Production Capacity not found.");

            PCEEvaluationReturnDto latestEvaluation = null;

            if (pce != null && pce.CurrentStatus != "New" && pce.CurrentStatus != "Reestimate" && pce.CurrentStatus != "Returned")
            {
                latestEvaluation = await GetValuationByPCEId(UserId, PCEId);
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
                                                                .ThenInclude(p => p.ProductionCapacities)
                                                        .Where(e => e.PCEId == PCEId && (latestEvaluation == null || e.Id != latestEvaluation.Id))
                                                        .ToListAsync();

            return new PCEValuationHistoryDto
            {
                LatestEvaluation = latestEvaluation,
                PreviousEvaluations = _mapper.Map<IEnumerable<PCEEvaluationReturnDto>>(previousEvaluations)
            };
        }
    }
}