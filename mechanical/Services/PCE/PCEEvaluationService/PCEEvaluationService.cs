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

                pce.CurrentStage = "Maker Officer";
                pce.CurrentStatus = "Pending";
                _cbeContext.ProductionCapacities.Update(pce);

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
        
        public async Task<PCEEvaluationReturnDto> UpdatePCEEvaluation(Guid UserId, PCEEvaluationUpdateDto Dto)
        {
            using var transaction = await _cbeContext.Database.BeginTransactionAsync();
            try
            {                
                var pceEntity = await _cbeContext.PCEEvaluations.Include(e => e.ShiftHours).FirstOrDefaultAsync(e => e.Id == Dto.Id);

                if (pceEntity == null)
                {
                    _logger.LogWarning("PCEEvaluation with id {Id} not found", Dto.Id);
                    throw new KeyNotFoundException("PCEEvaluation not found");
                }

                // Extract IDs from DTO and use HashSet for faster lookup
                var dtoShiftIds = new HashSet<Guid>(Dto.ShiftHours.Select(sh => sh.Id));

                // Remove shift hours not present in the DTO
                var shiftHoursToRemove = pceEntity.ShiftHours.Where(sh => !dtoShiftIds.Contains(sh.Id)).ToList();
                foreach (var shiftHour in shiftHoursToRemove)
                {
                    _cbeContext.TimeIntervals.Remove(shiftHour); // Remove from the database context
                    pceEntity.ShiftHours.Remove(shiftHour); // Remove from the in-memory list
                }

                // Update existing shift hours and add new ones
                // pceEntity.ShiftHours.Clear(); 
                foreach (var shiftHourDto in Dto.ShiftHours)
                {
                    var existingShiftHour = pceEntity.ShiftHours.FirstOrDefault(sh => sh.Id == shiftHourDto.Id);

                    if (existingShiftHour != null)
                    {
                        // Update existing shift hour
                        _mapper.Map(shiftHourDto, existingShiftHour);
                    }
                    else
                    {
                        // Add new shift hour
                        var newShiftHour = _mapper.Map<TimeInterval>(shiftHourDto);
                        pceEntity.ShiftHours.Add(newShiftHour);
                    }
                }

                // pceEntity.ShiftHours.Clear(); 
                _mapper.Map(Dto, pceEntity); 

                pceEntity.UpdatedBy = UserId;
                pceEntity.UpdatedAt = DateTime.Now;

                // Handle deleted files
                // if (Dto.DeletedFileIds != null && Dto.DeletedFileIds.Count > 0)
                if (!string.IsNullOrEmpty(Dto.DeletedFileIds))
                {
                    var deletedFileGuids = Dto.DeletedFileIds.Split(',').Select(id => Guid.Parse(id)).ToList();
 
                    var filesToDelete = await _cbeContext.UploadFiles
                        .Where(file => deletedFileGuids.Contains(file.Id))
                        // .Where(file => Dto.DeletedFileIds.Contains(file.Id))
                        .ToListAsync();

                    _cbeContext.UploadFiles.RemoveRange(filesToDelete);
                }

                var pce = await _cbeContext.ProductionCapacities.FindAsync(pceEntity.PCEId);

                // Handle new file uploads
                if (Dto.NewSupportingEvidences != null && Dto.NewSupportingEvidences.Count > 0)
                {
                    foreach (var file in Dto.NewSupportingEvidences)
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

                if (Dto.NewProductionProcessFlowDiagrams != null && Dto.NewProductionProcessFlowDiagrams.Count > 0)
                {
                    foreach (var file in Dto.NewProductionProcessFlowDiagrams)
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
                    .Where(file => file.CollateralId == pceEntity.Id)
                    .ToListAsync();

                // Delete physical files
                foreach (var file in relatedFiles)
                {
                    if (File.Exists(file.Path))
                    {
                        File.Delete(file.Path);
                    }
                }

                // Remove Evaluations and related files from database
                _cbeContext.UploadFiles.RemoveRange(relatedFiles);
                _cbeContext.PCEEvaluations.Remove(pceEntity);

                var pce = await _cbeContext.ProductionCapacities.FindAsync(pceEntity.PCEId);
                pce.CurrentStage = "Maker Officer";
                pce.CurrentStatus = "New";
                _cbeContext.ProductionCapacities.Update(pce);

                var previousCaseAssignment = await _cbeContext.ProductionCaseAssignments.Where(res => res.ProductionCapacityId == pce.Id && res.UserId == pceEntity.EvaluatorId).FirstOrDefaultAsync();
                previousCaseAssignment.Status = "New";
                _cbeContext.ProductionCaseAssignments.Update(previousCaseAssignment);

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

                var pce = await _cbeContext.ProductionCapacities.FindAsync(pceEntity.PCEId);
                pce.CurrentStage = "Relational Manager";
                pce.CurrentStatus = "Evaluated";
                _cbeContext.ProductionCapacities.Update(pce);

                // var previousCaseAssignment = await _cbeContext.ProductionCaseAssignments.Where(res => res.ProductionCapacityId == pceEntity.PCEId && res.UserId == pceEntity.PCE.PCECase.RMUserId).FirstOrDefaultAsync();
                var previousCaseAssignment = await _cbeContext.ProductionCaseAssignments.Where(res => res.ProductionCapacityId == pceEntity.PCEId).FirstOrDefaultAsync();
                previousCaseAssignment.Status = "Evaluated";
                _cbeContext.ProductionCaseAssignments.Update(previousCaseAssignment);

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
              
                var pce = await _cbeContext.ProductionCapacities.FindAsync(Dto.PCEId);
                pce.CurrentStage = "Relational Manager";
                pce.CurrentStatus = "Rejected";
                _cbeContext.ProductionCapacities.Update(pce);

                var previousPCECaseAssignment = await _cbeContext.ProductionCaseAssignments.FirstOrDefaultAsync(res => res.ProductionCapacityId == Dto.PCEId && res.UserId == UserId);
                previousPCECaseAssignment.Status = "Rejected";
                _cbeContext.Update(previousPCECaseAssignment);   

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

        // public async Task<bool> ReworkPCEEvaluation(Guid UserId, Guid Id)
        // // public async Task<PCEEvaluationReturnDto> ReworkPCEEvaluation(Guid UserId, Guid Id, PCEEvaluationPostDto Dto)
        // {
        //     using var transaction = await _cbeContext.Database.BeginTransactionAsync();
        //     try
        //     {
        //         var pceEntity = await _cbeContext.PCEEvaluations.FindAsync(Id);
        //         if (pceEntity == null)
        //         {
        //             _logger.LogWarning("PCEEvaluation with id {Id} not found", Id);
        //             throw new KeyNotFoundException("PCEEvaluation not found");
        //         }

        //         _cbeContext.PCEEvaluations.Update(pceEntity);

        //         var pce = await _cbeContext.ProductionCapacities.FindAsync(pceEntity.PCEId);
        //         pce.CurrentStage = "Maker Officer";
        //         pce.CurrentStatus = "Rework";
        //         _cbeContext.ProductionCapacities.Update(pce);
                
        //         var previousCaseAssignment = await _cbeContext.ProductionCaseAssignments.Where(res => res.ProductionCapacityId == pceEntity.PCEId && res.UserId == pceEntity.EvaluatorId).FirstOrDefaultAsync();
        //         previousCaseAssignment.Status = "Rework";
        //         _cbeContext.ProductionCaseAssignments.Update(previousCaseAssignment);

        //         // await _pceCaseTimeLineService.CreateCaseTimeLine(new PCECaseTimeLinePostDto
        //         await _pceCaseTimeLineService.PCECaseTimeLine(new PCECaseTimeLinePostDto
        //         {
        //             CaseId = pce.PCECaseId,
        //             Activity = $"<strong> PCE Case Evaluation returned to Maker Officer for rework.</strong>",
        //             CurrentStage = "Maker Manager"
        //         });
        //         await _pceCaseTimeLineService.PCECaseTimeLine(new PCECaseTimeLinePostDto
        //         {
        //             CaseId = pce.PCECaseId,
        //             Activity = $"<strong>Evaluated PCE Case has been returned for rework.</strong>",
        //             CurrentStage = "Maker Officer",
        //             UserId = pceEntity.EvaluatorId
        //         });
                
        //         await _cbeContext.SaveChangesAsync(); 
        //         await transaction.CommitAsync();           
            
        //         return true;
     
        //     }
        //     catch (Exception ex)
        //     {
        //         _logger.LogError(ex, "Error reworking PCEEvaluation");
        //         await transaction.RollbackAsync();
        //         throw new ApplicationException("An error occurred while reworking the PCEEvaluation.");
        //     }
        // }

        ///////// PCE Evaluation //////////////
        public async Task<PCEEvaluationReturnDto> GetPCEEvaluation(Guid UserId, Guid Id)
        {
            try
            {
                var pceEntity = await _cbeContext.PCEEvaluations
                                        .Include(e => e.ShiftHours)
                                        .Include(e => e.TimeConsumedToCheck)
                                        .Include(e => e.PCE)
                                        .ThenInclude(e => e.PCECase)
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
                                        .ThenInclude(e => e.PCECase)
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
        
        ///////// PCE Case //////////////
        public async Task<PCECaseReturntDto> GetPCECase(Guid UserId, Guid Id)
        {
            var pCECase = await _cbeContext.PCECases
                           .Include(res => res.BussinessLicence)
                           .Include(res => res.District)
                           .Include(res => res.ProductionCapacities)
                           .FirstOrDefaultAsync(c => c.Id == Id);
            return _mapper.Map<PCECaseReturntDto>(pCECase);
        }

        public async Task<IEnumerable<PCENewCaseDto>> GetPCECases(Guid UserId, string Status)
        {
            var PCECaseAssignmentsQuery = _cbeContext.ProductionCaseAssignments
                                                .Include(res => res.ProductionCapacity)
                                                .ThenInclude(res => res.PCECase)
                                                .Where(Ca => Ca.UserId == UserId);

            if (!string.IsNullOrEmpty(Status) && !Status.Equals("All", StringComparison.OrdinalIgnoreCase))
            {
                PCECaseAssignmentsQuery = PCECaseAssignmentsQuery.Where(Ca => Ca.Status == Status);
            }

            var PCECaseAssignments = await PCECaseAssignmentsQuery.ToListAsync();
            var UniquePCECases = PCECaseAssignments.Select(ca => ca.ProductionCapacity.PCECase).DistinctBy(c => c.Id).ToList();
            var ReturnDtos = _mapper.Map<IEnumerable<PCENewCaseDto>>(UniquePCECases);
           
            var pceCaseIds = UniquePCECases.Select(pc => pc.Id).ToList();
            var productionCapacities = await _cbeContext.ProductionCapacities.Where(pc => pceCaseIds.Contains(pc.PCECaseId)).ToListAsync();

            foreach (var returnDto in ReturnDtos)
            {
                // returnDto.NoOfCollateral = productionCapacities.Count(pc => pc.PCECaseId == returnDto.Id && pc.CurrentStatus == "New");
                returnDto.NoOfCollateral = productionCapacities.Count(pc => pc.PCECaseId == returnDto.Id && pc.CurrentStatus == Status);
                returnDto.TotalNoOfCollateral = productionCapacities.Count(pc => pc.PCECaseId == returnDto.Id);
            }

            return ReturnDtos;
        }

        public async Task<PCECasesCountDto> GetDashboardPCECaseCount(Guid UserId)
        {
            var NewPCEs = await _cbeContext.ProductionCaseAssignments.Include(res => res.ProductionCapacity).Where(res => res.UserId == UserId && res.Status == "New").ToListAsync();
            var PendingPCEs = await _cbeContext.ProductionCaseAssignments.Include(res => res.ProductionCapacity).Where(res => res.UserId == UserId && res.Status == "Pending").ToListAsync();
            var EvaluatedPCEs = await _cbeContext.ProductionCaseAssignments.Include(res => res.ProductionCapacity).Where(res => res.UserId == UserId && res.Status == "Evaluated").ToListAsync();
            var ReturnedPCEs = await _cbeContext.ProductionCaseAssignments.Include(res => res.ProductionCapacity).Where(res => res.UserId == UserId && res.Status == "Returned").ToListAsync();
            var ReevaluatedPCEs = await _cbeContext.ProductionCaseAssignments.Include(res => res.ProductionCapacity).Where(res => res.UserId == UserId && res.Status == "Reevaluated").ToListAsync();
            var RejectedPCEs = await _cbeContext.ProductionCaseAssignments.Include(res => res.ProductionCapacity).Where(res => res.UserId == UserId && res.Status == "Rejected").ToListAsync();
            var TotalPCEs = await _cbeContext.ProductionCaseAssignments.Include(res => res.ProductionCapacity).Where(res => res.UserId == UserId ).ToListAsync();

            return new PCECasesCountDto()
            {
                NewPCECasesCount = NewPCEs.Select(res => res.ProductionCapacity.PCECaseId).Distinct().Count(),
                NewPCEsCount = await _cbeContext.ProductionCaseAssignments.Where(res => res.UserId == UserId && res.Status == "New").CountAsync(),

                PendingPCECasesCount = PendingPCEs.Select(res => res.ProductionCapacity.PCECaseId).Distinct().Count(),
                PendingPCEsCount = await _cbeContext.ProductionCaseAssignments.Where(res => res.UserId == UserId && res.Status == "Pending").CountAsync(),

                EvaluatedPCECasesCount = EvaluatedPCEs.Select(res => res.ProductionCapacity.PCECaseId).Distinct().Count(),
                EvaluatedPCEsCount = await _cbeContext.ProductionCaseAssignments.Where(res => res.UserId == UserId && res.Status == "Evaluated").CountAsync(),

                ReturnedPCECasesCount = ReturnedPCEs.Select(res => res.ProductionCapacity.PCECaseId).Distinct().Count(),
                ReturnedPCEsCount = await _cbeContext.ProductionCaseAssignments.Where(res => res.UserId == UserId && res.Status == "Returned").CountAsync(),

                ReevaluatedPCECasesCount = ReevaluatedPCEs.Select(res => res.ProductionCapacity.PCECaseId).Distinct().Count(),
                ReevaluatedPCEsCount = await _cbeContext.ProductionCaseAssignments.Where(res => res.UserId == UserId && res.Status == "Reevaluated").CountAsync(),

                RejectedPCECasesCount = RejectedPCEs.Select(res => res.ProductionCapacity.PCECaseId).Distinct().Count(),
                RejectedPCEsCount = await _cbeContext.ProductionCaseAssignments.Where(res => res.UserId == UserId && res.Status == "Rejected").CountAsync(),

                TotalPCECasesCount = TotalPCEs.Select(res => res.ProductionCapacity.PCECaseId).Distinct().Count(),
                TotalPCEsCount = await _cbeContext.ProductionCaseAssignments.Where(res => res.UserId == UserId).CountAsync(),
            };
        }

        //////////////////////////////// PCE /////////////////////////////////////////

        public async Task<IEnumerable<ReturnProductionDto>> GetPCEs(Guid UserId, Guid? PCECaseId = null, string Stage = null, string Status = null)
        {
            var query = _cbeContext.ProductionCapacities.Join(_cbeContext.ProductionCaseAssignments,
                                                            pc => pc.Id,
                                                            pca => pca.ProductionCapacityId,
                                                            (pc, pca) => new { ProductionCapacity = pc, ProductionCaseAssignment = pca })
                                                        .Where(x => x.ProductionCaseAssignment.UserId == UserId
                                                                || x.ProductionCapacity.EvaluatorUserID == UserId    
                                                                // && x.ProductionCapacity.CurrentStage == Stage
                                                                );     
        
            if (PCECaseId.HasValue)
            {
                query = query.Where(x => x.ProductionCapacity.PCECaseId == PCECaseId.Value);
            }

            if (!string.IsNullOrEmpty(Status) && !Status.Equals("All", StringComparison.OrdinalIgnoreCase))
            {
                query = query.Where(x => x.ProductionCapacity.CurrentStatus == Status);
            }

            if (!string.IsNullOrEmpty(Stage))
            {
                query = query.Where(x => x.ProductionCapacity.CurrentStage == Stage);
            }

            var productions = await query.Select(x => x.ProductionCapacity).ToListAsync();

            return _mapper.Map<IEnumerable<ReturnProductionDto>>(productions);
        }
       
        public async Task<int> GetPCEsCountAsync(Guid UserId, Guid? PCECaseId = null, string Stage = null, string Status = null)
        {
            var query = _cbeContext.ProductionCapacities.Join(_cbeContext.ProductionCaseAssignments,
                                                            pc => pc.Id,
                                                            pca => pca.ProductionCapacityId,
                                                            (pc, pca) => new { ProductionCapacity = pc, ProductionCaseAssignment = pca })
                                                        .Where(x => x.ProductionCaseAssignment.UserId == UserId
                                                                 || x.ProductionCapacity.EvaluatorUserID == UserId
                                                                //  && x.ProductionCapacity.CurrentStage == Stage
                                                                );

            if (PCECaseId.HasValue)
            {
                query = query.Where(x => x.ProductionCapacity.PCECaseId == PCECaseId.Value);
            }

            if (!string.IsNullOrEmpty(Status) && !Status.Equals("All", StringComparison.OrdinalIgnoreCase))
            {
                query = query.Where(x => x.ProductionCapacity.CurrentStatus == Status);
            }
            
            if (!string.IsNullOrEmpty(Stage))
            {
                query = query.Where(x => x.ProductionCapacity.CurrentStage == Stage);
            }

            return await query.CountAsync();
        }        

        public async Task<int> GetPCEsCount(Guid UserId, Guid? PCECaseId, string Stage = null, string Status = null)
        {
            return (await GetPCEs(UserId, PCECaseId, Stage, Status)).Count();
        }

        public async Task<PCEsCountDto> GetDashboardPCECount(Guid UserId, Guid? PCECaseId = null, string Stage = null)
        {        
            var NewPCEsCount = await GetPCEsCountAsync(UserId, PCECaseId, Stage, "New");
            var PendingPCEsCount = await GetPCEsCountAsync(UserId, PCECaseId, Stage, "Pending");
            var EvaluatedPCEsCount = await GetPCEsCountAsync(UserId, PCECaseId, Stage, "Evaluated");
            var ReturnedPCEsCount = await GetPCEsCountAsync(UserId, PCECaseId, Stage, "Returned");
            var ReevaluatedPCEsCount = await GetPCEsCountAsync(UserId, PCECaseId, Stage, "Reevaluated");
            var RejectedPCEsCount = await GetPCEsCountAsync(UserId, PCECaseId, Stage, "Rejected");
            var TotalPCEsCount = await GetPCEsCountAsync(UserId, PCECaseId, Stage);

            return new PCEsCountDto()
            {
                NewPCEsCount = NewPCEsCount,
                PendingPCEsCount = PendingPCEsCount,
                EvaluatedPCEsCount = EvaluatedPCEsCount,
                ReturnedPCEsCount = ReturnedPCEsCount,
                ReevaluatedPCEsCount = ReevaluatedPCEsCount,
                RejectedPCEsCount = RejectedPCEsCount,
                TotalPCEsCount = TotalPCEsCount
            };
        }    
        public async Task<IEnumerable<ReturnProductionDto>> GetRejectedPCEs(Guid UserId)
        {
            var rejectedCapacitiesQuery = from reject in _cbeContext.ProductionRejects
                                        join capacity in _cbeContext.ProductionCapacities
                                        on reject.PCEId equals capacity.Id
                                        where reject.RejectedBy == UserId
                                        select capacity;

            var rejectedCapacities = await rejectedCapacitiesQuery.ToListAsync();


            return _mapper.Map<IEnumerable<ReturnProductionDto>>(rejectedCapacities);;
        }
    }
}