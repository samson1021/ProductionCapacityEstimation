using AutoMapper;
using Newtonsoft.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using mechanical.Models.Dto.TaskManagmentDto;
using mechanical.Services.TaskManagmentService;
using iText.Forms.Xfdf;

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

        [HttpGet]
        public IActionResult SharedCases()
        {
            return View();
        }

        [HttpGet]
        public IActionResult SharedTasks()
        {
            return View();
        }

        [HttpGet]
        public async Task<JsonResult> GetSharedTasks(string mode = "active")
        {
            var tasks = await _taskManagmentService.GetSharedTasks(base.GetCurrentUserId(), mode: mode);
            return Json(tasks);
            // return Content(JsonConvert.SerializeObject(tasks), "application/json");
        }

        [HttpGet]
        public IActionResult ReceivedTasks()
        {
            return View();
        }

        [HttpGet]
        public async Task<JsonResult> GetReceivedTasks(string mode = "active")
        {
            var tasks = await _taskManagmentService.GetReceivedTasks(base.GetCurrentUserId(), mode: mode);
            return Json(tasks);
        }

        [HttpGet]
        public async Task<JsonResult> GetTask(Guid id)
        {
            var task = await _taskManagmentService.GetTask(base.GetCurrentUserId(), id);
            return Json(task);
        }

        [HttpGet]
        public async Task<IActionResult> Detail(Guid id)
        {
            try
            {
                var task = await _taskManagmentService.GetTask(base.GetCurrentUserId(), id);
                if (task == null)
                {
                    return NotFound();
                }
                TempData["myTaskInfo"] = task.Id;
                return PartialView("_taskDetailsPartial", task);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while fetching task details.");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ShareTask(string selectedCaseIds, TaskManagmentPostDto createTaskManagmentDto)
        {
            try
            {
               var MessagetResult= await _taskManagmentService.ShareTask(base.GetCurrentUserId(), selectedCaseIds, createTaskManagmentDto);
                //return RedirectToAction("NewCases", "Case");
                return Json(MessagetResult);
            }
            catch (Exception ex)
            {
               // return BadRequest(error);
                return RedirectToAction("NewCases", "Case");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ShareTasks(ShareTasksDto dto)
        {
            if (!ModelState.IsValid)
            {
                
                // return BadRequest(new {  success=false, message = "Task is not shared successfully" });
                return Json(new
                {
                    success = false,
                    errors = ModelState
                        .Where(x => x.Value.Errors.Any())
                        .ToDictionary(
                            kvp => kvp.Key,
                            kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToArray()
                        )
                });
            }
            try
            {
                
                if (dto.Deadline < DateTime.Today)
                {
                    return Ok(new { success = false, message = "Deadline must be today or in the future." });
                }

                var response = await _taskManagmentService.ShareTasks(base.GetCurrentUserId(), dto);
                return Ok(new { success=response.Success, message = response.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Task is not shared successfully" });
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

                return PartialView("_updateTaskPartial", _mapper.Map<TaskManagmentUpdateDto>(task));
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while fetching task details.");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateTask(TaskManagmentUpdateDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new {  success=false, message = "Task is not updated successfully" });
            }
            try
            {
                if (dto.Deadline < DateTime.Today)
                {
                    return Ok(new { success = false, message = "Deadline must be today or in the future." });
                }

                var response = await _taskManagmentService.UpdateTask(base.GetCurrentUserId(), dto);
                return Ok(new { success=response.Success, message = response.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Task is not updated successfully" });
            }
        }

        [HttpPost]
        // [ValidateAntiForgeryToken]
        public async Task<IActionResult> CompleteTask(Guid id)
        {
            try
            {
                var response = await _taskManagmentService.CompleteTask(base.GetCurrentUserId(), id);
                return Ok(new { success=response.Success, message = response.Message });
            }
            catch (ArgumentException ex)
            {
                return Ok(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = "An error occurred while completing the task." });
            }
        }

        // [HttpPost]
        // public async Task<IActionResult> ReassignTask(Guid id, Guid newAssignedId)
        // {
        //     try
        //     {
        //         var response = await _taskManagmentService.ReassignTask(base.GetCurrentUserId(), id, newAssignedId);
        //         return Ok(new { success=response.Success, message = response.Message });
        //         // return Ok(new { success = true, message = "Task reassigned successfully." });
        //     }
        //     catch (ArgumentException ex)
        //     {
        //         return Ok(new { success = false, message = "An error occurred while reassigning the task." });
        //     }
        //     catch (Exception ex)
        //     {
        //         return Ok(new { success = false, message = "An error occurred while reassigning the task." });
        //     }
        // }

        // [HttpPost]
        // public async Task<IActionResult> DeleteTask(Guid id)
        // {
        //     try
        //     {
        //         var response = await _taskManagmentService.DeleteTask(base.GetCurrentUserId(), id);
        //         return Ok(new { success=response.Success, message = response.Message });
        //         // return Ok(new { success = true, message = "Task deleted successfully." });
        //     }
        //     catch (ArgumentException ex)
        //     {
        //         return Ok(new { success = false, message = "An error occurred while deleting the task." });
        //     }
        //     catch (Exception ex)
        //     {
        //         return Ok(new { success = false, message = "An error occurred while deleting the task." });
        //     }
        // }

        [HttpPost]
        public async Task<IActionResult> RevokeTask(Guid id)
        {
            try
            {
                var response = await _taskManagmentService.RevokeTask(base.GetCurrentUserId(), id);
                return Ok(new { success=response.Success, message = response.Message });
                // return Ok(new { success = true, message = "Task revoked successfully." });
            }
            catch (ArgumentException ex)
            {
                return Ok(new { success = false, message = "An error occurred while deleting the task." });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = "An error occurred while deleting the task." });
            }
        }

        [HttpPost]
        public async Task<IActionResult> ReturnTask(Guid id)
        {
            try
            {
                var response = await _taskManagmentService.ReturnTask(base.GetCurrentUserId(), id);
                return Ok(new { success=response.Success, message = response.Message });
                // return Ok(new { success = true, message = "Task returned successfully." });
            }
            catch (ArgumentException ex)
            {
                return Ok(new { success = false, message = "An error occurred while returning the task." });
            }
            catch (Exception ex)
            {
                return Ok(new { success = false, message = "An error occurred while returning the task." });
            }
        }

        [HttpPost]
        public async Task<IActionResult> CommentTask([FromBody] TaskCommentPostDto dto)
        {
            try
            {
                await _taskManagmentService.CommentTask(base.GetCurrentUserId(), dto);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetTaskComment(Guid TaskId)
        {
            var response = new
            {
                userId = base.GetCurrentUserId(),
                comments = await _taskManagmentService.GetTaskComment(TaskId)
            };

            //return Ok(response);
            return Content(JsonConvert.SerializeObject(response), "application/json");
        }


    }
}
