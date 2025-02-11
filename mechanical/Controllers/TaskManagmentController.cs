using static System.Runtime.InteropServices.JavaScript.JSType;
using Newtonsoft.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using mechanical.Data;
using mechanical.Controllers;
using mechanical.Models.Dto.TaskManagmentDto;
using mechanical.Models.Dto.TaskManagmentDto;
using mechanical.Services.TaskManagmentService;
using mechanical.Services.TaskManagmentService;
using mechanical.Services.CaseServices;
using mechanical.Services.UserService;

// [ApiController]
// [Route("api/tasks")]

namespace mechanical.Controllers
{
    public class TaskManagmentController : BaseController
    {
        private readonly ITaskManagmentService _taskManagmentService;
        private readonly CbeContext _cbeContext;
        private readonly ICaseService _caseService;
        private readonly IUserService _userService;
        
        public TaskManagmentController(ITaskManagmentService taskManagmentService, ICaseService caseService, IUserService userService, CbeContext cbeContext)
        {
            _taskManagmentService = taskManagmentService;
            _cbeContext = cbeContext;
            _caseService = caseService;
            _userService = userService;
        }
        
        [HttpPost]
        public async Task<IActionResult> ShareTask(string selectedCaseIds, TaskManagmentPostDto createTaskManagmentDto)
        {
            try
            {
                await _taskManagmentService.ShareTask(selectedCaseIds, base.GetCurrentUserId(), createTaskManagmentDto);          
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

        public async Task<IActionResult> GetSharedTask()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetSharedTasks()
        {
            var tasks = await _taskManagmentService.GetSharedTasks(base.GetCurrentUserId());
            return Json(tasks);
            // return Content(JsonConvert.SerializeObject(tasks), "application/json");
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
        public async Task<IActionResult> SharedTasks(string status = "All")
        {
            // var model = new ShareTasksDto();
            ViewData["status"] = status;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ShareTasks(ShareTasksDto model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var task = await _taskManagmentService.ShareTasks(base.GetCurrentUserId(), model);
                    var response = new { message = "Task is assigned successfully" };
                    return RedirectToAction("SharedTasks");
                }
                else
                {                    
                    var response = new { message = "Task is not assigned successfully" };
                    return RedirectToAction("SharedTasks");
                }

            }
            catch (Exception ex)
            {
                var error = new { message = "Task is not assigned successfully" };
                return BadRequest(error);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetMyCases()
        {
            var cases = await _caseService.GetMyCases(base.GetCurrentUserId());
            var result = cases.Select(c => new { Id = c.Id, CaseNo = c.CaseNo });
            return Json(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetRMs()
        {
            var rms = await _userService.GetRMs(base.GetCurrentUserId());
            var result = rms.Select(rm => new { Id = rm.Id, Name = rm.Name });
            return Json(result);
        }
    }
}
