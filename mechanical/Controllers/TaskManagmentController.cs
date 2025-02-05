using Azure;
using mechanical.Controllers;
using mechanical.Models.Dto.TaskManagmentDto;
using mechanical.Services.TaskManagmentService;
using Microsoft.AspNetCore.Mvc;
using static System.Runtime.InteropServices.JavaScript.JSType;

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
        public async Task<IActionResult> ShareTask(Guid selectedCaseIds, TaskManagmentPostDto createTaskManagmentDto)
        {
            try
            {
                await _taskManagmentService.ShareTask(selectedCaseIds, createTaskManagmentDto);
                var response = new { message = "Task assigned successfully" };
                return Ok(response);
            }
            catch (Exception ex)
            {
                var error = new { message = "Task not assigned successfully" };
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




