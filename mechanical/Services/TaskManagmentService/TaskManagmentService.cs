using AutoMapper;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

using mechanical.Data;
using mechanical.Hubs;
using mechanical.Utils;
using mechanical.Models.Entities;
using mechanical.Models.Dto.CaseTimeLineDto;
using mechanical.Models.Dto.TaskManagmentDto;
using mechanical.Services.CaseTimeLineService;

namespace mechanical.Services.TaskManagmentService
{
    public class TaskManagmentService : ITaskManagmentService
    {
        private readonly CbeContext _cbeContext;
        private readonly IHubContext<NotificationHub> _hubContext;
        private readonly IMapper _mapper;      
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<TaskManagmentService> _logger;      
        private readonly ICaseTimeLineService _caseTimeLineService;
        
        public TaskManagmentService(CbeContext cbeContext, IHubContext<NotificationHub> hubContext, IMapper mapper, ILogger<TaskManagmentService> logger, IHttpContextAccessor httpContextAccessor,  ICaseTimeLineService caseTimeLineService)
        {
            _cbeContext = cbeContext;
            _hubContext = hubContext;
            _mapper = mapper;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
            _caseTimeLineService = caseTimeLineService;
        }

        public async Task<TaskManagment> ShareTask(Guid selectedCaseIds, TaskManagmentPostDto createTaskManagmentDto)
        {
            if (createTaskManagmentDto == null)
            {
                throw new ArgumentNullException(nameof(createTaskManagmentDto), "Task management DTO cannot be null.");
            }

            using var transaction = await _cbeContext.Database.BeginTransactionAsync();
            try
            {
                // Encode/Sanitize inputs in Dto to avoid unsafe data being saved
                EncodingHelper.EncodeObject(createTaskManagmentDto);

                // Fetch the user details
                var user = await _cbeContext.CreateUsers
                    .Include(u => u.District)
                    .Include(u => u.Role)
                    .FirstOrDefaultAsync(u => u.Id == createTaskManagmentDto.CaseOrginatorId);
                var Asigneuser = await _cbeContext.CreateUsers
                    .Include(u => u.District)
                    .Include(u => u.Role)
                    .FirstOrDefaultAsync(u => u.Id == createTaskManagmentDto.AssignedId);
                var sharedCase = await _cbeContext.Cases                    
                    .FirstOrDefaultAsync(u => u.Id == createTaskManagmentDto.CaseId);

                if (user == null)
                {
                    throw new KeyNotFoundException("User not found.");
                }

                // Map DTO to TaskManagment entity
                var task = _mapper.Map<TaskManagment>(createTaskManagmentDto);
                task.Id = Guid.NewGuid();
                task.CaseId = createTaskManagmentDto.CaseId;
                task.TaskName = createTaskManagmentDto.TaskName;
                task.PriorityType = createTaskManagmentDto.PriorityType;
                task.SharingReason = createTaskManagmentDto.SharingReason;
                task.CompletionDate = createTaskManagmentDto.Deadline;
                task.CaseOrginatorId = user.Id;
                task.AssignedId = createTaskManagmentDto.AssignedId;
                task.TaskStatus = "New"; // Use an enum or constant
                task.AssignedDate = DateTime.Now;
                task.Deadline = createTaskManagmentDto.Deadline;

                // Add task to the context
                await _cbeContext.TaskManagments.AddAsync(task);

                // Create a task notification
                var taskNotification = new TaskNotification
                {
                    TaskId = task.Id,
                    UserId = task.AssignedId,
                    Date = DateTime.Now,
                    Notification = "New Task",
                    Status ="New" // Use an enum or constant
                };
                await _cbeContext.TaskNotifications.AddAsync(taskNotification);

                // Log the timeline event
                await _caseTimeLineService.CreateCaseTimeLine(new CaseTimeLinePostDto
                {
                    CaseId = task.CaseId,
                    Activity = $"<strong>A {sharedCase.ApplicantName} appicant case has been shared to {Asigneuser.Role.Name} by {user.Role.Name}</strong>",
                    CurrentStage = user.Role.Name
                });

                // Save changes and commit the transaction
                await _cbeContext.SaveChangesAsync();
                await transaction.CommitAsync();

                return task;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sharing task.");
                await transaction.RollbackAsync();
                throw new ApplicationException("An error occurred while sharing the task.", ex);
            }
        }

        public async Task<bool> DeleteTask(Guid AssignorId, Guid Id)
        {
            throw new NotImplementedException();
        }

        public async Task<TaskManagmentReturnDto> GetSharedTask(Guid AssignorId)
        {
            throw new NotImplementedException();
        }

        public Task<TaskManagmentReturnDto> GetTaskDetails(Guid AssignorId, Guid Id)
        {
            throw new NotImplementedException();
        }

        public async Task<TaskManagment> UpdateTask(Guid AssignorId, Guid AssigneeId, Guid TaskId, TaskManagmentUpdateDto updateTaskManagmentDto)
        {
            throw new NotImplementedException();
        }

        
        
        public async Task<bool> ShareTasks(Guid userId, ShareTasksDto dto)
        {
            if (dto == null)
            {
                throw new ArgumentNullException(nameof(dto), "Task management DTO cannot be null.");
            }

            using var transaction = await _cbeContext.Database.BeginTransactionAsync();
            try
            {
                // Encode/Sanitize inputs in Dto to avoid unsafe data being saved
                EncodingHelper.EncodeObject(dto);

                var sharedCase = await _cbeContext.Cases
                                                .Include(c => c.CaseOriginator)
                                                    .ThenInclude(u => u.Role)
                                                .FirstOrDefaultAsync(u => u.Id == dto.CaseId);
                
                if (sharedCase == null)
                {
                    throw new KeyNotFoundException("Case not found.");
                }
                
                foreach (var rmId in dto.SelectedRMs)
                {
                    // Fetch the user details
                    var assignedUser = await _cbeContext.CreateUsers.Include(u => u.Role).FirstOrDefaultAsync(u => u.Id == rmId);
                    if (assignedUser != null)
                    {
                        // Map DTO to TaskManagment entity
                        // var task = _mapper.Map<TaskManagment>(dto);
                        var task =  new TaskManagment{
                            Id = Guid.NewGuid(),
                            CaseId = dto.CaseId,
                            TaskName = dto.TaskName,
                            PriorityType = dto.PriorityType,
                            SharingReason = dto.SharingReason,
                            Deadline = dto.Deadline,
                            CaseOrginatorId = sharedCase.CaseOriginatorId,
                            AssignedId = rmId,
                            TaskStatus = "New",
                            AssignedDate = DateTime.Now,
                            CompletionDate = null,
                        };
                        // Add task to the context
                        await _cbeContext.TaskManagments.AddAsync(task);

                        // Log the timeline event
                        await _caseTimeLineService.CreateCaseTimeLine(new CaseTimeLinePostDto
                        {
                            CaseId = task.CaseId,
                            Activity = $"<strong>A {sharedCase.ApplicantName} appicant case has been shared to {assignedUser.Role.Name} by {sharedCase.CaseOriginator.Role.Name}</strong>",
                            CurrentStage = assignedUser.Role.Name
                        });

                        // Create a task notification
                        string notificationMessage = $"New task assigned: {task.TaskName}";
                        var taskNotification = new TaskNotification
                        {
                            TaskId = task.Id,
                            UserId = task.AssignedId,
                            Date = DateTime.Now,
                            Notification = notificationMessage,
                            Status ="New"
                        };
                        
                        var notification = new Notification
                        {
                            UserId = rmId,
                            Message = notificationMessage,
                            CreatedAt = DateTime.Now,
                            Status ="New",
                            IsRead = false
                        };
                        _cbeContext.Notifications.Add(notification);

                        // Send real-time notification
                        await _cbeContext.TaskNotifications.AddAsync(taskNotification);
                        await _hubContext.Clients.User(assignedUser.Id.ToString()).SendAsync("ReceiveNotification", notificationMessage);
                    }
                }
                // Save changes and commit the transaction
                await _cbeContext.SaveChangesAsync();
                await transaction.CommitAsync();

                return true;
            }

            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sharing task.");
                await transaction.RollbackAsync();
                throw new ApplicationException("An error occurred while sharing the task.", ex);
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
        
    }
}
