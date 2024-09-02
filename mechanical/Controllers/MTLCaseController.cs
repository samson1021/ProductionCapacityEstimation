using mechanical.Services.CaseAssignmentService;
using mechanical.Services.CaseScheduleService;
using mechanical.Services.CaseServices;
using mechanical.Services.CaseTerminateService;
using mechanical.Services.MMCaseService;
using mechanical.Services.PCE.PCECaseService;
using mechanical.Services.PCE.ProductionCaseAssignmentServices;
using mechanical.Services.PCE.PCEEvaluationService;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace mechanical.Controllers
{
    public class MTLCaseController : BaseController
    {
        private readonly ICaseService _caseService;
        private readonly IMMCaseService _mmCaseService;
        private readonly ICaseAssignmentService _caseAssignmentService;
        private readonly ICaseScheduleService _caseScheduleService;
        private readonly ICaseTerminateService _caseTermnateService;
        private readonly IPCECaseService _PCECaseService;
        private readonly IProductionCaseAssignmentServices _productionCaseAssignmentServices;
        private readonly IPCEEvaluationService _PCEEvaluationService;

        public MTLCaseController(ICaseService caseService, ICaseTerminateService caseTermnateService, IPCECaseService PCECaseService, IProductionCaseAssignmentServices productionCaseAssignmentServices, IPCEEvaluationService PCEEvaluationService, ICaseScheduleService caseScheduleService, ICaseAssignmentService caseAssignment,IMMCaseService mMCaseService)
        {
            _caseService = caseService;
            _caseAssignmentService = caseAssignment;
            _mmCaseService = mMCaseService; 
            _caseScheduleService = caseScheduleService;
            _caseTermnateService = caseTermnateService;
            _PCECaseService = PCECaseService;
            _productionCaseAssignmentServices = productionCaseAssignmentServices;
            _PCEEvaluationService = PCEEvaluationService;
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
            
            await _productionCaseAssignmentServices.AssignProductionMakerOfficer(base.GetCurrentUserId(),selectedPCEIds, employeeId);
            var response = new { message = "Productions assigned to MO successfully" };
            return Ok(response);
        }
        [HttpPost]
        public async Task<IActionResult> PCEReAssignMakerOfficer(string selectedPCEIds, string employeeId)
        {
            await _productionCaseAssignmentServices.ReAssignProductionMakerOfficer(base.GetCurrentUserId(), selectedPCEIds, employeeId);
            var response = new { message = "Productions re-assigned to MO successfully" };
            return Ok(response);
        }
        
        [HttpGet]
        public IActionResult MyPCECases(string Status = "New")
        {
            ViewData["Title"] = Status + " PCE Cases";
            ViewBag.Url = "/MTLCase/GetMyPCECases";
            ViewBag.Status = Status;
            return View("PCECases");
        }

        [HttpGet]
        public async Task<IActionResult> GetMyPCECases(string Status)
        {
            var pcecase = await _PCEEvaluationService.GetPCECases(base.GetCurrentUserId(), Status);
            if (pcecase == null)
            {
                return BadRequest("Unable to load {Status} PCE Cases");
            }
            string jsonData = JsonConvert.SerializeObject(pcecase);
            return Content(jsonData, "application/json");
        }

        [HttpGet]
        public async Task<IActionResult> GetPCEs(Guid PCECaseId, string Status)
        {
            var Stage = string.Empty;
            var productions = await _PCEEvaluationService.GetPCEs(base.GetCurrentUserId(), PCECaseId, Stage, Status);

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
            var pcecase = await _PCEEvaluationService.GetDashboardPCECasesCount(base.GetCurrentUserId());
            string jsonData = JsonConvert.SerializeObject(pcecase);
            return Content(jsonData, "application/json");
        }

        [HttpGet]
        public async Task<IActionResult> PCECaseDetail(Guid Id, string Status)
        {

            var pcecase = await _PCEEvaluationService.GetPCECase(base.GetCurrentUserId(), Id);
            if (pcecase == null)
            {
                return RedirectToAction("MyPCECases");
            }
            ViewData["PCECaseId"] = Id;
            ViewData["PCECase"] = pcecase;
            ViewData["Title"] = Status + " PCE Case Details";
            ViewBag.Status = Status;

            return View();
        }
        
        [HttpGet]
        public async Task<IActionResult> GetMyAssignmentPCECases()
        {
            var myPCECase = await _PCECaseService.GetMyAssignmentPCECases(base.GetCurrentUserId());
            if (myPCECase == null) { return BadRequest("Unable to load PCEcase"); }
            string jsonData = JsonConvert.SerializeObject(myPCECase);
            return Content(jsonData, "application/json");
        }

        public IActionResult MyPCEAssignments()
        {
            return View();
        }

        public async Task<IActionResult> MyPCEAssignment(Guid Id)
        {            
            var pcecase = await _PCEEvaluationService.GetPCECase(base.GetCurrentUserId(), Id);
            if (pcecase == null)
            {
                return RedirectToAction("MyPCECases");
            }
            ViewData["PCECaseId"] = Id;

            return View();
        }
    }
}
