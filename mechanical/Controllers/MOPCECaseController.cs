﻿using AutoMapper;
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
using mechanical.Models.PCE.Dto.PCECaseScheduleDto;
using mechanical.Services.MailService;
using mechanical.Services.PCE.MOPCECaseService;
using mechanical.Services.PCE.PCECaseScheduleService;
using mechanical.Services.PCE.PCECaseTerminateService;

namespace mechanical.Controllers
{
    // [Authorize(Roles = "Maker Manager,District Valuation Manager ,Maker Officer, Maker TeamLeader, Relation Manager,Checker Manager, Checker TeamLeader, Checker Officer")]

    public class MOPCECaseController : BaseController
    {
        private readonly IMailService _MailService;
        private readonly ILogger<MOPCECaseController> _logger;
        private readonly IMOPCECaseService _MOPCECaseService;
        private readonly IPCECaseScheduleService _PCECaseScheduleService;
        private readonly IPCECaseTerminateService _PCECaseTerminateService;

        public MOPCECaseController(ILogger<MOPCECaseController> Logger, IMOPCECaseService MOPCECaseService, IPCECaseScheduleService PCECaseScheduleService, IMailService MailService, IPCECaseTerminateService PCECaseTerminateService)
        {
            _logger = Logger;
            _MailService = MailService;    
            _MOPCECaseService = MOPCECaseService;
            _PCECaseScheduleService = PCECaseScheduleService;
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
                    if (pceDetail.PCECase != null)
                    {
                        return RedirectToAction("PCECaseDetail", "MOPCECase", new { Id = pceDetail.PCECase.Id });   
                    }
                    return RedirectToAction("MyPCECases");
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
        public async Task<IActionResult> PCECaseDetail(Guid Id, string Status = "All")
        {
            var userId = base.GetCurrentUserId();
            var pceCase = await _MOPCECaseService.GetPCECase(userId, Id);
            if (pceCase == null)
            {
                return RedirectToAction("MyPCECases");
            }
            var pceCaseTerminate = await _PCECaseTerminateService.GetCaseTerminates(Id);
            var pceCaseSchedule = await _PCECaseScheduleService.GetPCECaseSchedules(Id);
            
            ViewData["CurrentUser"] = await _MOPCECaseService.GetUser(userId);
            ViewData["PCECaseId"] = pceCase.Id;
            ViewData["PCECase"] = pceCase;
            ViewData["PCECaseTerminate"] = pceCaseTerminate;
            ViewData["PCECaseSchedule"] = pceCaseSchedule;
            ViewData["Title"] = Status + " PCE Case Details";             
            ViewBag.Status = Status;

            return View();
        }


        [HttpGet]
        public IActionResult MyPCECases(string Status = "All")
        {
            ViewData["Title"] = Status + " PCE Cases";
            ViewBag.Url = "/MOCase/GetMyPCECases";
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
        public async Task<IActionResult> GetMyDashboardPCECasesCount()
        {
            var pceCasesCount = await _MOPCECaseService.GetDashboardPCECasesCount(base.GetCurrentUserId());
            string jsonData = JsonConvert.SerializeObject(pceCasesCount);
            return Content(jsonData, "application/json");
        }

        //// PCEs /////////
        [HttpGet]
        public IActionResult PCEs(string Status = "All")
        {
            ViewData["Title"] = Status + " PCEs";
            ViewBag.Status = Status;
            return View("PCEs");
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

        //// My PCEs /////////
        [HttpGet]
        public IActionResult MyPCEs(string Status = "All")
        {
            ViewData["Title"] = "My " + Status + " PCEs";
            ViewBag.Status = Status;
            return View("PCEs");
        }

        [HttpGet]
        public async Task<IActionResult> GetMyPCEs(string Status = "All")
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
            var pceCaseSchedule = await _PCECaseScheduleService.GetPCECaseSchedules(Id);
            if (pceCase == null) 
            { 
                return RedirectToAction("MyPCECases"); 
            }
            ViewData["PCECase"] = pceCase;
            ViewData["PCECaseSchedule"] = pceCaseSchedule;
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
        public async Task<IActionResult> CreateSchedule(PCECaseSchedulePostDto Dto)
        {
            var pceCaseSchedule = await _PCECaseScheduleService.CreatePCECaseSchedule(base.GetCurrentUserId(), Dto);
            
            if (pceCaseSchedule == null) 
            { 
                return BadRequest("Unable to create PCECase Schedule"); 
            }
            
            return await SendScheduleEmail(pceCaseSchedule, "Valuation Schedule for PCECase Number ");
        }

        [HttpPost]
        public async Task<IActionResult> UpdateSchedule(Guid id, PCECaseSchedulePostDto Dto)
        {
            var pceCaseSchedule = await _PCECaseScheduleService.UpdatePCECaseSchedule(base.GetCurrentUserId(), id, Dto);
            
            if (pceCaseSchedule == null) 
            { 
                return BadRequest("Unable to update PCECase Schedule"); 
            }

            return await SendScheduleEmail(pceCaseSchedule, "Valuation Schedule Update for PCECase Number ");
        }

        [HttpPost]
        public async Task<IActionResult> ApproveSchedule(Guid id)
        {
            var pceCaseSchedule = await _PCECaseScheduleService.ApprovePCECaseSchedule(id);
            
            if (pceCaseSchedule == null) 
            { 
                return BadRequest("Unable to approve PCECase Schedule"); 
            }
            
            return Ok(pceCaseSchedule);
        }

        private async Task<IActionResult> SendScheduleEmail(PCECaseScheduleReturnDto pceCaseSchedule, string subjectPrefix)
        {
            var pceCaseInfo = await _MOPCECaseService.GetPCECase(base.GetCurrentUserId(), pceCaseSchedule.PCECaseId);

            await _MailService.SendEmail(new MailPostDto
            {
                SenderEmail = "sender@cbe.com.et",
                SenderPassword = "test@1234",
                RecipantEmail = "recipient@cbe.com.et",
                Subject = $"{subjectPrefix}{pceCaseInfo.CaseNo}",
                Body = $"Dear! Valuation Schedule For Applicant: {pceCaseInfo.ApplicantName} is {pceCaseSchedule.ScheduleDate}. For further details, please check the Production Valuation System."
            });

            string jsonData = JsonConvert.SerializeObject(pceCaseSchedule);
            
            return Ok(pceCaseSchedule);
        }
    }
}
