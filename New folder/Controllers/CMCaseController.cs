﻿using mechanical.Services.CaseAssignmentService;
using mechanical.Services.CaseScheduleService;
using mechanical.Services.CaseServices;
using mechanical.Services.MMCaseService;
using mechanical.Services.CaseTerminateService;
using mechanical.Services.MOCaseService;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace mechanical.Controllers
{
    public class CMCaseController : BaseController
    {
        //private readonly ICaseService _caseService;
        //private readonly ICMCaseService _cMCaseService;
        private readonly ICaseAssignmentService _caseAssignmentService;
        private readonly IMMCaseService _mMCaseService;
        private readonly ICaseService _caseService;
        private readonly ICaseScheduleService _caseScheduleService;
        private readonly ICaseTerminateService _caseTermnateService;

        public CMCaseController(/*ICaseService caseService*/ ICaseTerminateService caseTermnateService, IMMCaseService mMCaseService,ICaseScheduleService caseScheduleService, ICaseAssignmentService caseAssignment, ICaseService caseService)
        {
            //_caseService = caseService;
            _caseAssignmentService = caseAssignment;
            //_cMCaseService = cMCaseService;
            _mMCaseService = mMCaseService;
            _caseService = caseService; 
            _caseScheduleService = caseScheduleService;
            _caseTermnateService = caseTermnateService;
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
            return View();
        }


        [HttpGet]
        public async Task<IActionResult> GetMyCases()
        {
            var myCase = await _mMCaseService.GetCMNewCases(GetCurrentUserId());
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
            await _caseAssignmentService.AssignCheckerTeamleader(base.GetCurrentUserId(),selectedCollateralIds, employeeId);
            var response = new { message = "Collaterals assigned successfully" };
            return Ok(response);
        }
        [HttpPost]
        public async Task<IActionResult> ReAssignCheckerTeamLeader(string selectedCollateralIds, string employeeId)
        {
            await _caseAssignmentService.ReAssignCheckerTeamleader(base.GetCurrentUserId(), selectedCollateralIds, employeeId);
            var response = new { message = "Collaterals assigned successfully" };
            return Ok(response);
        }

        [HttpGet]
        public IActionResult MyPendingCases()
        {
            return View();
        }

        //[HttpGet]
        //public async Task<IActionResult> GetMyPendingCases()
        //{
        //    var myCase = await _cMCaseService.GetMmPendingCases();
        //    string jsonData = JsonConvert.SerializeObject(myCase);
        //    return Content(jsonData, "application/json");
        //}
        //[HttpGet]
        //public async Task<IActionResult> GetDashboardCaseCount()
        //{
        //    var myCase = await _cMCaseService.GetDashboardCaseCount();
        //    string jsonData = JsonConvert.SerializeObject(myCase);
        //    return Content(jsonData, "application/json");
        //}
    }
}
