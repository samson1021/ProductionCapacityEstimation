using AutoMapper;
using System;
using System.Data;
using System.Linq;
using System.Net.Mail;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Collections.Generic;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;

using mechanical.Data;
using mechanical.Models;
using mechanical.Models.Entities;
using mechanical.Models.Dto.MailDto;
using mechanical.Models.PCE.Dto.ProductionCaseScheduleDto;
using mechanical.Services.MailService;
using mechanical.Services.PCE.MOPCECaseService;
using mechanical.Services.PCE.ProductionCaseScheduleService;
using mechanical.Services.PCE.PCECaseTerminateService;
using mechanical.Models.Dto.CaseScheduleDto;
using mechanical.Services.PCE.PCECaseService;
// using mechanical.Services.PCE.ProductionCaseTerminateService;

namespace mechanical.Controllers
{
    // [Authorize(Roles = "Maker Manager,District Valuation Manager ,Maker Officer, Maker TeamLeader, Relation Manager,Checker Manager, Checker TeamLeader, Checker Officer")]

    public class MOPCECaseController : BaseController
    {
        private readonly IMailService _mailService;
        private readonly ILogger<MOPCECaseController> _logger;
        private readonly IMOPCECaseService _MOPCECaseService;
        private readonly IProductionCaseScheduleService _ProductionCaseScheduleService;
        private readonly IPCECaseTerminateService _PCECaseTerminateService;
        private readonly CbeContext _cbeContext;
        private readonly IPCECaseService _PCECaseService;
        public MOPCECaseController(ILogger<MOPCECaseController> Logger, IPCECaseService pCECaseService,CbeContext cbeContext,  IMOPCECaseService MOPCECaseService, IProductionCaseScheduleService ProductionCaseScheduleService, IMailService mailService, IPCECaseTerminateService PCECaseTerminateService)
        // public MOPCECaseController(ILogger<MOPCECaseController> Logger, IMOPCECaseService MOPCECaseService, IProductionCaseTerminateService ProductionCaseTerminateService, IProductionCaseScheduleService ProductionCaseScheduleService, IMailService mailService)
        {
            _logger = Logger;
            _cbeContext = cbeContext;
            _mailService = mailService;    
            _MOPCECaseService = MOPCECaseService;
            _ProductionCaseScheduleService = ProductionCaseScheduleService;
             _PCECaseTerminateService = PCECaseTerminateService;
            _PCECaseService = pCECaseService;
        }

        // [HttpGet("{Id}")]
        [HttpGet]
        public async Task<IActionResult> PCEDetail(Guid PCEId)
        {
            try
            {
                var pceDetail = await _MOPCECaseService.GetPCEDetails(base.GetCurrentUserId(), PCEId);
                if (pceDetail.ProductionCapacity == null)
                {
                    return RedirectToAction("MyPCEs");
                }
                
                var currentUser = await _MOPCECaseService.GetUser(base.GetCurrentUserId());
                ViewData["CurrentUser"] = currentUser;
                // ViewData["CurrentUserRole"] = pceDetail.CurrentUserRole;
                ViewData["Reestimation"] = pceDetail.Reestimation;
                ViewData["PCE"] = pceDetail.ProductionCapacity;
                ViewData["LatestEvaluation"] = pceDetail.PCEValuationHistory.LatestEvaluation;
                ViewData["PreviousEvaluations"] = pceDetail.PCEValuationHistory.PreviousEvaluations;
                ViewData["PCECase"] = pceDetail.PCECase;
                ViewData["ProductionFiles"] = pceDetail.RelatedFiles;
                ViewData["Remark"] = pceDetail.Remark;
                ViewData["Reject"] = pceDetail.Reject;
                ViewData["RejectedBy"] = pceDetail.RejectedBy;

                return View(pceDetail.ProductionCapacity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching PCE details for ID {PCEId}", PCEId);
                return View("Error", new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetPCECase(Guid Id)
        {
            var pceCase = await _MOPCECaseService.GetPCECase(base.GetCurrentUserId(), Id);
            if (pceCase == null)
            {
                return BadRequest("Unable to load PCE Cases");
            }
            string jsonData = JsonConvert.SerializeObject(pceCase);
            return Content(jsonData, "application/json");
        }

        [HttpGet]
        public async Task<IActionResult> PCECaseDetail(Guid Id, string Status)
        {
            var pceCase = await _MOPCECaseService.GetPCECase(base.GetCurrentUserId(), Id);
            if (pceCase == null)
            {
                return RedirectToAction("MyPCECases");
            }
            var ProductionCaseSchedule = await _ProductionCaseScheduleService.GetProductionCaseSchedules(Id);
            var currentUser = await _MOPCECaseService.GetUser(base.GetCurrentUserId());
            ViewData["CurrentUser"] = currentUser;
           // ViewData["CurrentUser"] = base.GetCurrentUserId();
            ViewData["PCECaseId"] = pceCase.Id;
            ViewData["PCECase"] = pceCase;
            ViewData["ProductionCaseSchedule"] = ProductionCaseSchedule;
            ViewData["Title"] = Status + " PCE Case Details";
            ViewBag.Status = Status;
            return View();
        }


        [HttpGet]
        public IActionResult MyPCECases(string Status = "New")
        {
            ViewData["Title"] = Status + " PCE Cases";
            ViewBag.Url = "/MOCase/GetMyPCECases";
            ViewBag.Status = Status;
            return View("PCECases");
        }

        [HttpGet]
        public async Task<IActionResult> GetMyPCECases(string Status)
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
        public async Task<IActionResult> GetLatestMyPCECases(string Status)
        {
            var Limit = 10;
            var pceCases = await _MOPCECaseService.GetPCECases(base.GetCurrentUserId(), Status, Limit);
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

        //// PCEs /////////
        [HttpGet]
        public IActionResult PCEs(string Status)
        {
            ViewData["Title"] = Status + " PCEs";
            ViewBag.Status = Status;
            return View("PCEs");
        }

        [HttpGet]
        public async Task<IActionResult> GetPCEs(Guid PCECaseId, string Status)
        {
            var productions = await _MOPCECaseService.GetPCEs(base.GetCurrentUserId(), PCECaseId, Status: Status);

            if (productions == null)
            {
                return BadRequest("Unable to load {Status} PCEs with PCECase ID: {PCECaseId}");
            }

            string jsonData = JsonConvert.SerializeObject(productions);

            return Content(jsonData, "application/json");
        }

        //// My PCEs /////////
        [HttpGet]
        public IActionResult MyPCEs(string Status)
        {
            ViewData["Title"] = "My " + Status + " PCEs";
            ViewBag.Status = Status;
            return View("PCEs");
        }

        [HttpGet]
        public async Task<IActionResult> GetMyPCEs(string Status)
        {
            var myPCEs = await _MOPCECaseService.GetPCEs(base.GetCurrentUserId(), Status: Status);
            if (myPCEs == null)
            {
                return BadRequest("Unable to load {Status} PCEs");
            }
            string jsonData = JsonConvert.SerializeObject(myPCEs);
            return Content(jsonData, "application/json");
        }


        // [HttpGet]
        // public async Task<IActionResult> GetMyDashboardPCECount()
        // {
        //     var myPCEs = await _MOPCECaseService.GetDashboardPCECount(base.GetCurrentUserId());
        //     string jsonData = JsonConvert.SerializeObject(myPCEs);
        //     return Content(jsonData, "application/json");
        // }

        [HttpGet]
        public async Task<IActionResult> GetMyLatestValuation(Guid PCEId)
        {
            var valuationHistory = await _MOPCECaseService.GetValuationHistory(base.GetCurrentUserId(), PCEId);    
            string jsonData = JsonConvert.SerializeObject(valuationHistory.LatestEvaluation, new JsonSerializerSettings{ReferenceLoopHandling = ReferenceLoopHandling.Ignore});
            return Content(jsonData, "application/json");
        }

        [HttpGet]
        public async Task<IActionResult> GetMyPreviousValuations(Guid PCEId)
        {
            var valuationHistory = await _MOPCECaseService.GetValuationHistory(base.GetCurrentUserId(), PCEId);    
            string jsonData = JsonConvert.SerializeObject(valuationHistory.PreviousEvaluations, new JsonSerializerSettings{ReferenceLoopHandling = ReferenceLoopHandling.Ignore});
            return Content(jsonData, "application/json");
        }

        public IActionResult RemarkPCECases()
        {
            return View();
        }

        public async Task<IActionResult> RemarkPCECase(Guid Id)
        {
            var pceCase = await _MOPCECaseService.GetPCECase(base.GetCurrentUserId(), Id);
            var ProductionCaseSchedule = await _ProductionCaseScheduleService.GetProductionCaseSchedules(Id);
            if (pceCase == null) 
            { 
                return RedirectToAction("NewPCECases"); 
            }
            ViewData["pcecaseDtos"] = pceCase;
            ViewData["ProductionCaseSchedule"] = ProductionCaseSchedule;
            ViewData["Id"] = base.GetCurrentUserId();
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetRemarkedPCECases()
        {
            var pceCases = await _MOPCECaseService.GetRemarkedPCECases(GetCurrentUserId());
            if (pceCases == null) 
            { 
                return BadRequest("Unable to load PCE Cases"); 
            }
            string jsonData = JsonConvert.SerializeObject(pceCases);
            return Content(jsonData, "application/json");
        }
        

   
    }
}
