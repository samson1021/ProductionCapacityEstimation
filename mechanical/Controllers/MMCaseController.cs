using mechanical.Services.CaseAssignmentService;
using mechanical.Services.CaseScheduleService;
using mechanical.Services.CaseServices;
using mechanical.Services.CaseTerminateService;
using mechanical.Services.MMCaseService;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using mechanical.Services.PCE.ProductionCapacityServices;
using mechanical.Services.PCE.ProductionCaseAssignmentServices;
using mechanical.Services.PCE.PCEEvaluationService;

namespace mechanical.Controllers
{
    public class MMCaseController : BaseController
    {
        private readonly ICaseService _caseService;
        private readonly IMMCaseService _mMCaseService;
        private readonly ICaseAssignmentService _caseAssignmentService;
        private readonly ICaseScheduleService _caseScheduleService;
        private readonly ICaseTerminateService _caseTermnateService;
        private readonly IProductionCaseAssignmentServices _productionCaseAssignmentServices;
        private readonly IPCEEvaluationService _PCEEvaluationService;



        public MMCaseController(ICaseService caseService, IProductionCaseAssignmentServices productionCaseAssignmentServices, IPCEEvaluationService PCEEvaluationService, ICaseTerminateService caseTerminateService,ICaseScheduleService caseScheduleService,IMMCaseService mMCaseService , ICaseAssignmentService caseAssignment)
        {
            _caseService = caseService;
            _caseAssignmentService = caseAssignment;
            _mMCaseService = mMCaseService; 
            _caseScheduleService = caseScheduleService;
            _caseTermnateService = caseTerminateService;
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
            var myCase = await _mMCaseService.GetMMNewCases(GetCurrentUserId());
            if (myCase == null) { return BadRequest("Unable to load case"); }
            string jsonData = JsonConvert.SerializeObject(myCase);
            return Content(jsonData, "application/json");
        }
      
        [HttpGet]
        public async Task<IActionResult> GetMyAssignmentCases()
        {
            var myCase = await _mMCaseService.GetMyAssignmentCases(GetCurrentUserId());
            if (myCase == null) { return BadRequest("Unable to load case"); }
            string jsonData = JsonConvert.SerializeObject(myCase);
            return Content(jsonData, "application/json");
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
        public async Task<IActionResult> AssignTeamleader(string selectedCollateralIds, string employeeId)
        {
            await _caseAssignmentService.AssignMakerTeamleader(base.GetCurrentUserId(),selectedCollateralIds, employeeId);
            var response = new { message = "Collaterals assigned successfully" };
            return Ok(response); 
        }
        
        [HttpPost]
        public async Task<IActionResult> ReAssignTeamleader(string selectedCollateralIds, string employeeId)
        {
            await _caseAssignmentService.ReAssignMakerTeamleader(base.GetCurrentUserId(), selectedCollateralIds, employeeId);
            var response = new { message = "Collaterals re-assigned successfully" };
            return Ok(response);
        }


        //// PCE Cases /////////

        [HttpPost]
        public async Task<IActionResult> PCEAssignTeamleader(string selectedPCEIds, string employeeId)
        {
            await _productionCaseAssignmentServices.AssignProductMakerTeamleader(base.GetCurrentUserId(), selectedPCEIds, employeeId);
            var response = new { message = "Productions assigned successfully" };
            return Ok(response);
        }
        
        [HttpPost]
        public async Task<IActionResult> PCEReAssignTeamleader(string selectedPCEIds, string employeeId)
        {
            await _productionCaseAssignmentServices.ReAssignProductionMakerTeamleader(base.GetCurrentUserId(), selectedPCEIds, employeeId);
            var response = new { message = "Productions re-assigned successfully" };
            return Ok(response);
        }

        [HttpGet]
        public IActionResult MyPCECases(string Status = "New")
        {
            ViewData["Title"] = Status + " PCE Cases";
            ViewBag.Url = "/MMCase/GetMyPCECases";
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
        public async Task<IActionResult> GetMyDashboardPCECaseCount()
        {
            var pcecase = await _PCEEvaluationService.GetDashboardPCECaseCount(base.GetCurrentUserId());
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

        //[HttpGet]
        //public IActionResult MyPendingCases()
        //{
        //    return View();
        //}

        //[HttpGet]
        //public async Task<IActionResult> GetMyPendingCases()
        //{
        //    var myCase = await _mMCaseService.GetMmPendingCases();
        //    string jsonData = JsonConvert.SerializeObject(myCase);
        //    return Content(jsonData, "application/json");
        //}
        //[HttpGet]
        //public async Task<IActionResult> GetDashboardCaseCount()
        //{
        //    var myCase = await _mMCaseService.GetDashboardCaseCount();
        //    string jsonData = JsonConvert.SerializeObject(myCase);
        //    return Content(jsonData, "application/json");
        //}
    }
}
