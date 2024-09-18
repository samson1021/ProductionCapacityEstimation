using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
using mechanical.Models.PCE.Dto.ProductionCaseAssignmentDto;

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
                var pceEntity = _mapper.Map<PCEEvaluation>(Dto);
                InitializePCEEntity(pceEntity, UserId);

                await _cbeContext.PCEEvaluations.AddAsync(pceEntity);
                var pce = await _cbeContext.ProductionCapacities.FindAsync(pceEntity.PCEId);

                await HandleFileUploads(UserId, Dto.SupportingEvidences, "Supporting Evidence", pce.PCECaseId, pceEntity.Id);
                await HandleFileUploads(UserId, Dto.ProductionProcessFlowDiagrams, "Production Process Flow Diagram", pce.PCECaseId, pceEntity.Id);

                UpdatePCEStatus(pce, "Pending", "Maker Officer");
                await UpdateCaseAssignmentStatus(pce.Id, pceEntity.EvaluatorId, "Pending");

                await LogPCECaseTimeline(pce, "PCE Case Evaluation Created and Pending.");
                await _cbeContext.SaveChangesAsync();
                await transaction.CommitAsync();

                return _mapper.Map<PCEEvaluationReturnDto>(pceEntity);
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
                var pceEntity = await FindPCEEntity(Id);
                _mapper.Map(Dto, pceEntity);
                UpdatePCEEntity(pceEntity, UserId);

                var filesToDelete = await HandleDeletedFiles(Dto.DeletedFileIds);

                await HandleFileUploads(UserId, Dto.NewSupportingEvidences, "Supporting Evidence", pceEntity.PCE.PCECaseId, pceEntity.Id);
                await HandleFileUploads(UserId, Dto.NewProductionProcessFlowDiagrams, "Production Process Flow Diagram", pceEntity.PCE.PCECaseId, pceEntity.Id);

                await _cbeContext.SaveChangesAsync();
                await transaction.CommitAsync();
                DeleteFiles(filesToDelete);

                return _mapper.Map<PCEEvaluationReturnDto>(pceEntity);
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
                var pceEntity = await FindPCEEntity(Id);
                await DeleteRelatedFiles(pceEntity.Id);

                _cbeContext.PCEEvaluations.Remove(pceEntity);
                await UpdatePCEStatusAfterDeletion(pceEntity);

                await LogPCECaseTimeline(pceEntity.PCE, "PCE Case Evaluation is retracted.");
                await _cbeContext.SaveChangesAsync();
                await transaction.CommitAsync();

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
                var pceEntity = await FindPCEEntity(Id);
                UpdatePCEStatus(pceEntity.PCE, "Completed", "Maker Officer");

                await UpdateCaseAssignmentStatus(pceEntity.PCEId, UserId, "Completed", DateTime.Now);
                await LogPCECaseTimeline(pceEntity.PCE, "PCE Case Evaluation is completed to Relation Manager.");

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
                UpdatePCEStatus(pce, "Rejected", "Maker Officer");
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

        private void InitializePCEEntity(PCEEvaluation Entity, Guid UserId)
        {
            Entity.Id = Guid.NewGuid();
            Entity.EvaluatorId = UserId;
            Entity.CreatedBy = UserId;
            Entity.CreatedAt = DateTime.Now;
        }

        private void UpdatePCEEntity(PCEEvaluation Entity, Guid UserId)
        {
            Entity.UpdatedBy = UserId;
            Entity.UpdatedAt = DateTime.Now;
        }

        private async Task<PCEEvaluation> FindPCEEntity(Guid Id)
        {
            var Entity = await _cbeContext.PCEEvaluations
                .Include(e => e.ShiftHours)
                .Include(e => e.TimeConsumedToCheck)
                .Include(e => e.PCE)
                .FirstOrDefaultAsync(e => e.Id == Id);

            if (Entity == null)
            {
                _logger.LogWarning("PCEEvaluation with Id {Id} not found", Id);
                throw new KeyNotFoundException("PCEEvaluation not found");
            }

            return Entity;
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

        private async Task UpdatePCEStatusAfterDeletion(PCEEvaluation Entity)
        {
            var previousValuation = await _cbeContext.PCEEvaluations
                .Where(res => res.PCEId == Entity.PCEId && res != Entity)
                .ToListAsync();

            var currentStatus = previousValuation.Any() ? "Reestimate" : "New";
            UpdatePCEStatus(Entity.PCE, currentStatus, "Maker Officer");

            await UpdateCaseAssignmentStatus(Entity.PCE.Id, Entity.EvaluatorId, currentStatus);
        }

        private async Task UpdateCaseAssignmentStatus(Guid PCEId, Guid UserId, string Status, DateTime? CompletionDate = null)
        {
            var assignment = await _cbeContext.ProductionCaseAssignments
                .FirstOrDefaultAsync(res => res.ProductionCapacityId == PCEId && res.UserId == UserId);

            if (assignment != null)
            {
                assignment.Status = Status;
                assignment.CompletionDate = CompletionDate;
                _cbeContext.ProductionCaseAssignments.Update(assignment);
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
                var pceEntity = await _cbeContext.PCEEvaluations
                                                .AsNoTracking()
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

                var uploadFiles = await _cbeContext.UploadFiles.AsNoTracking().Where(uf => uf.CollateralId == pceEntity.Id).ToListAsync();
                var supportingEvidences = uploadFiles.Where(uf => uf.Catagory == "Supporting Evidence").ToList();
                var productionProcessFlowDiagrams = uploadFiles.Where(uf => uf.Catagory == "Production Process Flow Diagram").ToList();

                var pceEntityDto = _mapper.Map<PCEEvaluationReturnDto>(pceEntity);
                pceEntityDto.SupportingEvidences = _mapper.Map<ICollection<ReturnFileDto>>(supportingEvidences);
                pceEntityDto.ProductionProcessFlowDiagrams = _mapper.Map<ICollection<ReturnFileDto>>(productionProcessFlowDiagrams);

                return pceEntityDto;
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
                var pceEntity = await _cbeContext.PCEEvaluations
                                                .AsNoTracking()
                                                .Include(e => e.ShiftHours)
                                                .Include(e => e.TimeConsumedToCheck)
                                                .Include(e => e.PCE)
                                                .ThenInclude(e => e.PCECase)
                                                // .OrderByDescending(e => e.UpdatedAt.HasValue ? e.UpdatedAt.Value : e.CreatedAt)
                                                .OrderByDescending(e => e.UpdatedAt)
                                                .ThenByDescending(e => e.CreatedAt)
                                                .FirstOrDefaultAsync(e => e.PCEId == PCEId);

                if (pceEntity == null)
                {
                    return _mapper.Map<PCEEvaluationReturnDto>(pceEntity);
                }
                var uploadFiles = await _cbeContext.UploadFiles.AsNoTracking().Where(uf => uf.CollateralId == pceEntity.Id).ToListAsync();
                var supportingEvidences = uploadFiles.Where(uf => uf.Catagory == "Supporting Evidence").ToList();
                var productionProcessFlowDiagrams = uploadFiles.Where(uf => uf.Catagory == "Production Process Flow Diagram").ToList();

                var pceEntityDto = _mapper.Map<PCEEvaluationReturnDto>(pceEntity);
                pceEntityDto.SupportingEvidences = _mapper.Map<ICollection<ReturnFileDto>>(supportingEvidences);
                pceEntityDto.ProductionProcessFlowDiagrams = _mapper.Map<ICollection<ReturnFileDto>>(productionProcessFlowDiagrams);

                return pceEntityDto;
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

                var pceEntityIds = pceEntities.Select(e => e.Id).ToList();
                var uploadFiles = await _cbeContext.UploadFiles.AsNoTracking().Where(uf => pceEntityIds.Contains(uf.CollateralId.Value)).ToListAsync();          
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


        ///////// PCE Case //////////////
        public async Task<PCECaseReturntDto> GetPCECase(Guid UserId, Guid Id)
        {
            var pCECase = await _cbeContext.PCECases
                                            .AsNoTracking()
                                            .Include(res => res.BussinessLicence)
                                            .Include(res => res.District)
                                            .Include(res => res.ProductionCapacities)
                                            .FirstOrDefaultAsync(c => c.Id == Id);
            return _mapper.Map<PCECaseReturntDto>(pCECase);
        }

        
        public async Task<IEnumerable<PCENewCaseDto>> GetPCECases(Guid UserId, string Status, int? Limit = null)
        {
            var PCECaseAssignmentsQuery = _cbeContext.ProductionCaseAssignments
                                                    .AsNoTracking()
                                                    .Include(ca => ca.ProductionCapacity)
                                                    .ThenInclude(pc => pc.PCECase)
                                                    .Where(ca => ca.UserId == UserId);

            if (!string.IsNullOrEmpty(Status) && !Status.Equals("All", StringComparison.OrdinalIgnoreCase))
            {
                PCECaseAssignmentsQuery = PCECaseAssignmentsQuery.Where(ca => ca.Status == Status);
            }

            var PCECaseAssignments = await PCECaseAssignmentsQuery.ToListAsync();
            var UniquePCECases = PCECaseAssignments
                                .Select(ca => ca.ProductionCapacity.PCECase)
                                .DistinctBy(c => c.Id)
                                .ToList();

            var productionCapacities = await _cbeContext.ProductionCapacities
                                                        .AsNoTracking()
                                                        .Where(pc => UniquePCECases.Select(c => c.Id).Contains(pc.PCECaseId) &&
                                                                    _cbeContext.ProductionCaseAssignments
                                                                                .Any(ca => ca.ProductionCapacityId == pc.Id && ca.UserId == UserId))
                                                        .ToListAsync();

            var returnDtos = UniquePCECases.Select(pceCase =>
            {
                var Dto = _mapper.Map<PCENewCaseDto>(pceCase);
                Dto.NoOfCollateral = string.IsNullOrEmpty(Status) || Status.Equals("All", StringComparison.OrdinalIgnoreCase)
                                    ? productionCapacities.Count(pc => pc.PCECaseId == pceCase.Id)
                                    : productionCapacities.Count(pc => pc.PCECaseId == pceCase.Id && pc.CurrentStatus == Status);
                Dto.TotalNoOfCollateral = productionCapacities.Count(pc => pc.PCECaseId == pceCase.Id);
                return Dto;
            });
            if (Limit.HasValue && Limit.Value > 0)
            {
                returnDtos = returnDtos.Take(Limit.Value);
            }

            return returnDtos.ToList();
        }


        public async Task<PCECasesCountDto> GetDashboardPCECasesCount(Guid UserId)
        {
            var allPCEs = await _cbeContext.ProductionCaseAssignments
                                            .AsNoTracking()
                                            .Include(res => res.ProductionCapacity)
                                            .Where(res => res.UserId == UserId)
                                            .ToListAsync();

            var newPCEs = allPCEs.Where(res => res.Status == "New").ToList();
            var pendingPCEs = allPCEs.Where(res => res.Status == "Pending").ToList();
            var completedPCEs = allPCEs.Where(res => res.Status == "Completed").ToList();
            var reestimatedPCEs = allPCEs.Where(res => res.Status == "Reestimated").ToList();
            var resubmittedPCEs = allPCEs.Where(res => res.Status == "Reestimate").ToList();
            var rejectedPCEs = allPCEs.Where(res => res.Status == "Rejected").ToList();

            return new PCECasesCountDto()
            {
                NewPCECasesCount = newPCEs.Select(res => res.ProductionCapacity.PCECaseId).Distinct().Count(),
                NewPCEsCount = newPCEs.Count,

                PendingPCECasesCount = pendingPCEs.Select(res => res.ProductionCapacity.PCECaseId).Distinct().Count(),
                PendingPCEsCount = pendingPCEs.Count,

                CompletedPCECasesCount = completedPCEs.Select(res => res.ProductionCapacity.PCECaseId).Distinct().Count(),
                CompletedPCEsCount = completedPCEs.Count,

                ReestimatedPCECasesCount = reestimatedPCEs.Select(res => res.ProductionCapacity.PCECaseId).Distinct().Count(),
                ReestimatedPCEsCount = reestimatedPCEs.Count,

                ResubmittedPCECasesCount = resubmittedPCEs.Select(res => res.ProductionCapacity.PCECaseId).Distinct().Count(),
                ResubmittedPCEsCount = resubmittedPCEs.Count,

                RejectedPCECasesCount = rejectedPCEs.Select(res => res.ProductionCapacity.PCECaseId).Distinct().Count(),
                RejectedPCEsCount = rejectedPCEs.Count,

                TotalPCECasesCount = allPCEs.Select(res => res.ProductionCapacity.PCECaseId).Distinct().Count(),
                TotalPCEsCount = allPCEs.Count,
            };
        }

        //////////////////////////////// PCE /////////////////////////////////////////

        public async Task<IEnumerable<ReturnProductionDto>> GetPCEs(Guid UserId, Guid? PCECaseId = null, string Stage = null, string Status = null)
        {
            // var query = _cbeContext.ProductionCapacities.AsNoTracking()
            //                                             .Where(pc => pc.ProductionCaseAssignments
            //                                             .Any(pca => pca.UserId == UserId || pc.EvaluatorUserID == UserId));

            var query = _cbeContext.ProductionCapacities
                                    .AsNoTracking()
                                    .Include(pc => pc.PCECase)
                                    .Join(
                                        _cbeContext.ProductionCaseAssignments,
                                        pc => pc.Id,
                                        pca => pca.ProductionCapacityId,
                                        (pc, pca) => new { ProductionCapacity = pc, ProductionCaseAssignment = pca }
                                        )
                                        .Where(x => (x.ProductionCaseAssignment.UserId == UserId || x.ProductionCapacity.EvaluatorUserID == UserId)
                                                && (Status == null || Status == "All" || x.ProductionCaseAssignment.Status == Status))
                                    .Select(x => x.ProductionCapacity); 

            if (PCECaseId.HasValue)
            {
                query = query.Where(pc => pc.PCECaseId == PCECaseId.Value);
            }

            if (!string.IsNullOrEmpty(Stage))
            {
                query = query.Where(pc => pc.CurrentStage == Stage);
            }

            if (!string.IsNullOrEmpty(Status) && !Status.Equals("All", StringComparison.OrdinalIgnoreCase))
            {
                query = query.Where(pc => pc.CurrentStatus == Status);
            }
            else
            {
                query = query.Where(pc => pc.CurrentStatus != "Rejected");
            }

            var productions = await query.ToListAsync();
            return _mapper.Map<IEnumerable<ReturnProductionDto>>(productions);
        }

        // public async Task<int> GetPCEsCount(Guid UserId, Guid? PCECaseId, string Stage = null, string Status = null)
        // {
        //     return (await GetPCEs(UserId, PCECaseId, Stage, Status)).Count();
        // }

        public async Task<int> GetPCEsCountAsync(Guid UserId, Guid? PCECaseId = null, string Stage = null, string Status = null)
        {
            var query = _cbeContext.ProductionCapacities
                .AsNoTracking()
                .Join(
                    _cbeContext.ProductionCaseAssignments,
                    pc => pc.Id,
                    pca => pca.ProductionCapacityId,
                    (pc, pca) => new { ProductionCapacity = pc, ProductionCaseAssignment = pca }
                )
                .Where(x => (x.ProductionCaseAssignment.UserId == UserId || x.ProductionCapacity.EvaluatorUserID == UserId)
                        && (Status == null || Status == "All" || x.ProductionCaseAssignment.Status == Status))
                .Select(x => x.ProductionCapacity); 

            if (PCECaseId.HasValue)
            {
                query = query.Where(x => x.PCECaseId == PCECaseId.Value);
            }

            if (!string.IsNullOrEmpty(Stage))
            {
                query = query.Where(x => x.CurrentStage == Stage);
            }

            if (!string.IsNullOrEmpty(Status) && !Status.Equals("All", StringComparison.OrdinalIgnoreCase))
            {
                query = query.Where(x => x.CurrentStatus == Status);
            }
            else
            {
                query = query.Where(pc => pc.CurrentStatus != "Rejected");
            }

            return await query.CountAsync();
        }

        public async Task<PCEsCountDto> GetDashboardPCECount(Guid UserId, Guid? PCECaseId = null, string Stage = null)
        {
            var Statuses = new[] { "New", "Pending", "Completed", "Reestimate", "Reestimated" };
            var tasks = Statuses.Select(Status => GetPCEsCountAsync(UserId, PCECaseId, Stage, Status)).ToList();

            var counts = await Task.WhenAll(tasks);

            return new PCEsCountDto()
            {
                NewPCEsCount = counts[0],
                PendingPCEsCount = counts[1],
                CompletedPCEsCount = counts[2],
                ResubmittedPCEsCount = counts[3],
                ReestimatedPCEsCount = counts[4],
                TotalPCEsCount = await GetPCEsCountAsync(UserId, PCECaseId, Stage)
            };
        }
    
        // public async Task<IEnumerable<ReturnProductionDto>> GetReturnedPCEs(Guid UserId)
        // {
       
        //     var rejectedCapacitiesQuery = from reject in _cbeContext.ProductionRejects
        //                                   join capacity in _cbeContext.ProductionCapacities
        //                                   on reject.PCEId equals capacity.Id
        //                                   // .AsNoTracking()
        //                                   where reject.RejectedBy == UserId
        //                                   select capacity;

        //     var rejectedCapacities = await rejectedCapacitiesQuery.ToListAsync();


        //     return _mapper.Map<IEnumerable<ReturnProductionDto>>(rejectedCapacities); ;
        // }

        public async Task<PCEDetailDto> GetPCEDetails(Guid UserId, Guid PCEId)
        {

            var pce = await _cbeContext.ProductionCapacities.AsNoTracking().Include(pc => pc.PCECase).FirstOrDefaultAsync(res => res.Id == PCEId);
            var reestimation = await _cbeContext.ProductionCapacityReestimations.AsNoTracking().FirstOrDefaultAsync(res => res.ProductionCapacityId == PCEId); 
            var relatedFiles = await _UploadFileService.GetUploadFileByCollateralId(PCEId);          
            var valuationHistory = await GetValuationHistory(UserId, PCEId);
     
            return new PCEDetailDto
            {
                PCECase = pce.PCECase,
                ProductionCapacity = _mapper.Map<ReturnProductionDto>(pce),
                PCEValuationHistory = valuationHistory,
                Reestimation = reestimation,
                RelatedFiles = relatedFiles
            };
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