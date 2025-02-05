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

        public async Task<TaskManagment> ShareTask(Guid AssignorId, Guid AssigneeId, Guid selectedCaseIds, TaskManagmentPostDto createTaskManagmentDto)
        {
            using var transaction = await _cbeContext.Database.BeginTransactionAsync();
            try
            {
                // Encode/Sanitize inputs in Dto to avoid unsafe data being saved
                EncodingHelper.EncodeObject(createTaskManagmentDto);
                // SanitizerHelper.SanitizeObject(createProductionDto);
                var asignorId = _cbeContext.CreateUsers.Include(res => res.District).Include(res => res.Role).FirstOrDefault(res => res.Id == AssignorId);
                if (asignorId == null)
                {
                    throw new Exception("user not found");
                }
                var task = _mapper.Map<TaskManagment>(createTaskManagmentDto);
                task.Id = Guid.NewGuid();
                task.CaseId = selectedCaseIds;
                task.AssignedDate = DateTime.Now;
                task.CaseOrginatorId = AssignorId;
                task.AssignedId = AssigneeId;
                task.TaskStatus = "New";

                await _cbeContext.TaskManagments.AddAsync(task);

                var taskNotification = new TaskNotification
                {
                    TaskId = task.Id,
                    UserId = AssigneeId,
                    Date = DateTime.Now,
                    Notification = " New Task",
                    Status = "New"
                };
                await _cbeContext.TaskNotifications.AddAsync(taskNotification);
               

               await _caseTimeLineService.CreateCaseTimeLine(new CaseTimeLinePostDto
               {
                   CaseId = task.CaseId,
                   Activity = $"<strong>A new case with ID {task.CaseId} has been shared to {task.AssignedId} by {task.CaseOrginatorId}</strong>",
                   CurrentStage = asignorId.Role.Name
               });


                await _cbeContext.SaveChangesAsync();
                await transaction.CommitAsync();

                return task;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating production");
                await transaction.RollbackAsync();
                throw new ApplicationException("An error occurred while creating production.");
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
