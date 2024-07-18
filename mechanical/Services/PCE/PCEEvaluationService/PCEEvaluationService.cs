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
using mechanical.Models.PCE.Dto.PlantCapacityEstimationDto;
using mechanical.Models.PCE.Dto.PCECaseDto;
using mechanical.Models.PCE.Dto.RMDashboardDto;


// using mechanical.Models.Dto.CaseAssignmentDto;
// using mechanical.Models.Dto.CaseTimeLineDto;
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
            try
            {
                var pceEntity = _mapper.Map<PCEEvaluation>(Dto);
                pceEntity.Id = Guid.NewGuid();
                pceEntity.EvaluatorID = UserId; 
                pceEntity.CreatedBy = UserId; 
                pceEntity.CreatedAt = DateTime.Now;
                pceEntity.Status = Status.New; 

                await _cbeContext.PCEEvaluations.AddAsync(pceEntity);
                await _cbeContext.SaveChangesAsync();

                if (Dto.SupportingEvidences != null && Dto.SupportingEvidences.Count > 0)
                {
                    foreach (var file in Dto.SupportingEvidences)
                    {
                        var supportingEvidenceFile = new CreateFileDto
                        {
                            File = file,
                            Catagory = "Supporting Evidence",
                            CaseId = pceEntity.PCE.PCECaseId,
                            CollateralId = pceEntity.PCEId
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
                            CaseId = pceEntity.PCE.PCECaseId,
                            CollateralId = pceEntity.PCEId
                        };

                        await _uploadFileService.CreateUploadFile(UserId, productionProcessFlowDiagramFile);
                    }
                }
                await _cbeContext.SaveChangesAsync();

                // var PCE = await _cbeContext.PCEs.FindAsync(pceentity.PCEId);
                // PCE.CurrentStage = "Maker Officer";
                // PCECase.CurrentStatus = "Pending";
                // _cbeContext.Update(PCE);
                // await _cbeContext.SaveChangesAsync();

                // await _pceCaseTimeLineService.CreatePCECaseTimeLine(new PCECaseTimeLinePostDto
                // {
                //     PCECaseId = PCE.PCECaseId,
                //     Activity = $" <strong class=\"text-sucess\">PCE has been Evaluated and sent to Relational Manager. <br> <i class='text-purple'>Evaluation Center:</i> {PCE.PCECase.District.Name}."</strong> <br> <i class='text-purple'>PCE Catagory:</i> {EnumHelper.GetEnumDisplayName(PCE.Catagory)}. &nbsp; <i class='text-purple'>PCE Type:</i> {EnumHelper.GetEnumDisplayName(PCE.Type)}.",
                //     CurrentStage = "Relational Manager"
                // });


                return _mapper.Map<PCEEvaluationReturnDto>(pceEntity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating PCEEvaluation");
                throw new ApplicationException("An error occurred while creating the PCEEvaluation.");
            }
        }

        public async Task<PCEEvaluationReturnDto> UpdatePCEEvaluation(Guid UserId, Guid id, PCEEvaluationPostDto Dto)
        {
            try
            {
                var pceEntity = await _cbeContext.PCEEvaluations.FindAsync(id);
                if (pceEntity == null)
                {
                    _logger.LogWarning("PCEEvaluation with id {Id} not found", id);
                    throw new KeyNotFoundException("PCEEvaluation not found");
                }

                _mapper.Map(Dto, pceEntity);
                _cbeContext.Update(pceEntity);
                await _cbeContext.SaveChangesAsync();

                if (Dto.SupportingEvidences != null && Dto.SupportingEvidences.Count > 0)
                {
                    foreach (var file in Dto.SupportingEvidences)
                    {
                        var supportingEvidenceFile = new CreateFileDto
                        {
                            File = file,
                            Catagory = "Supporting Evidence",
                            CaseId = pceEntity.PCE.PCECaseId,
                            CollateralId = pceEntity.PCEId
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
                            CaseId = pceEntity.PCE.PCECaseId,
                            CollateralId = pceEntity.PCEId
                        };

                        await _uploadFileService.CreateUploadFile(UserId, productionProcessFlowDiagramFile);
                    }
                }

                await _cbeContext.SaveChangesAsync();
            
                return _mapper.Map<PCEEvaluationReturnDto>(pceEntity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating PCEEvaluation");
                throw new ApplicationException("An error occurred while updating the PCEEvaluation.");
            }
        }        

        public async Task<PCEEvaluationReturnDto> PendPCEEvaluation(Guid UserId, Guid Id, PCEEvaluationPostDto Dto)
        {
            try
            {
                var pceEntity = await _cbeContext.PCEEvaluations.FindAsync(Id);
                if (pceEntity == null)
                {
                    _logger.LogWarning("PCEEvaluation with id {Id} not found", Id);
                    throw new KeyNotFoundException("PCEEvaluation not found");
                }

                _mapper.Map(Dto, pceEntity);
                pceEntity.Status = Status.Pending;
                await _cbeContext.SaveChangesAsync();

                // await _pceCaseTimeLineService.CreatePCECaseTimeLine(new PCECaseTimeLinePostDto
                // {
                //     PCECaseId = PCE.PCECaseId,
                //     Activity = $" <strong class=\"text-sucess\">PCE has been Started. <br> <i class='text-purple'>Evaluation Center:</i> {PCE.PCECase.District.Name}."</strong> <br> <i class='text-purple'>PCE Catagory:</i> {EnumHelper.GetEnumDisplayName(PCE.Catagory)}. &nbsp; <i class='text-purple'>PCE Type:</i> {EnumHelper.GetEnumDisplayName(PCE.Type)}.",
                //     CurrentStage = "Relational Manager"
                // });

                return _mapper.Map<PCEEvaluationReturnDto>(pceEntity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error pending PCEEvaluation");
                throw new ApplicationException("An error occurred while pending the PCEEvaluation.");
            }
        }

        public async Task<PCEEvaluationReturnDto> EvaluatePCEEvaluation(Guid UserId, Guid Id, PCEEvaluationPostDto Dto)
        {
            try
            {
                var pceEntity = await _cbeContext.PCEEvaluations.FindAsync(Id);
                if (pceEntity == null)
                {
                    _logger.LogWarning("PCEEvaluation with id {Id} not found", Id);
                    throw new KeyNotFoundException("PCEEvaluation not found");
                }

                _mapper.Map(Dto, pceEntity);
                pceEntity.Status = Status.Evaluated;
                await _cbeContext.SaveChangesAsync();
                
                // var PCE = await _cbeContext.PCEs.FindAsync(pceentity.PCEId);
                // PCE.CurrentStage = "Relational Manager";
                // PCE.CurrentStatus = "Evaluated";
                // _cbeContext.PCEs.Update(PCE);
                // await _cbeContext.SaveChangesAsync();

                // await _pceCaseTimeLineService.CreatePCECaseTimeLine(new PCECaseTimeLinePostDto
                // {
                //     PCECaseId = PCE.PCECaseId,
                //     Activity = $" <strong class=\"text-sucess\">PCE has been Evaluated and sent to Relational Manager. <br> <i class='text-purple'>Evaluation Center:</i> {PCE.PCECase.District.Name}."</strong> <br> <i class='text-purple'>PCE Catagory:</i> {EnumHelper.GetEnumDisplayName(PCE.Catagory)}. &nbsp; <i class='text-purple'>PCE Type:</i> {EnumHelper.GetEnumDisplayName(PCE.Type)}.",
                //     CurrentStage = "Relational Manager"
                // });

                return _mapper.Map<PCEEvaluationReturnDto>(pceEntity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error evaluating PCEEvaluation");
                throw new ApplicationException("An error occurred while evaluating the PCEEvaluation.");
            }
        }
        public async Task<bool> RejectPCEEvaluation(Guid UserId, PCERejectPostDto Dto)
        {  
            try
            {   
            

                var assignedPCECases = await _cbeContext.ProductionCapacities.FirstOrDefaultAsync(res => res.Id == Dto.PCEId);
              
                var returnPCE = _mapper.Map<ProductionReject>(Dto);
                returnPCE.CreationDate = DateTime.Now;
                returnPCE.RejectedBy = UserId;
                await _cbeContext.ProductionRejects.AddAsync(returnPCE);
                await _cbeContext.SaveChangesAsync();
              
                // var productionCapacity = await _cbeContext.ProductionCapacities.FindAsync(Dto.PCEId);
                // productionCapacity.CurrentStage = "Relational Manager";
                // productionCapacity.CurrentStatus = "Rejected";
                // _cbeContext.ProductionCapacities.Update(productionCapacity);
                // await _cbeContext.SaveChangesAsync();

                // var PCECaseAssignment = await _cbeContext.ProductionCaseAssignments.FirstOrDefaultAsync(res => res.PCEId == Dto.PCEId && res.UserId == Return.RejectedBy);
                // PCECaseAssignment.Status = "Return";
                // _cbeContext.Update(PCECaseAssignment);            
                // await _cbeContext.SaveChangesAsync(); 

                // await _pceCaseTimeLineService.CreatePCECaseTimeLine(new PCECaseTimeLinePostDto
                // {
                //     PCECaseId = assignedPCECases.PCECaseId,
                //     Activity = $"<strong>PCE is Rejected.</strong> <br> <i class='text-purple'>",
                //     CurrentStage = "Maker Manager",
                // });

                // await _pceCaseTimeLineService.CreatePCECaseTimeLine(new PCECaseTimeLinePostDto
                // {
                //     PCECaseId = PCE.PCECaseId,
                //     Activity = $" <strong class=\"text-sucess\">PCE has been Rejected and sent to Relational Manager. <br> <i class='text-purple'>Evaluation Center:</i> {PCE.PCECase.District.Name}."</strong> <br> <i class='text-purple'>PCE Catagory:</i> {EnumHelper.GetEnumDisplayName(PCE.Catagory)}. &nbsp; <i class='text-purple'>PCE Type:</i> {EnumHelper.GetEnumDisplayName(PCE.Type)}.",
                //     CurrentStage = "Relational Manager"
                // });

                // await _cbeContext.SaveChangesAsync(); 
                
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error returning PCEEvaluation");
                throw new ApplicationException("An error occurred while returning the PCEEvaluation.");
            }
        }
    
           
        public async Task<PCEEvaluationReturnDto> ReevaluatePCEEvaluation(Guid UserId, Guid Id, PCEEvaluationPostDto Dto)
        {
            try
            {
                var pceEntity = await _cbeContext.PCEEvaluations.FindAsync(Id);
                if (pceEntity == null)
                {
                    _logger.LogWarning("PCEEvaluation with id {Id} not found", Id);
                    throw new KeyNotFoundException("PCEEvaluation not found");
                }

                _mapper.Map(Dto, pceEntity);

                // var pceEntity = _mapper.Map<PCEEvaluation>(Dto);
                // pceEntity.NetEstimationValue = pceEntity.MarketShareFactor * pceEntity.DepreciationRate * pceEntity.EqpmntConditionFactor * pceEntity.ReplacementCost;
                pceEntity.Status = Status.Reevaluated;
                await _cbeContext.SaveChangesAsync();

                // var PCE = await _cbeContext.PCEs.FindAsync(pceentity.PCEId);
                // PCE.CurrentStage = "Relational Manager";
                // PCE.CurrentStatus = "Revaluated";
                // _cbeContext.PCEs.Update(PCE);
                // await _cbeContext.SaveChangesAsync();

                return _mapper.Map<PCEEvaluationReturnDto>(pceEntity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error reevaluating PCEEvaluation");
                throw new ApplicationException("An error occurred while reevaluating the PCEEvaluation.");
            }
        }

        public async Task<PCEEvaluationReturnDto> CompletePCEEvaluation(Guid UserId, Guid Id, PCEEvaluationPostDto Dto)
        {
            try
            {
                var pceEntity = await _cbeContext.PCEEvaluations.FindAsync(Id);
                if (pceEntity == null)
                {
                    _logger.LogWarning("PCEEvaluation with id {Id} not found", Id);
                    throw new KeyNotFoundException("PCEEvaluation not found");
                }

                _mapper.Map(Dto, pceEntity);
                pceEntity.Status = Status.Completed;
                await _cbeContext.SaveChangesAsync();

                // var PCE = await _cbeContext.PCEs.FindAsync(pceentity.PCEId);
                // PCE.CurrentStage = "Relational Manager";
                // PCE.CurrentStatus = "Completed";
                // _cbeContext.PCEs.Update(PCE);
                // await _cbeContext.SaveChangesAsync();

                return _mapper.Map<PCEEvaluationReturnDto>(pceEntity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error completing PCEEvaluation");
                throw new ApplicationException("An error occurred while completing the PCEEvaluation.");
            }
        }

        public async Task<bool> DeletePCEEvaluation(Guid UserId, Guid Id)
        {
            try
            {
                var pceEntity = await _cbeContext.PCEEvaluations.FindAsync(Id);
                if (pceEntity == null)
                {
                    _logger.LogWarning("PCEEvaluation with id {Id} not found", Id);
                    throw new KeyNotFoundException("PCEEvaluation not found");
                }

                _cbeContext.PCEEvaluations.Remove(pceEntity);
                await _cbeContext.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting PCEEvaluation");
                throw new ApplicationException("An error occurred while deleting the PCEEvaluation.");
            }
        }

        public async Task<PCEEvaluationReturnDto> GetPCEEvaluation(Guid UserId, Guid Id)
        {
            try
            {
                var pceEntity = await _cbeContext.PCEEvaluations
                    .Include(e => e.ShiftHours)
                    .Include(e => e.TimeConsumedToCheck)
                    .Include(e => e.SupportingDocuments)
                    .Include(e => e.PCE)
                    .FirstOrDefaultAsync(e => e.Id == Id);
                if (pceEntity == null)
                {
                    _logger.LogWarning("PCEEvaluation with id {Id} not found", Id);
                    throw new KeyNotFoundException("PCEEvaluation not found");
                }
                return _mapper.Map<PCEEvaluationReturnDto>(pceEntity);
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
                    .Include(e => e.ShiftHours)
                    .Include(e => e.TimeConsumedToCheck)
                    .Include(e => e.SupportingDocuments)
                    .Include(e => e.PCE)
                    .FirstOrDefaultAsync(e => e.PCEId == PCEId);
                // if (pceEntity == null)
                // {
                //     _logger.LogWarning("PCEEvaluation with PCE Id {PCEId} not found", PCEId);
                //     throw new KeyNotFoundException("PCEEvaluation with PCEID {PCEId} not found");
                // }
                return _mapper.Map<PCEEvaluationReturnDto>(pceEntity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching PCEEvaluation with PCEID {PCEId}");
                throw new ApplicationException("An error occurred while fetching the PCEEvaluation with PCEID {PCEId}.");
            }
        }
        
        public async Task<IEnumerable<PCEEvaluationReturnDto>> GetAllPCEEvaluations(Guid UserId)
        {
            try
            {
                var pceEntities = await _cbeContext.PCEEvaluations
                    .Include(e => e.ShiftHours)
                    .Include(e => e.TimeConsumedToCheck)
                    .Include(e => e.SupportingDocuments)
                    .Include(e => e.PCE)
                    .ToListAsync();
                return _mapper.Map<IEnumerable<PCEEvaluationReturnDto>>(pceEntities);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching all PCEEvaluations");
                throw new ApplicationException("An error occurred while fetching all PCEEvaluations.");
            }
        }

        public async Task<IEnumerable<PCEEvaluationReturnDto>> GetPCEEvaluationsWithStatus(Guid UserId, Status status)
        {
            try
            {
                var pceEntities = await _cbeContext.PCEEvaluations.Where(e => e.Status == status)
                    .Include(e => e.ShiftHours)
                    .Include(e => e.TimeConsumedToCheck)
                    .Include(e => e.SupportingDocuments)
                    .Include(e => e.PCE)
                    .ToListAsync();
                return _mapper.Map<IEnumerable<PCEEvaluationReturnDto>>(pceEntities);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching {status} PCEEvaluations", EnumHelper.GetEnumDisplayName(status));
                throw new ApplicationException($"An error occurred while fetching {status} PCEEvaluations.", new Exception(EnumHelper.GetEnumDisplayName(status)));
            }
        }
        
        public async Task<PCEEvaluationPostDto> GetRejectedPCEEvaluation(Guid UserId, Guid PCEId)
        {
            try
            {            
                // PCECaseCommenAttributeDto PCECaseCommenAttributeDto = new PCECaseCommenAttributeDto();
                // ReturnEvaluatedPCECaseDto returnEvaluatedPCECaseDto = new ReturnEvaluatedPCECaseDto();

                var pceEntity = await _cbeContext.PCEEvaluations
                    .Include(e => e.ShiftHours)
                    .Include(e => e.TimeConsumedToCheck)
                    .Include(e => e.SupportingDocuments)
                    .FirstOrDefaultAsync(e => e.PCEId == PCEId);

                if (pceEntity == null)
                {
                    _logger.LogWarning("PCEEvaluation with PCE Id {PCEId} not found", PCEId);
                    throw new KeyNotFoundException("PCEEvaluation with PCEID {PCEId} not found");
                }
                return _mapper.Map<PCEEvaluationPostDto>(pceEntity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching rejected PCEEvaluation");
                throw new ApplicationException("An error occurred while fetching the rejected PCEEvaluation.");
            }
        }
        public async Task<IEnumerable<CorrectionRetunDto>> GetComments(Guid UserId, Guid PCEId)
        {
            try
            {
                var comments = await _cbeContext.Corrections.Where(c => c.Id == PCEId).ToListAsync();
                return _mapper.Map<IEnumerable<CorrectionRetunDto>>(comments);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching comments for PCEEvaluation with PCEId {PCEId}", PCEId);
                throw new ApplicationException($"An error occurred while fetching comments for PCEEvaluation with PCEId {PCEId}.");
            }
        }

        public async Task<PCEEvaluationReturnDto> ReworkPCEEvaluation(Guid UserId, Guid Id, PCEEvaluationPostDto Dto)
        {
            try
            {
                var pceEntity = await _cbeContext.PCEEvaluations.FindAsync(Id);
                if (pceEntity == null)
                {
                    _logger.LogWarning("PCEEvaluation with id {Id} not found", Id);
                    throw new KeyNotFoundException("PCEEvaluation not found");
                }

                _mapper.Map(Dto, pceEntity);
                pceEntity.Status = Status.Rework;
                await _cbeContext.SaveChangesAsync();

                // var PCE = await _cbeContext.PCEs.FindAsync(pceentity.PCEId);
                // PCE.CurrentStage = "Maker Officer";
                // PCE.CurrentStatus = "Rework";
                // _cbeContext.PCEs.Update(PCE);
                // await _cbeContext.SaveChangesAsync();

                return _mapper.Map<PCEEvaluationReturnDto>(pceEntity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error reworking PCEEvaluation");
                throw new ApplicationException("An error occurred while reworking the PCEEvaluation.");
            }
        }

        public async Task<PCEEvaluationReturnDto> ApprovePCEEvaluation(Guid UserId, Guid Id, PCEEvaluationPostDto Dto)
        {
            try
            {
                var pceEntity = await _cbeContext.PCEEvaluations.FindAsync(Id);
                if (pceEntity == null)
                {
                    _logger.LogWarning("PCEEvaluation with id {Id} not found", Id);
                    throw new KeyNotFoundException("PCEEvaluation not found");
                }

                _mapper.Map(Dto, pceEntity);
                pceEntity.Status = Status.Approved;
                await _cbeContext.SaveChangesAsync();

                // var PCE = await _cbeContext.PCEs.FindAsync(pceentity.PCEId);
                // PCE.CurrentStage = "Relational Manager";
                // PCE.CurrentStatus = "Approved";
                // _cbeContext.PCEs.Update(PCE);
                // await _cbeContext.SaveChangesAsync();

                return _mapper.Map<PCEEvaluationReturnDto>(pceEntity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error completing PCEEvaluation");
                throw new ApplicationException("An error occurred while completing the PCEEvaluation.");
            }
        }

        public async Task CompletePCEEvaluations(Guid UserId, IEnumerable<Guid> SelectedPCEIds, Guid CenterId)
        // public async Task ApprovePCEEvaluations(Guid UserId, IEnumerable<Guid> SelectedPCEIds, Guid CenterId)
        {
            try
            {
                var PCEEvaluations = await _cbeContext.PCEEvaluations.Where(e => SelectedPCEIds.Contains(e.Id)).ToListAsync();
                foreach (var PCEEvaluation in PCEEvaluations)
                {
                    PCEEvaluation.Status = Status.Completed;
                    // PCEEvaluation.Status = Status.Approved;
                }
                await _cbeContext.SaveChangesAsync();
            }

            catch (Exception ex)
            {
                _logger.LogError(ex, "Error senting PCEEvaluations for approval");
                throw new ApplicationException("An error occurred while senting PCEEvaluations for approval.");
            }
        }

        public async Task<IEnumerable<int>> GetDashboardPCEEvaluationCount(Guid UserId)
        {
            try
            {
                var newCount = await _cbeContext.PCEEvaluations.CountAsync(e => e.Status == Status.New);
                var pendingCount = await _cbeContext.PCEEvaluations.CountAsync(e => e.Status == Status.Pending);
                var evaluatedCount = await _cbeContext.PCEEvaluations.CountAsync(e => e.Status == Status.Evaluated);
                var rejectedCount = await _cbeContext.PCEEvaluations.CountAsync(e => e.Status == Status.Rejected);
                var reevaluatedCount = await _cbeContext.PCEEvaluations.CountAsync(e => e.Status == Status.Reevaluated);
                var completedCount = await _cbeContext.PCEEvaluations.CountAsync(e => e.Status == Status.Completed);
                var allCount = await _cbeContext.PCEEvaluations.CountAsync();

                return new[] { newCount, pendingCount, evaluatedCount, rejectedCount, reevaluatedCount, completedCount, allCount };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching dashboard PCEEvaluation count");
                throw new ApplicationException("An error occurred while fetching the dashboard PCEEvaluation count.");
            }
        }
        
        public async Task<MyPCECaseCountDto> GetDashboardPCECaseCount(Guid userId)
        {
            var NewPCECollateral = await _cbeContext.ProductionCaseAssignments.Include(res => res.ProductionCapacity).Where(res => res.UserId == userId && res.Status == "New").ToListAsync();
            var PendPCECollateral = await _cbeContext.ProductionCaseAssignments.Include(res => res.ProductionCapacity).Where(res => res.UserId == userId && res.Status == "Pending").ToListAsync();
            var CompPCECollateral = await _cbeContext.ProductionCaseAssignments.Include(res => res.ProductionCapacity).Where(res => res.UserId == userId && res.Status == "Completed").ToListAsync();
            var TotalPCECollateral = await _cbeContext.ProductionCaseAssignments.Include(res => res.ProductionCapacity).Where(res => res.UserId == userId ).ToListAsync();

            return new MyPCECaseCountDto()
            {
                NewPCECaseCount = NewPCECollateral.Select(res => res.ProductionCapacity.PCECaseId).Distinct().Count(),
                NewPCECollateralCount = await _cbeContext.ProductionCaseAssignments.Where(res => res.UserId == userId && res.Status == "New").CountAsync(),

                PendingPCECaseCount = PendPCECollateral.Select(res => res.ProductionCapacity.PCECaseId).Distinct().Count(),
                PendingPCECollateralCount = await _cbeContext.ProductionCaseAssignments.Where(res => res.UserId == userId && res.Status == "Pending").CountAsync(),

                CompletedPCECaseCount = CompPCECollateral.Select(res => res.ProductionCapacity.PCECaseId).Distinct().Count(),
                CompletedPCECollateralCount = await _cbeContext.ProductionCaseAssignments.Where(res => res.UserId == userId && res.Status == "Completed").CountAsync(),

                TotalPCECaseCount = TotalPCECollateral.Select(res => res.ProductionCapacity.PCECaseId).Distinct().Count(),
                TotalPCECollateralCount = await _cbeContext.ProductionCaseAssignments.Where(res => res.UserId == userId).CountAsync(),
            };
        }

        public async Task<bool> SendToRM(Guid UserId, Guid Id)
        {
            try
            {
                var pceEntity = await _cbeContext.PCEEvaluations.FindAsync(Id);
                if (pceEntity == null)
                {
                    _logger.LogWarning("PCEEvaluation with id {Id} not found", Id);
                    throw new KeyNotFoundException("PCEEvaluation not found");
                }

                pceEntity.Status = Status.Evaluated;
                _cbeContext.PCEEvaluations.Update(pceEntity);
                await _cbeContext.SaveChangesAsync();

                // var pce = await _cbeContext.ProductionCapacities.FindAsync(pceEntity.PCEId);
                pceEntity.PCE.CurrentStage = "Relational Manager";
                pceEntity.PCE.CurrentStatus = "Evaluated";
                _cbeContext.ProductionCapacities.Update(pceEntity.PCE);
                await _cbeContext.SaveChangesAsync();

                // var previousCaseAssignment = await _cbeContext.ProductionCaseAssignments.Where(res => res.ProductionCapacityId == pceEntity.PCEId && res.UserId == pceEntity.PCE.PCECase.RMUserId).FirstOrDefaultAsync();
                var previousCaseAssignment = await _cbeContext.ProductionCaseAssignments.Where(res => res.ProductionCapacityId == pceEntity.PCEId).FirstOrDefaultAsync();
                previousCaseAssignment.Status = "RM Evaluated";
                _cbeContext.ProductionCaseAssignments.Update(previousCaseAssignment);
                await _cbeContext.SaveChangesAsync();

                await _pceCaseTimeLineService.PCECaseTimeLine(new PCECaseTimeLinePostDto
                {
                    CaseId = pceEntity.PCE.PCECaseId,
                    Activity = $"<strong> PCE Case Evaluation sent to Relational Manager.</strong>",
                    CurrentStage = "Maker Manager"
                });

                await _pceCaseTimeLineService.PCECaseTimeLine(new PCECaseTimeLinePostDto
                {
                    CaseId = pceEntity.PCE.PCECaseId,
                    Activity = $"<strong>New PCE Case has been evaluated.</strong>",
                    CurrentStage = "Relational Manager",
                    // UserId = pceEntity.PCE.CreatedBy
                    // UserId = pceEntity.PCE.PCECase.RMUserId
                }); 

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending PCEEvaluation to RM");
                throw new ApplicationException("An error occurred while sending PCEEvaluation to RM.");
            }
        }

        public async Task<bool> SendToMO(Guid UserId, Guid Id)
        {
            try
            {
                var pceEntity = await _cbeContext.PCEEvaluations.FindAsync(Id);
                if (pceEntity == null)
                {
                    _logger.LogWarning("PCEEvaluation with id {Id} not found", Id);
                    throw new KeyNotFoundException("PCEEvaluation not found");
                }

                pceEntity.Status = Status.Rework;
                _cbeContext.PCEEvaluations.Update(pceEntity);
                await _cbeContext.SaveChangesAsync();

                var pce = await _cbeContext.ProductionCapacities.FindAsync(pceEntity.PCEId);
                pce.CurrentStage = "Maker Officer";
                pce.CurrentStatus = "Rework";
                _cbeContext.ProductionCapacities.Update(pce);
                await _cbeContext.SaveChangesAsync();
                
                var previousCaseAssignment = await _cbeContext.ProductionCaseAssignments.Where(res => res.ProductionCapacityId == pceEntity.PCEId && res.UserId == pceEntity.EvaluatorID).FirstOrDefaultAsync();
                previousCaseAssignment.Status = "MO Rework";
                _cbeContext.ProductionCaseAssignments.Update(previousCaseAssignment);
                await _cbeContext.SaveChangesAsync();


                // await _pceCaseTimeLineService.CreateCaseTimeLine(new PCECaseTimeLinePostDto
                await _pceCaseTimeLineService.PCECaseTimeLine(new PCECaseTimeLinePostDto
                {
                    CaseId = pce.PCECaseId,
                    Activity = $"<strong> PCE Case Evaluation returned to Maker Officer for rework.</strong>",
                    CurrentStage = "Maker Manager"
                });
                await _pceCaseTimeLineService.PCECaseTimeLine(new PCECaseTimeLinePostDto
                {
                    CaseId = pce.PCECaseId,
                    Activity = $"<strong>Evaluated PCE Case has been returned for rework.</strong>",
                    CurrentStage = "Maker Officer",
                    UserId = pceEntity.EvaluatorID
                });           
            
                return true;
     
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending PCEEvaluation to MO");
                throw new ApplicationException("An error occurred while sending PCEEvaluation to MO.");
            }
        }

        public async Task<PCECaseReturntDto> GetPCECase(Guid UserId, Guid Id)
        {
            var loanPCECase = await _cbeContext.PCECases
                           .Include(res => res.BussinessLicence).Include(res => res.District).Include(res => res.ProductionCapacities)
                           .FirstOrDefaultAsync(c => c.Id == Id && c.RMUserId == UserId);
            return _mapper.Map<PCECaseReturntDto>(loanPCECase);
        }
        public async Task<PCECaseReturntDto> GetPCECaseDetail(Guid userId, Guid Id)
        {
            var loanPCECase = await _cbeContext.PCECases
                           .Include(res => res.BussinessLicence).Include(res => res.District).Include(res => res.ProductionCapacities)
                           .FirstOrDefaultAsync(c => c.Id == Id);
            return _mapper.Map<PCECaseReturntDto>(loanPCECase);
        }

        public async Task<IEnumerable<PCECaseReturntDto>> GetPCECasesWithStatus(Guid UserId, string status)
        {
            var PCECaseAssignments = await _cbeContext.ProductionCaseAssignments.Include(res => res.ProductionCapacity).ThenInclude(res => res.PCECase).Where(Ca => Ca.UserId == UserId && Ca.Status==status).ToListAsync();
            var UniquePCECases = PCECaseAssignments.Select(ca => ca.ProductionCapacity.PCECase).DistinctBy(c => c.Id).ToList();
            var ReturnDtos = _mapper.Map<IEnumerable<PCECaseReturntDto>>(UniquePCECases);
            foreach (var ReturnDto in ReturnDtos)
            {
                ReturnDto.TotalNoOfCollateral = await _cbeContext.ProductionCapacities.CountAsync(res => res.PCECaseId == ReturnDto.Id);
            }
            return ReturnDtos;
        }

        // public async Task<IEnumerable<PCECaseReturntDto>> GetPendingPCECases(Guid UserId)
        // {

        //     var PCECaseAssignments = await _cbeContext.ProductionCaseAssignments.Include(res => res.ProductionCapacity).ThenInclude(res => res.PCECase).Where(Ca => Ca.UserId == UserId && Ca.Status == "Pending").ToListAsync();
        //     var UniquePCECases = PCECaseAssignments.Select(ca => ca.ProductionCapacity.PCECase).DistinctBy(c => c.Id).ToList();
        //     var ReturnDtos = _mapper.Map<IEnumerable<PCECaseReturntDto>>(UniquePCECases);
        //     foreach (var ReturnDto in ReturnDtos)
        //     {
        //         ReturnDto.TotalNoOfCollateral = await _cbeContext.ProductionCapacities.CountAsync(res => res.PCECaseId == ReturnDto.Id);
        //     }
        //     return ReturnDtos;
        // }

        // public async Task<IEnumerable<PCECaseReturntDto>> GetCompletedPCECases(Guid UserId)
        // {

        //     var PCECaseAssignments = await _cbeContext.ProductionCaseAssignments.Include(res => res.ProductionCapacity).ThenInclude(res => res.PCECase).Where(Ca => Ca.UserId == UserId && Ca.Status=="Completed").ToListAsync();
        //     var UniquePCECases = PCECaseAssignments.Select(ca => ca.ProductionCapacity.PCECase) .DistinctBy(c => c.Id).ToList();
        //     var ReturnDtos = _mapper.Map<IEnumerable<PCECaseReturntDto>>(UniquePCECases);
        //     foreach (var ReturnDto in ReturnDtos)
        //     {
        //         ReturnDto.TotalNoOfCollateral = await _cbeContext.ProductionCapacities.CountAsync(res => res.PCECaseId == ReturnDto.Id);
        //     }
        //     return ReturnDtos;
        // }

        // public async Task<IEnumerable<PCECaseReturntDto>> GetRejectedPCECases(Guid UserId)
        // {

        //     var PCECaseAssignments = await _cbeContext.ProductionCaseAssignments.Include(res => res.ProductionCapacity).ThenInclude(res => res.PCECase).Where(Ca => Ca.UserId == UserId && Ca.Status=="Rejected").ToListAsync();
        //     var UniquePCECases = PCECaseAssignments.Select(ca => ca.ProductionCapacity.PCECase) .DistinctBy(c => c.Id).ToList();
        //     var ReturnDtos = _mapper.Map<IEnumerable<PCECaseReturntDto>>(UniquePCECases);
        //     foreach (var ReturnDto in ReturnDtos)
        //     {
        //         ReturnDto.TotalNoOfCollateral = await _cbeContext.ProductionCapacities.CountAsync(res => res.PCECaseId == ReturnDto.Id);
        //     }
        //     return ReturnDtos;
        // }

        // public async Task<IEnumerable<PCECaseReturntDto>> GetResubmittedPCECases(Guid UserId)
        // {

        //     var PCECaseAssignments = await _cbeContext.ProductionCaseAssignments.Include(res => res.ProductionCapacity).ThenInclude(res => res.PCECase).Where(Ca => Ca.UserId == UserId && Ca.Status=="Resubmitted").ToListAsync();
        //     var UniquePCECases = PCECaseAssignments.Select(ca => ca.ProductionCapacity.PCECase) .DistinctBy(c => c.Id).ToList();
        //     var ReturnDtos = _mapper.Map<IEnumerable<PCECaseReturntDto>>(UniquePCECases);
        //     foreach (var ReturnDto in ReturnDtos)
        //     {
        //         ReturnDto.TotalNoOfCollateral = await _cbeContext.ProductionCapacities.CountAsync(res => res.PCECaseId == ReturnDto.Id);
        //     }
        //     return ReturnDtos;
        // }

        public async Task<IEnumerable<PCECaseReturntDto>> GetTotalPCECases(Guid UserId)
        {

            // var cases = await _cbeContext.PCECases.Where(res => res.CurrentStage == "Maker Officer").ToListAsync();

            var PCECaseAssignments = await _cbeContext.ProductionCaseAssignments.Include(res => res.ProductionCapacity).ThenInclude(res => res.PCECase).Where(Ca => Ca.UserId == UserId).ToListAsync();
            // var PCECaseAssignments = await _cbeContext.ProductionCaseAssignments.Include(res => res.ProductionCapacity).ThenInclude(res => res.PCECase).ThenInclude(res => res.RMUserId).Where(Ca => Ca.UserId == UserId).ToListAsync();
            var UniquePCECases = PCECaseAssignments.Select(ca => ca.ProductionCapacity.PCECase) .DistinctBy(c => c.Id).ToList();
            var ReturnDtos = _mapper.Map<IEnumerable<PCECaseReturntDto>>(UniquePCECases);
            foreach (var ReturnDto in ReturnDtos)
            {
                ReturnDto.TotalNoOfCollateral = await _cbeContext.ProductionCapacities.CountAsync(res => res.PCECaseId == ReturnDto.Id);
            }
            return ReturnDtos;
        }

        public async Task<ReturnProductionDto> MyRejectedPCE(Guid UserId, Guid Id)
        {
            List<ProductionCaseAssignment> PCECaseAssignments = await _cbeContext.ProductionCaseAssignments.Where(ca => ca.UserId == UserId && ca.ProductionCapacityId == Id && ca.Status == "Correction").ToListAsync();

            List<ReturnProductionDto> returnProductionDtos = new List<ReturnProductionDto>();
            if (PCECaseAssignments != null)
            {
                foreach (var PCECaseAssignment in PCECaseAssignments)
                {
                    var productionCapacity = await _cbeContext.ProductionCapacities.FirstOrDefaultAsync(ca => ca.Id == PCECaseAssignment.ProductionCapacityId && ca.CurrentStatus == "Correction");
                    if (productionCapacity != null)
                    {
                        returnProductionDtos.Add(_mapper.Map<ReturnProductionDto>(productionCapacity));
                    }
                }
            }
            return returnProductionDtos[0];
        }

        public async Task<ReturnProductionDto> MyResubmittedPCE(Guid UserId, Guid Id)
        {
            List<ProductionCaseAssignment> PCECaseAssignments = await _cbeContext.ProductionCaseAssignments.Where(ca => ca.UserId == UserId && ca.ProductionCapacityId == Id && ca.Status == "Resubmitted").ToListAsync();

            List<ReturnProductionDto> returnProductionDtos = new List<ReturnProductionDto>();
            if (PCECaseAssignments != null)
            {
                foreach (var PCECaseAssignment in PCECaseAssignments)
                {
                    var productionCapacity = await _cbeContext.ProductionCapacities.FirstOrDefaultAsync(ca => ca.Id == PCECaseAssignment.ProductionCapacityId && ca.Id == Id && ca.CurrentStatus == "Resubmitted");
                    if (productionCapacity != null)
                    {
                        returnProductionDtos.Add(_mapper.Map<ReturnProductionDto>(productionCapacity));
                    }
                }
            }
            return returnProductionDtos[0];

        }

        public async Task<IEnumerable<ReturnProductionDto>> GetProductionCapacities(Guid PCECaseId)
        {
            var productions = await _cbeContext.ProductionCapacities.Where(res => res.PCECaseId == PCECaseId && res.CurrentStage == "Maker Officer").ToListAsync();
            return _mapper.Map<IEnumerable<ReturnProductionDto>>(productions);
        }

        public async Task<IEnumerable<ReturnProductionDto>> GetProductionCapacitiesWithStatus(Guid PCECaseId, string status)
        {
            var productions = await _cbeContext.ProductionCapacities.Where(res => res.PCECaseId == PCECaseId && (res.CurrentStatus == status && res.CurrentStage == "Maker Officer")).ToListAsync();
            return _mapper.Map<IEnumerable<ReturnProductionDto>>(productions);
        }

        public async Task<IEnumerable<PCEReturnCollateralDto>> GetPlantCapacities(Guid PCECaseId)
        {
            var plants = await _cbeContext.ProductionCapacities.Where(res => res.PCECaseId == PCECaseId && res.CurrentStage == "Maker Officer").ToListAsync();
            return _mapper.Map<IEnumerable<PCEReturnCollateralDto>>(plants);
        }


        public async Task<IEnumerable<PCEReturnCollateralDto>> GetPlantCapacitiesWithStatus(Guid PCECaseId, string status)
        {
            var plants = await _cbeContext.ProductionCapacities.Where(res => res.PCECaseId == PCECaseId && (res.CurrentStatus == status && res.CurrentStage == "Maker Officer")).ToListAsync();
            return _mapper.Map<IEnumerable<PCEReturnCollateralDto>>(plants);
        }

        public async Task<IEnumerable<ReturnProductionDto>> GetProductionCapacitiesWithStatusAndRole(Guid PCECaseId, string status, string role)
        {
            var productions = await _cbeContext.ProductionCapacities.Where(res => res.PCECaseId == PCECaseId && (res.CurrentStatus == status && res.CurrentStage == role)).ToListAsync();
            return _mapper.Map<IEnumerable<ReturnProductionDto>>(productions);
        }

        public async Task<IEnumerable<PCEReturnCollateralDto>> GetPlantCapacitiesWithStatusAndRole(Guid PCECaseId, string status, string role)
        {
            var plants = await _cbeContext.ProductionCapacities.Where(res => res.PCECaseId == PCECaseId && (res.CurrentStatus == status && res.CurrentStage == role)).ToListAsync();
            return _mapper.Map<IEnumerable<PCEReturnCollateralDto>>(plants);
        }

        // public async Task<IEnumerable<ReturnProductionDto>> MyRejectedPCEs(Guid UserId)
        // {
        //     List<ProductionCaseAssignment> PCECaseAssignments = await _cbeContext.ProductionCaseAssignments.Where(ca => ca.UserId == UserId && ca.Status== "Correction").ToListAsync();
        //     List<ProductionCapacity> ProductionCapacities = await _cbeContext.ProductionCapacities.Where(ca => ca.CurrentStage == "Maker Officer" && ca.CurrentStatus== "Correction").ToListAsync();

        //     List<ReturnProductionDto> returnProductionDtos = new List<ReturnProductionDto>();
        //     if (ProductionCapacities != null)
        //     {
        //         foreach (var PCECaseAssignment in PCECaseAssignments)
        //         {
        //             var productionCapacity = await _cbeContext.ProductionCapacities.FirstOrDefaultAsync(ca => ca.Id == PCECaseAssignment.ProductionCapacityId && ca.CurrentStatus == "Correction");
        //             if (productionCapacity != null)
        //             {
        //                 returnProductionDtos.Add(_mapper.Map<ReturnProductionDto>(productionCapacity));
        //             }
        //         }
        //     }
        //     return _mapper.Map<List<ReturnProductionDto>>(ProductionCapacities);
        // }

        // public async Task<IEnumerable<ReturnProductionDto>> MyResubmittedPCEs(Guid UserId)
        // {
        //     List<ProductionCaseAssignment> PCECaseAssignments = await _cbeContext.ProductionCaseAssignments.Where(ca => ca.UserId == UserId && ca.Status == "Resubmitted").ToListAsync();

        //     List<ReturnProductionDto> returnProductionDtos = new List<ReturnProductionDto>();
        //     if (PCECaseAssignments != null)
        //     {
        //         foreach (var PCECaseAssignment in PCECaseAssignments)
        //         {
        //             var productionCapacity = await _cbeContext.ProductionCapacities.FirstOrDefaultAsync(ca => ca.Id == PCECaseAssignment.ProductionCapacityId && ca.CurrentStatus == "Resubmitted");
        //             if (productionCapacity != null)
        //             {
        //                 returnProductionDtos.Add(_mapper.Map<ReturnProductionDto>(productionCapacity));
        //             }
        //         }
        //     }
        //     return returnProductionDtos;
        // }




    }
}