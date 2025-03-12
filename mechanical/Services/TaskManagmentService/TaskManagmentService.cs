using AutoMapper;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Concurrent;

using mechanical.Data;
using mechanical.Hubs;
using mechanical.Utils;
using mechanical.Models.Dto;
using mechanical.Models.Entities;
using mechanical.Models.Dto.CaseDto;
using mechanical.Models.Dto.CaseTimeLineDto;
using mechanical.Models.Dto.NotificationDto;
using mechanical.Models.Dto.TaskManagmentDto;
using mechanical.Models.Dto.CaseAssignmentDto;
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

        public async Task<TaskManagment> ShareTask(Guid AssignorId, string selectedCaseIds, TaskManagmentPostDto createTaskManagmentDto)

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
                var currentDate = DateTime.UtcNow;
                foreach (var caseId in caseList)
                {
                    var caseData = await _cbeContext.Cases
                        .FirstOrDefaultAsync(c => c.Id == caseId)
                        ?? throw new ArgumentException($"Case not found: {caseId}");

                    var taskData = await _cbeContext.TaskManagments
                        .Where(c => c.CaseId == caseId
                            && c.AssignedId == createTaskManagmentDto.AssignedId
                            && c.CaseOrginatorId == AssignorId
                            && c.TaskName == createTaskManagmentDto.TaskName
                            && c.IsActive
                            && c.Deadline < currentDate)
                        .ToListAsync();

                    if (taskData.Any())
                    {
                        var taskName = createTaskManagmentDto.TaskName;
                        // Log the message if needed
                        _logger.LogInformation($"The '{taskName}' task is already added for case {caseId}. Skipping to the next case.");
                        continue; // Skip to the next case in the loop
                    }
                    else
                    {
                        var task = _mapper.Map<TaskManagment>(createTaskManagmentDto);
                        task.Id = Guid.NewGuid();
                        task.CaseId = caseId;
                        task.TaskStatus = "New"; // Consider using an enum
                        task.AssignedDate = DateTime.UtcNow;
                        task.CaseOrginatorId = AssignorId;
                        
                        // task.TaskName = dto.TaskName;
                        // task.PriorityType = dto.PriorityType;
                        // task.SharingReason = dto.SharingReason;
                        // task.Deadline = dto.Deadline;
                        task.AssignedId = asigneeUser.Id;
                        task.IsActive = true;
                        task.UpdatedDate = null;
                        task.CompletionDate = null;

                        await _cbeContext.TaskManagments.AddAsync(task);

                        await _caseTimeLineService.CreateCaseTimeLine(new CaseTimeLinePostDto
                        {
                            CaseId = caseId,
                            Activity = $"<strong>A case number {caseData.CaseNo} of '{createTaskManagmentDto.TaskName}' has been shared with {asigneeUser.Role.Name} by {user.Role.Name}</strong>",
                            CurrentStage = user.Role.Name
                        });

                    string notificationContent = $"New task assigned: {createTaskManagmentDto.TaskName}";
                    var notification = await _notificationService.AddNotification(AssignorId, notificationContent, "Task", $"/Taskmanagment/Detail/{task.Id}");

                        taskShares.Add(task);
                        await _notificationService.SendNotification(notification);
                    }
                }

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

        private async Task<(IEnumerable<NotificationReturnDto> notifications, List<ResultDto> assignmentResults)> AssignCaseToUsers(Case sharedCase, ShareTasksDto dto)
        {
            var assignmentResults = new List<ResultDto>();
            var shareTasks = new List<TaskManagment>();

            foreach (var userId in dto.SelectedRMs)
            {
                var existingTask = await _cbeContext.TaskManagments
                    .AnyAsync(t => t.CaseId == sharedCase.Id &&
                                t.AssignedId == userId &&
                                t.TaskName == dto.TaskName &&
                                t.IsActive &&
                                t.Deadline > DateTime.UtcNow);

                if (existingTask)
                {
                    assignmentResults.Add(new ResultDto
                    {
                        StatusCode = 200,
                        Success = false,
                        Message = $"Task '{dto.TaskName}' is already assigned to user {userId} and is still active with a valid deadline."
                    });
                    continue;
                }

                shareTasks.Add(new TaskManagment
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
                    IsActive = true,
                    AssignedDate = DateTime.UtcNow,
                    UpdatedDate = null,
                    CompletionDate = null
                });
            }

            // // Prepare task assignments in bulk
            // var shareTasks = dto.SelectedRMs
            //     .Where(userId => !_cbeContext.TaskManagments
            //         .Any(t => t.CaseId == sharedCase.Id &&
            //                 t.AssignedId == userId &&
            //                 t.TaskName == dto.TaskName &&
            //                 t.IsActive &&
            //                 t.Deadline > DateTime.UtcNow))
            //     .Select(userId => new TaskManagment
            //     {
            //         Id = Guid.NewGuid(),
            //         CaseId = dto.CaseId,
            //         TaskName = dto.TaskName,
            //         PriorityType = dto.PriorityType,
            //         SharingReason = dto.SharingReason,
            //         Deadline = dto.Deadline,
            //         CaseOrginatorId = sharedCase.CaseOriginatorId,
            //         AssignedId = userId,
            //         TaskStatus = "New",
            //         IsActive = true,
            //         AssignedDate = DateTime.UtcNow,
            //         UpdatedDate = null,
            //         CompletionDate = null
            //     })
            //     .ToList();

            // Add tasks to the database in bulk
            await _cbeContext.TaskManagments.AddRangeAsync(shareTasks);
            await _cbeContext.SaveChangesAsync();

            // Retrieve the saved tasks with their related entities
            var tasks = await _cbeContext.TaskManagments
                .Include(t => t.Case)
                .Include(t => t.Assigned)
                    .ThenInclude(u => u.Role)
                .Where(t => shareTasks.Select(st => st.Id).Contains(t.Id))
                .ToListAsync();

            // Prepare notifications
            var notificationsBatch = new List<(Guid userId, string content, string type, string link)>();
            foreach (var task in tasks)
            {
                var activity = $"<strong>A {sharedCase.ApplicantName} applicant case has been shared to {task.Assigned.Role.Name} by {sharedCase.CaseOriginator.Role.Name}</strong>";
                await LogTimelineEvent(task.CaseId, activity, task.Assigned.Role.Name);

                string notificationContent = $"New task assigned: {dto.TaskName}";
                notificationsBatch.Add((task.AssignedId, notificationContent, "Task", $"/Case/Detail/{task.CaseId}"));

                assignmentResults.Add(new ResultDto
                {
                    StatusCode = 200,
                    Success = true,
                    Message = $"Task '{task.TaskName}' is already assigned to user {task.AssignedId} and is still active with a valid deadline."
                });
            }

            var notifications = await _notificationService.AddNotifications(notificationsBatch);

            return (notifications, assignmentResults);
        }

        public async Task<List<ResultDto>> ShareTasks(Guid userId, ShareTasksDto dto)
        {
            var result = new List<ResultDto>();

            if (dto == null)
            {
                result.Add(new ResultDto { StatusCode = 500, Success = false, Message = "Task management DTO cannot be null." });
                return result;
            }

            using var transaction = await _cbeContext.Database.BeginTransactionAsync();
            try
            {
                // Encode/Sanitize inputs in Dto
                EncodingHelper.EncodeObject(dto);

                var sharedCase = await _caseService.GetCaseById(dto.CaseId);

                if (sharedCase == null)
                {
                    result.Add(new ResultDto { StatusCode = 404, Success = false, Message = "Case not found." });
                    return result;
                }

                if (sharedCase.CaseOriginatorId != userId)
                {
                    result.Add(new ResultDto { StatusCode = 403, Success = false, Message = "Access denied! User cannot share this task (not owner)." });
                    return result;
                }

                var (notifications, assignmentResults) = await AssignCaseToUsers(sharedCase, dto);

                await _cbeContext.SaveChangesAsync();
                await transaction.CommitAsync();

                await _notificationService.SendNotifications(notifications);

                return assignmentResults;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sharing task.");
                await transaction.RollbackAsync();
                result.Add(new ResultDto { StatusCode = 500, Success = false, Message = $"An error occurred while sharing the task. {ex}" });
                return result;
            }
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

        public async Task<IEnumerable<TaskManagmentReturnDto>> GetSharedTasks(Guid userId, string? mode = null)
        {
            var tasks = await _cbeContext.TaskManagments
                                        .Include(t => t.Case)
                                        .Include(t => t.Assigned)
                                        .Where(res => res.CaseOrginatorId == userId &&
                                                    (string.IsNullOrEmpty(mode) || mode.Equals("all", StringComparison.OrdinalIgnoreCase) ||
                                                    mode.Equals("active", StringComparison.OrdinalIgnoreCase) && res.IsActive ||
                                                    mode.Equals("inactive", StringComparison.OrdinalIgnoreCase) && !res.IsActive))
                                        .ToListAsync();

            return _mapper.Map<IEnumerable<TaskManagmentReturnDto>>(tasks);
        }

        public async Task<IEnumerable<TaskManagmentReturnDto>> GetReceivedTasks(Guid userId, string? mode = null)
        {
            var tasks = await _cbeContext.TaskManagments
                                        .Include(t => t.Case)
                                        .Include(t => t.CaseOrginator)
                                        .Where(res => res.AssignedId == userId &&
                                                    (string.IsNullOrEmpty(mode) || mode.Equals("all", StringComparison.OrdinalIgnoreCase) ||
                                                    mode.Equals("active", StringComparison.OrdinalIgnoreCase) && res.IsActive ||
                                                    mode.Equals("inactive", StringComparison.OrdinalIgnoreCase) && !res.IsActive))
                                        .ToListAsync();

            return _mapper.Map<IEnumerable<TaskManagmentReturnDto>>(tasks);
        }

        public async Task<ResultDto> UpdateTask(Guid userId, TaskManagmentUpdateDto dto)
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
                    return new ResultDto { StatusCode = 404, Success = false, Message = "Task not found." };
                }

                if (task.CaseOrginatorId != userId)
                {
                    return new ResultDto { StatusCode = 403, Success = false, Message = "Access denied! User cannot update this task." };
                }

                // task.TaskName = dto.TaskName;
                task.Deadline = dto.Deadline;
                task.PriorityType = dto.PriorityType;
                task.SharingReason = dto.SharingReason;
                task.UpdatedDate = DateTime.UtcNow;

                _cbeContext.TaskManagments.Update(task);

                // Log timeline event
                var activity = $"Task '{task.TaskName}' updated by user {task.Assigned.Name}.";
                await LogTimelineEvent(task.CaseId, activity, task.Assigned.Role.Name);

                // Send notification to the assigned user
                string notificationContent = $"Task '{task.TaskName}' has been updated.";
                var notification = await _notificationService.AddNotification(task.AssignedId, notificationContent, "Task", $"/Taskmanagment/Detail/{task.Id}");

                await _cbeContext.SaveChangesAsync();
                await transaction.CommitAsync();
                await _notificationService.SendNotification(notification);

                return new ResultDto { StatusCode = 200, Success = true, Message = "Task updated successfully." };
            }

            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating task.");
                await transaction.RollbackAsync();
                return new ResultDto { StatusCode = 500, Success = false, Message = $"An error occurred while updating the task. {ex}" };
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
                    return new ResultDto { StatusCode = 404, Success = false, Message = "Task not found." };
                }

                if (task.AssignedId != userId)
                {
                    return new ResultDto { StatusCode = 403, Success = false, Message = "Access denied! User cannot complete this task (not assigne)." };
                }

                task.TaskStatus = "Completed";
                task.CompletionDate = DateTime.UtcNow;

                _cbeContext.TaskManagments.Update(task);

                // Log timeline event
                var user = await _userService.GetUserById(userId);
                var activity = $"Task '{task.TaskName}' marked as completed by user {user.Name}.";
                await LogTimelineEvent(task.CaseId, activity, task.Assigned.Role.Name);

                // Send notification to the assigned user
                string notificationContent = $"Task '{task.TaskName}' has been marked as completed.";
                var notification = await _notificationService.AddNotification(task.AssignedId, notificationContent, "Task", $"/Taskmanagment/Detail/{task.Id}");

                await _cbeContext.SaveChangesAsync();
                await transaction.CommitAsync();
                await _notificationService.SendNotification(notification);

                return new ResultDto { StatusCode = 200, Success = true, Message = "Task marked as completed." };
            }

            catch (Exception ex)
            {
                _logger.LogError(ex, "Error completing task.");
                await transaction.RollbackAsync();
                return new ResultDto { StatusCode = 500, Success = false, Message = $"An error occurred while completing the task. {ex}" };
            }
        }

        // public async Task<ResultDto> ReassignTask(Guid userId, Guid taskId, Guid reassignedUserId)
        // {
        //     using var transaction = await _cbeContext.Database.BeginTransactionAsync();
        //     try
        //     {
        //         var task = await _cbeContext.TaskManagments
        //                                     .Include(t => t.Case)
        //                                     .Include(t => t.Assigned)
        //                                         .ThenInclude(u => u.Role)
        //                                     .Include(t => t.CaseOrginator)
        //                                     .FirstOrDefaultAsync(t => t.Id == taskId);

        //         if (task == null)
        //         {
        //             return new ResultDto { StatusCode = 404, Success = false, Message = "Task not found." };
        //         }

        //         if (task.CaseOrginatorId != userId)
        //         {
        //             return new ResultDto { StatusCode = 403, Success = false, Message = "Access denied! User cannot reassign this task (not owner)." };
        //         }

        //         var newAssignedUser = await _userService.GetUserById(reassignedUserId);
        //         if (newAssignedUser == null)
        //         {
        //             return new ResultDto { StatusCode = 404, Success = false, Message = "User not found." };
        //         }

        //         var previousAssignedUser = task.Assigned.Name;
        //         task.AssignedId = newAssignedUser.Id;
        //         task.AssignedDate = DateTime.UtcNow;
        //         // task.TaskStatus = "In Progress";

        //         _cbeContext.TaskManagments.Update(task);

        //         // Log timeline event
        //         var activity = $"Task '{task.TaskName}' reassigned from user {previousAssignedUser} to {newAssignedUser.Name}.";
        //         await LogTimelineEvent(task.CaseId, activity, newAssignedUser.Role.Name);

        //         // Send notification to the new assigned user
        //         string notificationContent = $"Task reassigned: '{task.TaskName}' has been reassigned to you.";
        //         var notification = await _notificationService.AddNotification(newAssignedUser.Id, notificationContent, "Task", $"/Taskmanagment/Detail/{taskId}"
        //         );

        //         await _cbeContext.SaveChangesAsync();
        //         await transaction.CommitAsync();
        //         await _notificationService.SendNotification(notification);

        //         return new ResultDto { StatusCode = 200, Success = true, Message = "Task reassigned successfully." };
        //     }

        //     catch (Exception ex)
        //     {
        //         _logger.LogError(ex, "Error reassigning task.");
        //         await transaction.RollbackAsync();
        //         return new ResultDto { StatusCode = 500, Success = false, Message = $"An error occurred while reassigning the task. {ex}" };
        //     }
        // }

        // public async Task<ResultDto> DeleteTask(Guid userId, Guid taskId)
        // {
        //     using var transaction = await _cbeContext.Database.BeginTransactionAsync();
        //     try
        //     {
        //         var task = await _cbeContext.TaskManagments
        //                                     .Include(t => t.Case)
        //                                     .Include(t => t.Assigned)
        //                                         .ThenInclude(u => u.Role)
        //                                     .FirstOrDefaultAsync(t => t.Id == taskId);

        //         if (task == null)
        //         {
        //             return new ResultDto { StatusCode = 404, Success = false, Message = "Task not found." };
        //        }

        //         if (task.CaseOrginatorId != userId)
        //         {
        //             return new ResultDto { StatusCode = 403, Success = false, Message = "Access denied! User cannot delete this task (not owner)." };
        //         }
        //         _cbeContext.TaskManagments.Remove(task);

        //         // Log timeline event
        //         var user = await _userService.GetUserById(userId);
        //         var activity = $"Task '{task.TaskName}' deleted by user {user.Name}.";
        //         await LogTimelineEvent(task.CaseId, activity, task?.Assigned?.Role?.Name);

        //         // Send notification to the assigned user
        //         string notificationContent = $"Task '{task.TaskName}' has been deleted.";
        //         var notification = await _notificationService.AddNotification(task.AssignedId, notificationContent, "Task", $"/Taskmanagment/Detail/{task.Id}");

        //         await _cbeContext.SaveChangesAsync();
        //         await transaction.CommitAsync();
        //         await _notificationService.SendNotification(notification);

        //         return new ResultDto { StatusCode = 200, Success = true, Message = "Task deleted successfully." };
        //     }

        //     catch (Exception ex)
        //     {
        //         _logger.LogError(ex, "Error deleting task.");
        //         await transaction.RollbackAsync();
        //         return new ResultDto { StatusCode = 500, Success = false, Message = $"An error occurred while deleting the task. {ex}" };
        //     }
        // }
        public async Task<ResultDto> RevokeTask(Guid userId, Guid taskId)
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
                    return new ResultDto { StatusCode = 404, Success = false, Message = "Task not found." };
                }

                if (task.CaseOrginatorId != userId)
                {
                    return new ResultDto { StatusCode = 403, Success = false, Message = "Access denied! User cannot revoke this task (not owner)." };
                }
                task.IsActive = false;
                _cbeContext.TaskManagments.Update(task);

                // Log timeline event
                var user = await _userService.GetUserById(userId);
                var activity = $"Task '{task.TaskName}' revoked by user {user.Name}.";
                await LogTimelineEvent(task.CaseId, activity, task?.Case?.CaseOriginator?.Name);

                // Send notification to the assigned user
                string notificationContent = $"Task '{task.TaskName}' has been revoked.";
                var notification = await _notificationService.AddNotification(userId, notificationContent, "Task", $"/Taskmanagment/Detail/{task.Id}");

                // Send notification to the case assigned
                string notificationContentAssigned = $"Task '{task.TaskName}' has been revoked by {user.Name}.";
                var notificationOwner = await _notificationService.AddNotification(task.AssignedId, notificationContentAssigned, "Task", $"/Taskmanagment/Detail/{task.Id}");

                await _cbeContext.SaveChangesAsync();
                await transaction.CommitAsync();
                await _notificationService.SendNotification(notification);

                return new ResultDto { StatusCode = 200, Success = true, Message = "Task revoked successfully." };
            }

            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting task.");
                await transaction.RollbackAsync();
                return new ResultDto { StatusCode = 500, Success = false, Message = $"An error occurred while deleting the task. {ex}" };
            }
        }

        public async Task<ResultDto> ReturnTask(Guid userId, Guid taskId)
        {
            using var transaction = await _cbeContext.Database.BeginTransactionAsync();
            try
            {
                var task = await _cbeContext.TaskManagments
                    .Include(t => t.Case)
                        .ThenInclude(c => c.CaseOriginator)
                    .FirstOrDefaultAsync(t => t.Id == taskId);

                if (task == null)
                {
                    return new ResultDto { StatusCode = 404, Success = false, Message = "Task not found." };
                }

                if (task.AssignedId != userId)
                {
                    return new ResultDto { StatusCode = 403, Success = false, Message = "Access denied! User cannot return this task (not assignee)." };
                }

                task.TaskStatus = "Returned";
                _cbeContext.TaskManagments.Update(task);

                // Log timeline event
                var user = await _userService.GetUserById(userId);
                var activity = $"Task '{task.TaskName}' returned by user {user.Name}.";
                await LogTimelineEvent(task.CaseId, activity, task?.Case?.CaseOriginator?.Name);

                // Send notification to the assigned user
                string notificationContent = $"Task '{task.TaskName}' has been returned.";
                var notification = await _notificationService.AddNotification(userId, notificationContent, "Task", $"/Taskmanagment/Detail/{task.Id}");

                // Send notification to the case owner
                string notificationContentOwner = $"Task '{task.TaskName}' has been returned by {user.Name}.";
                var notificationOwner = await _notificationService.AddNotification(task.CaseOrginatorId, notificationContentOwner, "Task", $"/Taskmanagment/Detail/{task.Id}");

                await _cbeContext.SaveChangesAsync();
                await transaction.CommitAsync();
                await _notificationService.SendNotification(notification);

                return new ResultDto { StatusCode = 200, Success = true, Message = "Task returned successfully." };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error returning task.");
                await transaction.RollbackAsync();
                return new ResultDto { StatusCode = 500, Success = false, Message = $"An error occurred while returning the task. {ex}" };
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
                comment.UserId = userId;
                comment.TaskId = dto.TaskId;
                comment.Comment = dto.Comment;
                comment.CommentDate = DateTime.UtcNow;
                await _cbeContext.TaskComments.AddAsync(comment);
                await _cbeContext.SaveChangesAsync();
                await transaction.CommitAsync();
                return comment;

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sharing task.");
                await transaction.RollbackAsync();
                throw new ApplicationException("An error occurred while sharing the task.", ex);
            }
        }

        public async Task<IEnumerable<TaskCommentReturnDto>> GetTaskComment(Guid taskId)
        {
            var comments = await _cbeContext.TaskComments
                                            .Include(res=>res.User)
                                            .Where(t=>t.TaskId == taskId)
                                            .OrderBy(d => d.CommentDate)
                                            .ToListAsync();
            return _mapper.Map<IEnumerable<TaskCommentReturnDto>>(comments);
        }

    }
}
