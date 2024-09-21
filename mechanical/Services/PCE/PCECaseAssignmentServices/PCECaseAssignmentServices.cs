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

namespace mechanical.Services.PCE.PCECaseAssignmentServices
{
    public class PCECaseAssignmentServices : IPCECaseAssignmentServices
    {
        private readonly CbeContext _cbeContext;
        private readonly IMapper _mapper;
        private readonly ILogger<PCECaseAssignmentServices> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IPCECaseTimeLineService _IPCECaseTimeLineService;

        public PCECaseAssignmentServices(CbeContext cbeContext, IMapper mapper, ILogger<PCECaseAssignmentServices> logger, IHttpContextAccessor httpContextAccessor, IPCECaseTimeLineService IPCECaseTimeLineService)
        {
            _cbeContext = cbeContext;
            _mapper = mapper;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
            _IPCECaseTimeLineService = IPCECaseTimeLineService;

        }

        public Task<List<PCECaseAssignmentDto>> AssignProductionMakerTeamleader(Guid UserId, string SelectedPCEIds, string EmployeeId) =>
            AssignOrReAssignProduction(UserId, SelectedPCEIds, EmployeeId, false);

        public Task<List<PCECaseAssignmentDto>> ReAssignProductionMakerTeamleader(Guid UserId, string SelectedPCEIds, string EmployeeId) =>
            AssignOrReAssignProduction(UserId, SelectedPCEIds, EmployeeId, true);

        public Task<List<PCECaseAssignmentDto>> AssignProductionMakerOfficer(Guid UserId, string SelectedPCEIds, string EmployeeId) =>
            AssignOrReAssignProduction(UserId, SelectedPCEIds, EmployeeId, false);

        public Task<List<PCECaseAssignmentDto>> ReAssignProductionMakerOfficer(Guid UserId, string SelectedPCEIds, string EmployeeId) =>
            AssignOrReAssignProduction(UserId, SelectedPCEIds, EmployeeId, true);

        public async Task<List<PCECaseAssignmentDto>> AssignOrReAssignProduction(Guid UserId, string SelectedPCEIds, string EmployeeId, bool isReassign)
        {
            using var transaction = await _cbeContext.Database.BeginTransactionAsync();
            try
            {    
                var assignedUser = await _cbeContext.CreateUsers.Include(res => res.Role).FirstOrDefaultAsync(res => res.Id == Guid.Parse(EmployeeId));
                List<PCECaseAssignmentDto> pceCaseAssignments = new List<PCECaseAssignmentDto>();
                List<Guid> PCEIdList = SelectedPCEIds.Split(',').Select(x => Guid.Parse(x.Trim())).ToList();
                PCECaseTimeLinePostDto PCECaseTimeLinePostDto = null;

                foreach (Guid PCEId in PCEIdList)
                {
                    var production = await _cbeContext.ProductionCapacities.FindAsync(PCEId);
                    if (production == null) continue;

                    production.CurrentStage = assignedUser.Role.Name;
                    production.CurrentStatus = "New";

                    // Check and update the status of the existing case assignment in case od reestimation
                    var existingCaseAssignment = await _cbeContext.PCECaseAssignments
                                                                .Where(res => res.ProductionCapacityId == PCEId && res.UserId == assignedUser.Id)
                                                                .FirstOrDefaultAsync();

                    if (existingCaseAssignment != null)
                    {
                        existingCaseAssignment.Status = "New";
                        _cbeContext.PCECaseAssignments.Update(existingCaseAssignment);
                    }
                    else
                    {
                        var pceCaseAssignment = new PCECaseAssignment
                        {
                            ProductionCapacityId = PCEId,
                            UserId = assignedUser.Id,
                            Status = "New",
                            AssignmentDate = DateTime.Now
                        };
                        await _cbeContext.PCECaseAssignments.AddAsync(pceCaseAssignment);
                        pceCaseAssignments.Add(_mapper.Map<PCECaseAssignmentDto>(pceCaseAssignment));
                    }

                    // Update production
                    _cbeContext.ProductionCapacities.Update(production);

                    // Prepare timeline activity
                    if (PCECaseTimeLinePostDto == null)
                    {
                        PCECaseTimeLinePostDto = new PCECaseTimeLinePostDto
                        {
                            CaseId = production.PCECaseId,
                            Activity = $" <strong>A production has been {(isReassign ? "re-assigned" : "assigned")} for {assignedUser.Name} {assignedUser.Role.Name}. </strong> <br>",
                            CurrentStage = assignedUser.Role.Name
                        };
                    }
                    PCECaseTimeLinePostDto.Activity += $"<i class='text-purple'>Property Owner:</i> {production.PropertyOwner}. &nbsp; " +
                        $"<i class='text-purple'>Role:</i> {production.Role}.&nbsp; " +
                        $"<i class='text-purple'>Production Category:</i> {production.Category}. &nbsp; " +
                        $"<i class='text-purple'>Production Type:</i> {production.Type}. <br>";

                    // Update status of the previous case assignment
                    var previousCaseAssignment = await _cbeContext.PCECaseAssignments
                                                                .Where(res => res.UserId == UserId && res.ProductionCapacityId == production.Id)
                                                                .FirstOrDefaultAsync();
                    if (previousCaseAssignment != null)
                    {
                        previousCaseAssignment.Status = "Pending";
                        _cbeContext.PCECaseAssignments.Update(previousCaseAssignment);
                    }
                }

                if (PCECaseTimeLinePostDto != null)
                {
                    await _IPCECaseTimeLineService.PCECaseTimeLine(PCECaseTimeLinePostDto);
                }

                await _cbeContext.SaveChangesAsync();
                await transaction.CommitAsync();

                return pceCaseAssignments;                            
            }

            catch (Exception ex)
            {
                _logger.LogError(ex, "Error assigning production");
                await transaction.RollbackAsync();
                throw new ApplicationException("An error occurred while assigning production.");
            }
        }

        public async Task<List<PCECaseAssignmentDto>> SendProductionForReestimation(string ReestimationReason, string SelectedPCEIds, string CenterId)
        {
            using var transaction = await _cbeContext.Database.BeginTransactionAsync();
            try
            {   
                var centerId = Guid.Parse(CenterId);
                var user = await _cbeContext.CreateUsers.Include(res => res.District).Include(res => res.Role).FirstOrDefaultAsync(res => res.DistrictId == centerId && res.Role.Name == "Maker Manager");
                if (user == null)
                {
                    throw new Exception("sorry the center is not ready.");
                }
                List<PCECaseAssignmentDto> pceCaseAssignments = new List<PCECaseAssignmentDto>();
                List<Guid> PCEIdList = SelectedPCEIds.Split(',').Select(x => Guid.Parse(x.Trim())).ToList();
                PCECaseTimeLinePostDto PCECaseTimeLinePostDto = null;

                foreach (Guid PCEId in PCEIdList)
                {
                    var production = await _cbeContext.ProductionCapacities.FindAsync(PCEId);

                    if (production != null)
                    {
                        production.CurrentStage = "Maker Manager";
                        // production.CurrentStatus = "Reestimate";
                        production.CurrentStatus = "New";
                        var previousCaseAssignment = await _cbeContext.PCECaseAssignments.Where(res => res.ProductionCapacityId == PCEId && res.UserId == user.Id).FirstOrDefaultAsync();
                        if (previousCaseAssignment != null)
                        {
                            // previousCaseAssignment.Status = "Reestimate";
                            previousCaseAssignment.Status = "New";
                            _cbeContext.PCECaseAssignments.Update(previousCaseAssignment);
                        }
                        else
                        {
                            var pceCaseAssignment = new PCECaseAssignment()
                            {
                                ProductionCapacityId = PCEId,
                                UserId = user.Id,
                                Status = "New",
                                AssignmentDate = DateTime.Now
                            };
                            await _cbeContext.PCECaseAssignments.AddAsync(pceCaseAssignment);
                            _cbeContext.ProductionCapacities.Update(production);
                            pceCaseAssignments.Add(_mapper.Map<PCECaseAssignmentDto>(pceCaseAssignment));
                        }

                        var previousRM = await _cbeContext.PCECaseAssignments.Where(res => res.ProductionCapacityId == PCEId && res.UserId == production.CreatedById).FirstOrDefaultAsync();
                        if (previousRM != null)
                        {
                            previousRM.Status = "Pending";
                            _cbeContext.PCECaseAssignments.Update(previousRM);
                        }

                        if (PCECaseTimeLinePostDto == null)
                        {
                            PCECaseTimeLinePostDto = new PCECaseTimeLinePostDto()
                            {
                                CaseId = production.PCECaseId,
                                Activity = $"<strong>production assigned for Re-estimation for <a href='/UserManagment/Profile?id={user.Id}'>{user.Name}</a> {user.Role.Name}.</strong> <br> <i class='text-purple'>Evaluation Center:</i> {user.District.Name}.",
                                CurrentStage = user.Role.Name
                            };
                        }
                        PCECaseTimeLinePostDto.Activity += $"<i class='text-purple'>Property Owner:</i> {production.PropertyOwner}. &nbsp; <i class='text-purple'>Role:</i> {production.Role}.&nbsp; <i class='text-purple'>production Catagory:</i> {EnumHelper.GetEnumDisplayName(production.Category)}. &nbsp; <i class='text-purple'>production Type:</i> {production.Type}. <br>";

                    }
                    var caseReEstimation = new ProductionReestimation
                    {
                        ProductionCapacityId = PCEId,
                        Reason = ReestimationReason,
                        CreatedAt = DateTime.Now,
                    };
                    await _cbeContext.ProductionReestimations.AddAsync(caseReEstimation);
                }
                if (PCECaseTimeLinePostDto != null) 
                {
                    await _IPCECaseTimeLineService.PCECaseTimeLine(PCECaseTimeLinePostDto);
                }  

                await _cbeContext.SaveChangesAsync();
                await transaction.CommitAsync();

                return pceCaseAssignments;  
            }

            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending production for reestimation");
                await transaction.RollbackAsync();
                throw new ApplicationException("An error occurred while sending production for reestimation.");
            }
        }




        public async Task<List<PCECaseAssignmentDto>> SendProductionForValuation(string SelectedPCEIds, string CenterId) 
        {    
            using var transaction = await _cbeContext.Database.BeginTransactionAsync();
            try
            {    
                var centerId = Guid.Parse(CenterId); 
                var districtName = await _cbeContext.Districts.Where(c => c.Id == centerId).Select(c => c.Name).FirstOrDefaultAsync(); 
                var CivilUser = await _cbeContext.CreateUsers.Include(res => res.District).Include(res => res.Role).FirstOrDefaultAsync(res => res.DistrictId == centerId && res.Department == "Civil" && (res.Role.Name == "Maker Manager" || res.Role.Name == "District Valuation Manager")); 
                var MechanicalUser = await _cbeContext.CreateUsers.Include(res => res.District).Include(res => res.Role).FirstOrDefaultAsync(res => res.DistrictId == centerId && res.Department == "Mechanical" && (res.Role.Name == "Maker Manager" || res.Role.Name == "District Valuation Manager")); 
                var AgricultureUser = await _cbeContext.CreateUsers.Include(res => res.District).Include(res => res.Role).FirstOrDefaultAsync(res => res.DistrictId == centerId && res.Department == "Agriculture" && (res.Role.Name == "Maker Manager" || res.Role.Name == "District Valuation Manager")); 
        
                List<PCECaseAssignmentDto> pceCaseAssignments = new List<PCECaseAssignmentDto>(); 
                List<Guid> PCEIdList = SelectedPCEIds.Split(',').Select(x => Guid.Parse(x.Trim())).ToList(); 
                PCECaseTimeLinePostDto PCECaseTimeLinePostDto = null; 
        
                foreach (Guid PCEId in PCEIdList) 
                { 
        
                    var production = await _cbeContext.ProductionCapacities.FindAsync(PCEId); 
                    if (production != null) 
                    { 
        
                        if (districtName != null && districtName == "Head Office") 
                        { 
                            production.CurrentStage = "Maker Manager"; 
                        } 
                        else 
                        { 
                            production.CurrentStage = "District Valuation Manager"; 
                        } 
                        production.CurrentStatus = "New"; 
                        var UserID = Guid.Empty;
                        string UserName = "";
                        string UserRoleName = "";
                        string District = ""; 
        
                        if (production.ProductionType == "Manufacturing") 
                        { 
                            if (MechanicalUser == null) 
                            { 
                                throw new Exception("sorry the Mechanical Evaluation center is not ready."); 
                            } 
                            else 
                            { 
                                UserID = MechanicalUser.Id;
                                UserName = MechanicalUser.Name;
                                UserRoleName = MechanicalUser.Role.Name;
                            } 
                        } 
                        else
                        {
                            if (production.ProductionType == "Plant") 
                            { 
                                if (MechanicalUser == null) 
                                { 
                                    throw new Exception("sorry the Plant Evaluation center is not ready."); 
                                } 
                                else 
                                { 
                                    UserID = MechanicalUser.Id; 
                                    UserName = MechanicalUser.Name;
                                    UserRoleName = MechanicalUser.Role.Name;
                                } 
                            } 
                        }
        
        
                        var previousCaseAssignment = await _cbeContext.PCECaseAssignments.Where(res => res.ProductionCapacityId == PCEId && res.UserId == UserID).FirstOrDefaultAsync(); 
                        if (previousCaseAssignment != null) 
                        { 
                            previousCaseAssignment.Status = "New"; 
                            _cbeContext.PCECaseAssignments.Update(previousCaseAssignment);
                        } 
                        else 
                        { 
                            var pceCaseAssignment = new PCECaseAssignment() 
                            { 
                                ProductionCapacityId = PCEId, 
                                UserId = UserID, 
                                Status = "New", 
                                AssignmentDate = DateTime.Now 
                            }; 
                            await _cbeContext.PCECaseAssignments.AddAsync(pceCaseAssignment);
                            _cbeContext.ProductionCapacities.Update(production); 
                            pceCaseAssignments.Add(_mapper.Map<PCECaseAssignmentDto>(pceCaseAssignment)); 
                        } 
                        ///####################################################################################################################################################### 
                        ///edit the relatinal manager status to pending  
        
                        var previousRM= await _cbeContext.PCECaseAssignments.Where(res => res.ProductionCapacityId == PCEId && res.UserId == production.CreatedById).FirstOrDefaultAsync(); 
                        if (previousRM != null) 
                        { 
                            previousRM.Status = "Pending"; 
                            _cbeContext.PCECaseAssignments.Update(previousRM); 
                        }
                        //else
                        //{
                        //    var pceCaseAssignment = new PCECaseAssignment()
                        //    {
                        //        ProductionCapacityId = PCEId,
                        //        UserId  = new Guid(production.CreatedById.ToString()),
                        //        Status = "Pending",
                        //        AssignmentDate = DateTime.Now
                        //    };
                        //    await _cbeContext.PCECaseAssignments.AddAsync(pceCaseAssignment);
                        //}
                        ///####################################################################################################################################################### 
        
            
        
                        if (PCECaseTimeLinePostDto == null) 
                        { 
                            PCECaseTimeLinePostDto = new PCECaseTimeLinePostDto() 
                            { 
                                CaseId = production.PCECaseId, 
                                Activity = $"<strong>PCE assigned for estimation for <a href='/UserManagment/Profile?id={UserID}'>{UserName}</a> {UserRoleName}.</strong> <br> <i class='text-purple'>Evaluation Center:</i> {districtName}.", 
                                CurrentStage = UserRoleName
                            }; 
                        } 
                    //  PCECaseTimeLinePostDto.Activity += $"<i class='text-purple'>Property Owner:</i> {production.PropertyOwner}. &nbsp; <i class='text-purple'>Role:</i> {production.Role}.&nbsp; <i class='text-purple'>production Catagory:</i> {EnumHelper.GetEnumDisplayName(production.Category)}. &nbsp; <i class='text-purple'>production Type:</i> {production.Type}. <br>"; 
        
                    } 
                } 
                if (PCECaseTimeLinePostDto != null) 
                {
                    await _IPCECaseTimeLineService.PCECaseTimeLine(PCECaseTimeLinePostDto);
                } 
                           
                await _cbeContext.SaveChangesAsync();
                await transaction.CommitAsync();

                return pceCaseAssignments;  
            }

            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending production for valuation");
                await transaction.RollbackAsync();
                throw new ApplicationException("An error occurred while sending production for valuation.");
            }
        } 
    


    }
}


