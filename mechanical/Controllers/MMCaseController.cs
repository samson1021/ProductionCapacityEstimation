﻿using Newtonsoft.Json;
using Microsoft.AspNetCore.Mvc;

using mechanical.Services.MailService;
using mechanical.Services.CaseServices;
using mechanical.Services.MMCaseService;
using mechanical.Services.CaseScheduleService;
using mechanical.Services.CaseTerminateService;
using mechanical.Services.CaseAssignmentService;

using mechanical.Models.PCE.Dto.PCECaseCommentDto;
using mechanical.Services.PCE.PCECaseService;
using mechanical.Services.PCE.MOPCECaseService;
using mechanical.Services.PCE.PCECaseCommentService;
using mechanical.Services.PCE.PCECaseScheduleService;
using mechanical.Services.PCE.PCECaseTerminateService;
using mechanical.Services.PCE.ProductionCapacityServices;
using mechanical.Services.PCE.ProductionCaseAssignmentServices;

namespace mechanical.Controllers
{
    public class MMCaseController : BaseController
    {
        private readonly ICaseService _caseService;
        private readonly IMMCaseService _mMCaseService;  
        private readonly ICaseScheduleService _caseScheduleService;
        private readonly ICaseTerminateService _caseTermnateService;  
        private readonly ICaseAssignmentService _caseAssignmentService; 
        
        private readonly IPCECaseService _PCECaseService;
        private readonly IMOPCECaseService _MOPCECaseService;  
        private readonly IPCECaseCommentService _PCEcaseCommentService;
        private readonly IPCECaseScheduleService _PCECaseScheduleService;
        private readonly IPCECaseTerminateService _PCECaseTerminateService;
        private readonly IProductionCaseAssignmentServices _productionCaseAssignmentServices;




        public MMCaseController(ICaseService caseService, IPCECaseService PCECaseService, IPCECaseScheduleService PCECaseScheduleService, IProductionCaseAssignmentServices productionCaseAssignmentServices, IMOPCECaseService MOPCECaseService, ICaseTerminateService caseTerminateService,ICaseScheduleService caseScheduleService,IMMCaseService mMCaseService , ICaseAssignmentService caseAssignment, IPCECaseTerminateService PCECaseTerminateService, IPCECaseCommentService PCEcaseCommentService)
        {
            _caseService = caseService;
            _caseAssignmentService = caseAssignment;
            _mMCaseService = mMCaseService; 
            _caseScheduleService = caseScheduleService;
            _caseTermnateService = caseTerminateService;
            _PCECaseService = PCECaseService;
            _PCECaseScheduleService = PCECaseScheduleService;
            _productionCaseAssignmentServices = productionCaseAssignmentServices;
            _MOPCECaseService = MOPCECaseService;
            _PCECaseTerminateService = PCECaseTerminateService;
            _PCEcaseCommentService = PCEcaseCommentService;
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

        //// PCE Cases /////////

        [HttpPost]
        public async Task<IActionResult> PCEAssignTeamleader(string selectedPCEIds, string employeeId)
        {
            await _productionCaseAssignmentServices.AssignProductionMakerTeamleader(base.GetCurrentUserId(), selectedPCEIds, employeeId);
            var response = new { message = "Productions assigned to MTL successfully" };
            return Ok(response);
        }
        
        [HttpPost]
        public async Task<IActionResult> PCEReAssignTeamleader(string selectedPCEIds, string employeeId)
        {
            await _productionCaseAssignmentServices.ReAssignProductionMakerTeamleader(base.GetCurrentUserId(), selectedPCEIds, employeeId);
            var response = new { message = "Productions re-assigned to MTL successfully" };
            return Ok(response);
        }

        [HttpGet]
        public IActionResult MyPCECases(string Status = "All")
        {
            ViewData["Title"] = Status + " PCE Cases";
            ViewBag.Url = "/MMCase/GetMyPCECases";
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
