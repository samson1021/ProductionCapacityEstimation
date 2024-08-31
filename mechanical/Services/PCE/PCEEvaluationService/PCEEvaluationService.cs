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
using mechanical.Models.PCE.Entities;
using mechanical.Models.PCE.Dto.PCEEvaluationDto;
using mechanical.Models.Dto.UploadFileDto;
using mechanical.Models.PCE.Dto.PCECaseTimeLineDto;
using mechanical.Models.PCE.Enum.PCEEvaluation;
using mechanical.Services.MailService;
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
        private readonly IUploadFileService _uploadFileService;
        private readonly IPCECaseTimeLineService _pceCaseTimeLineService;
        private readonly IMailService _mailService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public PCEEvaluationService(CbeContext context, IMapper mapper, ILogger<PCEEvaluationService> logger, IUploadFileService uploadFileService, IPCECaseTimeLineService PCECaseTimeLineService, IMailService mailService, IHttpContextAccessor httpContextAccessor)
        {
            _cbeContext = context;
            _mapper = mapper;
            _logger = logger;
            _uploadFileService = uploadFileService;
            _pceCaseTimeLineService = PCECaseTimeLineService;
            _mailService = mailService;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<PCEEvaluationReturnDto> CreatePCEEvaluation(Guid UserId, PCEEvaluationPostDto Dto)
        {
            using var transaction = await _cbeContext.Database.BeginTransactionAsync();
            try
            {
                var pceEntity = _mapper.Map<PCEEvaluation>(Dto);
                pceEntity.Id = Guid.NewGuid();
                pceEntity.EvaluatorId = UserId;
                pceEntity.CreatedBy = UserId;
                pceEntity.CreatedAt = DateTime.Now;

                await _cbeContext.PCEEvaluations.AddAsync(pceEntity);

                var pce = await _cbeContext.ProductionCapacities.FindAsync(pceEntity.PCEId);

                if (Dto.SupportingEvidences != null && Dto.SupportingEvidences.Count > 0)
                {
                    foreach (var file in Dto.SupportingEvidences)
                    {
                        var supportingEvidenceFile = new CreateFileDto
                        {
                            File = file,
                            Catagory = "Supporting Evidence",
                            CaseId = pce.PCECaseId, 
                            CollateralId = pceEntity.Id,
                        };

                        await _uploadFileService.CreateUploadFile(UserId, supportingEvidenceFile);
                    }
                }
                if (Dto.ProductionProcessFlowDiagrams != null && Dto.ProductionProcessFlowDiagrams.Count > 0)
                {
                    foreach (var file in Dto.ProductionProcessFlowDiagrams)
                    {
                        var productionProcessFlowDiagramFile = new CreateFileDto
                        {
                            File = file,
                            Catagory = "Production Process Flow Diagram",
                            CaseId = pce.PCECaseId,
                            CollateralId = pceEntity.Id,
                        };

                        await _uploadFileService.CreateUploadFile(UserId, productionProcessFlowDiagramFile);
                    }
                }

                var currentStatus = "Pending";
                var CurrentStage = "Maker Officer";

                pce.CurrentStage = CurrentStage;
                pce.CurrentStatus = currentStatus;
                _cbeContext.ProductionCapacities.Update(pce);
                
                var previousCaseAssignment = await _cbeContext.ProductionCaseAssignments.Where(res => res.ProductionCapacityId == pce.Id && res.UserId == pceEntity.EvaluatorId).FirstOrDefaultAsync();
                previousCaseAssignment.Status = currentStatus;
                _cbeContext.ProductionCaseAssignments.Update(previousCaseAssignment);

                await _pceCaseTimeLineService.PCECaseTimeLine(new PCECaseTimeLinePostDto
                {
                    Activity = $"<strong class=\"text-info\"> PCE Case Evaluation Created and Pending.</strong> <br> <i class='text-purple'>Property Owner:</i> {pce.PropertyOwner}. &nbsp; <i class='text-purple'>Role:</i> {pce.Role}. &nbsp; <i class='text-purple'>Production Type</i>.{pce.PlantName}",
                    CurrentStage = CurrentStage,
                    CaseId = pce.PCECaseId,
                    // UserId = pce.CreatedBy
                });
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
                var pceEntity = await _cbeContext.PCEEvaluations
                                                .Include(e => e.ShiftHours)
                                                .Include(e => e.TimeConsumedToCheck)
                                                .Include(e => e.PCE)
                                                .FirstOrDefaultAsync(e => e.Id == Id);
                                        
                if (pceEntity == null)
                {
                    _logger.LogWarning("PCEEvaluation with id {Id} not found", Id);
                    throw new KeyNotFoundException("PCEEvaluation not found");
                }

                _mapper.Map(Dto, pceEntity);              
                
                pceEntity.UpdatedBy = UserId;
                pceEntity.UpdatedAt = DateTime.Now;
                _cbeContext.PCEEvaluations.Update(pceEntity);

                // Handle deleted files
                if (!string.IsNullOrEmpty(Dto.DeletedFileIds))
                {
                    var deletedFileGuids = Dto.DeletedFileIds.Split(',').Select(id => Guid.Parse(id)).ToList();
                    var filesToDelete = await _cbeContext.UploadFiles.Where(file => deletedFileGuids.Contains(file.Id)).ToListAsync();
                    
                    foreach (var file in filesToDelete)
                    {
                        if (File.Exists(file.Path))
                        {
                            File.Delete(file.Path);
                        }
                    }
                    _cbeContext.UploadFiles.RemoveRange(filesToDelete);
                }           
                
                // Handle new file uploads
                if (Dto.NewSupportingEvidences != null && Dto.NewSupportingEvidences.Count > 0)
                {
                    foreach (var file in Dto.NewSupportingEvidences)
                    {
                        var supportingEvidenceFile = new CreateFileDto
                        {
                            File = file,
                            Catagory = "Supporting Evidence",
                            CaseId = pceEntity.PCE.PCECaseId,
                            CollateralId = pceEntity.Id,
                        };

                        await _uploadFileService.CreateUploadFile(UserId, supportingEvidenceFile);
                    }
                }
                
                if (Dto.NewProductionProcessFlowDiagrams != null && Dto.NewProductionProcessFlowDiagrams.Count > 0)
                {
                    foreach (var file in Dto.NewProductionProcessFlowDiagrams)
                    {
                        var productionProcessFlowDiagramFile = new CreateFileDto
                        {
                            File = file,
                            Catagory = "Production Process Flow Diagram",
                            CaseId = pceEntity.PCE.PCECaseId,
                            CollateralId = pceEntity.Id,
                        };

                        await _uploadFileService.CreateUploadFile(UserId, productionProcessFlowDiagramFile);
                    }
                }
                
                await _cbeContext.SaveChangesAsync();
                await transaction.CommitAsync();

                return _mapper.Map<PCEEvaluationReturnDto>(pceEntity);
            }

            // catch (DbUpdateException ex)
            // {
            //     throw new ApplicationException("Error updating PCEEvaluation entity", ex);
            // }
            // catch (InvalidOperationException ex)
            // {
            //     throw new ApplicationException("Error updating PCEEvaluation entity", ex);
            // }

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

                var pceEntity = await _cbeContext.PCEEvaluations
                                                .Include(e => e.ShiftHours)
                                                .Include(e => e.TimeConsumedToCheck)
                                                .Include(e => e.PCE)
                                                .ThenInclude(e => e.PCECase)
                                                .FirstOrDefaultAsync(e => e.Id == Id);
                                                // .FindAsync(Id);

                if (pceEntity == null)
                {
                    _logger.LogWarning("PCEEvaluation with id {Id} not found", Id);
                    throw new KeyNotFoundException("PCEEvaluation not found");
                }

                var relatedFiles = await _cbeContext.UploadFiles.Where(file => file.CollateralId == pceEntity.Id).ToListAsync();

                // Delete physical files
                foreach (var file in relatedFiles)
                {
                    if (File.Exists(file.Path))
                    {
                        File.Delete(file.Path);
                    }
                }

                // Remove Evaluations with related entries and files from database
                _cbeContext.UploadFiles.RemoveRange(relatedFiles);
                _cbeContext.PCEEvaluations.Remove(pceEntity);

                var previousValuation = await _cbeContext.PCEEvaluations.Where(res => res.PCEId == pceEntity.PCEId && res != pceEntity).ToListAsync();
                var currentStatus = "New";
                var CurrentStage = "Maker Officer";

                // if (previousValuation.Count > 1){
                if (previousValuation.Any()){
                    currentStatus = "Reestimate";
                }

                pceEntity.PCE.CurrentStage = CurrentStage;
                pceEntity.PCE.CurrentStatus = currentStatus;
                _cbeContext.ProductionCapacities.Update(pceEntity.PCE);

                var previousCaseAssignment = await _cbeContext.ProductionCaseAssignments.Where(res => res.ProductionCapacityId == pceEntity.PCE.Id && res.UserId == pceEntity.EvaluatorId).FirstOrDefaultAsync();
                previousCaseAssignment.Status = currentStatus;
                _cbeContext.ProductionCaseAssignments.Update(previousCaseAssignment);

                await _pceCaseTimeLineService.PCECaseTimeLine(new PCECaseTimeLinePostDto
                {
                    CaseId = pceEntity.PCE.PCECaseId,
                    Activity = $"<strong class=\"text-warning\"> PCE Case Evaluation is retracted.</strong> <br> <i class='text-purple'>Property Owner:</i> {pceEntity.PCE.PropertyOwner}. &nbsp; <i class='text-purple'>Role:</i> {pceEntity.PCE.Role}. &nbsp; <i class='text-purple'>Production Type</i>.{pceEntity.PCE.PlantName}",
                    CurrentStage = CurrentStage,
                    // UserId = pceEntity.PCE.CreatedBy
                });

                await _cbeContext.SaveChangesAsync();
                await transaction.CommitAsync();

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting PCEEvaluation with id {Id}", Id);
                await transaction.RollbackAsync();
                throw new ApplicationException("An error occurred while deleting the PCEEvaluation.");
            }
        }

        public async Task<bool> EvaluatePCEEvaluation(Guid UserId, Guid Id)
        {
            using var transaction = await _cbeContext.Database.BeginTransactionAsync();
            try
            {
                var pceEntity = await _cbeContext.PCEEvaluations
                                        .Include(e => e.ShiftHours)
                                        .Include(e => e.TimeConsumedToCheck)
                                        .Include(e => e.PCE)
                                        .ThenInclude(e => e.PCECase)
                                        .FirstOrDefaultAsync(e => e.Id == Id);
                                        
                if (pceEntity == null)
                {
                    _logger.LogWarning("PCEEvaluation with id {Id} not found", Id);
                    throw new KeyNotFoundException("PCEEvaluation not found");
                }

                _cbeContext.PCEEvaluations.Update(pceEntity);

                var Status = "Completed";
                var CurrentStage = "Maker Officer";
                // var CurrentStage = "Relation Manager";
                var activity = "completed";
                // var activity = "submitted";

                if (pceEntity.PCE.CurrentStatus == "Reestimated")
                {
                    Status = "Reestimated";
                    activity = "reestimated";
                    // activity = "resubmitted";
                }
                pceEntity.PCE.CurrentStage = CurrentStage;
                pceEntity.PCE.CurrentStatus = Status;
                _cbeContext.ProductionCapacities.Update(pceEntity.PCE);

                var previousCaseAssignment = await _cbeContext.ProductionCaseAssignments.Where(res => res.ProductionCapacityId == pceEntity.PCEId && res.UserId == UserId).FirstOrDefaultAsync();
                previousCaseAssignment.Status = Status;
                previousCaseAssignment.CompletionDate = DateTime.Now;// DateTime.UtcNow
                _cbeContext.ProductionCaseAssignments.Update(previousCaseAssignment);

                await _pceCaseTimeLineService.PCECaseTimeLine(new PCECaseTimeLinePostDto
                {
                    CaseId = pceEntity.PCE.PCECaseId,
                    Activity = $"<strong class=\"text-success\"> PCE Case Evaluation is {activity} to Relation Manager.</strong>  <br> <i class='text-purple'>Property Owner:</i> {pceEntity.PCE.PropertyOwner}. &nbsp; <i class='text-purple'>Role:</i> {pceEntity.PCE.Role}. &nbsp; <i class='text-purple'>Production Type</i>.{pceEntity.PCE.PlantName}",
                    // Activity =  $"<strong class=\"text-success\">New PCE Case has been {activity}.</strong>  <br> <i class='text-purple'>Property Owner:</i> {pceEntity.PCE.PropertyOwner}. &nbsp; <i class='text-purple'>Role:</i> {pceEntity.PCE.Role}. &nbsp; <i class='text-purple'>Production Type</i>.{pceEntity.PCE.PlantName}",
                    CurrentStage = CurrentStage
                });

                await _cbeContext.SaveChangesAsync();
                await transaction.CommitAsync();

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending PCEEvaluation to RM");
                await transaction.RollbackAsync();
                throw new ApplicationException("An error occurred while sending PCEEvaluation to RM.");
            }
        }

        public async Task<bool> RejectPCEEvaluation(Guid UserId, PCERejectPostDto Dto)
        {
            using var transaction = await _cbeContext.Database.BeginTransactionAsync();
            try
            {
                var assignedPCECases = await _cbeContext.ProductionCapacities.FirstOrDefaultAsync(res => res.Id == Dto.PCEId);

                var returnPCE = _mapper.Map<ProductionReject>(Dto);
                returnPCE.CreationDate = DateTime.Now;
                returnPCE.RejectedBy = UserId;
                await _cbeContext.ProductionRejects.AddAsync(returnPCE);

                var pce = await _cbeContext.ProductionCapacities.FindAsync(Dto.PCEId);
                
                var Status = "Rejected";
                var CurrentStage = "Maker Officer";

                pce.CurrentStage = CurrentStage;
                pce.CurrentStatus = Status;
                _cbeContext.ProductionCapacities.Update(pce);

                var previousPCECaseAssignment = await _cbeContext.ProductionCaseAssignments.FirstOrDefaultAsync(res => res.ProductionCapacityId == Dto.PCEId && res.UserId == UserId);
                previousPCECaseAssignment.Status = Status;              
                _cbeContext.Update(previousPCECaseAssignment);

                await _pceCaseTimeLineService.PCECaseTimeLine(new PCECaseTimeLinePostDto
                {
                    CaseId = pce.PCECaseId, 
                    Activity = $" <strong class=\"text-danger\">PCE is rejected by MO as inadequate for evaluation and returned to Relation Manager for correction.</strong> <br> <i class='text-purple'>Property Owner:</i> {pce.PropertyOwner}. &nbsp; <i class='text-purple'>Role:</i> {pce.Role}. &nbsp; <i class='text-purple'>Production Type</i>.{pce.PlantName}",
                    CurrentStage = CurrentStage
                });

                await _cbeContext.SaveChangesAsync();
                await transaction.CommitAsync();

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error returning PCEEvaluation");
                await transaction.RollbackAsync();
                throw new ApplicationException("An error occurred while returning the PCEEvaluation.");
            }
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
                    _logger.LogWarning("PCEEvaluation with id {Id} not found", Id);
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

        public async Task<PCEEvaluationReturnDto> GetPCEEvaluationsByPCEId(Guid UserId, Guid PCEId)
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

        
        public async Task<IEnumerable<PCENewCaseDto>> GetPCECases(Guid UserId, string Status)
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
                var dto = _mapper.Map<PCENewCaseDto>(pceCase);
                dto.NoOfCollateral = string.IsNullOrEmpty(Status) || Status.Equals("All", StringComparison.OrdinalIgnoreCase)
                                    ? productionCapacities.Count(pc => pc.PCECaseId == pceCase.Id)
                                    : productionCapacities.Count(pc => pc.PCECaseId == pceCase.Id && pc.CurrentStatus == Status);
                dto.TotalNoOfCollateral = productionCapacities.Count(pc => pc.PCECaseId == pceCase.Id);
                return dto;
            }).ToList();

            return returnDtos;
        }


        public async Task<PCECasesCountDto> GetDashboardPCECaseCount(Guid UserId)
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
            var statuses = new[] { "New", "Pending", "Completed", "Reestimate", "Reestimated" };
            var tasks = statuses.Select(status => GetPCEsCountAsync(UserId, PCECaseId, Stage, status)).ToList();

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
            // var pceCase = await _cbeContext.PCECases.AsNoTracking().FirstOrDefaultAsync(res => res.Id == pce.PCECaseId);                 
            var reestimation = await _cbeContext.ProductionCapacityReestimations.AsNoTracking().FirstOrDefaultAsync(res => res.ProductionCapacityId == PCEId); 
            var relatedFiles = await _uploadFileService.GetUploadFileByCollateralId(PCEId);          
            var currentUser = await _cbeContext.CreateUsers.AsNoTracking().Include(res => res.Role).FirstOrDefaultAsync(res => res.Id == UserId);             
            var valuationHistory = await GetValuationHistory(UserId, PCEId);
                  
            return new PCEDetailDto
            {
                PCECase = pce.PCECase,
                ProductionCapacity = _mapper.Map<ReturnProductionDto>(pce),
                PCEValuationHistory = valuationHistory,
                Reestimation = reestimation,
                RelatedFiles = relatedFiles,
                CurrentUser = currentUser
            };
        }                       
    
        public async Task<PCEValuationHistoryDto> GetValuationHistory(Guid UserId, Guid PCEId)
        {

            var pce = await _cbeContext.ProductionCapacities.AsNoTracking().FirstOrDefaultAsync(res => res.Id == PCEId);                 
            
            PCEEvaluationReturnDto latestEvaluation = null;

            if (pce.CurrentStatus != "New" && pce.CurrentStatus != "Reestimate")
            {  
                latestEvaluation = await GetPCEEvaluationsByPCEId(UserId, PCEId);
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
    }
}