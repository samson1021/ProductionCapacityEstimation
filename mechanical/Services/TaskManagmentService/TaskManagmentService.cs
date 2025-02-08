using AutoMapper;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Wordprocessing;
using mechanical.Data;
using mechanical.Models.Dto.CaseAssignmentDto;
using mechanical.Models.Dto.CaseDto;
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

        public async Task<IEnumerable<TaskManagmentReturnDto>> GetSharedTasks(Guid AssignorId)
        {
            var sharedCases = await _cbeContext.TaskManagments
            .Where(res => res.CaseOrginatorId == AssignorId)
            .OrderByDescending(d => d.AssignedDate)
            .ToListAsync();

            var sharedCaseDtos = _mapper.Map<IEnumerable<TaskManagmentReturnDto>>(sharedCases);

            return sharedCaseDtos;
        }

        public async Task<TaskManagment> UpdateTask(Guid AssignorId, Guid AssigneeId, Guid TaskId, TaskManagmentUpdateDto updateTaskManagmentDto)
        {
            throw new NotImplementedException();
        }
    }
}
