using AutoMapper;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

using mechanical.Data;

using mechanical.Models.Dto.CaseAssignmentDto;

using mechanical.Hubs;
using mechanical.Utils;
using mechanical.Models.Dto;
using mechanical.Models.Entities;
using mechanical.Models.Dto.CaseDto;
using mechanical.Models.Dto.CaseTimeLineDto;
using mechanical.Models.Dto.NotificationDto;
using mechanical.Models.Dto.TaskManagmentDto;
using mechanical.Services.CaseTimeLineService;
using mechanical.Services.NotificationService;
using mechanical.Services.CaseServices;
using mechanical.Services.UserService;

namespace mechanical.Services.TaskManagmentService
{
    public class TaskManagmentService : ITaskManagmentService
    {
        private readonly IMapper _mapper;
        private readonly CbeContext _cbeContext;
        private readonly IHubContext<NotificationHub> _hubContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<TaskManagmentService> _logger;
        private readonly ICaseTimeLineService _caseTimeLineService;
        private readonly INotificationService _notificationService;
        private readonly ICaseService _caseService;
        private readonly IUserService _userService;
        
        public TaskManagmentService(CbeContext cbeContext, IHubContext<NotificationHub> hubContext, IMapper mapper, ILogger<TaskManagmentService> logger, IHttpContextAccessor httpContextAccessor, ICaseService caseService, IUserService userService, ICaseTimeLineService caseTimeLineService, INotificationService notificationService)
        {
            _mapper = mapper;
            _logger = logger;
            _cbeContext = cbeContext;
            _hubContext = hubContext;
            _httpContextAccessor = httpContextAccessor;
            _caseTimeLineService = caseTimeLineService;
            _notificationService = notificationService;
            _caseService = caseService;
            _userService = userService;
        }

        public async Task<TaskManagment> SharesTask(string selectedCaseIds, Guid AssignorId, TaskManagmentPostDto createTaskManagmentDto)

        {
            using var transaction = await _cbeContext.Database.BeginTransactionAsync();
            try
            {
                EncodingHelper.EncodeObject(createTaskManagmentDto);

                var user = await _cbeContext.CreateUsers
                    .Include(u => u.District)
                    .Include(u => u.Role)
                    .FirstOrDefaultAsync(u => u.Id == AssignorId)
                    ?? throw new ArgumentException("User not found.", nameof(AssignorId));

                var asigneeUser = await _cbeContext.CreateUsers
                    .Include(u => u.Role)

                    .FirstOrDefaultAsync(u => u.Id == createTaskManagmentDto.AssignedId)
                    ?? throw new ArgumentException("Assignee user not found.", nameof(createTaskManagmentDto.AssignedId));

                /* .FirstOrDefaultAsync(u => u.Id == createTaskManagmentDto.AssignedId);
                var sharedCase = await _cbeContext.Cases
                    .FirstOrDefaultAsync(u => u.Id == createTaskManagmentDto.CaseId); */


                var caseList = selectedCaseIds.Split(',')
                    .Select(x => Guid.Parse(x.Trim()))
                    .ToList();

                if (caseList.Count == 0)
                    throw new ArgumentException("No valid case IDs provided.");

                var taskShares = new List<TaskManagment>();

                foreach (var caseId in caseList)
                {

                    var caseData = await _cbeContext.Cases
                        .FirstOrDefaultAsync(c => c.Id == caseId)
                        ?? throw new ArgumentException($"Case not found: {caseId}");

                    var task = _mapper.Map<TaskManagment>(createTaskManagmentDto);
                    task.Id = Guid.NewGuid();
                    task.CaseId = caseId;
                    task.TaskStatus = "New"; // Consider using an enum
                    task.AssignedDate = DateTime.UtcNow;
                    task.CaseOrginatorId = AssignorId;

                    await _cbeContext.TaskManagments.AddAsync(task);

                    await _cbeContext.TaskNotifications.AddAsync(new TaskNotification
                    {
                        TaskId = task.Id,
                        UserId = task.AssignedId,
                        Date = DateTime.UtcNow,
                        Notification = "New Task",
                        Status = "New"
                    });

                    await _caseTimeLineService.CreateCaseTimeLine(new CaseTimeLinePostDto
                    {
                        CaseId = caseId,
                        Activity = $"<strong>A {caseData.ApplicantName} applicant case has been shared to {asigneeUser.Role.Name} by {user.Role.Name}</strong>",
                        CurrentStage = user.Role.Name
                    });

                    taskShares.Add(task);
                }
/*
                    CaseId = task.CaseId,
                    Activity = $"<strong>A {sharedCase.ApplicantName} appicant case has been shared to {Asigneuser.Role.Name} by {user.Role.Name}</strong>",
                    CurrentStage = user.Role.Name
                }); */


                await _cbeContext.SaveChangesAsync();
                await transaction.CommitAsync();

                return taskShares.First(); // Returns first task; ensure list is not empty
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sharing task.");
                await transaction.RollbackAsync();
                throw new ApplicationException("An error occurred while sharing the task.", ex);
            }
        }
        
        public async Task LogTimelineEvent(Guid caseId, string activity, string currentStage)
        {
            await _caseTimeLineService.CreateCaseTimeLine(new CaseTimeLinePostDto
            {
                CaseId = caseId,
                Activity = activity,
                CurrentStage = currentStage
            });
        }

        private async Task<NotificationReturnDto> AssignCaseToUser(Case sharedCase, Guid userId, ShareTasksDto dto)
        {
            var task = new TaskManagment
            {
                Id = Guid.NewGuid(),
                CaseId = dto.CaseId,
                TaskName = dto.TaskName,
                PriorityType = dto.PriorityType,
                SharingReason = dto.SharingReason,
                Deadline = dto.Deadline,
                CaseOrginatorId = sharedCase.CaseOriginatorId,
                AssignedId = userId,
                TaskStatus = "New",
                AssignedDate = DateTime.UtcNow,
                CompletionDate = null,
            };

            await _cbeContext.TaskManagments.AddAsync(task);
            
            var assignedUser = await _userService.GetUserById(userId);
            var activity = $"<strong>A {sharedCase.ApplicantName} appicant case has been shared to {assignedUser.Role.Name} by {sharedCase.CaseOriginator.Role.Name}</strong>";
            await LogTimelineEvent(sharedCase.Id, activity, assignedUser.Role.Name);

            string notificationMessage = $"New task assigned: {task.TaskName}";
            var notification = await _notificationService.SendNotification(userId, notificationMessage, "Type", "");
            
            return notification;
        }

        private async Task<IEnumerable<NotificationReturnDto>> AssignCaseToUsers(Case sharedCase, IEnumerable<Guid> userIds, ShareTasksDto dto)
        {
            // Prepare task assignments in bulk
            var tasks = userIds.Select(userId => new TaskManagment
            {
                Id = Guid.NewGuid(),
                CaseId = dto.CaseId,
                TaskName = dto.TaskName,
                PriorityType = dto.PriorityType,
                SharingReason = dto.SharingReason,
                Deadline = dto.Deadline,
                CaseOrginatorId = sharedCase.CaseOriginatorId,
                AssignedId = userId,
                TaskStatus = "New",
                AssignedDate = DateTime.UtcNow,
                CompletionDate = null
            }).ToList();

            await _cbeContext.TaskManagments.AddRangeAsync(tasks);

            // // Send notifications & log timeline events concurrently
            // var logTimeLineTasks = tasks.Select(async task =>
            // {
            //     var assignedUser = await _userService.GetUserById(task.AssignedId);
            //     var activity = $"<strong>A {sharedCase.ApplicantName} applicant case has been shared to {assignedUser.Role.Name} by {sharedCase.CaseOriginator.Role.Name}</strong>";
            //     await LogTimelineEvent(sharedCase.Id, activity, assignedUser.Role.Name);
            // });

            // await Task.WhenAll(logTimeLineTasks);

            foreach (var task in tasks)
            {
                var assignedUser = await _userService.GetUserById(task.AssignedId);
                var activity = $"<strong>A {sharedCase.ApplicantName} applicant case has been shared to {assignedUser.Role.Name} by {sharedCase.CaseOriginator.Role.Name}</strong>";
                await LogTimelineEvent(sharedCase.Id, activity, assignedUser.Role.Name);
            }

            string notificationMessage = $"New task assigned: {dto.TaskName}";
            var notifications = await _notificationService.SendNotifications(userIds, notificationMessage, "Type", "");

            return notifications;
        }

        public async Task<ResultDto> ShareTasks(Guid userId, ShareTasksDto dto)
        {
            if (dto == null)
            {
                return new ResultDto { Success = false, Message = "Task management DTO cannot be null." };
            }

            using var transaction = await _cbeContext.Database.BeginTransactionAsync();
            try
            {
                // Encode/Sanitize inputs in Dto
                EncodingHelper.EncodeObject(dto);
                
                var sharedCase = await _caseService.GetCaseById(dto.CaseId);
                
                if (sharedCase == null)
                {
                    return new ResultDto { Success = false, Message = "Case not found." };
                }
                
                var notifications = await AssignCaseToUsers(sharedCase, dto.SelectedRMs, dto);
                await _cbeContext.SaveChangesAsync();
                await transaction.CommitAsync();
                await _notificationService.SendRealTimeNotifications(notifications);

                return new ResultDto { Success = true, Message = "Case is shared Successfully!" };
            }

            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sharing task.");
                await transaction.RollbackAsync();
                return new ResultDto { Success = false, Message = $"An error occurred while sharing the task. {ex}" };
            }
        }

        public async Task<IEnumerable<TaskManagmentReturnDto>> GetSharedTasks(Guid userId)
        {
            var tasks = await _cbeContext.TaskManagments.Include(t => t.Case).Include(t => t.Assigned).Where(res => res.CaseOrginatorId == userId).ToListAsync();
            return _mapper.Map<IEnumerable<TaskManagmentReturnDto>>(tasks);
        }

        public async Task<IEnumerable<TaskManagmentReturnDto>> GetReceivedTasks(Guid userId)
        {
            var tasks = await _cbeContext.TaskManagments.Include(t => t.Case).Include(t => t.CaseOrginator).Where(t => t.AssignedId==userId).ToListAsync();
            return _mapper.Map<IEnumerable<TaskManagmentReturnDto>>(tasks);
        }

        public async Task<TaskManagmentReturnDto> GetTask(Guid userId, Guid taskId)
        {
            var task = await _cbeContext.TaskManagments
                                        .Include(t => t.Case)
                                        .Include(t => t.Assigned)
                                        .Include(t => t.CaseOrginator)
                                        .FirstOrDefaultAsync(t => t.Id == taskId);
            
            return _mapper.Map<TaskManagmentReturnDto>(task);
        }

        public async Task<ResultDto> UpdateTask(Guid userId, UpdateTaskDto dto)
        {
            using var transaction = await _cbeContext.Database.BeginTransactionAsync();
            try
            {
                var task = await _cbeContext.TaskManagments
                                            .Include(t => t.Case)
                                            .Include(t => t.Assigned)
                                                .ThenInclude(u => u.Role)
                                            .Include(t => t.CaseOrginator)
                                            .FirstOrDefaultAsync(t => t.Id == dto.Id);

                if (task == null)
                {
                    return new ResultDto { Success = false, Message = "Task not found." };
                }

                task.TaskName = dto.TaskName;
                task.Deadline = dto.Deadline;
                task.PriorityType = dto.PriorityType;
                task.SharingReason = dto.SharingReason;

                _cbeContext.TaskManagments.Update(task);

                // Log timeline event
                var activity = $"Task '{task.TaskName}' updated by user {task.Assigned.Name}.";
                await LogTimelineEvent(task.CaseId, activity, task.Assigned.Role.Name);

                // Send notification to the assigned user
                string notificationMessage = $"Task '{task.TaskName}' has been updated.";
                var notification = await _notificationService.SendNotification(task.AssignedId, notificationMessage, "Type");

                await _cbeContext.SaveChangesAsync();
                await transaction.CommitAsync();
                await _notificationService.SendRealTimeNotification(notification);

                return new ResultDto { Success = true, Message = "Task updated successfully." };
            }

            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating task.");
                await transaction.RollbackAsync();
                return new ResultDto { Success = false, Message = $"An error occurred while updating the task. {ex}" };
            }
        }

        public async Task<ResultDto> ReassignTask(Guid userId, Guid taskId, Guid reassignedUserId)
        {
            using var transaction = await _cbeContext.Database.BeginTransactionAsync();
            try
            {
                var task = await _cbeContext.TaskManagments
                                            .Include(t => t.Case)
                                            .Include(t => t.Assigned)
                                                .ThenInclude(u => u.Role)
                                            .Include(t => t.CaseOrginator)
                                            .FirstOrDefaultAsync(t => t.Id == taskId);

                if (task == null)
                {
                    return new ResultDto { Success = false, Message = "Task not found." };
                }

                var newAssignedUser = await _userService.GetUserById(reassignedUserId);
                if (newAssignedUser == null)
                {
                    return new ResultDto { Success = false, Message = "User not found." };
                }

                var previousAssignedUser = task.Assigned.Name;
                task.AssignedId = newAssignedUser.Id;
                task.AssignedDate = DateTime.UtcNow;
                // task.TaskStatus = "In Progress";

                _cbeContext.TaskManagments.Update(task);

                // Log timeline event
                var activity = $"Task '{task.TaskName}' reassigned from user {previousAssignedUser} to {newAssignedUser.Name}.";
                await LogTimelineEvent(task.CaseId, activity, newAssignedUser.Role.Name);

                // Send notification to the new assigned user
                string notificationMessage = $"Task reassigned: '{task.TaskName}' has been reassigned to you.";
                var notification = await _notificationService.SendNotification(
                    userId: newAssignedUser.Id,
                    message: notificationMessage,
                    type: "task",
                    link: $"/taskmanagments/{taskId}"
                );

                await _cbeContext.SaveChangesAsync();
                await transaction.CommitAsync();
                await _notificationService.SendRealTimeNotification(notification);

                return new ResultDto { Success = true, Message = "Task reassigned successfully." };
            }

            catch (Exception ex)
            {
                _logger.LogError(ex, "Error reassigning task.");
                await transaction.RollbackAsync();
                return new ResultDto { Success = false, Message = $"An error occurred while reassigning the task. {ex}" };
            }
        }

        public async Task<ResultDto> DeleteTask(Guid userId, Guid taskId)
        {
            using var transaction = await _cbeContext.Database.BeginTransactionAsync();
            try
            {
                var task = await _cbeContext.TaskManagments
                                            .Include(t => t.Case)
                                            .Include(t => t.Assigned)
                                                .ThenInclude(u => u.Role)
                                            .FirstOrDefaultAsync(t => t.Id == taskId);

                if (task == null)
                {
                    return new ResultDto { Success = false, Message = "Task not found." };
                }

                _cbeContext.TaskManagments.Remove(task);

                // Log timeline event
                var user = await _userService.GetUserById(userId);
                var activity = $"Task '{task.TaskName}' deleted by user {user.Name}.";
                await LogTimelineEvent(task.CaseId, activity, task.Assigned.Role.Name);

                // Send notification to the assigned user
                string notificationMessage = $"Task '{task.TaskName}' has been deleted.";
                var notification = await _notificationService.SendNotification(task.AssignedId, notificationMessage, "Type");

                await _cbeContext.SaveChangesAsync();
                await transaction.CommitAsync();
                await _notificationService.SendRealTimeNotification(notification);

                return new ResultDto { Success = true, Message = "Task deleted successfully." };
            }

            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting task.");
                await transaction.RollbackAsync();
                return new ResultDto { Success = false, Message = $"An error occurred while deleting the task. {ex}" };
            }
        }

        public async Task<ResultDto> CompleteTask(Guid userId, Guid taskId)
        {
            using var transaction = await _cbeContext.Database.BeginTransactionAsync();
            try
            {
                var task = await _cbeContext.TaskManagments
                                            .Include(t => t.Case)
                                            .Include(t => t.Assigned)
                                                .ThenInclude(u => u.Role)
                                            .Include(t => t.CaseOrginator)
                                            .FirstOrDefaultAsync(t => t.Id == taskId);

                if (task == null)
                {
                    return new ResultDto { Success = false, Message = "Task not found." };
                }

                task.TaskStatus = "Completed";
                task.CompletionDate = DateTime.UtcNow;

                _cbeContext.TaskManagments.Update(task);

                // Log timeline event
                var user = await _userService.GetUserById(userId);
                var activity = $"Task '{task.TaskName}' marked as completed by user {user.Name}.";
                await LogTimelineEvent(task.CaseId, activity, task.Assigned.Role.Name);

                // Send notification to the assigned user
                string notificationMessage = $"Task '{task.TaskName}' has been marked as completed.";
                var notification = await _notificationService.SendNotification(task.AssignedId, notificationMessage, "Type");

                await _cbeContext.SaveChangesAsync();
                await transaction.CommitAsync();
                await _notificationService.SendRealTimeNotification(notification);

                return new ResultDto { Success = true, Message = "Task marked as completed." };
            }

            catch (Exception ex)
            {
                _logger.LogError(ex, "Error completing task.");
                await transaction.RollbackAsync();
                return new ResultDto { Success = false, Message = $"An error occurred while completing the task. {ex}" };
            }
        }

        public async Task<TaskComment> CommentTask(Guid userId, TaskCommentPostDto dto)
        {
            using var transaction = await _cbeContext.Database.BeginTransactionAsync();
            try
            {
                EncodingHelper.EncodeObject(dto);


                var comment = _mapper.Map<TaskComment>(dto);
                comment.Id = Guid.NewGuid();
                comment.CommenterId = userId; // userId is already a Guid
                comment.CommenteeId = dto.CommenteeId; // Consider using an enum
                comment.TaskId = dto.TaskId;
                comment.Comment = dto.Comment;
                comment.CommentDate = DateTime.UtcNow; // Use UtcNow for consistency

                //  await _cbeContext.TaskManagments.AddAsync(comment);
                await _cbeContext.SaveChangesAsync();
                await transaction.CommitAsync();
                return comment; // Returns first task; ensure list is not empty

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sharing task.");
                await transaction.RollbackAsync();
                throw new ApplicationException("An error occurred while sharing the task.", ex);
            }

        }

        public async Task<IEnumerable<TaskCommentReturnDto>> GetTaskComment(Guid userId, Guid taskId)
        {
            var comments = await _cbeContext.TaskComments.Where(u => (u.CommenterId == userId && u.TaskId == taskId) || (u.CommenteeId == userId && u.TaskId == taskId)).ToListAsync();
            return _mapper.Map<IEnumerable<TaskCommentReturnDto>>(comments);
        }

    }
}
