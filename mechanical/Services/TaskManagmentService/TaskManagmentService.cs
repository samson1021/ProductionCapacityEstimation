using AutoMapper;
using System.Linq;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System.DirectoryServices.ActiveDirectory;

using mechanical.Data;

using mechanical.Models.Dto.CaseAssignmentDto;

using mechanical.Hubs;
using mechanical.Utils;
using mechanical.Models.Entities;

using mechanical.Models.Dto.CaseDto;
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

        public async Task<TaskManagment> ShareTask(string selectedCaseIds, Guid AssignorId, TaskManagmentPostDto createTaskManagmentDto)

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

                    Console.WriteLine("dfghgfhg");
                    Console.WriteLine(dto.CaseId);
                    Console.WriteLine(rmId);
                    Console.WriteLine("dfghgfhg");
                    // Fetch the user details
                    var assignedUser = await _cbeContext.CreateUsers.Include(u => u.Role).FirstOrDefaultAsync(u => u.Id == rmId);
                    if (assignedUser != null)
                    {
                        // Map DTO to TaskManagment entity
                        // var task = _mapper.Map<TaskManagment>(dto);

                        var task = new TaskManagment
                        {
                            Id = Guid.NewGuid(),
                            CaseId = dto.CaseId,
                            TaskName = dto.TaskName,
                            PriorityType = dto.PriorityType,
                            SharingReason = dto.SharingReason,
                            Deadline = dto.Deadline,
                            CaseOrginatorId = sharedCase.CaseOriginatorId,
                            AssignedId = rmId,
                            TaskStatus = "New",
                            // AssignedDate = DateTime.Now,
                            // CompletionDate = null,
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
                        var taskNotification = new TaskNotification
                        {
                            TaskId = task.Id,
                            UserId = task.AssignedId,
                            Date = DateTime.Now,
                            Notification = "New Task",
                            Status = "New"
                        };

                        string notificationMessage = $"New task assigned: {task.TaskName}";
                        // var notification = new Notification
                        // {
                        //     UserId = rmId,
                        //     Message = notificationMessage,
                        //     Date = DateTime.Now,
                        //     Status ="New",
                        //     IsRead = false
                        // };
                        // _cbeContext.Notifications.Add(notification);

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
