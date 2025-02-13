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
                    task.AssignedDate = DateTime.Now;
                    task.CaseOrginatorId = AssignorId;

                    await _cbeContext.TaskManagments.AddAsync(task);

                    await _cbeContext.TaskNotifications.AddAsync(new TaskNotification
                    {
                        TaskId = task.Id,
                        UserId = task.AssignedId,
                        Date = DateTime.Now,
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

        public async Task<TaskManagment> UpdateTask(Guid AssignorId, Guid AssigneeId, Guid TaskId, TaskManagmentUpdateDto updateTaskManagmentDto)
        {
            throw new NotImplementedException();
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

        public async Task AssignCaseToUsers(Case sharedCase, IEnumerable<Guid> userIds, ShareTasksDto dto)
        {
            foreach (var userId in userIds)
            {
                
                var assignedUser = await _userService.GetUserById(userId);
                // Map DTO to TaskManagment entity
                // var task = _mapper.Map<TaskManagment>(dto);
                var task = new TaskManagment
                {
                    Id = Guid.NewGuid(),
                    CaseId = sharedCase.Id,
                    TaskName = dto.TaskName,
                    PriorityType = dto.PriorityType,
                    SharingReason = dto.SharingReason,
                    Deadline = dto.Deadline,
                    CaseOrginatorId = sharedCase.CaseOriginatorId,
                    AssignedId = userId,
                    TaskStatus = "New",
                    AssignedDate = DateTime.Now,
                    CompletionDate = null,
                };

                await _cbeContext.TaskManagments.AddAsync(task);

                string notificationMessage = $"New task assigned: {task.TaskName}";
                await _notificationService.SendNotification(userId, notificationMessage);
                
                var activity = $"<strong>A {sharedCase.ApplicantName} appicant case has been shared to {assignedUser.Role.Name} by {sharedCase.CaseOriginator.Role.Name}</strong>";
                await LogTimelineEvent(sharedCase.Id, activity, assignedUser.Role.Name);
            }

            await _cbeContext.SaveChangesAsync();
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
                
                await AssignCaseToUsers(sharedCase, dto.SelectedRMs, dto);

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

        public async Task<IEnumerable<TaskManagmentReturnDto>> GetAssignedTasks(Guid userId)
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

        public async Task<ResultDto> ReassignTask(Guid userId, Guid taskId, Guid newAssignedId)
        {
            var task = await _cbeContext.TaskManagments.FindAsync(taskId);
            if (task == null)
            {
                return new ResultDto { Success = false, Message = "Task not found." };
            }

            var user =  await _cbeContext.CreateUsers.FindAsync(newAssignedId);
            if (user == null)
            {
                return new ResultDto { Success = false, Message = "User not found." };
            }

            task.Assigned = user;
            task.AssignedId = newAssignedId;
            task.AssignedDate = DateTime.UtcNow;
            // task.TaskStatus = "In Progress";

            _cbeContext.TaskManagments.Update(task);
            await _cbeContext.SaveChangesAsync();
            
            return new ResultDto { Success = true, Message = "Task reassigned successfully." };
        }

        public async Task<ResultDto> DeleteTask(Guid userId, Guid taskId)
        {
            var task = await _cbeContext.TaskManagments.FindAsync(taskId);
            if (task == null)
            {
                return new ResultDto { Success = false, Message = "Task not found." };
            }

            _cbeContext.TaskManagments.Remove(task);
            await _cbeContext.SaveChangesAsync();

            return new ResultDto { Success = true, Message = "Task deleted successfully." };
        }

        public async Task<ResultDto> CompleteTask(Guid userId, Guid taskId)
        {
            var task = await _cbeContext.TaskManagments.FindAsync(taskId);
            if (task == null)
            {
                return new ResultDto { Success = false, Message = "Task not found." };
            }
            task.TaskStatus = "Completed";

            _cbeContext.TaskManagments.Update(task);
            await _cbeContext.SaveChangesAsync();

            return new ResultDto { Success = true, Message = "Task marked as completed." };
        }

        public async Task<ResultDto> UpdateTask(Guid userId, UpdateTaskDto dto)
        {
            var task = await _cbeContext.TaskManagments.FindAsync(dto.Id);

            if (task == null)
            {
                return new ResultDto { Success = false, Message = "Task not found." };
            }

            task.TaskName = dto.TaskName;
            task.Deadline = dto.Deadline;
            task.PriorityType = dto.PriorityType;
            task.SharingReason = dto.SharingReason;

            _cbeContext.TaskManagments.Update(task);
            await _cbeContext.SaveChangesAsync();
            
            return new ResultDto { Success = true, Message = "Task updated successfully." };
        }
    }
}
