﻿using AutoMapper;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;

using mechanical.Data;
using mechanical.Models;
using mechanical.Models.Entities;

using mechanical.Models.PCE.Entities;
using mechanical.Models.PCE.Dto.PCECaseTimeLineDto;
using mechanical.Models.PCE.Dto.PCECaseAssignmentDto;
using mechanical.Services.PCE.PCECaseTimeLineService;
using DocumentFormat.OpenXml.Spreadsheet;

namespace mechanical.Services.PCE.PCECaseAssignmentService
{
    public class PCECaseAssignmentService : IPCECaseAssignmentService
    {
        private readonly CbeContext _cbeContext;
        private readonly IMapper _mapper;
        private readonly ILogger<PCECaseAssignmentService> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IPCECaseTimeLineService _IPCECaseTimeLineService;

        public PCECaseAssignmentService(CbeContext cbeContext, IMapper mapper, ILogger<PCECaseAssignmentService> logger, IHttpContextAccessor httpContextAccessor, IPCECaseTimeLineService IPCECaseTimeLineService)
        {
            _cbeContext = cbeContext;
            _mapper = mapper;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
            _IPCECaseTimeLineService = IPCECaseTimeLineService;

        }

        public Task<List<PCECaseAssignmentDto>> AssignProduction(Guid UserId, string SelectedPCEIds, string EmployeeId) =>
            AssignOrReAssignProduction(UserId, SelectedPCEIds, EmployeeId, false);

        public Task<List<PCECaseAssignmentDto>> ReAssignProduction(Guid UserId, string SelectedPCEIds, string EmployeeId) =>
            AssignOrReAssignProduction(UserId, SelectedPCEIds, EmployeeId, true);

        public async Task<List<PCECaseAssignmentDto>> AssignOrReAssignProduction(Guid UserId, string SelectedPCEIds, string employeeId, bool isReassign)
        {
            return await ProcessProductionAssignments(UserId, SelectedPCEIds, employeeId, isReassign, "Assign");
        }

        public async Task<List<PCECaseAssignmentDto>> SendForReestimation(Guid UserId, string ReestimationReason, string SelectedPCEIds, string centerId)
        {
            return await ProcessProductionAssignments(UserId, SelectedPCEIds, centerId, false, "Reestimation", ReestimationReason);
        }

        public async Task<List<PCECaseAssignmentDto>> SendForValuation(Guid UserId, string SelectedPCEIds, string centerId)
        {
            return await ProcessProductionAssignments(UserId, SelectedPCEIds, centerId, false, "Valuation");
        }

        private async Task<List<PCECaseAssignmentDto>> ProcessProductionAssignments(
                Guid UserId, string SelectedPCEIds, string EmployeeOrCenterId, bool isReassign, string OperationType, string ReestimationReason = null
        )
        {
            using var transaction = await _cbeContext.Database.BeginTransactionAsync();
            try
            {
                if (string.IsNullOrEmpty(SelectedPCEIds))
                {
                    if (OperationType == "Assign"){
                        throw new Exception("Please, select at least one production to assign.");
                    }
                    throw new Exception("Please, select at least one production to send for estimation.");
                }

                if (string.IsNullOrEmpty(EmployeeOrCenterId))
                {
                    if (OperationType == "Assign"){
                        throw new Exception("Please, select a user to assign.");
                    }
                    throw new Exception("Please, select an evaluation center.");
                }
                
                var assignedUser = await GetAssignedUser(EmployeeOrCenterId, OperationType);
                if (assignedUser == null)
                {
                    if (OperationType == "Assign"){
                        throw new Exception("The assigned user is not found.");
                    }                    
                    throw new Exception("The evaluation center is not ready.");
                }

                PCECaseTimeLinePostDto pceCaseTimeLineDto = null;
                List<PCECaseAssignmentDto> pceCaseAssignments = new List<PCECaseAssignmentDto>();
                List<Guid> PCEIdList = SelectedPCEIds.Split(',').Select(Guid.Parse).ToList();

                foreach (var PCEId in PCEIdList)
                {
                    var production = await GetProductionById(PCEId);
                    if (production == null) continue;
                    
                    await UpdateProduction(production, assignedUser, OperationType);
                    await AssignOrUpdateCase(UserId, PCEId, assignedUser, pceCaseAssignments, ReestimationReason, OperationType);
                    
                    if (pceCaseTimeLineDto == null)
                    {
                        pceCaseTimeLineDto = CreateTimelineDto(production.PCECaseId, assignedUser, isReassign, OperationType);
                    }
                    UpdateTimelineDto(production, pceCaseTimeLineDto);
                    await UpdatePreviousAssignments(UserId, PCEId);
                }
                if (pceCaseTimeLineDto != null)
                {
                    await _IPCECaseTimeLineService.PCECaseTimeLine(pceCaseTimeLineDto);
                }

                await _cbeContext.SaveChangesAsync();
                await transaction.CommitAsync();

                return pceCaseAssignments;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error during {OperationType} operation");
                await transaction.RollbackAsync();
                throw new ApplicationException(ex.Message);
            }
        }

        private async Task<CreateUser> GetAssignedUser(string id, string OperationType)
        {
            if (OperationType == "Assign")
            {
                return await _cbeContext.CreateUsers
                                        .Include(res => res.Role)
                                        .Include(res => res.District)
                                        .FirstOrDefaultAsync(res => res.Id == Guid.Parse(id));
            }
            else
            {
                return await _cbeContext.CreateUsers
                                        .Include(res => res.Role)
                                        .Include(res => res.District)
                                        .FirstOrDefaultAsync(res => res.DistrictId == Guid.Parse(id) &&
                                                            // res.Department == "Mechanical" && 
                                                            (res.Role.Name == "Maker Manager" || res.Role.Name == "District Valuation Manager"));
            }
        }

        private async Task<ProductionCapacity> GetProductionById(Guid PCEId)
        {
            return await _cbeContext.ProductionCapacities.Include(res => res.PCECase).FirstOrDefaultAsync(pc => pc.Id == PCEId);//.FindAsync(PCEId);
                
        }

        private async Task AssignOrUpdateCase(Guid UserId, Guid PCEId, CreateUser assignedUser, List<PCECaseAssignmentDto> caseAssignments, string ReestimationReason, string OperationType)
        {
            var existingAssignment = await _cbeContext.PCECaseAssignments.FirstOrDefaultAsync(res => res.ProductionCapacityId == PCEId && res.UserId == assignedUser.Id);

            if (existingAssignment != null)
            {
                existingAssignment.Status = "New";
                _cbeContext.PCECaseAssignments.Update(existingAssignment);
            }
            else
            {
                var newAssignment = new PCECaseAssignment
                {
                    ProductionCapacityId = PCEId,
                    UserId = assignedUser.Id,
                    Status = "New",
                    AssignmentDate = DateTime.Now
                };
                await _cbeContext.PCECaseAssignments.AddAsync(newAssignment);
                caseAssignments.Add(_mapper.Map<PCECaseAssignmentDto>(newAssignment));
            }

            if (OperationType == "Reestimation")
            {
                var reEstimation = new ProductionCapacityReestimation
                {
                    ProductionCapacityId = PCEId,
                    Reason = ReestimationReason,
                    CreatedAt = DateTime.Now
                };
                await _cbeContext.ProductionCapacityReestimations.AddAsync(reEstimation);
            }
        }

        private async Task UpdatePreviousAssignments(Guid UserId, Guid productionId)
        {
            var previousAssignment = await _cbeContext.PCECaseAssignments.FirstOrDefaultAsync(res => res.ProductionCapacityId == productionId && res.UserId == UserId);

            if (previousAssignment != null)
            {
                previousAssignment.Status = "Pending";
                _cbeContext.PCECaseAssignments.Update(previousAssignment);
            }
        }

        private async Task UpdateProduction(ProductionCapacity production, CreateUser assignedUser, string OperationType)
        {
            production.CurrentStage = assignedUser.Role.Name;
            production.CurrentStatus = "New";
            if (OperationType == "Reestimation" || OperationType == "Valuation")
            {
                production.PCECase.Status = "Pending";
                if (OperationType == "Reestimation")
                {
                    production.CurrentStatus = "Reestimate";
                }
                else
                {
                    production.CurrentStatus = "New";
                }
            }
            _cbeContext.ProductionCapacities.Update(production);
        }

        private PCECaseTimeLinePostDto CreateTimelineDto(Guid PCECaseId, CreateUser assignedUser, bool isReassign, string OperationType)
        {
            string activity = $"<strong>Production has been {(isReassign ? "re-assigned" : "assigned")} to {assignedUser.Name} ({assignedUser.Role.Name}).</strong><br>";
            if (OperationType == "Reestimation" || OperationType == "Valuation")
            {
                activity = $"<strong>Production assigned for {OperationType} to {assignedUser.Name} ({assignedUser.Role.Name}).</strong><br>";
            }

            return new PCECaseTimeLinePostDto
            {
                CaseId = PCECaseId,
                Activity = activity,
                CurrentStage = assignedUser.Role.Name
            };
        }

        private void UpdateTimelineDto(ProductionCapacity production, PCECaseTimeLinePostDto pceCaseTimeLineDto)
        {
            pceCaseTimeLineDto.Activity += $"<i class='text-purple'>Property Owner:</i> {production.PropertyOwner}. " +
                                    $"<i class='text-purple'>Role:</i> {production.Role}. " +
                                    $"<i class='text-purple'>Production Category:</i> {production.Category}. " +
                                    $"<i class='text-purple'>Production Type:</i> {production.Type}.<br>";
        }

        
        // public Task<List<PCECaseAssignmentDto>> AssignProductionMakerTeamleader(Guid UserId, string SelectedPCEIds, string EmployeeId) =>
        //     AssignOrReAssignProduction(UserId, SelectedPCEIds, EmployeeId, false);

        // public Task<List<PCECaseAssignmentDto>> ReAssignProductionMakerTeamleader(Guid UserId, string SelectedPCEIds, string EmployeeId) =>
        //     AssignOrReAssignProduction(UserId, SelectedPCEIds, EmployeeId, true);

        // public Task<List<PCECaseAssignmentDto>> AssignProductionMakerOfficer(Guid UserId, string SelectedPCEIds, string EmployeeId) =>
        //     AssignOrReAssignProduction(UserId, SelectedPCEIds, EmployeeId, false);

        // public Task<List<PCECaseAssignmentDto>> ReAssignProductionMakerOfficer(Guid UserId, string SelectedPCEIds, string EmployeeId) =>
        //     AssignOrReAssignProduction(UserId, SelectedPCEIds, EmployeeId, true);

        // public async Task<List<PCECaseAssignmentDto>> AssignOrReAssignProduction(Guid UserId, string SelectedPCEIds, string EmployeeId, bool isReassign)
        // {
        //     using var transaction = await _cbeContext.Database.BeginTransactionAsync();
        //     try
        //     {    
        //         var assignedUser = await _cbeContext.CreateUsers.Include(res => res.Role).FirstOrDefaultAsync(res => res.Id == Guid.Parse(EmployeeId));
                
        //         if (assignedUser == null)
        //         {
        //             throw new Exception("Sorry the assigned user is not found.");
        //         }
        //         List<PCECaseAssignmentDto> pceCaseAssignments = new List<PCECaseAssignmentDto>();
        //         List<Guid> PCEIdList = SelectedPCEIds.Split(',').Select(x => Guid.Parse(x.Trim())).ToList();
        //         PCECaseTimeLinePostDto PCECaseTimeLinePostDto = null;

        //         foreach (Guid PCEId in PCEIdList)
        //         {
        //             var production = await _cbeContext.ProductionCapacities.FindAsync(PCEId);
        //             if (production == null) continue;

        //             production.CurrentStage = assignedUser.Role.Name;
        //             production.CurrentStatus = "New";

        //             // Check and update the status of the existing case assignment in case od reestimation
        //             var existingCaseAssignment = await _cbeContext.PCECaseAssignments
        //                                                         .Where(res => res.ProductionCapacityId == PCEId && res.UserId == assignedUser.Id)
        //                                                         .FirstOrDefaultAsync();

        //             if (existingCaseAssignment != null)
        //             {
        //                 existingCaseAssignment.Status = "New";
        //                 _cbeContext.PCECaseAssignments.Update(existingCaseAssignment);
        //             }
        //             else
        //             {
        //                 var pceCaseAssignment = new PCECaseAssignment
        //                 {
        //                     ProductionCapacityId = PCEId,
        //                     UserId = assignedUser.Id,
        //                     Status = "New",
        //                     AssignmentDate = DateTime.Now
        //                 };
        //                 await _cbeContext.PCECaseAssignments.AddAsync(pceCaseAssignment);
        //                 pceCaseAssignments.Add(_mapper.Map<PCECaseAssignmentDto>(pceCaseAssignment));
        //             }

        //             // Update production
        //             _cbeContext.ProductionCapacities.Update(production);

        //             // Prepare timeline activity
        //             if (PCECaseTimeLinePostDto == null)
        //             {
        //                 PCECaseTimeLinePostDto = new PCECaseTimeLinePostDto
        //                 {
        //                     CaseId = production.PCECaseId,
        //                     Activity = $" <strong>A production has been {(isReassign ? "re-assigned" : "assigned")} for {assignedUser.Name} {assignedUser.Role.Name}. </strong> <br>",
        //                     CurrentStage = assignedUser.Role.Name
        //                 };
        //             }
        //             PCECaseTimeLinePostDto.Activity += $"<i class='text-purple'>Property Owner:</i> {production.PropertyOwner}. &nbsp; " +
        //                 $"<i class='text-purple'>Role:</i> {production.Role}.&nbsp; " +
        //                 $"<i class='text-purple'>Production Category:</i> {production.Category}. &nbsp; " +
        //                 $"<i class='text-purple'>Production Type:</i> {production.Type}. <br>";

        //             // Update status of the previous case assignment
        //             var previousCaseAssignment = await _cbeContext.PCECaseAssignments
        //                                                         .Where(res => res.UserId == UserId && res.ProductionCapacityId == production.Id)
        //                                                         .FirstOrDefaultAsync();
        //             if (previousCaseAssignment != null)
        //             {
        //                 previousCaseAssignment.Status = "Pending";
        //                 _cbeContext.PCECaseAssignments.Update(previousCaseAssignment);
        //             }
        //         }

        //         if (PCECaseTimeLinePostDto != null)
        //         {
        //             await _IPCECaseTimeLineService.PCECaseTimeLine(PCECaseTimeLinePostDto);
        //         }
                
        //         await _cbeContext.SaveChangesAsync();
        //         await transaction.CommitAsync();

        //         return pceCaseAssignments;                            
        //     }

        //     catch (Exception ex)
        //     {
        //         _logger.LogError(ex, "Error assigning production");
        //         await transaction.RollbackAsync();
        //         throw new ApplicationException("An error occurred while assigning production.");
        //     }
        // }

        // public async Task<List<PCECaseAssignmentDto>> SendForReestimation(string ReestimationReason, string SelectedPCEIds, string CenterId)
        // {
        //     using var transaction = await _cbeContext.Database.BeginTransactionAsync();
        //     try
        //     {   
        //         var assignedUser = await _cbeContext.CreateUsers.Include(res => res.District).Include(res => res.Role).FirstOrDefaultAsync(res => res.DistrictId == Guid.Parse(CenterId) && (res.Role.Name == "Maker Manager" || res.Role.Name == "District Valuation Manager"));
        //         if (assignedUser == null)
        //         {
        //             throw new Exception("Sorry the evaluation center is not ready.");
        //         }
        //         List<PCECaseAssignmentDto> pceCaseAssignments = new List<PCECaseAssignmentDto>();
        //         List<Guid> PCEIdList = SelectedPCEIds.Split(',').Select(x => Guid.Parse(x.Trim())).ToList();
        //         PCECaseTimeLinePostDto PCECaseTimeLinePostDto = null;

        //         foreach (Guid PCEId in PCEIdList)
        //         {
        //             var production = await _cbeContext.ProductionCapacities.Include(res => res.PCECase).FindAsync(PCEId);

        //             if (production != null)
        //             {
        //                 production.CurrentStage = assignedUser.Role.Name;
        //                 production.CurrentStatus = "New";
        //                 production.PCECase.Status = "Pending";
        //                 var previousCaseAssignment = await _cbeContext.PCECaseAssignments.Where(res => res.ProductionCapacityId == PCEId && res.UserId == assignedUser.Id).FirstOrDefaultAsync();
        //                 if (previousCaseAssignment != null)
        //                 {
        //                     previousCaseAssignment.Status = "New";
        //                     _cbeContext.PCECaseAssignments.Update(previousCaseAssignment);
        //                 }
        //                 else
        //                 {
        //                     var pceCaseAssignment = new PCECaseAssignment()
        //                     {
        //                         ProductionCapacityId = PCEId,
        //                         UserId = assignedUser.Id,
        //                         Status = "New",
        //                         AssignmentDate = DateTime.Now
        //                     };
        //                     await _cbeContext.PCECaseAssignments.AddAsync(pceCaseAssignment);
        //                     _cbeContext.ProductionCapacities.Update(production);
        //                     pceCaseAssignments.Add(_mapper.Map<PCECaseAssignmentDto>(pceCaseAssignment));
        //                 }

        //                 var previousRM = await _cbeContext.PCECaseAssignments.Where(res => res.ProductionCapacityId == PCEId && res.UserId == production.CreatedById).FirstOrDefaultAsync();
        //                 if (previousRM != null)
        //                 {
        //                     previousRM.Status = "Pending";
        //                     _cbeContext.PCECaseAssignments.Update(previousRM);
        //                 }

        //                 if (PCECaseTimeLinePostDto == null)
        //                 {
        //                     PCECaseTimeLinePostDto = new PCECaseTimeLinePostDto()
        //                     {
        //                         CaseId = production.PCECaseId,
        //                         Activity = $"<strong>production assigned for Re-estimation for <a href='/UserManagment/Profile?id={assignedUser.Id}'>{assignedUser.Name}</a> {assignedUser.Role.Name}.</strong> <br> <i class='text-purple'>Evaluation Center:</i> {assignedUser.District.Name}.",
        //                         CurrentStage = assignedUser.Role.Name
        //                     };
        //                 }
        //                 PCECaseTimeLinePostDto.Activity += $"<i class='text-purple'>Property Owner:</i> {production.PropertyOwner}. &nbsp; <i class='text-purple'>Role:</i> {production.Role}.&nbsp; <i class='text-purple'>production Catagory:</i> {EnumHelper.GetEnumDisplayName(production.Category)}. &nbsp; <i class='text-purple'>production Type:</i> {production.Type}. <br>";

        //             }
        //             var caseReEstimation = new ProductionReestimation
        //             {
        //                 ProductionCapacityId = PCEId,
        //                 Reason = ReestimationReason,
        //                 CreatedAt = DateTime.Now,
        //             };
        //             await _cbeContext.ProductionReestimations.AddAsync(caseReEstimation);
        //         }

        //         if (PCECaseTimeLinePostDto != null) 
        //         {
        //             await _IPCECaseTimeLineService.PCECaseTimeLine(PCECaseTimeLinePostDto);
        //         }  

        //         await _cbeContext.SaveChangesAsync();
        //         await transaction.CommitAsync();

        //         return pceCaseAssignments;  
        //     }

        //     catch (Exception ex)
        //     {
        //         _logger.LogError(ex, "Error sending production for reestimation");
        //         await transaction.RollbackAsync();
        //         throw new ApplicationException("An error occurred while sending production for reestimation.");
        //     }
        // }




        // public async Task<List<PCECaseAssignmentDto>> SendForValuation(string SelectedPCEIds, string CenterId) 
        // {    
        //     using var transaction = await _cbeContext.Database.BeginTransactionAsync();
        //     try
        //     {     
        //         var assignedUser = await _cbeContext.CreateUsers.Include(res => res.District).Include(res => res.Role).FirstOrDefaultAsync(res => res.DistrictId == Guid.Parse(CenterId) && (res.Role.Name == "Maker Manager" || res.Role.Name == "District Valuation Manager")); 
        //         // var MechanicalUser = await _cbeContext.CreateUsers.Include(res => res.District).Include(res => res.Role).FirstOrDefaultAsync(res => res.DistrictId == Guid.Parse(CenterId) && res.Department == "Mechanical" && (res.Role.Name == "Maker Manager" || res.Role.Name == "District Valuation Manager")); 
        //         // var CivilUser = await _cbeContext.CreateUsers.Include(res => res.District).Include(res => res.Role).FirstOrDefaultAsync(res => res.DistrictId == Guid.Parse(CenterId) && res.Department == "Civil" && (res.Role.Name == "Maker Manager" || res.Role.Name == "District Valuation Manager")); 
        //         // var AgricultureUser = await _cbeContext.CreateUsers.Include(res => res.District).Include(res => res.Role).FirstOrDefaultAsync(res => res.DistrictId == Guid.Parse(CenterId) && res.Department == "Agriculture" && (res.Role.Name == "Maker Manager" || res.Role.Name == "District Valuation Manager")); 
                
        //         if (assignedUser == null)
        //         {
        //             throw new Exception("Sorry the evaluation center is not ready.");
        //         }
                
        //         List<PCECaseAssignmentDto> pceCaseAssignments = new List<PCECaseAssignmentDto>(); 
        //         List<Guid> PCEIdList = SelectedPCEIds.Split(',').Select(x => Guid.Parse(x.Trim())).ToList(); 
        //         PCECaseTimeLinePostDto PCECaseTimeLinePostDto = null; 
        
        //         foreach (Guid PCEId in PCEIdList) 
        //         { 
        
        //             var production = await _cbeContext.ProductionCapacities.Include(res => res.PCECase).FindAsync(PCEId); 
        //             if (production != null) 
        //             {         
        //                 production.CurrentStage = assignedUser.Role.Name;
        //                 production.CurrentStatus = "New"; 
        //                 production.PCECase.Status = "Pending";                     
        
        //                 var previousCaseAssignment = await _cbeContext.PCECaseAssignments.Where(res => res.ProductionCapacityId == PCEId && res.UserId == assignedUser.Id).FirstOrDefaultAsync(); 
        //                 if (previousCaseAssignment != null) 
        //                 { 
        //                     previousCaseAssignment.Status = "New"; 
        //                     _cbeContext.PCECaseAssignments.Update(previousCaseAssignment);
        //                 } 
        //                 else 
        //                 { 
        //                     var pceCaseAssignment = new PCECaseAssignment() 
        //                     { 
        //                         ProductionCapacityId = PCEId, 
        //                         UserId = assignedUser.Id, 
        //                         Status = "New", 
        //                         AssignmentDate = DateTime.Now 
        //                     }; 
        //                     await _cbeContext.PCECaseAssignments.AddAsync(pceCaseAssignment);
        //                     _cbeContext.ProductionCapacities.Update(production); 
        //                     pceCaseAssignments.Add(_mapper.Map<PCECaseAssignmentDto>(pceCaseAssignment)); 
        //                 } 
                        
        //                 var previousRM= await _cbeContext.PCECaseAssignments.Where(res => res.ProductionCapacityId == PCEId && res.UserId == production.CreatedById).FirstOrDefaultAsync(); 
                        
        //                 if (previousRM != null) 
        //                 { 
        //                     previousRM.Status = "Pending"; 
        //                     _cbeContext.PCECaseAssignments.Update(previousRM); 
        //                 }           
        
        //                 if (PCECaseTimeLinePostDto == null) 
        //                 { 
        //                     PCECaseTimeLinePostDto = new PCECaseTimeLinePostDto() 
        //                     { 
        //                         CaseId = production.PCECaseId, 
        //                         Activity = $"<strong>PCE assigned for estimation for <a href='/UserManagment/Profile?id={assignedUser.Id}'>{assignedUser.Name}</a> {assignedUser.Role.Name}.</strong> <br> <i class='text-purple'>Evaluation Center:</i> {assignedUser.District.Name}.", 
        //                         CurrentStage = assignedUser.Role.Name
        //                     }; 
        //                 } 
        //             PCECaseTimeLinePostDto.Activity += $"<i class='text-purple'>Property Owner:</i> {production.PropertyOwner}. &nbsp; <i class='text-purple'>Role:</i> {production.Role}.&nbsp; <i class='text-purple'>production Catagory:</i> {EnumHelper.GetEnumDisplayName(production.Category)}. &nbsp; <i class='text-purple'>production Type:</i> {production.Type}. <br>"; 
        
        //             } 
        //         } 

        //         if (PCECaseTimeLinePostDto != null) 
        //         {
        //             await _IPCECaseTimeLineService.PCECaseTimeLine(PCECaseTimeLinePostDto);
        //         } 
                           
        //         await _cbeContext.SaveChangesAsync();
        //         await transaction.CommitAsync();

        //         return pceCaseAssignments;  
        //     }

        //     catch (Exception ex)
        //     {
        //         _logger.LogError(ex, "Error sending production for valuation");
        //         await transaction.RollbackAsync();
        //         throw new ApplicationException("An error occurred while sending production for valuation.");
        //     }
        // } 
    }
}