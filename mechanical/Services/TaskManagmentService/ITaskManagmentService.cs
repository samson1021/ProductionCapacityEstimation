using mechanical.Models.Entities;
using mechanical.Models.Dto;
using mechanical.Models.Dto.TaskManagmentDto;

namespace mechanical.Services.TaskManagmentService
{
    public interface ITaskManagmentService
    {
        Task<TaskManagmentReturnDto> GetTask(Guid userId, Guid taskId);
        Task<IEnumerable<TaskManagmentReturnDto>> GetSharedTasks(Guid userId, string? mode);
        Task<IEnumerable<TaskManagmentReturnDto>> GetReceivedTasks(Guid userId, string? mode);
        Task<List<ResultDto>> ShareTask(Guid AssignorId, string selectedCaseIds, TaskManagmentPostDto createTaskManagmentDto);
        Task<ResultDto> ShareTasks(Guid userId, ShareTasksDto dto);
        Task<ResultDto> UpdateTask(Guid userId, TaskManagmentUpdateDto dto);
        Task<ResultDto> CompleteTask(Guid userId, Guid taskId);
        // Task<ResultDto> ReassignTask(Guid userId, Guid taskId, Guid newAssignedId);
        // Task<ResultDto> DeleteTask(Guid userId, Guid taskId);
        Task<ResultDto> RevokeTask(Guid userId, Guid taskId);
        Task<ResultDto> ReturnTask(Guid userId, Guid taskId);
        Task<TaskComment> CommentTask(Guid userId, TaskCommentPostDto dto);
        Task<IEnumerable<TaskCommentReturnDto>> GetTaskComment(Guid taskId);
    }
}
