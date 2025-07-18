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
using mechanical.Models.Dto.NotificationDto;
using mechanical.Services.NotificationService;

namespace mechanical.Services.PCE.PCECaseAssignmentService
{
    public class PCECaseAssignmentService : IPCECaseAssignmentService
    {
        private readonly CbeContext _cbeContext;
        private readonly IMapper _mapper;
        private readonly ILogger<PCECaseAssignmentService> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IPCECaseTimeLineService _IPCECaseTimeLineService;
        private readonly INotificationService _notificationService;

        public PCECaseAssignmentService(CbeContext cbeContext, IMapper mapper, ILogger<PCECaseAssignmentService> logger, IHttpContextAccessor httpContextAccessor, IPCECaseTimeLineService IPCECaseTimeLineService, INotificationService notificationService)
        {
            _cbeContext = cbeContext;
            _mapper = mapper;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
            _IPCECaseTimeLineService = IPCECaseTimeLineService;
            _notificationService = notificationService;

        }

        public Task<List<PCECaseAssignmentDto>> AssignProduction(Guid UserId, string SelectedPCEIds, string EmployeeId) =>
            AssignOrReAssignProduction(UserId, SelectedPCEIds, EmployeeId, false);

        public Task<List<PCECaseAssignmentDto>> ReAssignProduction(Guid UserId, string SelectedPCEIds, string EmployeeId) =>
            AssignOrReAssignProduction(UserId, SelectedPCEIds, EmployeeId, true);

        public async Task<List<PCECaseAssignmentDto>> AssignOrReAssignProduction(Guid UserId, string SelectedPCEIds, string employeeId, bool isReassign)
        {
            return await ProcessProductionAssignments(UserId, SelectedPCEIds, employeeId, isReassign, "Assign");
        }

        public async Task<List<PCECaseAssignmentDto>> SendForValuation(Guid UserId, string SelectedPCEIds, string centerId)
        {
            return await ProcessProductionAssignments(UserId, SelectedPCEIds, centerId, false, "Valuation");
        }

        public async Task<List<PCECaseAssignmentDto>> SendForReestimation(Guid UserId, string ReestimationReason, string SelectedPCEIds, string centerId)
        {
            return await ProcessProductionAssignments(UserId, SelectedPCEIds, centerId, false, "Reestimation", ReestimationReason);
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
                    if (OperationType == "Assign")
                    {
                        throw new Exception("Please, select at least one production to assign.");
                    }
                    throw new Exception("Please, select at least one production to send for estimation.");
                }

                if (string.IsNullOrEmpty(EmployeeOrCenterId))
                {
                    if (OperationType == "Assign")
                    {
                        throw new Exception("Please, select a user to assign.");
                    }
                    throw new Exception("Please, select an evaluation center.");
                }

                var assignedUser = await GetAssignedUser(EmployeeOrCenterId, OperationType);
                if (assignedUser == null)
                {
                    if (OperationType == "Assign")
                    {
                        throw new Exception("The assigned user is not found.");
                    }
                    throw new Exception("The evaluation center is not ready.");
                }

                PCECaseTimeLinePostDto pceCaseTimeLineDto = null;
                List<PCECaseAssignmentDto> pceCaseAssignments = new List<PCECaseAssignmentDto>();
                List<Guid> PCEIdList = SelectedPCEIds.Split(',').Select(Guid.Parse).ToList();
                //notification 
                var notificationContent = "New PCECase sent for estimation";
                var notificationType = "PCECase Send for Estimation";
                var link = $"";
                NotificationReturnDto notification = null;

                foreach (var PCEId in PCEIdList)
                {
                    var production = await GetProductionById(PCEId);
                    link = $"/ProductionCapacity/Detail?Id={production.Id}";
                    if (production == null) continue;

                    await UpdateProduction(production, assignedUser, OperationType);
                    await AssignOrUpdateCase(UserId, PCEId, assignedUser, pceCaseAssignments, ReestimationReason, OperationType);

                    if (pceCaseTimeLineDto == null)
                    {
                        pceCaseTimeLineDto = CreateTimelineDto(production.PCECaseId, assignedUser, isReassign, OperationType);
                    }
                    UpdateTimelineDto(production, pceCaseTimeLineDto);
                    await UpdatePreviousAssignments(UserId, PCEId);

                    // Add Notification
                    notification = await _notificationService.AddNotification(assignedUser.Id, notificationContent, notificationType, link);

                }
                if (pceCaseTimeLineDto != null)
                {
                    await _IPCECaseTimeLineService.PCECaseTimeLine(pceCaseTimeLineDto);
                }

                await _cbeContext.SaveChangesAsync();
                await transaction.CommitAsync();
                // Realtime Nofication
                if (notification != null) await _notificationService.SendNotification(notification);

                return pceCaseAssignments;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error during {OperationType} operation");
                await transaction.RollbackAsync();
                throw new ApplicationException(ex.Message);
            }
        }

        private async Task<User> GetAssignedUser(string id, string OperationType)
        {
            if (OperationType == "Assign")
            {
                return await _cbeContext.Users
                                        .Include(res => res.Role)
                                        .Include(res => res.District)
                                        .FirstOrDefaultAsync(res => res.Id == Guid.Parse(id));
            }
            else
            {
                return await _cbeContext.Users
                                        .Include(res => res.Role)
                                        .Include(res => res.District)
                                        .FirstOrDefaultAsync(res => res.DistrictId == Guid.Parse(id) &&
                                                             res.Department == "Mechanical" &&
                                                            (res.Role.Name == "Maker Manager" || res.Role.Name == "District Valuation Manager"));
            }
        }

        private async Task<ProductionCapacity> GetProductionById(Guid PCEId)
        {
            return await _cbeContext.ProductionCapacities.Include(res => res.PCECase).FirstOrDefaultAsync(pc => pc.Id == PCEId);//.FindAsync(PCEId);

        }

        private async Task AssignOrUpdateCase(Guid UserId, Guid PCEId, User assignedUser, List<PCECaseAssignmentDto> caseAssignments, string ReestimationReason, string OperationType)
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
                    AssignmentDate = DateTime.UtcNow
                };
                await _cbeContext.PCECaseAssignments.AddAsync(newAssignment);
                caseAssignments.Add(_mapper.Map<PCECaseAssignmentDto>(newAssignment));
            }

            if (OperationType == "Reestimation")
            {
                var reestimation = new ProductionReestimation
                {
                    ProductionCapacityId = PCEId,
                    Reason = ReestimationReason,
                    CreatedAt = DateTime.UtcNow
                };
                await _cbeContext.ProductionReestimations.AddAsync(reestimation);
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

        private async Task UpdateProduction(ProductionCapacity production, User assignedUser, string OperationType)
        {
            production.CurrentStage = assignedUser.Role.Name;

            if (OperationType == "Reestimation" || OperationType == "Valuation")
            {
                production.PCECase.Status = "Pending";

                if (OperationType == "Reestimation")
                {
                    production.CurrentStatus = "Reestimate";
                }
                else
                {
                    var previousEvaluations = await _cbeContext.PCEEvaluations.AsNoTracking().Where(res => res.PCEId == production.Id).ToListAsync();

                    if (previousEvaluations != null && previousEvaluations.Any())
                    {
                        production.CurrentStatus = "Reestimate";
                    }
                    else
                    {
                        production.CurrentStatus = "New";
                    }
                }
            }
            else
            {
                if (assignedUser.Role.Name == "Maker Officer")
                {
                    production.AssignedEvaluatorId = assignedUser.Id;
                }
            }

            _cbeContext.ProductionCapacities.Update(production);
        }

        private PCECaseTimeLinePostDto CreateTimelineDto(Guid PCECaseId, User assignedUser, bool isReassign, string OperationType)
        {
            string activity = $"<strong>Production has been {(isReassign ? "re-assigned" : "assigned")} to {assignedUser.Name} ({assignedUser.Role.Name}).</strong><br>";
            if (OperationType == "Reestimation" || OperationType == "Valuation")
            {
                activity = $"<strong>Production assigned for {OperationType} to {assignedUser.Name} ({assignedUser.Role.Name}).</strong><br>";
            }

            return new PCECaseTimeLinePostDto
            {
                PCECaseId = PCECaseId,
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
    }
}