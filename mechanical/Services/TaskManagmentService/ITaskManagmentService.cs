using mechanical.Models.Dto.TaskManagmentDto;
using mechanical.Models.Entities;

namespace mechanical.Services.TaskManagmentService
{
    public interface ITaskManagmentService
    {
        Task<TaskManagment> ShareTask(Guid selectedCaseIds, TaskManagmentPostDto createTaskManagmentDto);
        Task<TaskManagment> UpdateTask(Guid AssignorId, Guid AssigneeId,  Guid TaskId, TaskManagmentUpdateDto updateTaskManagmentDto);
        Task<bool> DeleteTask(Guid AssignorId, Guid Id);
        Task<TaskManagmentReturnDto> GetTaskDetails(Guid AssignorId, Guid Id);

        Task<bool> ShareTasks(Guid userId, ShareTasksDto dto);
        Task<TaskManagmentReturnDto> GetSharedTask(Guid userId);
        Task<IEnumerable<TaskManagmentReturnDto>> GetSharedTasks(Guid userId);
        Task<IEnumerable<TaskManagmentReturnDto>> GetAssignedTasks(Guid userId);
    }
}
