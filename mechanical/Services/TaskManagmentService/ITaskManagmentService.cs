using AutoMapper;
using mechanical.Data;
using mechanical.Models.Dto.TaskManagmentDto;
using mechanical.Models.Entities;
using mechanical.Models.PCE.Dto.ProductionCapacityDto;
using mechanical.Models.PCE.Entities;
using mechanical.Services.PCE.PCECaseTimeLineService;
using mechanical.Services.PCE.PCEEvaluationService;
using mechanical.Services.PCE.ProductionCapacityService;
using mechanical.Services.UploadFileService;

namespace mechanical.Services.TaskManagmentService
{
   
    public interface ITaskManagmentService
    {
        Task<TaskManagment> ShareTask( Guid selectedCaseIds, TaskManagmentPostDto createTaskManagmentDto);
        Task<TaskManagment> UpdateTask(Guid AssignorId, Guid AssigneeId,  Guid TaskId, TaskManagmentUpdateDto updateTaskManagmentDto);
        Task<bool> DeleteTask(Guid AssignorId, Guid Id);
        Task<TaskManagmentReturnDto> GetTaskDetails(Guid AssignorId, Guid Id);
        Task<TaskManagmentReturnDto> GetTask(Guid AssignorId, Guid Id);
        Task<IEnumerable<TaskManagmentReturnDto>> GetTasks(Guid AssignorId, Guid? CaseId = null, string Status = null);

    }
}
