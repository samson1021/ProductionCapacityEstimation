using mechanical.Controllers;
using mechanical.Models.Dto.TaskManagmentDto;
using mechanical.Services.TaskManagmentService;
using Microsoft.AspNetCore.Mvc;

namespace mechanical.Controllers
{
    public class TaskManagmentController : BaseController
    {
        private readonly ITaskManagmentService _taskManagmentService;
        public TaskManagmentController(ITaskManagmentService taskManagmentService)
        {
            _taskManagmentService = taskManagmentService;
        }
        [HttpPost]
        public async Task<IActionResult> ShareTask(Guid selectedCaseIds, Guid AssignorId, Guid AssigneeId, TaskManagmentPostDto createTaskManagmentDto)
        {
            try
            {
                await _taskManagmentService.ShareTask(selectedCaseIds, AssignorId, AssigneeId, createTaskManagmentDto);
                var response = new { message = "Task assigned successfully" };
                return Ok(response);
            }
            catch (Exception ex)
            {
                var error = new { message = ex.Message };
                return BadRequest(error);
            }

        }

        public async Task<IActionResult> GetSharedTask()
        {
            return View();
        }
        public async Task<IActionResult> GetSharedTasks()
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
    } 
}




