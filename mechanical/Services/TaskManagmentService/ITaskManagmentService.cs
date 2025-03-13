using mechanical.Models.Entities;
using mechanical.Models.Dto;
using mechanical.Models.Dto.NotificationDto;
using mechanical.Models.Dto.TaskManagmentDto;

namespace mechanical.Services.TaskManagmentService
{
    public interface ITaskManagmentService
    {
        Task<TaskManagmentReturnDto> GetTask(Guid userId, Guid taskId);
<<<<<<< HEAD
        Task<IEnumerable<TaskManagmentReturnDto>> GetSharedTasks(Guid userId, string? mode);
        Task<IEnumerable<TaskManagmentReturnDto>> GetReceivedTasks(Guid userId, string? mode);
        Task<List<ResultDto>> ShareTask(Guid AssignorId, string selectedCaseIds, TaskManagmentPostDto createTaskManagmentDto);
        Task<ResultDto> ShareTasks(Guid userId, ShareTasksDto dto);
=======
        Task<IEnumerable<TaskManagmentReturnDto>> GetSharedTasks(Guid userId, string? mode = null);
        Task<IEnumerable<TaskManagmentReturnDto>> GetReceivedTasks(Guid userId, string? mode = null);
        Task<TaskManagment> ShareTask(Guid AssignorId, string selectedCaseIds, TaskManagmentPostDto createTaskManagmentDto);
        Task<List<ResultDto>> ShareTasks(Guid userId, ShareTasksDto dto);
>>>>>>> d0b869176cea676f0013ab39bcd4fab737327e5f
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
