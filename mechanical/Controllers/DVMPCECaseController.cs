using Newtonsoft.Json;
using Microsoft.AspNetCore.Mvc;

using mechanical.Services.MailService;

using mechanical.Models.PCE.Dto.PCECaseCommentDto;
using mechanical.Services.PCE.PCECaseService;
using mechanical.Services.PCE.MOPCECaseService;
using mechanical.Services.PCE.PCECaseCommentService;
using mechanical.Services.PCE.PCECaseScheduleService;
using mechanical.Services.PCE.PCECaseTerminateService;
using mechanical.Services.PCE.ProductionCapacityServices;
using mechanical.Services.PCE.PCECaseAssignmentServices;

namespace mechanical.Controllers
{
    public class DVMPCECaseController : BaseController
    {        
        private readonly IPCECaseService _PCECaseService;
        private readonly IMOPCECaseService _MOPCECaseService;  
        private readonly IPCECaseCommentService _PCEcaseCommentService;
        private readonly IPCECaseScheduleService _PCECaseScheduleService;
        private readonly IPCECaseTerminateService _PCECaseTerminateService;
        private readonly IPCECaseAssignmentServices _pceCaseAssignmentServices;

        public DVMPCECaseController(IPCECaseService PCECaseService, IPCECaseScheduleService PCECaseScheduleService, IPCECaseAssignmentServices pceCaseAssignmentServices, IMOPCECaseService MOPCECaseService, IPCECaseTerminateService PCECaseTerminateService, IPCECaseCommentService PCEcaseCommentService)
        {
         
            _PCECaseService = PCECaseService;
            _PCECaseScheduleService = PCECaseScheduleService;
            _pceCaseAssignmentServices = pceCaseAssignmentServices;
            _MOPCECaseService = MOPCECaseService;
            _PCECaseTerminateService = PCECaseTerminateService;
            _PCEcaseCommentService = PCEcaseCommentService;
        }
       
        //// PCE Cases /////////

        [HttpPost]
        public async Task<IActionResult> PCEAssignTeamleader(string selectedPCEIds, string employeeId)
        {
            await _pceCaseAssignmentServices.AssignProductionMakerTeamleader(base.GetCurrentUserId(), selectedPCEIds, employeeId);
            var response = new { message = "Productions assigned to Maker Officer successfully" };
            return Ok(response);
        }
        
        [HttpPost]
        public async Task<IActionResult> PCEReAssignTeamleader(string selectedPCEIds, string employeeId)
        {
            await _pceCaseAssignmentServices.ReAssignProductionMakerTeamleader(base.GetCurrentUserId(), selectedPCEIds, employeeId);
            var response = new { message = "Productions re-assigned to Maker Officer successfully" };
            return Ok(response);
        }

        [HttpGet]
        public IActionResult MyPCECases(string Status = "All")
        {
            ViewData["Title"] = Status + " PCE Cases";
            ViewBag.Url = "/DVMPCECase/GetMyPCECases";
            ViewBag.Status = Status;
            return View("PCECases");
        }

        [HttpGet]
        public async Task<IActionResult> GetMyPCECases(string Status = "All")
        {
            var pceCases = await _MOPCECaseService.GetPCECases(base.GetCurrentUserId(), Status);
            if (pceCases == null)
            {
                return BadRequest("Unable to load {Status} PCE Cases");
            }
            string jsonData = JsonConvert.SerializeObject(pceCases);
            return Content(jsonData, "application/json");
        }

        [HttpGet]
        public async Task<IActionResult> GetPCEs(Guid PCECaseId, string Status = "All")
        {
            var productions = await _MOPCECaseService.GetPCEs(base.GetCurrentUserId(), PCECaseId, Status: Status);

            if (productions == null)
            {
                return BadRequest("Unable to load {Status} PCEs with PCECase ID: {PCECaseId}");
            }

            string jsonData = JsonConvert.SerializeObject(productions);

            return Content(jsonData, "application/json");
        }

        [HttpGet]
        public async Task<IActionResult> GetMyDashboardPCECasesCount()
        {
            var pceCasesCount = await _MOPCECaseService.GetDashboardPCECasesCount(base.GetCurrentUserId());
            string jsonData = JsonConvert.SerializeObject(pceCasesCount);
            return Content(jsonData, "application/json");
        }
        
        [HttpGet]
        public async Task<IActionResult> PCECaseDetail(Guid Id, string Status = "All")
        {
            var userId = base.GetCurrentUserId();
            var pceCase = await _MOPCECaseService.GetPCECase(userId, Id);

            if (pceCase == null)
            {
                return RedirectToAction("MyPCECases");
            }

            var PCECaseTerminate = await _PCECaseTerminateService.GetCaseTerminates(Id);
            var pceCaseSchedule = await _PCECaseScheduleService.GetPCECaseSchedules(Id);

            ViewData["CurrentUser"] = await _MOPCECaseService.GetUser(userId);
            ViewData["PCECaseId"] = pceCase.Id;
            ViewData["PCECase"] = pceCase;
            ViewData["PCECaseTerminate"] = PCECaseTerminate;
            ViewData["PCECaseSchedule"] = pceCaseSchedule;
            ViewData["Title"] = Status + " PCE Case Details";             
            ViewBag.Status = Status;

            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetMyAssignmentPCECases()
        {
            var myPCECases = await _PCECaseService.GetMyAssignmentPCECases(base.GetCurrentUserId());
            if (myPCECases == null) 
            { 
                return BadRequest("Unable to load PCEcase"); 
            }
            string jsonData = JsonConvert.SerializeObject(myPCECases);
            return Content(jsonData, "application/json");
        }

        public IActionResult MyPCEAssignments()
        {
            return View();
        }

        public async Task<IActionResult> MyPCEAssignment(Guid Id)
        {            
            var pceCase = await _MOPCECaseService.GetPCECase(base.GetCurrentUserId(), Id);
            if (pceCase == null)
            {
                return RedirectToAction("MyPCECases");
            }
            ViewData["PCECaseId"] = Id;

            return View();
        }
        
        [HttpGet]
        public async Task<IActionResult> GetMyPreviousValuations(Guid PCEId)
        {
            var valuationHistory = await _MOPCECaseService.GetValuationHistory(base.GetCurrentUserId(), PCEId);    
            string jsonData = JsonConvert.SerializeObject(valuationHistory.PreviousEvaluations, new JsonSerializerSettings{ReferenceLoopHandling = ReferenceLoopHandling.Ignore});
            return Content(jsonData, "application/json");
        }
        [HttpGet]
        public async Task<IActionResult> MyPCECase(Guid Id)
        {
            var pceCase = await _PCECaseService.GetCaseDetail(Id);
            var pceCaseTerminate = await _PCECaseTerminateService.GetCaseTerminates(Id);
            var pceCaseSchedule = await _PCECaseScheduleService.GetPCECaseSchedules(Id);
            
            if (pceCase == null) 
            { 
                return RedirectToAction("MyPCECases"); 
            }
            
            ViewData["PCECase"] = pceCase;
            ViewData["PCECaseTerminate"] = pceCaseTerminate;
            ViewData["PCECaseSchedule"] = pceCaseSchedule;

            return View();
        }
        
        [HttpGet]
        public async Task<IActionResult> GetMyLatestPCECases()
        {
            var limit = 5;
            var pceCases = await _MOPCECaseService.GetPCECases(base.GetCurrentUserId(), Limit: limit);
            if (pceCases == null)
            {
                return BadRequest("Unable to load {Status} PCE Cases");
            }
            string jsonData = JsonConvert.SerializeObject(pceCases);
            return Content(jsonData, "application/json");
        }
    }
}
