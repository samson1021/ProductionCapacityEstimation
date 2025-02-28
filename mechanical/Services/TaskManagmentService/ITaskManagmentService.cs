using mechanical.Models.Entities;
using mechanical.Models.Dto;
using mechanical.Models.Dto.TaskManagmentDto;

namespace mechanical.Services.TaskManagmentService
{
    public interface ITaskManagmentService
    {

        Task<TaskManagment> SharesTask(string selectedCaseIds, Guid AssignorId, TaskManagmentPostDto createTaskManagmentDto);
        Task<TaskManagmentReturnDto> GetTask(Guid userId, Guid taskId);
        Task<IEnumerable<TaskManagmentReturnDto>> GetSharedTasks(Guid userId);
        Task<IEnumerable<TaskManagmentReturnDto>> GetReceivedTasks(Guid userId);
        Task<ResultDto> ShareTasks(Guid userId, ShareTasksDto dto);
        Task<ResultDto> UpdateTask(Guid userId, UpdateTaskDto dto);
        Task<ResultDto> ReassignTask(Guid userId, Guid taskId, Guid newAssignedId);
        Task<ResultDto> DeleteTask(Guid userId, Guid taskId);
        Task<ResultDto> CompleteTask(Guid userId, Guid taskId);
        Task<TaskComment> CommentTask(Guid userId, TaskCommentPostDto dto);
        Task<IEnumerable<TaskCommentReturnDto>> GetTaskComment(Guid taskId);
    }
}
