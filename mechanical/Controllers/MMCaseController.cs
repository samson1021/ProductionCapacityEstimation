using mechanical.Services.CaseAssignmentService;
using mechanical.Services.CaseScheduleService;
using mechanical.Services.CaseServices;
using mechanical.Services.CaseTerminateService;
using mechanical.Services.MMCaseService;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using mechanical.Services.PCE.ProductionCapacityServices;
using mechanical.Services.PCE.ProductionCaseAssignmentServices;

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



        public MMCaseController(ICaseService caseService, IProductionCaseAssignmentServices productionCaseAssignmentServices, ICaseTerminateService caseTerminateService,ICaseScheduleService caseScheduleService,IMMCaseService mMCaseService , ICaseAssignmentService caseAssignment)
        {
            _caseService = caseService;
            _caseAssignmentService = caseAssignment;
            _mMCaseService = mMCaseService; 
            _caseScheduleService = caseScheduleService;
            _caseTermnateService = caseTerminateService;
            _productionCaseAssignmentServices = productionCaseAssignmentServices;
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
        public async Task<IActionResult> PCEAssignTeamleader(string selectedCollateralIds, string employeeId)
        {
            await _productionCaseAssignmentServices.AssignProductMakerTeamleader(base.GetCurrentUserId(), selectedCollateralIds, employeeId);
            var response = new { message = "Collaterals assigned successfully" };
            return Ok(response);
        }
        
        [HttpPost]
        public async Task<IActionResult> ReAssignTeamleader(string selectedCollateralIds, string employeeId)
        {
            await _caseAssignmentService.ReAssignMakerTeamleader(base.GetCurrentUserId(), selectedCollateralIds, employeeId);
            var response = new { message = "Collaterals assigned successfully" };
            return Ok(response);
        }
        [HttpGet]
        public IActionResult MyPCEs(string Status)
        {
            ViewData["Title"] = "My " + Status + " PCEs";
            ViewBag.Status = Status;
            return View("MyPCEs");
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
