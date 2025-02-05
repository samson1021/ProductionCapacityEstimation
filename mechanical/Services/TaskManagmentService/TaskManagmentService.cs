using AutoMapper;
using DocumentFormat.OpenXml.Spreadsheet;
using mechanical.Data;
using mechanical.Models.Dto.CaseTimeLineDto;
using mechanical.Models.Dto.TaskManagmentDto;
using mechanical.Models.Entities;
using mechanical.Services.CaseTimeLineService;
using mechanical.Services.PCE.PCECaseTimeLineService;
using mechanical.Utils;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.DirectoryServices.ActiveDirectory;

namespace mechanical.Services.TaskManagmentService
{
    public class TaskManagmentService : ITaskManagmentService
    {
        private readonly CbeContext _cbeContext;
        private readonly IMapper _mapper;      
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<TaskManagmentService> _logger;      
        private readonly ICaseTimeLineService _caseTimeLineService;

        public TaskManagmentService(CbeContext cbeContext, IMapper mapper, ILogger<TaskManagmentService> logger, IHttpContextAccessor httpContextAccessor,  ICaseTimeLineService caseTimeLineService)
        {
            _cbeContext = cbeContext;
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
                task.AssignedId = user.Id;
                task.TaskStatus = "New"; // Use an enum or constant
                task.AssignedDate = DateTime.Now;
                task.Deadline = createTaskManagmentDto.Deadline;

                // Add task to the context
                await _cbeContext.TaskManagments.AddAsync(task);

                // Create a task notification
                var taskNotification = new TaskNotification
                {
                    TaskId = task.Id,
                    UserId = user.Id,
                    Date = DateTime.Now,
                    Notification = "New Task",
                    Status ="New" // Use an enum or constant
                };
                await _cbeContext.TaskNotifications.AddAsync(taskNotification);

                // Log the timeline event
                await _caseTimeLineService.CreateCaseTimeLine(new CaseTimeLinePostDto
                {
                    CaseId = task.CaseId,
                    Activity = $"<strong>A new case with ID {task.CaseId} has been shared to {task.AssignedId} by {task.CaseOrginatorId}</strong>",
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


        public Task<bool> DeleteTask(Guid AssignorId, Guid Id)
        {
            throw new NotImplementedException();
        }

        public Task<TaskManagmentReturnDto> GetTask(Guid AssignorId, Guid Id)
        {
            throw new NotImplementedException();
        }

        public Task<TaskManagmentReturnDto> GetTaskDetails(Guid AssignorId, Guid Id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<TaskManagmentReturnDto>> GetTasks(Guid AssignorId, Guid? CaseId = null, string Status = null)
        {
            throw new NotImplementedException();
        }

        public Task<TaskManagment> UpdateTask(Guid AssignorId, Guid AssigneeId, Guid TaskId, TaskManagmentUpdateDto updateTaskManagmentDto)
        {
            throw new NotImplementedException();
        }
    }
}
