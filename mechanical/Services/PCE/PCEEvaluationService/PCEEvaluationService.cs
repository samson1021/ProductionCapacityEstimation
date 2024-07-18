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
using mechanical.Models.PCE.Dto.RMDashboardDto;
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
                pceEntity.EvaluatorID = UserId; 
                pceEntity.CreatedBy = UserId; 
                pceEntity.CreatedAt = DateTime.Now;

                await _cbeContext.PCEEvaluations.AddAsync(pceEntity);
                await _cbeContext.SaveChangesAsync();

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
                await _cbeContext.SaveChangesAsync();

                pce.CurrentStage = "Maker Officer";
                pce.CurrentStatus = "Pending";
                _cbeContext.ProductionCapacities.Update(pce);
                await _cbeContext.SaveChangesAsync();

                await _pceCaseTimeLineService.PCECaseTimeLine(new PCECaseTimeLinePostDto
                {
                    CaseId = pce.PCECaseId,
                    Activity = $"<strong> PCE Case Evaluation Created and Pending.</strong>",
                    CurrentStage = "Maker Officer",
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

        public async Task<PCEEvaluationReturnDto> UpdatePCEEvaluation(Guid UserId, Guid Id, PCEEvaluationReturnDto Dto)
        {
            using var transaction = await _cbeContext.Database.BeginTransactionAsync();
            try
            {
                var pceEntity = await _cbeContext.PCEEvaluations
                    // .Include(e => e.ShiftHours)
                    // .Include(e => e.TimeConsumedToCheck)
                    // .Include(e => e.PCE)
                    .FirstOrDefaultAsync(e => e.Id == Id);
                
                if (pceEntity == null)
                {
                    _logger.LogWarning("PCEEvaluation with id {Id} not found", Id);
                    throw new KeyNotFoundException("PCEEvaluation not found");
                }

                _mapper.Map(Dto, pceEntity);
                pceEntity.EvaluatorID = UserId; 
                pceEntity.UpdatedBy = UserId; 
                pceEntity.UpdatedAt = DateTime.Now;
                // _mapper.Map(Dto.ShiftHours, pceEntity.ShiftHours);

                // pceEntity.ShiftHours.Clear();
                // foreach (var shiftHours in Dto.ShiftHours)
                // {                       
                //         // pceEntity.ShiftHours 
                //         pceEntity.ShiftHours.Add(_mapper.Map<TimeRange>(shiftHours));            
                //         // pceEntity.ShiftHours.Add(new TimeRange
                //         // {
                //         //     Start = shiftHours.Start,
                //         //     End = shiftHours.End
                //         // });
                // }

                _cbeContext.Update(pceEntity);
                await _cbeContext.SaveChangesAsync();

                // var pce = await _cbeContext.ProductionCapacities.FindAsync(pceEntity.PCEId);

                // if (Dto.SupportingEvidences != null && Dto.SupportingEvidences.Count > 0)
                // {
                //     foreach (var file in Dto.SupportingEvidences)
                //     {
                //         var supportingEvidenceFile = new CreateFileDto
                //         {
                //             File = file,
                //             Catagory = "Supporting Evidence",
                //             CaseId = pce.PCECaseId,
                //             CollateralId = pceEntity.Id,
                //         };

                //         await _uploadFileService.CreateUploadFile(UserId, supportingEvidenceFile);
                //     }
                // }
                // if (Dto.ProductionProcessFlowDiagrams != null && Dto.ProductionProcessFlowDiagrams.Count > 0)
                // {
                //     foreach (var file in Dto.ProductionProcessFlowDiagrams)
                //     {
                //         var productionProcessFlowDiagramFile = new CreateFileDto
                //         {
                //             File = file,
                //             Catagory = "Production Process Flow Diagram",
                //             CaseId = pce.PCECaseId,
                //             CollateralId = pceEntity.Id,
                //         };

                //         await _uploadFileService.CreateUploadFile(UserId, productionProcessFlowDiagramFile);
                //     }
                // }

                // await _cbeContext.SaveChangesAsync();
                await transaction.CommitAsync();
            
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
                var pceEntity = await _cbeContext.PCEEvaluations.FindAsync(Id);
                if (pceEntity == null)
                {
                    _logger.LogWarning("PCEEvaluation with id {Id} not found", Id);
                    throw new KeyNotFoundException("PCEEvaluation not found");
                }

                var relatedFiles = await _cbeContext.UploadFiles
                    .Where(file => file.CollateralId == Id)
                    // .Where(file => file.CollateralId == pceEntity.Id)
                    .ToListAsync();

                // Delete physical files
                foreach (var file in relatedFiles)
                {
                    await _uploadFileService.DeleteFile(file.Id);
                }
                // Remove Evaluations and related files from database
                _cbeContext.UploadFiles.RemoveRange(relatedFiles);
                _cbeContext.PCEEvaluations.Remove(pceEntity);

                var pce = await _cbeContext.ProductionCapacities.FindAsync(pceEntity.PCEId);
                pce.CurrentStage = "Maker Officer";
                pce.CurrentStatus = "New";
                _cbeContext.ProductionCapacities.Update(pce);
                await _cbeContext.SaveChangesAsync();

                await _pceCaseTimeLineService.PCECaseTimeLine(new PCECaseTimeLinePostDto
                {
                    CaseId = pce.PCECaseId,
                    Activity = $"<strong> PCE Case Evaluation is retracted.</strong>",
                    CurrentStage = "Maker Officer",
                    // UserId = pce.CreatedBy
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
                var pceEntity = await _cbeContext.PCEEvaluations.FindAsync(Id);
                if (pceEntity == null)
                {
                    _logger.LogWarning("PCEEvaluation with id {Id} not found", Id);
                    throw new KeyNotFoundException("PCEEvaluation not found");
                }

                _cbeContext.PCEEvaluations.Update(pceEntity);
                await _cbeContext.SaveChangesAsync();

                var pce = await _cbeContext.ProductionCapacities.FindAsync(pceEntity.PCEId);
                pce.CurrentStage = "Relational Manager";
                pce.CurrentStatus = "Evaluated";
                _cbeContext.ProductionCapacities.Update(pce);
                await _cbeContext.SaveChangesAsync();

                // var previousCaseAssignment = await _cbeContext.ProductionCaseAssignments.Where(res => res.ProductionCapacityId == pceEntity.PCEId && res.UserId == pceEntity.PCE.PCECase.RMUserId).FirstOrDefaultAsync();
                var previousCaseAssignment = await _cbeContext.ProductionCaseAssignments.Where(res => res.ProductionCapacityId == pceEntity.PCEId).FirstOrDefaultAsync();
                previousCaseAssignment.Status = "Evaluated";
                _cbeContext.ProductionCaseAssignments.Update(previousCaseAssignment);
                await _cbeContext.SaveChangesAsync();

                await _pceCaseTimeLineService.PCECaseTimeLine(new PCECaseTimeLinePostDto
                {
                    CaseId = pce.PCECaseId,
                    Activity = $"<strong> PCE Case Evaluation sent to Relational Manager.</strong>",
                    CurrentStage = "Maker Manager",
                    // UserId = pce.PCECase.CreatedBy
                });

                await _pceCaseTimeLineService.PCECaseTimeLine(new PCECaseTimeLinePostDto
                {
                    CaseId = pce.PCECaseId,
                    Activity = $"<strong>New PCE Case has been evaluated.</strong>",
                    CurrentStage = "Relational Manager",
                    // UserId = pce.PCECase.CreatedBy
                    // UserId = pce.PCECase.RMUserId
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
                await _cbeContext.SaveChangesAsync();
              
                var pce = await _cbeContext.ProductionCapacities.FindAsync(Dto.PCEId);
                pce.CurrentStage = "Relational Manager";
                pce.CurrentStatus = "Rejected";
                _cbeContext.ProductionCapacities.Update(pce);
                await _cbeContext.SaveChangesAsync();

                var previousPCECaseAssignment = await _cbeContext.ProductionCaseAssignments.FirstOrDefaultAsync(res => res.ProductionCapacityId == Dto.PCEId && res.UserId == UserId);
                previousPCECaseAssignment.Status = "Rejected";
                _cbeContext.Update(previousPCECaseAssignment);            
                await _cbeContext.SaveChangesAsync(); 

                await _pceCaseTimeLineService.PCECaseTimeLine(new PCECaseTimeLinePostDto
                {
                    CaseId = pce.PCECaseId,
                    Activity = $"<strong>PCE is Rejected.</strong> <br> <i class='text-purple'>",
                    CurrentStage = "Maker Manager",
                    // UserId = pce.PCECase.CreatedBy
                });

                await _pceCaseTimeLineService.PCECaseTimeLine(new PCECaseTimeLinePostDto
                {
                    CaseId = pce.PCECaseId,
                    Activity = $" <strong class=\"text-sucess\">PCE has been Rejected and sent to Relational Manager. ",//<br> <i class='text-purple'>Evaluation Center:</i> {pce.PCECase.District.Name}."</strong> <br> <i class='text-purple'>PCE Catagory:</i> {EnumHelper.GetEnumDisplayName(pce.Catagory)}. &nbsp; <i class='text-purple'>PCE Type:</i> {EnumHelper.GetEnumDisplayName(pce.ProductionType)}.",
                    CurrentStage = "Relational Manager",
                    // UserId = pce.PCECase.CreatedBy,
                    // UserId = pce.PCECase.RMUserId
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

        public async Task<bool> ReworkPCEEvaluation(Guid UserId, Guid Id)
        // public async Task<PCEEvaluationReturnDto> ReworkPCEEvaluation(Guid UserId, Guid Id, PCEEvaluationPostDto Dto)
        {
            using var transaction = await _cbeContext.Database.BeginTransactionAsync();
            try
            {
                var pceEntity = await _cbeContext.PCEEvaluations.FindAsync(Id);
                if (pceEntity == null)
                {
                    _logger.LogWarning("PCEEvaluation with id {Id} not found", Id);
                    throw new KeyNotFoundException("PCEEvaluation not found");
                }

                _cbeContext.PCEEvaluations.Update(pceEntity);
                await _cbeContext.SaveChangesAsync();

                var pce = await _cbeContext.ProductionCapacities.FindAsync(pceEntity.PCEId);
                pce.CurrentStage = "Maker Officer";
                pce.CurrentStatus = "Rework";
                _cbeContext.ProductionCapacities.Update(pce);
                await _cbeContext.SaveChangesAsync();
                
                var previousCaseAssignment = await _cbeContext.ProductionCaseAssignments.Where(res => res.ProductionCapacityId == pceEntity.PCEId && res.UserId == pceEntity.EvaluatorID).FirstOrDefaultAsync();
                previousCaseAssignment.Status = "Rework";
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
                
                await _cbeContext.SaveChangesAsync(); 
                await transaction.CommitAsync();           
            
                return true;
     
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error reworking PCEEvaluation");
                await transaction.RollbackAsync();
                throw new ApplicationException("An error occurred while reworking the PCEEvaluation.");
            }
        }

        public async Task<MyPCECaseCountDto> GetDashboardPCECaseCount(Guid userId)
        {
            var NewPCEs = await _cbeContext.ProductionCaseAssignments.Include(res => res.ProductionCapacity).Where(res => res.UserId == userId && res.Status == "New").ToListAsync();
            var PendPCEs = await _cbeContext.ProductionCaseAssignments.Include(res => res.ProductionCapacity).Where(res => res.UserId == userId && res.Status == "Pending").ToListAsync();
            var CompPCEs = await _cbeContext.ProductionCaseAssignments.Include(res => res.ProductionCapacity).Where(res => res.UserId == userId && res.Status == "Completed").ToListAsync();
            var TotalPCEs = await _cbeContext.ProductionCaseAssignments.Include(res => res.ProductionCapacity).Where(res => res.UserId == userId ).ToListAsync();

            return new MyPCECaseCountDto()
            {
                NewPCECaseCount = NewPCEs.Select(res => res.ProductionCapacity.PCECaseId).Distinct().Count(),
                NewPCEsCount = await _cbeContext.ProductionCaseAssignments.Where(res => res.UserId == userId && res.Status == "New").CountAsync(),

                PendingPCECaseCount = PendPCEs.Select(res => res.ProductionCapacity.PCECaseId).Distinct().Count(),
                PendingPCEsCount = await _cbeContext.ProductionCaseAssignments.Where(res => res.UserId == userId && res.Status == "Pending").CountAsync(),

                CompletedPCECaseCount = CompPCEs.Select(res => res.ProductionCapacity.PCECaseId).Distinct().Count(),
                CompletedPCEsCount = await _cbeContext.ProductionCaseAssignments.Where(res => res.UserId == userId && res.Status == "Completed").CountAsync(),

                TotalPCECaseCount = TotalPCEs.Select(res => res.ProductionCapacity.PCECaseId).Distinct().Count(),
                TotalPCEsCount = await _cbeContext.ProductionCaseAssignments.Where(res => res.UserId == userId).CountAsync(),
            };
        }

        public async Task<PCEEvaluationReturnDto> GetPCEEvaluation(Guid UserId, Guid Id)
        {
            try
            {
                var pceEntity = await _cbeContext.PCEEvaluations
                    .Include(e => e.ShiftHours)
                    .Include(e => e.TimeConsumedToCheck)
                    .Include(e => e.PCE)
                    // .Include(pe => pe.UploadFiles)
                    .FirstOrDefaultAsync(e => e.Id == Id);
                
                if (pceEntity == null)
                {
                    _logger.LogWarning("PCEEvaluation with id {Id} not found", Id);
                    throw new KeyNotFoundException("PCEEvaluation not found");
                }

                var uploadFiles = await _cbeContext.UploadFiles
                    .Where(uf => uf.CollateralId == pceEntity.Id)
                    .ToListAsync();

                // var supportingEvidences = pceEntity.UploadFiles
                var supportingEvidences = uploadFiles
                    .Where(uf => uf.Catagory == "Supporting Evidence")
                    .ToList();
                var productionProcessFlowDiagrams = uploadFiles
                    .Where(uf => uf.Catagory == "Production Process Flow Diagram")
                    .ToList();

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
                    .Include(e => e.ShiftHours)
                    .Include(e => e.TimeConsumedToCheck)
                    .Include(e => e.PCE)
                    .FirstOrDefaultAsync(e => e.PCEId == PCEId);

                if (pceEntity == null)
                {
                    return _mapper.Map<PCEEvaluationReturnDto>(pceEntity);
                }
                var uploadFiles = await _cbeContext.UploadFiles
                    .Where(uf => uf.CollateralId == pceEntity.Id)
                    .ToListAsync();

                // var supportingEvidences = pceEntity.UploadFiles
                var supportingEvidences = uploadFiles
                    .Where(uf => uf.Catagory == "Supporting Evidence")
                    .ToList();
                var productionProcessFlowDiagrams = uploadFiles
                    .Where(uf => uf.Catagory == "Production Process Flow Diagram")
                    .ToList();

                var pceEntityDto = _mapper.Map<PCEEvaluationReturnDto>(pceEntity);
                pceEntityDto.SupportingEvidences = _mapper.Map<ICollection<ReturnFileDto>>(supportingEvidences);
                pceEntityDto.ProductionProcessFlowDiagrams = _mapper.Map<ICollection<ReturnFileDto>>(productionProcessFlowDiagrams);

                return pceEntityDto;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching PCEEvaluation with PCEID {PCEId}");
                throw new ApplicationException("An error occurred while fetching the PCEEvaluation with PCEID {PCEId}.");
            }
        }
        
        public async Task<PCECaseReturntDto> GetPCECase(Guid UserId, Guid Id)
        {
            var pCECase = await _cbeContext.PCECases
                           .Include(res => res.BussinessLicence)
                           .Include(res => res.District)
                           .Include(res => res.ProductionCapacities)
                           .FirstOrDefaultAsync(c => c.Id == Id);
            return _mapper.Map<PCECaseReturntDto>(pCECase);
        }

        public async Task<IEnumerable<PCECaseReturntDto>> GetPCECasesWithStatus(Guid UserId, string status)
        {
            var PCECaseAssignments = await _cbeContext.ProductionCaseAssignments
                                        .Include(res => res.ProductionCapacity)
                                        .ThenInclude(res => res.PCECase)
                                        .Where(Ca => Ca.UserId == UserId && Ca.Status==status)
                                        .ToListAsync();

            var UniquePCECases = PCECaseAssignments.Select(ca => ca.ProductionCapacity.PCECase).DistinctBy(c => c.Id).ToList();
            var ReturnDtos = _mapper.Map<IEnumerable<PCECaseReturntDto>>(UniquePCECases);
            foreach (var ReturnDto in ReturnDtos)
            {
                ReturnDto.TotalNoOfCollateral = await _cbeContext.ProductionCapacities.CountAsync(res => res.PCECaseId == ReturnDto.Id);
            }
            return ReturnDtos;
        }

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

        public async Task<IEnumerable<ReturnProductionDto>> GetProductionCapacitiesWithStatusAndRole(Guid PCECaseId, string status, string role)
        {
            var productions = await _cbeContext.ProductionCapacities.Where(res => res.PCECaseId == PCECaseId && (res.CurrentStatus == status && res.CurrentStage == role)).ToListAsync();
            return _mapper.Map<IEnumerable<ReturnProductionDto>>(productions);
        }

        // public async Task<IEnumerable<ReturnProductionDto>> RejectedPCEs(Guid UserId)
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

        // public async Task<IEnumerable<ReturnProductionDto>> ResubmittedPCEs(Guid UserId)
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