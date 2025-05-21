using mechanical.Services.CaseAssignmentService;
using mechanical.Services.CaseScheduleService;
using mechanical.Services.CaseServices;
using mechanical.Services.CaseTerminateService;
using mechanical.Services.MMCaseService;
using mechanical.Services.UploadFileService;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace mechanical.Controllers
{
    public class CTLCaseController : BaseController
    {
        //private readonly ICTLCaseService _cTLCaseService;
        private readonly ICaseService _caseService;
        private readonly ICaseAssignmentService _caseAssignmentService;
        private readonly IMMCaseService _mmCaseService;
        private readonly ICaseScheduleService _caseScheduleService;
        private readonly ICaseTerminateService _caseTermnateService;
        private readonly IUploadFileService _uploadFileService;


        public CTLCaseController(/*ICTLCaseService cTLCaseService,*/ICaseTerminateService caseTermnateService, IUploadFileService uploadFileService, ICaseAssignmentService caseAssignment, ICaseScheduleService caseScheduleService, ICaseService caseService, IMMCaseService mMCaseService)
        {
            //_cTLCaseService = cTLCaseService;
            _caseAssignmentService = caseAssignment;
            _caseService = caseService;
            _mmCaseService = mMCaseService;
            _caseScheduleService = caseScheduleService;
            _caseTermnateService = caseTermnateService;
            _uploadFileService = uploadFileService;
        }

        [HttpGet]
        public IActionResult MyCases()
        {

            return View();
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
            var moFile = await _uploadFileService.GetMoUploadFile(Id);
            ViewData["moFile"] = moFile;
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
            var moFile = await _uploadFileService.GetMoUploadFile(Id);
            ViewData["moFile"] = moFile;
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> AssignCheckerOfficer(string selectedCollateralIds, string employeeId)
        {
            await _caseAssignmentService.AssignCheckerTeamleader(base.GetCurrentUserId(), selectedCollateralIds, employeeId);
            var response = new { message = "Collaterals assigned successfully" };
            return Ok(response);
        }
        [HttpPost]
        public async Task<IActionResult> ReAssignCheckerOfficer(string selectedCollateralIds, string employeeId)
        {
            await _caseAssignmentService.ReAssignCheckerTeamleader(base.GetCurrentUserId(), selectedCollateralIds, employeeId);
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
        //    var myCase = await _cTLCaseService.GetCTLPendingCases();
        //    string jsonData = JsonConvert.SerializeObject(myCase);
        //    return Content(jsonData, "application/json");
        //}
    }
}