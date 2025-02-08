using AutoMapper;
using mechanical.Data;
using mechanical.Models.Dto.CaseDto;
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
        Task<TaskManagment> ShareTask(string selectedCaseIds, Guid AssignorId, TaskManagmentPostDto createTaskManagmentDto);       
        Task<TaskManagment> UpdateTask(Guid AssignorId, Guid AssigneeId,  Guid TaskId, TaskManagmentUpdateDto updateTaskManagmentDto);
        Task<bool> DeleteTask(Guid AssignorId, Guid Id);
        Task<TaskManagmentReturnDto> GetTaskDetails(Guid AssignorId, Guid Id);
        Task<TaskManagmentReturnDto> GetSharedTask(Guid AssignorId);
        Task<IEnumerable<TaskManagmentReturnDto>> GetSharedTasks(Guid AssignorId);

    }
}
