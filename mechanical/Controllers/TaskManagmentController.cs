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
        public async Task<IActionResult> ShareTask(Guid selectedCaseIds, TaskManagmentPostDto createTaskManagmentDto)
        {
            try
            {
                await _taskManagmentService.ShareTask(selectedCaseIds, createTaskManagmentDto);
                var response = new { message = "Task is assigned successfully" };               
                return RedirectToAction("NewCases", "Case");
            }
            catch (Exception ex)
            {
                var error = new { message = "Task is not assigned successfully" };
               // return BadRequest(error);
                return RedirectToAction("NewCases", "Case");
            }
           
        }
        [HttpGet]
        public IActionResult SharedCases()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetCase(Guid CaseId)
        {
            var caseOriginator = await _cbeContext.Cases
                .Where(c => c.Id == CaseId)
                .Select(c => new  {c.CaseOriginatorId})
                 .FirstOrDefaultAsync();
            if (caseOriginator == null) { return BadRequest("Unable to get case Orignator"); }
            string jsonData = JsonConvert.SerializeObject(caseOriginator);
            return Content(jsonData, "application/json");
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
            if (!ModelState.IsValid)
            {
                try
                {
                    var response = await _taskManagmentService.ShareTasks(base.GetCurrentUserId(), model);
                    if (response){
                        return Ok(new { success=true, message = "Task is shared successfully" });
                    }
                    else
                    {
                        return Ok(new {  success=false, message = "Task is not shared successfully" });
                    }
                }
                catch (Exception ex)
                {
                    return BadRequest(new { message = ex.Message });
                }
            }
            return BadRequest(new {  success=false, message = "Task is not shared successfully" });
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
