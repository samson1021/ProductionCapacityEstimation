using Newtonsoft.Json;
using Microsoft.AspNetCore.Mvc;

using mechanical.Services.MailService;
using mechanical.Services.CaseServices;
using mechanical.Services.MMCaseService;
using mechanical.Services.CaseScheduleService;
using mechanical.Services.CaseTerminateService;
using mechanical.Services.CaseAssignmentService;
using mechanical.Services.UploadFileService;
using Microsoft.AspNetCore.Authorization;

namespace mechanical.Controllers
{
    [Authorize(Roles = "Maker Manager,District Valuation Manager ,Maker Officer, Maker TeamLeader, Relation Manager,Checker Manager, Checker TeamLeader, Checker Officer")]
    public class MMCaseController : BaseController
    {
        private readonly ICaseService _caseService;
        private readonly IMMCaseService _mMCaseService;  
        private readonly ICaseScheduleService _caseScheduleService;
        private readonly ICaseTerminateService _caseTermnateService;  
        private readonly ICaseAssignmentService _caseAssignmentService;
        private readonly IUploadFileService _uploadFileService;

        public MMCaseController(ICaseService caseService,IUploadFileService uploadFileService, ICaseTerminateService caseTerminateService,ICaseScheduleService caseScheduleService,IMMCaseService mMCaseService , ICaseAssignmentService caseAssignment)
        {
            _caseService = caseService;
            _mMCaseService = mMCaseService;
            _caseAssignmentService = caseAssignment;
            _caseScheduleService = caseScheduleService;
            _caseTermnateService = caseTerminateService;
            _uploadFileService = uploadFileService;
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
            var moFile = await _uploadFileService.GetMoUploadFile(Id);
            ViewData["moFile"] = moFile;
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
