﻿using AutoMapper;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;

using mechanical.Data;
using mechanical.Models;
using mechanical.Models.Entities;

using mechanical.Models.PCE.Entities;
using mechanical.Models.PCE.Dto.PCECaseTimeLineDto;
using mechanical.Models.PCE.Dto.ProductionCaseAssignmentDto;
using mechanical.Services.PCE.PCECaseTimeLineService;

namespace mechanical.Services.PCE.ProductionCaseAssignmentServices
{
    public class ProductionCaseAssignmentServices : IProductionCaseAssignmentServices
    {
        private readonly CbeContext _cbeContext;
        private readonly IMapper _mapper;
        private readonly ILogger<ProductionCaseAssignmentServices> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IPCECaseTimeLineService _IPCECaseTimeLineService;

        public ProductionCaseAssignmentServices(CbeContext cbeContext, IMapper mapper, ILogger<ProductionCaseAssignmentServices> logger, IHttpContextAccessor httpContextAccessor, IPCECaseTimeLineService IPCECaseTimeLineService)
        {
            _cbeContext = cbeContext;
            _mapper = mapper;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
            _IPCECaseTimeLineService = IPCECaseTimeLineService;

        }

        public async Task<List<ProductionCaseAssignmentDto>> AssignOrReAssignProduction(Guid UserId, string SelectedPCEIds, string EmployeeId, bool isReassign)
        {
            using var transaction = await _cbeContext.Database.BeginTransactionAsync();
            try
            {    
                var employeeId = Guid.Parse(EmployeeId);
                var user = await _cbeContext.CreateUsers.Include(res => res.Role).FirstOrDefaultAsync(res => res.Id == employeeId);
                List<ProductionCaseAssignmentDto> pceCaseAssignments = new List<ProductionCaseAssignmentDto>();
                List<Guid> PCEIdList = SelectedPCEIds.Split(',').Select(x => Guid.Parse(x.Trim())).ToList();
                PCECaseTimeLinePostDto PCECaseTimeLinePostDto = null;

                foreach (Guid PCEId in PCEIdList)
                {
                    var production = await _cbeContext.ProductionCapacities.FindAsync(PCEId);
                    if (production == null) continue;

                    production.CurrentStage = user.Role.Name;
                    production.CurrentStatus = "New";
                    var previousCaseAssignment = await _cbeContext.ProductionCaseAssignments
                        .Where(res => res.ProductionCapacityId == PCEId && res.UserId == user.Id)
                        .FirstOrDefaultAsync();

                    if (previousCaseAssignment != null)
                    {
                        previousCaseAssignment.Status = "New";
                        _cbeContext.ProductionCaseAssignments.Update(previousCaseAssignment);
                    }
                    else
                    {
                        var pceCaseAssignment = new ProductionCaseAssignment
                        {
                            ProductionCapacityId = PCEId,
                            UserId = UserId,
                            Status = "New",
                            AssignmentDate = DateTime.Now
                        };
                        await _cbeContext.ProductionCaseAssignments.AddAsync(pceCaseAssignment);
                        pceCaseAssignments.Add(_mapper.Map<ProductionCaseAssignmentDto>(pceCaseAssignment));
                    }

                    // Update production
                    _cbeContext.ProductionCapacities.Update(production);

                    // Prepare timeline activity
                    if (PCECaseTimeLinePostDto == null)
                    {
                        PCECaseTimeLinePostDto = new PCECaseTimeLinePostDto
                        {
                            CaseId = production.PCECaseId,
                            Activity = $" <strong>A production has been {(isReassign ? "Re-assigned" : "assigned")} for {user.Name} {user.Role.Name}. </strong> <br>",
                            CurrentStage = "Maker Manager"
                        };
                    }
                    PCECaseTimeLinePostDto.Activity += $"<i class='text-purple'>Property Owner:</i> {production.PropertyOwner}. &nbsp; " +
                        $"<i class='text-purple'>Role:</i> {production.Role}.&nbsp; " +
                        $"<i class='text-purple'>Production Category:</i> {production.Category}. &nbsp; " +
                        $"<i class='text-purple'>Production Type:</i> {production.Type}. <br>";

                    // Update status of the case assignment
                    if (previousCaseAssignment != null)
                    {
                        previousCaseAssignment.Status = "Pending";
                        _cbeContext.Update(previousCaseAssignment);
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


        public async Task<List<ProductionCaseAssignmentDto>> AssignProductionMakerTeamleader(Guid UserId, string SelectedPCEIds, string EmployeeId)
        {
            using var transaction = await _cbeContext.Database.BeginTransactionAsync(); // BeginTransactionAsync(IsolationLevel.RepeatableRead); // ReadCommitted // ReadUncommitted // Serializable
            try
            { 
                Guid PCECaseId = Guid.Empty;
                var employeeId = Guid.Parse(EmployeeId);
                var user = await _cbeContext.CreateUsers.Include(res => res.Role).FirstOrDefaultAsync(res => res.Id == employeeId);
                List<ProductionCaseAssignmentDto> pceCaseAssignments = new List<ProductionCaseAssignmentDto>();

                List<Guid> PCEIdList = SelectedPCEIds.Split(',').Select(x => Guid.Parse(x.Trim())).ToList();
                PCECaseTimeLinePostDto PCECaseTimeLinePostDto = null;
                foreach (Guid PCEId in PCEIdList)
                {
                    var production = await _cbeContext.ProductionCapacities.FindAsync(PCEId);
                    if (production != null)
                    {
                        production.CurrentStage = user.Role.Name;
                        production.CurrentStatus = "New";
                        var previousCaseAssignment = await _cbeContext.ProductionCaseAssignments.Where(res => res.ProductionCapacityId == PCEId && res.UserId == user.Id).FirstOrDefaultAsync();
                        if (previousCaseAssignment != null)
                        {
                            previousCaseAssignment.Status = "New";
                            _cbeContext.ProductionCaseAssignments.Update(previousCaseAssignment);
                        }
                        else
                        {
                            var pceCaseAssignment = new ProductionCaseAssignment()
                            {
                                ProductionCapacityId = PCEId,
                                UserId = user.Id,
                                Status = "New",
                                AssignmentDate = DateTime.Now
                            };
                            await _cbeContext.ProductionCaseAssignments.AddAsync(pceCaseAssignment);
                            pceCaseAssignments.Add(_mapper.Map<ProductionCaseAssignmentDto>(pceCaseAssignment));
                        }


                        _cbeContext.ProductionCapacities.Update(production);
                        if (PCECaseTimeLinePostDto == null)
                        {
                            PCECaseTimeLinePostDto = new PCECaseTimeLinePostDto()
                            {
                                CaseId = production.PCECaseId,
                                Activity = $" <strong>A production has been assigned for {user.Name} {user.Role.Name}. </strong> <br>",
                                CurrentStage = "Maker Manager"
                            };
                        }
                        PCECaseTimeLinePostDto.Activity += $"<i class='text-purple'>Property Owner:</i> {production.PropertyOwner}. &nbsp; <i class='text-purple'>Role:</i> {production.Role}.&nbsp; <i class='text-purple'>production Catagory:</i> . &nbsp; <i class='text-purple'>production Type:</i> {production.Type}. <br>";

                        if (PCECaseId == Guid.Empty)
                        {
                            PCECaseId = production.PCECaseId;
                        }
                    }
                    var pceCaseassign = await _cbeContext.ProductionCaseAssignments.Where(res => res.UserId == UserId && res.ProductionCapacityId == production.Id).FirstOrDefaultAsync();
                    pceCaseassign.Status = "Pending";
                    _cbeContext.Update(pceCaseassign);

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
                _logger.LogError(ex, "Error assigning Production to Maker Team leader");
                await transaction.RollbackAsync();
                throw new ApplicationException("An error occurred while assigning Production to Maker Team leader.");
            }
        }

        public async Task<List<ProductionCaseAssignmentDto>> ReAssignProductionMakerTeamleader(Guid UserId, string SelectedPCEIds, string EmployeeId)
        {
            using var transaction = await _cbeContext.Database.BeginTransactionAsync();
            try
            {                
                Guid PCECaseId = Guid.Empty;
                var employeeId = Guid.Parse(EmployeeId);
                var user = await _cbeContext.CreateUsers.Include(res => res.Role).FirstOrDefaultAsync(res => res.Id == employeeId);
                List<ProductionCaseAssignmentDto> pceCaseAssignments = new List<ProductionCaseAssignmentDto>();

                List<Guid> caseAssigmentIdList = SelectedPCEIds.Split(',').Select(x => Guid.Parse(x.Trim())).ToList();
                PCECaseTimeLinePostDto PCECaseTimeLinePostDto = null;
                foreach (Guid cassAssigmentId in caseAssigmentIdList)
                {
                    var pceCaseAssignment = await _cbeContext.ProductionCaseAssignments.FindAsync(cassAssigmentId);
                    var production = await _cbeContext.ProductionCapacities.FindAsync(pceCaseAssignment.ProductionCapacityId);
                    if (pceCaseAssignment != null)
                    {
                        if (pceCaseAssignment.Status == "New")
                        {
                            pceCaseAssignment.UserId = user.Id;
                            pceCaseAssignment.AssignmentDate = DateTime.Now;
                        }
                        _cbeContext.ProductionCaseAssignments.Update(pceCaseAssignment);

                        if (PCECaseTimeLinePostDto == null)
                        {
                            PCECaseTimeLinePostDto = new PCECaseTimeLinePostDto()
                            {
                                CaseId = production.PCECaseId,
                                Activity = $" <strong>A production has been Re-assigned for {user.Name} {user.Role.Name}. </strong> <br>",
                                CurrentStage = "Maker Manager"
                            };
                        }
                        PCECaseTimeLinePostDto.Activity += $"<i class='text-purple'>Property Owner:</i> {production.PropertyOwner}. &nbsp; <i class='text-purple'>Role:</i> {production.Role}.&nbsp; <i class='text-purple'>production Catagory:</i> {EnumHelper.GetEnumDisplayName(production.Category)}. &nbsp; <i class='text-purple'>production Type:</i> {production.Type}. <br>";
                        pceCaseAssignments.Add(_mapper.Map<ProductionCaseAssignmentDto>(pceCaseAssignment));
                        if (PCECaseId == Guid.Empty)
                        {
                            PCECaseId = production.PCECaseId;
                        }
                    }
                    var pceCaseassign = await _cbeContext.ProductionCaseAssignments.Where(res => res.UserId == UserId && res.ProductionCapacityId == production.Id).FirstOrDefaultAsync();
                    pceCaseassign.Status = "Pending";
                    _cbeContext.Update(pceCaseassign);

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
                _logger.LogError(ex, "Error reassigning Production to Maker Team leader");
                await transaction.RollbackAsync();
                throw new ApplicationException("An error occurred while reassigning Production to Maker Team leader.");
            }
        }


        public async Task<List<ProductionCaseAssignmentDto>> AssignProductionMakerOfficer(Guid UserId, string SelectedPCEIds, string EmployeeId)
        {
            using var transaction = await _cbeContext.Database.BeginTransactionAsync();
            try
            {  
                Guid PCECaseId = Guid.Empty;
                var employeeId = Guid.Parse(EmployeeId);
                var user = await _cbeContext.CreateUsers.Include(res => res.Role).FirstOrDefaultAsync(res => res.Id == employeeId);
                List<ProductionCaseAssignmentDto> pceCaseAssignments = new List<ProductionCaseAssignmentDto>();

                List<Guid> PCEIdList = SelectedPCEIds.Split(',').Select(x => Guid.Parse(x.Trim())).ToList();
                PCECaseTimeLinePostDto PCECaseTimeLinePostDto = null;
                foreach (Guid PCEId in PCEIdList)
                {
                    var production = await _cbeContext.ProductionCapacities.FindAsync(PCEId);
                    if (production != null)
                    {
                        production.CurrentStage = user.Role.Name;
                        production.CurrentStatus = "New";
                        var previousCaseAssignment = await _cbeContext.ProductionCaseAssignments.Where(res => res.ProductionCapacityId == PCEId && res.UserId == user.Id).FirstOrDefaultAsync();
                        if (previousCaseAssignment != null)
                        {
                            previousCaseAssignment.Status = "New";
                            _cbeContext.ProductionCaseAssignments.Update(previousCaseAssignment);
                        }
                        else
                        {
                            var pceCaseAssignment = new ProductionCaseAssignment()
                            {
                                ProductionCapacityId = PCEId,
                                UserId = user.Id,
                                Status = "New",
                                AssignmentDate = DateTime.Now
                            };
                            await _cbeContext.ProductionCaseAssignments.AddAsync(pceCaseAssignment);
                            pceCaseAssignments.Add(_mapper.Map<ProductionCaseAssignmentDto>(pceCaseAssignment));
                        }


                        _cbeContext.ProductionCapacities.Update(production);
                        if (PCECaseTimeLinePostDto == null)
                        {
                            PCECaseTimeLinePostDto = new PCECaseTimeLinePostDto()
                            {
                                CaseId = production.PCECaseId,
                                Activity = $" <strong>A production has been assigned for {user.Name} {user.Role.Name}. </strong> <br>",
                                CurrentStage = "Maker Manager"
                            };
                        }
                        PCECaseTimeLinePostDto.Activity += $"<i class='text-purple'>Property Owner:</i> {production.PropertyOwner}. &nbsp; <i class='text-purple'>Role:</i> {production.Role}.&nbsp; <i class='text-purple'>production Catagory:</i> . &nbsp; <i class='text-purple'>production Type:</i> {production.Type}. <br>";

                        if (PCECaseId == Guid.Empty)
                        {
                            PCECaseId = production.PCECaseId;
                        }
                    }
                    var pceCaseassign = await _cbeContext.ProductionCaseAssignments.Where(res => res.UserId == UserId && res.ProductionCapacityId == production.Id).FirstOrDefaultAsync();
                    pceCaseassign.Status = "Pending";
                    _cbeContext.Update(pceCaseassign);

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
                _logger.LogError(ex, "Error assigning Production to Maker officer");
                await transaction.RollbackAsync();
                throw new ApplicationException("An error occurred while assigning Production to Maker officer.");
            }
        }

        public async Task<List<ProductionCaseAssignmentDto>> ReAssignProductionMakerOfficer(Guid UserId, string SelectedPCEIds, string EmployeeId)
        {
            using var transaction = await _cbeContext.Database.BeginTransactionAsync();
            try
            { 
                Guid PCECaseId = Guid.Empty;
                var employeeId = Guid.Parse(EmployeeId);
                var user = await _cbeContext.CreateUsers.Include(res => res.Role).FirstOrDefaultAsync(res => res.Id == employeeId);
                List<ProductionCaseAssignmentDto> pceCaseAssignments = new List<ProductionCaseAssignmentDto>();

                List<Guid> caseAssigmentIdList = SelectedPCEIds.Split(',').Select(x => Guid.Parse(x.Trim())).ToList();
                PCECaseTimeLinePostDto PCECaseTimeLinePostDto = null;
                foreach (Guid cassAssigmentId in caseAssigmentIdList)
                {
                    var pceCaseAssignment = await _cbeContext.ProductionCaseAssignments.FindAsync(cassAssigmentId);
                    var production = await _cbeContext.ProductionCapacities.FindAsync(pceCaseAssignment.ProductionCapacityId);
                    if (pceCaseAssignment != null)
                    {
                        if (pceCaseAssignment.Status == "New")
                        {
                            pceCaseAssignment.UserId = user.Id;
                            pceCaseAssignment.AssignmentDate = DateTime.Now;
                        }
                        _cbeContext.ProductionCaseAssignments.Update(pceCaseAssignment);

                        if (PCECaseTimeLinePostDto == null)
                        {
                            PCECaseTimeLinePostDto = new PCECaseTimeLinePostDto()
                            {
                                CaseId = production.PCECaseId,
                                Activity = $" <strong>A production has been Re-assigned for {user.Name} {user.Role.Name}. </strong> <br>",
                                CurrentStage = "Maker Manager"
                            };
                        }
                        PCECaseTimeLinePostDto.Activity += $"<i class='text-purple'>Property Owner:</i> {production.PropertyOwner}. &nbsp; <i class='text-purple'>Role:</i> {production.Role}.&nbsp; <i class='text-purple'>production Catagory:</i> {EnumHelper.GetEnumDisplayName(production.Category)}. &nbsp; <i class='text-purple'>production Type:</i> {production.Type}. <br>";
                        pceCaseAssignments.Add(_mapper.Map<ProductionCaseAssignmentDto>(pceCaseAssignment));
                        if (PCECaseId == Guid.Empty)
                        {
                            PCECaseId = production.PCECaseId;
                        }
                    }
                    var pceCaseassign = await _cbeContext.ProductionCaseAssignments.Where(res => res.UserId == UserId && res.ProductionCapacityId == production.Id).FirstOrDefaultAsync();
                    pceCaseassign.Status = "Pending";
                    _cbeContext.Update(pceCaseassign);

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
                _logger.LogError(ex, "Error reassigning Production to Maker officer");
                await transaction.RollbackAsync();
                throw new ApplicationException("An error occurred while reassigning Production to Maker officer.");
            }
        }



        public async Task<List<ProductionCaseAssignmentDto>> SendProductionForReestimation(string ReestimationReason, string SelectedPCEIds, string CenterId)
        {
            using var transaction = await _cbeContext.Database.BeginTransactionAsync();
            try
            {   
                var centerId = Guid.Parse(CenterId);
                var user = await _cbeContext.CreateUsers.Include(res => res.District).FirstOrDefaultAsync(res => res.DistrictId == centerId && res.Role.Name == "Maker Manager");
                if (user == null)
                {
                    throw new Exception("sorry the center is not ready.");
                }
                List<ProductionCaseAssignmentDto> pceCaseAssignments = new List<ProductionCaseAssignmentDto>();
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
                        var previousCaseAssignment = await _cbeContext.ProductionCaseAssignments.Where(res => res.ProductionCapacityId == PCEId && res.UserId == user.Id).FirstOrDefaultAsync();
                        if (previousCaseAssignment != null)
                        {
                            // previousCaseAssignment.Status = "Reestimate";
                            previousCaseAssignment.Status = "New";
                            _cbeContext.ProductionCaseAssignments.Update(previousCaseAssignment);
                        }
                        else
                        {
                            var pceCaseAssignment = new ProductionCaseAssignment()
                            {
                                ProductionCapacityId = PCEId,
                                UserId = user.Id,
                                Status = "New",
                                AssignmentDate = DateTime.Now
                            };
                            await _cbeContext.ProductionCaseAssignments.AddAsync(pceCaseAssignment);
                            _cbeContext.ProductionCapacities.Update(production);
                            pceCaseAssignments.Add(_mapper.Map<ProductionCaseAssignmentDto>(pceCaseAssignment));
                        }


                        if (PCECaseTimeLinePostDto == null)
                        {
                            PCECaseTimeLinePostDto = new PCECaseTimeLinePostDto()
                            {
                                CaseId = production.PCECaseId,
                                Activity = $"<strong>production assigned for Re-estimation for <a href='/UserManagment/Profile?id={user.Id}'>{user.Name}</a> Maker Manager.</strong> <br> <i class='text-purple'>Evaluation Center:</i> {user.District.Name}.",
                                CurrentStage = "Maker Manager"
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




        public async Task<List<ProductionCaseAssignmentDto>> SendProductionForValuation(string SelectedPCEIds, string CenterId) 
        {    
            using var transaction = await _cbeContext.Database.BeginTransactionAsync();
            try
            {    
                var centerId = Guid.Parse(CenterId); 
                var districtName = await _cbeContext.Districts.Where(c => c.Id == centerId).Select(c => c.Name).FirstOrDefaultAsync(); 
                var CivilUser = await _cbeContext.CreateUsers.Include(res => res.District).FirstOrDefaultAsync(res => res.DistrictId == centerId && res.Department == "Civil" && (res.Role.Name == "Maker Manager" || res.Role.Name == "District Valuation Manager")); 
                var MechanicalUser = await _cbeContext.CreateUsers.Include(res => res.District).FirstOrDefaultAsync(res => res.DistrictId == centerId && res.Department == "Mechanical" && (res.Role.Name == "Maker Manager" || res.Role.Name == "District Valuation Manager")); 
                var AgricultureUser = await _cbeContext.CreateUsers.Include(res => res.District).FirstOrDefaultAsync(res => res.DistrictId == centerId && res.Department == "Agriculture" && (res.Role.Name == "Maker Manager" || res.Role.Name == "District Valuation Manager")); 
        
                List<ProductionCaseAssignmentDto> pceCaseAssignments = new List<ProductionCaseAssignmentDto>(); 
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
                                } 
                            } 
                        }
        
        
                        var previousCaseAssignment = await _cbeContext.ProductionCaseAssignments.Where(res => res.ProductionCapacityId == PCEId && res.UserId == UserID).FirstOrDefaultAsync(); 
                        if (previousCaseAssignment != null) 
                        { 
                            previousCaseAssignment.Status = "New"; 
                            _cbeContext.ProductionCaseAssignments.Update(previousCaseAssignment);
                        } 
                        else 
                        { 
                            var pceCaseAssignment = new ProductionCaseAssignment() 
                            { 
                                ProductionCapacityId = PCEId, 
                                UserId = UserID, 
                                Status = "New", 
                                AssignmentDate = DateTime.Now 
                            }; 
                            await _cbeContext.ProductionCaseAssignments.AddAsync(pceCaseAssignment);
                            _cbeContext.ProductionCapacities.Update(production); 
                            pceCaseAssignments.Add(_mapper.Map<ProductionCaseAssignmentDto>(pceCaseAssignment)); 
                        } 
                        ///####################################################################################################################################################### 
                        ///edit the relatinal manager status to pending  
        
                        var previousRM= await _cbeContext.ProductionCaseAssignments.Where(res => res.ProductionCapacityId == PCEId && res.UserId == production.CreatedById).FirstOrDefaultAsync(); 
                        if (previousRM != null) 
                        { 
                            previousRM.Status = "Pending"; 
                            _cbeContext.ProductionCaseAssignments.Update(previousRM); 
                        } 
                        ///####################################################################################################################################################### 
        
            
        
                        if (PCECaseTimeLinePostDto == null) 
                        { 
                            PCECaseTimeLinePostDto = new PCECaseTimeLinePostDto() 
                            { 
                                CaseId = production.PCECaseId, 
                                Activity = $"<strong>PCE assigned for estimation for <a href='/UserManagment/Profile?id={UserID}'>{UserName}</a> Maker Manager.</strong> <br> <i class='text-purple'>Evaluation Center:</i> {districtName}.", 
                                CurrentStage = "Maker Manager" 
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