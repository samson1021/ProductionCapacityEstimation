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

        public MOPCECaseController(ILogger<MOPCECaseController> Logger, IMOPCECaseService MOPCECaseService, IProductionCaseScheduleService ProductionCaseScheduleService, IMailService mailService, IPCECaseTerminateService PCECaseTerminateService)
        // public MOPCECaseController(ILogger<MOPCECaseController> Logger, IMOPCECaseService MOPCECaseService, IProductionCaseTerminateService ProductionCaseTerminateService, IProductionCaseScheduleService ProductionCaseScheduleService, IMailService mailService)
        {
            _logger = Logger;
            _mailService = mailService;    
            _MOPCECaseService = MOPCECaseService;
            _ProductionCaseScheduleService = ProductionCaseScheduleService;
             _PCECaseTerminateService = PCECaseTerminateService;
        }

        // [HttpGet("{Id}")]
        [HttpGet]
        public async Task<IActionResult> PCEDetail(Guid PCEId)
        {
            try
            {
                var userId = base.GetCurrentUserId();
                var pceDetail = await _MOPCECaseService.GetPCEDetails(userId, PCEId);

                if (pceDetail.ProductionCapacity == null)
                {
                    return RedirectToAction("PCENewCases");
                }
                
                ViewData["CurrentUser"] = await _MOPCECaseService.GetUser(userId);
                ViewData["Reestimation"] = pceDetail.Reestimation;
                ViewData["PCE"] = pceDetail.ProductionCapacity;
                ViewData["LatestEvaluation"] = pceDetail.PCEValuationHistory.LatestEvaluation;
                ViewData["PreviousEvaluations"] = pceDetail.PCEValuationHistory.PreviousEvaluations;
                ViewData["PCECase"] = pceDetail.PCECase;
                ViewData["ProductionFiles"] = pceDetail.RelatedFiles;
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
            var userId = base.GetCurrentUserId();
            var pceCase = await _MOPCECaseService.GetPCECase(userId, Id);
            if (pceCase == null)
            {
                return RedirectToAction("MyPCECases");
            }
             var pceCaseTerminate = await _PCECaseTerminateService.GetCaseTerminates(Id);
            var productionCaseSchedule = await _ProductionCaseScheduleService.GetProductionCaseSchedules(Id);
            
            ViewData["CurrentUser"] = await _MOPCECaseService.GetUser(userId);
            ViewData["PCECaseId"] = pceCase.Id;
            ViewData["PCECase"] = pceCase;
            ViewData["PCECaseTerminate"] = pceCaseTerminate;
            ViewData["ProductionCaseSchedule"] = productionCaseSchedule;
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
        public async Task<IActionResult> GetMyLatestPCECases(string Status)
        {
            var limit = 10;
            var pceCases = await _MOPCECaseService.GetPCECases(base.GetCurrentUserId(), Status, limit);
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
            var userId = base.GetCurrentUserId();
            var pceCase = await _MOPCECaseService.GetPCECase(userId, Id);
            var ProductionCaseSchedule = await _ProductionCaseScheduleService.GetProductionCaseSchedules(Id);
            if (pceCase == null) 
            { 
                return RedirectToAction("NewPCECases"); 
            }
            ViewData["PCECase"] = pceCase;
            ViewData["ProductionCaseSchedule"] = ProductionCaseSchedule;
            ViewData["Id"] = userId;
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
        
        // // Returned 
        // [HttpGet]
        // public IActionResult MyReturnedPCEs()
        // {
        //     ViewData["Title"] = "All My Returned PCEs";
        //     return View("ReturnedPCEs");
        // }

        // [HttpGet]
        // public async Task<IActionResult> GetMyReturnedPCEs()
        // {
        //     var myPCEs = await _MOPCECaseService.GetReturnedPCEs(base.GetCurrentUserId());
        //     string jsonData = JsonConvert.SerializeObject(myPCEs);
        //     return Content(jsonData, "application/json");
        // }

        [HttpPost]
        public async Task<IActionResult> CreateSchedule(ProductionCaseSchedulePostDto Dto)
        {
            var productionCaseSchedule = await _ProductionCaseScheduleService.CreateProductionCaseSchedule(base.GetCurrentUserId(), Dto);
            
            if (productionCaseSchedule == null) 
            { 
                return BadRequest("Unable to create PCECase Schedule"); 
            }
            
            return await SendScheduleEmail(productionCaseSchedule, "Valuation Schedule for PCECase Number ");
        }

        [HttpPost]
        public async Task<IActionResult> UpdateSchedule(Guid id, ProductionCaseSchedulePostDto Dto)
        {
            var productionCaseSchedule = await _ProductionCaseScheduleService.UpdateProductionCaseSchedule(base.GetCurrentUserId(), id, Dto);
            
            if (productionCaseSchedule == null) 
            { 
                return BadRequest("Unable to update PCECase Schedule"); 
            }

            return await SendScheduleEmail(productionCaseSchedule, "Valuation Schedule Update for PCECase Number ");
        }

        [HttpPost]
        public async Task<IActionResult> ApproveSchedule(Guid id)
        {
            var productionCaseSchedule = await _ProductionCaseScheduleService.ApproveProductionCaseSchedule(id);
            
            if (productionCaseSchedule == null) 
            { 
                return BadRequest("Unable to approve PCECase Schedule"); 
            }
            
            return Ok(productionCaseSchedule);
        }

        private async Task<IActionResult> SendScheduleEmail(ProductionCaseScheduleReturnDto productionCaseSchedule, string subjectPrefix)
        {
            var pceCaseInfo = await _MOPCECaseService.GetPCECase(base.GetCurrentUserId(), productionCaseSchedule.PCECaseId);

            await _mailService.SendEmail(new MailPostDto
            {
                SenderEmail = "sender@cbe.com.et",
                SenderPassword = "test@1234",
                RecipantEmail = "recipient@cbe.com.et",
                Subject = $"{subjectPrefix}{pceCaseInfo.CaseNo}",
                Body = $"Dear! Valuation Schedule For Applicant: {pceCaseInfo.ApplicantName} is {productionCaseSchedule.ScheduleDate}. For further details, please check the Production Valuation System."
            });

            string jsonData = JsonConvert.SerializeObject(productionCaseSchedule);
            
            return Ok(productionCaseSchedule);
        }
    }
}
