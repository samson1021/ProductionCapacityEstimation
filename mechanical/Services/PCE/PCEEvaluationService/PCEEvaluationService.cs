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

                // Encode/Sanitize inputs in Dto to avoid unsafe data being saved
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

                // UpdateProductionLines(pceEvaluation, Dto);

                if (Dto.NewWitnessForm != null)
                {
                    await HandleFileUploads(UserId, new List<IFormFile> { Dto.NewWitnessForm }, "Witness Form",  pceEvaluation.PCE.PCECaseId, pceEvaluation.Id);
                    
                    if (Dto.WitnessForm != null)
                    {
                        Dto.DeletedFileIds ??= new List<Guid>();

                        if(!Dto.DeletedFileIds.Contains(Dto.WitnessForm.Id))
                        {
                            Dto.DeletedFileIds.Add(Dto.WitnessForm.Id);
                        }
                    }
                }
                else if (Dto.WitnessForm != null && Dto.DeletedFileIds?.Contains(Dto.WitnessForm.Id) == true)
                {
                    Dto.DeletedFileIds.Remove(Dto.WitnessForm.Id);
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

                var filesToDelete = await GetFilesToDelete(PCEEvaluationId: pceEvaluation.Id);
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
                pceEvaluation.CompletedAt = DateTime.UtcNow;

                await UpdatePCEStatus(pceEvaluation.PCE, "Completed", "Relation Manager");
                await UpdateCaseAssignmentStatus(pceEvaluation.PCEId, UserId, "Completed", DateTime.UtcNow);
                await UpdatePCECaseAssignemntStatusForAll(pceEvaluation.PCE, UserId, "Completed");
                await UpdatePCECaseStatusIfAllCompleted(pceEvaluation.PCE);
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
                                                        // .ThenInclude(p => p.ProductionCapacities)
                                                .FirstOrDefaultAsync(pce => pce.Id == Id);
                                                // ?? throw new KeyNotFoundException("PCE Evaluation not found.");

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
                    var fileDto = new CreateFileDto
                    {
                        File = file ?? throw new ArgumentNullException(nameof(file)),
                        Category = Category,
                        CaseId = PCECaseId,
                        CollateralId = PCEEvaluationId
                    };

                    await _UploadFileService.CreateUploadFile(UserId, fileDto);
                }
            }
        }

        private async Task<List<UploadFile>> GetFilesToDelete(Guid? PCEEvaluationId = null, List<Guid>? FileIds = null)
        {
            List<UploadFile> filesToDelete = null;
            
            if (PCEEvaluationId != null)
            {
                filesToDelete = await _cbeContext.UploadFiles.Where(file => file.CollateralId == PCEEvaluationId).ToListAsync();
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

        private async Task UpdatePCEStatus(ProductionCapacity Production, string Status, string Stage)
        {
            Production.CurrentStage = Stage;
            Production.CurrentStatus = Status;
            _cbeContext.ProductionCapacities.Update(Production);
        }

        private async Task UpdatePCECaseStatusIfAllCompleted(ProductionCapacity Production)
        {
            // Check if all other capacities except this one are completed
            var caseInfo = await _cbeContext.PCECases
                                            .Where(p => p.Id == Production.PCECaseId)
                                            .Select(p => new
                                            {
                                                HasCapacities = p.ProductionCapacities.Any(),
                                                AllOthersCompleted = p.ProductionCapacities
                                                .Where(pc => pc.Id != Production.Id)
                                                .All(pc => pc.CurrentStatus == "Completed")
                                            })
                                            .FirstOrDefaultAsync();

            if (caseInfo?.HasCapacities == true && caseInfo.AllOthersCompleted)
            {
            // Update status
            await _cbeContext.PCECases
                .Where(p => p.Id == Production.PCECaseId)
                .ExecuteUpdateAsync(s => s
                .SetProperty(p => p.Status, "Completed")
                .SetProperty(p => p.CompletedAt, DateTime.UtcNow));
            }
        }
        
        private async Task UpdatePCECaseAssignemntStatusForAll(ProductionCapacity Production, Guid UserId, string Status = "Completed")
        {
            if (Production.CreatedById != null)
            {
                await UpdateCaseAssignmentStatus(Production.Id, Production.CreatedById, Status);
            }
            var MOUser = _cbeContext.Users.Include(res => res.Role).FirstOrDefault(res => res.Id == UserId);
            var MOSupervisor = _cbeContext.Users.Include(res => res.Role).FirstOrDefault(res => res.Id == MOUser.SupervisorId);
            
            await UpdateCaseAssignmentStatus(Production.Id, MOSupervisor.Id, Status);
            
            if (MOSupervisor.Role.Name == "Maker TeamLeader")
            {
                var MTLSupervisor = _cbeContext.Users.Include(res => res.Role).FirstOrDefault(res => res.Id == MOSupervisor.SupervisorId);
                if (MTLSupervisor != null)
                {
                    await UpdateCaseAssignmentStatus(Production.Id, MTLSupervisor.Id, Status);
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

        private void UpdateProductionLines(PCEEvaluation evaluation, PCEEvaluationUpdateDto dto)
        {
            var newProductionLines = dto.ProductionLines ?? new List<ProductionLineUpdateDto>();
            var existingProductionLines = evaluation.ProductionLines ??= new List<ProductionLine>();

            var newProductionLinesById = newProductionLines.Where(p => p.Id != Guid.Empty).ToDictionary(p => p.Id);
            var existingProductionLinesById = existingProductionLines.ToDictionary(p => p.Id);

            foreach (var existingLine in existingProductionLines.ToList())
            {
                if (!newProductionLinesById.ContainsKey(existingLine.Id))
                    existingProductionLines.Remove(existingLine);
            }

            foreach (var newLine in newProductionLines)
            {
                if (newLine.Id != Guid.Empty && existingProductionLinesById.TryGetValue(newLine.Id, out var existingLine))
                {
                    _mapper.Map(newLine, existingLine);
                    UpdateProductionLineInputs(existingLine, newLine.ProductionLineInputs);
                }
                else
                {
                    var line = _mapper.Map<ProductionLine>(newLine);
                    line.Id = Guid.NewGuid();
                    UpdateProductionLineInputs(line, newLine.ProductionLineInputs);
                    existingProductionLines.Add(line);
                }
            }
        }
        
        private void UpdateProductionLineInputs(ProductionLine line, List<ProductionLineInputUpdateDto> newProductionLineInputs)
        {
            var newInputs = newProductionLineInputs ?? new List<ProductionLineInputUpdateDto>();
            var existingInputs = line.ProductionLineInputs ??= new List<ProductionLineInput>();

            var newInputsById = newInputs.Where(i => i.Id != Guid.Empty).ToDictionary(i => i.Id);
            var existingInputsById = existingInputs.ToDictionary(i => i.Id);

            // Remove deleted
            foreach (var existingInput in existingInputs.ToList())
            {
                if (!newInputsById.ContainsKey(existingInput.Id))
                    existingInputs.Remove(existingInput);
            }

            // Add or update
            foreach (var newInput in newInputs)
            {
                if (newInput.Id != Guid.Empty && existingInputsById.TryGetValue(newInput.Id, out var existingInput))
                {
                    _mapper.Map(newInput, existingInput);
                }
                else
                {
                    var input = _mapper.Map<ProductionLineInput>(newInput);
                    input.Id = Guid.NewGuid();
                    existingInputs.Add(input);
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
                                                        // .ThenInclude(p => p.ProductionCapacities)
                                                .FirstOrDefaultAsync(e => e.Id == Id);
                                                // ?? throw new KeyNotFoundException("PCE Evaluation not found.");

                if (pceEvaluation == null)
                {
                    _logger.LogWarning("Production capacity valuation with Id {Id} not found", Id);
                    throw new KeyNotFoundException("Production capacity valuation not found");
                }

                var uploadedFiles = await _cbeContext.UploadFiles.AsNoTracking().Where(uf => uf.CollateralId == pceEvaluation.Id).ToListAsync();
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
                                                            // .ThenInclude(p => p.ProductionCapacities)
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
                                                            // .ThenInclude(p => p.ProductionCapacities)
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
                var witnessForm = uploadedFiles.Where(uf => uf.Category == "Witness Form");
                var supportingEvidences = uploadedFiles.Where(uf => uf.Category == "Supporting Evidence").ToList();
                var productionProcessFlowDiagrams = uploadedFiles.Where(uf => uf.Category == "Production Process Flow Diagram").ToList();

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
                                    // .Where(e=>e.EvaluatorId== UserId)
                                   // .OrderByDescending(e => e.UpdatedAt)
                                    //.ThenByDescending(e => e.CreatedAt)
                                    .ToListAsync();


                if (pceEntities == null || !pceEntities.Any())
                {
                    return Enumerable.Empty<PCEEvaluationReturnDto>();
                }
                var pceEntitiesDto = _mapper.Map<IEnumerable<PCEEvaluationReturnDto>>(pceEntities).ToList();

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
                                                                // .ThenInclude(p => p.ProductionCapacities)
                                                        .Where(e => e.PCEId == PCEId && (latestEvaluation == null || e.Id != latestEvaluation.Id))
                                                        .ToListAsync();

            return new PCEValuationHistoryDto
            {
                LatestEvaluation = latestEvaluation,
                PreviousEvaluations = _mapper.Map<IEnumerable<PCEEvaluationReturnDto>>(previousEvaluations)
            };
        
        }

        //Ho
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
                                                // .ThenInclude(p => p.ProductionCapacities)
                                                .FirstOrDefaultAsync(e => e.Id == Id);
                // ?? throw new KeyNotFoundException("PCE Evaluation not found.");

                if (pceEvaluation == null)
                {
                    _logger.LogWarning("Production capacity valuation with Id {Id} not found", Id);
                    throw new KeyNotFoundException("Production capacity valuation not found");
                }

                var uploadedFiles = await _cbeContext.UploadFiles.AsNoTracking().Where(uf => uf.CollateralId == pceEvaluation.Id).ToListAsync();
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
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching production capacity valuation");
                throw new ApplicationException("An error occurred while fetching production capacity valuation.");
            }
        }

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
                                                    // .ThenInclude(p => p.ProductionCapacities)
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
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching production capacity valuation with ID: {PCEId}");
                throw new ApplicationException("An error occurred while fetching production capacity valuation with ID: {PCEId}.");
            }
        }

        public async Task<PCEValuationHistoryDto> GetHOValuationHistory(Guid PCEId)
        {

            var pce = await _cbeContext.ProductionCapacities
                                        .AsNoTracking()
                                        .FirstOrDefaultAsync(res => res.Id == PCEId)
                                        ?? throw new KeyNotFoundException("Production Capacity not found.");

            PCEEvaluationReturnDto latestEvaluation = null;

            if (pce != null && pce.CurrentStatus != "New" && pce.CurrentStatus != "Reestimate" && pce.CurrentStatus != "Returned")
            {
                latestEvaluation = await GetHOValuationByPCEId(PCEId);
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
                                                        // .ThenInclude(p => p.ProductionCapacities)
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