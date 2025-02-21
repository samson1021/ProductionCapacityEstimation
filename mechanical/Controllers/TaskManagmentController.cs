using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AutoMapper;

using mechanical.Models.Dto.TaskManagmentDto;
using mechanical.Services.TaskManagmentService;

// [ApiController]
// [Route("api/tasks")]

namespace mechanical.Controllers
{
    public class TaskManagmentController : BaseController
    {
        private readonly ILogger<TaskManagmentController> _logger;
        private readonly ITaskManagmentService _taskManagmentService;
        private readonly IMapper _mapper;
        
        public TaskManagmentController(IMapper mapper, ILogger<TaskManagmentController> logger, ITaskManagmentService taskManagmentService)
        {
            _taskManagmentService = taskManagmentService;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<IActionResult> CommentTask()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CommentTask(TaskCommentPostDto dto)
        {
            try
            {
                await _taskManagmentService.CommentTask(base.GetCurrentUserId(), dto);
                return RedirectToAction("CommentTask", "TaskManagment");

            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }

        }

        [HttpGet]
        public async Task<IActionResult> GetTaskComment(Guid TaskId)
        {
            var comments = await _taskManagmentService.GetTaskComment(base.GetCurrentUserId(), TaskId);
            return Json(comments);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ShareTask(string selectedCaseIds, TaskManagmentPostDto createTaskManagmentDto)
        {
            try
            {
                await _taskManagmentService.SharesTask(selectedCaseIds, base.GetCurrentUserId(), createTaskManagmentDto);          
                return RedirectToAction("NewCases", "Case");
            }
            catch (Exception ex)
            {               
               // return BadRequest(error);
                return RedirectToAction("NewCases", "Case");
            }
           
        }

        [HttpGet]
        public IActionResult SharedCases()
        {
            return View();
        }

        public async Task<IActionResult> UpdateSharedTask()
        {
            return View();
        }
        public async Task<IActionResult> DetialSharedTask()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> SharedTasks()
        {
            return View();
        }

        [HttpGet]
        public async Task<JsonResult> GetSharedTasks()
        {
            var tasks = await _taskManagmentService.GetSharedTasks(base.GetCurrentUserId());
            return Json(tasks);
            // return Content(JsonConvert.SerializeObject(tasks), "application/json");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ShareTasks(ShareTasksDto dto)
        {
            if (!ModelState.IsValid)
            {                
                return BadRequest(new {  success=false, message = "Task is not shared successfully" });
            }
            try
            {
                
                if (dto.Deadline < DateTime.Today)
                {
                    return Json(new { success = false, message = "Deadline must be today or in the future." });
                }

                var response = await _taskManagmentService.ShareTasks(base.GetCurrentUserId(), dto);
                return Json(new { success=response.Success, message = response.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> UpdateTask(Guid id)
        {
            try
            {
                var task = await _taskManagmentService.GetTask(base.GetCurrentUserId(), id);
                if (task == null)
                {
                    return NotFound();
                }

                return PartialView("_updateTaskPartial", _mapper.Map<UpdateTaskDto>(task));
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while fetching task details.");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateTask(UpdateTaskDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new {  success=false, message = "Task is not updated successfully" });
            }
            try
            {
                if (dto.Deadline < DateTime.Today)
                {
                    return Json(new { success = false, message = "Deadline must be today or in the future." });
                }

                var response = await _taskManagmentService.UpdateTask(base.GetCurrentUserId(), dto);
                return Json(new { success=response.Success, message = response.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost]
        // [ValidateAntiForgeryToken]
        public async Task<IActionResult> ReassignTask(Guid id, Guid newAssignedId)
        {
            try
            {
                var response = await _taskManagmentService.ReassignTask(base.GetCurrentUserId(), id, newAssignedId);
                return Json(new { success=response.Success, message = response.Message });
                // return Json(new { success = true, message = "Task reassigned successfully." });
            }
            catch (ArgumentException ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "An error occurred while reassigning the task." });
            }
        }

        [HttpPost]
        // [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteTask(Guid id)
        {
            try
            {
                var response = await _taskManagmentService.DeleteTask(base.GetCurrentUserId(), id);
                return Json(new { success=response.Success, message = response.Message });
                // return Json(new { success = true, message = "Task deleted successfully." });
            }
            catch (ArgumentException ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "An error occurred while revoking the task." });
            }
        }

        [HttpGet]
        public async Task<IActionResult> DetailPartial(Guid id)
        {
            try
            {
                var task = await _taskManagmentService.GetTask(base.GetCurrentUserId(), id);
                if (task == null)
                {
                    return NotFound();
                }

                return PartialView("_TaskDetailsPartial", task);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while fetching task details.");
            }
        }

        [HttpGet]
        public async Task<JsonResult> GetTask(Guid id)
        {
            var task = await _taskManagmentService.GetTask(base.GetCurrentUserId(), id);
            return Json(task);
        }

        [HttpPost]
        // [ValidateAntiForgeryToken]
        public async Task<IActionResult> CompleteTask(Guid id)
        {
            try
            {
                var response = await _taskManagmentService.CompleteTask(base.GetCurrentUserId(), id);
                return Json(new { success=response.Success, message = response.Message });
            }
            catch (ArgumentException ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "An error occurred while completing the task." });
            }
        }

        [HttpGet]
        public async Task<IActionResult> ReceivedTasks()
        {
            return View();
        }

        [HttpGet]
        public async Task<JsonResult> GetReceivedTasks()
        {
            var tasks = await _taskManagmentService.GetReceivedTasks(base.GetCurrentUserId());
            return Json(tasks);
        }
    }
}
