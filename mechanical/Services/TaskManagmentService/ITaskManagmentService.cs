using mechanical.Models.Entities;
using mechanical.Models.Dto;
using mechanical.Models.Dto.TaskManagmentDto;

namespace mechanical.Services.TaskManagmentService
{
    public interface ITaskManagmentService
    {

        Task<TaskManagment> SharesTask(string selectedCaseIds, Guid AssignorId, TaskManagmentPostDto createTaskManagmentDto);       


        //Task<TaskManagment> UpdateTask(Guid AssignorId, Guid AssigneeId,  Guid TaskId, TaskManagmentUpdateDto updateTaskManagmentDto);
        // Task<TaskManagmentReturnDto> GetTaskDetails(Guid AssignorId, Guid Id);
        
        Task<TaskManagmentReturnDto> GetTask(Guid userId, Guid taskId);
        Task<IEnumerable<TaskManagmentReturnDto>> GetSharedTasks(Guid userId);
        Task<IEnumerable<TaskManagmentReturnDto>> GetAssignedTasks(Guid userId);
        Task<ResultDto> ShareTasks(Guid userId, ShareTasksDto dto);
        Task<ResultDto> UpdateTask(Guid userId, UpdateTaskDto dto);
        Task<ResultDto> ReassignTask(Guid userId, Guid taskId, Guid newAssignedId);
        Task<ResultDto> DeleteTask(Guid userId, Guid taskId);
        Task<ResultDto> CompleteTask(Guid userId, Guid taskId);
    }
}
