using Newtonsoft.Json;
using Microsoft.AspNetCore.Mvc;

using mechanical.Services.CaseServices;
using mechanical.Services.MMCaseService;
using mechanical.Services.CaseScheduleService;
using mechanical.Services.CaseTerminateService;
using mechanical.Services.CaseAssignmentService;

using mechanical.Services.PCE.PCECaseService;
using mechanical.Services.PCE.MOPCECaseService;
using mechanical.Services.PCE.PCECaseScheduleService;
using mechanical.Services.PCE.PCECaseTerminateService;
using mechanical.Services.PCE.PCECaseAssignmentServices;
using mechanical.Models.PCE.Dto.ProductionCapacityDto;

namespace mechanical.Controllers
{
    public class MTLCaseController : BaseController
    {
        private readonly ICaseService _caseService;
        private readonly IMMCaseService _mmCaseService;
        private readonly ICaseScheduleService _caseScheduleService;
        private readonly ICaseTerminateService _caseTermnateService;
        private readonly ICaseAssignmentService _caseAssignmentService;

        private readonly IPCECaseService _PCECaseService;
        private readonly IMOPCECaseService _MOPCECaseService;
        private readonly IPCECaseScheduleService _PCECaseScheduleService;
        private readonly IPCECaseTerminateService _PCECaseTerminateService;
        private readonly IPCECaseAssignmentServices _pceCaseAssignmentServices;

        public MTLCaseController(ICaseService caseService, ICaseTerminateService caseTermnateService, IPCECaseService PCECaseService, IPCECaseScheduleService PCECaseScheduleService, IPCECaseAssignmentServices pceCaseAssignmentServices, IMOPCECaseService MOPCECaseService, ICaseScheduleService caseScheduleService, ICaseAssignmentService caseAssignment,IMMCaseService mMCaseService, IPCECaseTerminateService PCECaseTerminateService)
        {
            _caseService = caseService;
            _caseAssignmentService = caseAssignment;
            _mmCaseService = mMCaseService; 
            _caseScheduleService = caseScheduleService;
            _caseTermnateService = caseTermnateService;

            _PCECaseService = PCECaseService;
            _MOPCECaseService = MOPCECaseService;
            _PCECaseScheduleService = PCECaseScheduleService;
            _PCECaseTerminateService = PCECaseTerminateService;
            _pceCaseAssignmentServices = pceCaseAssignmentServices;
        }

        [HttpGet]
        public IActionResult MyCases()
        {

            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetMyCases()
        {
            var myCase = await _mmCaseService.GetMMNewCases(GetCurrentUserId());
            if (myCase == null) { return BadRequest("Unable to load case"); }
            string jsonData = JsonConvert.SerializeObject(myCase);
            return Content(jsonData, "application/json");
        }
        [HttpGet]
        public async Task<IActionResult> GetMyAssignmentCases()
        {
            var myCase = await _mmCaseService.GetMyAssignmentCases(GetCurrentUserId());
            if (myCase == null) { return BadRequest("Unable to load case"); }
            string jsonData = JsonConvert.SerializeObject(myCase);
            return Content(jsonData, "application/json");
        }
        public IActionResult MyAssignments()
        {
            return View();
        }
        public async Task<IActionResult> MyAssignment(Guid Id)
        {
            var loanCase = await _caseService.GetCaseDetail(Id);
            if (loanCase == null) { return RedirectToAction("NewCases"); }
            ViewData["case"] = loanCase;
            return View();
        }      

        [HttpGet]
        public async Task<IActionResult> MyCase(Guid Id)
        {
            var loanCase = await _caseService.GetCaseDetail(Id);
            var caseSchedule = await _caseScheduleService.GetCaseSchedules(Id);
            var caseTerminate = await _caseTermnateService.GetCaseTerminates(Id);
            ViewData["caseTerminate"] = caseTerminate;
            if (loanCase == null) { return RedirectToAction("NewCases"); }
            ViewData["case"] = loanCase;
            ViewData["CaseSchedule"] = caseSchedule;
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> AssignMakerOfficer(string selectedCollateralIds, string employeeId)
        {
            
            await _caseAssignmentService.AssignMakerTeamleader(base.GetCurrentUserId(),selectedCollateralIds, employeeId);
            var response = new { message = "Collaterals assigned successfully" };
            return Ok(response);
        }
        [HttpPost]
        public async Task<IActionResult> ReAssignMakerOfficer(string selectedCollateralIds, string employeeId)
        {
            await _caseAssignmentService.ReAssignMakerTeamleader(base.GetCurrentUserId(), selectedCollateralIds, employeeId);
            var response = new { message = "Collaterals assigned successfully" };
            return Ok(response);
        }
        
        

        //[HttpGet]
        //public IActionResult MyPendingCases()
        //{
        //    return View();
        //}

        //[HttpGet]
        //public async Task<IActionResult> GetMyPendingCases()
        //{
        //    var myCase = await _caseService.GetMTLPendingCases();
        //    string jsonData = JsonConvert.SerializeObject(myCase);
        //    return Content(jsonData, "application/json");
        //}


        [HttpPost]
        public async Task<IActionResult> PCEAssignMakerOfficer(string selectedPCEIds, string employeeId)
        {
            
            await _pceCaseAssignmentServices.AssignProduction(base.GetCurrentUserId(),selectedPCEIds, employeeId);
            var response = new { message = "Productions assigned to Maker Officer successfully" };
            return Ok(response);
        }
        [HttpPost]
        public async Task<IActionResult> PCEReAssignMakerOfficer(string selectedPCEIds, string employeeId)
        {
            await _pceCaseAssignmentServices.ReAssignProduction(base.GetCurrentUserId(), selectedPCEIds, employeeId);
            var response = new { message = "Productions re-assigned to Maker Officer successfully" };
            return Ok(response);
        }
        
        [HttpGet]
        public async Task<IActionResult> MyPCECases(string Status = "All")
        {
            ViewData["CurrentUser"] = await _MOPCECaseService.GetUser(base.GetCurrentUserId());
            ViewData["Title"] = Status + " PCE Cases";
            ViewBag.Url = "/MTLCase/GetMyPCECases";
            ViewBag.Status = Status;
            return View("PCECases");
        }

        [HttpGet]
        public async Task<IActionResult> MyPCECase(Guid Id)
        {
            var pceCase = await _PCECaseService.GetCaseDetail(Id);
            var pceCaseSchedule = await _PCECaseScheduleService.GetPCECaseSchedules(Id);
            var pceCaseTerminate = await _PCECaseTerminateService.GetCaseTerminates(Id);
            
            if (pceCase == null) 
            { 
                return RedirectToAction("MyPCECases"); 
            }

            ViewData["CurrentUser"] = await _MOPCECaseService.GetUser(base.GetCurrentUserId());
            ViewData["PCECase"] = pceCase;
            ViewData["PCECaseTerminate"] = pceCaseTerminate;
            ViewData["PCECaseSchedule"] = pceCaseSchedule;

            return View();
        }   

        [HttpGet]
        public async Task<IActionResult> PCECaseDetail(Guid Id, string Status = "All")
        {

            var pceCase = await _MOPCECaseService.GetPCECase(base.GetCurrentUserId(), Id);
            if (pceCase == null)
            {
                return RedirectToAction("MyPCECases");
            }
            var PCECaseTerminate = await _PCECaseTerminateService.GetCaseTerminates(Id);
            var PCECaseSchedule = await _PCECaseScheduleService.GetPCECaseSchedules(Id);
            
            ViewData["CurrentUser"] = await _MOPCECaseService.GetUser(base.GetCurrentUserId());
            ViewData["PCECaseId"] = pceCase.Id;
            ViewData["PCECase"] = pceCase;
            ViewData["PCECaseTerminate"] = PCECaseTerminate;
            ViewData["PCECaseSchedule"] = PCECaseSchedule;
            ViewData["Title"] = Status + " PCE Case Details";             
            ViewBag.Status = Status;

            return View();
        }
        
        public async Task<IActionResult> MyPCEAssignments()
        {
            ViewData["CurrentUser"] = await _MOPCECaseService.GetUser(base.GetCurrentUserId());
            return View();
        }

        public async Task<IActionResult> MyPCEAssignment(Guid Id)
        {            
            var pceCase = await _MOPCECaseService.GetPCECase(base.GetCurrentUserId(), Id);
            
            if (pceCase == null)
            {
                return RedirectToAction("MyPCECases");
            }

            ViewData["CurrentUser"] = await _MOPCECaseService.GetUser(base.GetCurrentUserId());
            ViewData["PCECaseId"] = Id;

            return View();
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
        public async Task<IActionResult> GetMyDashboardPCECasesCount()
        {
            var pceCasesCount = await _MOPCECaseService.GetDashboardPCECasesCount(base.GetCurrentUserId());
            string jsonData = JsonConvert.SerializeObject(pceCasesCount);
            return Content(jsonData, "application/json");
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
        
        [HttpGet]
        public async Task<IActionResult> GetMyPreviousValuations(Guid PCEId)
        {
            var valuationHistory = await _MOPCECaseService.GetValuationHistory(base.GetCurrentUserId(), PCEId);    
            string jsonData = JsonConvert.SerializeObject(valuationHistory.PreviousEvaluations, new JsonSerializerSettings{ReferenceLoopHandling = ReferenceLoopHandling.Ignore});
            return Content(jsonData, "application/json");
        }    

        [HttpGet]
        public async Task<IActionResult> GetPCEs(Guid? PCECaseId = null, string Status = "All")
        {
            IEnumerable<ReturnProductionDto> productions = null;
            if (PCECaseId == null)
            {
                productions = await _MOPCECaseService.GetPCEs(base.GetCurrentUserId(), Status: Status);
            
                if (productions == null)
                {
                    return BadRequest("Unable to load {Status} PCEs");
                }
            }
            else
            {
                productions = await _MOPCECaseService.GetPCEs(base.GetCurrentUserId(), PCECaseId, Status: Status);
            
                if (productions == null)
                {
                    return BadRequest("Unable to load {Status} PCEs with PCECase ID: {PCECaseId}");
                }
            }

            string jsonData = JsonConvert.SerializeObject(productions);
            return Content(jsonData, "application/json");
        }
    }
}
