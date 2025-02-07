using Azure;
using mechanical.Controllers;
using mechanical.Data;
using mechanical.Models.Dto.TaskManagmentDto;
using mechanical.Services.TaskManagmentService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace mechanical.Controllers
{
    public class TaskManagmentController : BaseController
    {
        private readonly ITaskManagmentService _taskManagmentService;
        private readonly CbeContext _cbeContext;
        public TaskManagmentController(ITaskManagmentService taskManagmentService, CbeContext cbeContext)
        {
            _taskManagmentService = taskManagmentService;
            _cbeContext = cbeContext;
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
        public async Task<IActionResult> GetSharedTasks()
        {
            var newCases = await _taskManagmentService.GetSharedTasks(base.GetCurrentUserId());
            return Content(JsonConvert.SerializeObject(newCases), "application/json");

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




