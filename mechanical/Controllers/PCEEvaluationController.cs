using AutoMapper;
using Humanizer;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Diagnostics;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;

using mechanical.Data;
using mechanical.Models;
using mechanical.Models.Dto.MailDto;
using mechanical.Services.MailService;
using mechanical.Models.PCE.Entities;
using mechanical.Services.PCE.MOPCECaseService;
using mechanical.Models.PCE.Dto.PCEEvaluationDto;
using mechanical.Services.PCE.PCEEvaluationService;
using mechanical.Services.PCE.ProductionCapacityServices;

namespace mechanical.Controllers
{
    // [Authorize(Roles = "Maker Manager,District Valuation Manager ,Maker Officer, Maker TeamLeader, Relation Manager,Checker Manager, Checker TeamLeader, Checker Officer")]

    public class PCEEvaluationController : BaseController
    {
        private readonly IMapper _mapper;
        private readonly IMailService _MailService;
        private readonly ILogger<PCEEvaluationController> _logger;
        private readonly IMOPCECaseService _MOPCECaseService;
        private readonly IPCEEvaluationService _PCEEvaluationService;
        private readonly IProductionCapacityServices _ProductionCapacityService;

        public PCEEvaluationController(IMapper mapper, IMailService MailService, IMOPCECaseService MOPCECaseService, IPCEEvaluationService PCEEvaluationService, ILogger<PCEEvaluationController> logger, IProductionCapacityServices ProductionCapacityService)
        {
            _mapper = mapper;
            _logger = logger;
            _MailService = MailService;   
            _MOPCECaseService  = MOPCECaseService;
            _PCEEvaluationService = PCEEvaluationService;
            _ProductionCapacityService = ProductionCapacityService;            
        }

        [HttpGet]
        public async Task<IActionResult> Create(Guid PCEId)
        {
            try
            {
                var userId = base.GetCurrentUserId();
                var pceEvaluation = await _PCEEvaluationService.GetPCEEvaluationByPCEId(userId, PCEId);

                // if (pceEvaluation != null && pceEvaluation.PCE.CurrentStatus != "Reestimate")
                if (pceEvaluation != null)
                {            
                    return RedirectToAction("PCEDetail", "MOPCECase", new { PCEId = pceEvaluation.PCEId });
                }
                
                var pceDetail = await _MOPCECaseService.GetPCEDetails(userId, PCEId);     

                if (pceDetail.ProductionCapacity == null)
                {
                    return RedirectToAction("MyPCECases", "MOPCECase");
                }
            
                ViewData["Reestimation"] = pceDetail.Reestimation;
                ViewData["LatestEvaluation"] = pceDetail.PCEValuationHistory.LatestEvaluation;
                ViewData["PreviousEvaluations"] = pceDetail.PCEValuationHistory.PreviousEvaluations;
                ViewData["PCECase"] = pceDetail.PCECase;
                ViewData["PCE"] = pceDetail.ProductionCapacity;

                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating production valuation");
                return View("Error", new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PCEEvaluationPostDto Dto)
        {
            if (ModelState.IsValid)
            {
                var userId = base.GetCurrentUserId();
                try
                {
                    var pceEvaluation = await _PCEEvaluationService.CreatePCEEvaluation(userId, Dto);

                    return RedirectToAction("PCEDetail", "MOPCECase", new { PCEId = pceEvaluation.PCEId });
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error creating production valuation for user {userId}", userId);
                    ModelState.AddModelError("", "An error occurred while creating the production valuation.");
                }
            }
            return View(Dto);
        }

        [HttpGet]
        public async Task<IActionResult> Update(Guid Id)
        {
            try
            {
                var userId = base.GetCurrentUserId();
                var pceEvaluation = await _PCEEvaluationService.GetPCEEvaluation(userId, Id);

                if (pceEvaluation == null)
                {
                    return RedirectToAction("PCEDetail", "MOPCECase", new { PCEId = pceEvaluation.PCEId });
                }

                var pce = await _ProductionCapacityService.GetProduction(userId, pceEvaluation.PCEId);
                var pceCase = await _MOPCECaseService.GetPCECase(userId, pce.PCECaseId);

                ViewData["PCE"] = pce;
                ViewData["PCECase"] = pceCase;
                ViewData["PCECase"] = pceCase;

                return View(_mapper.Map<PCEEvaluationUpdateDto>(pceEvaluation));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching production valuation for updating, ID {Id}", Id);
                return View("Error", new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(Guid Id, PCEEvaluationUpdateDto Dto)
        {
            if (ModelState.IsValid)
            {
                if (Id != Dto.Id)
                {
                    return BadRequest();
                }
                try
                {
                    var pceEvaluation = await _PCEEvaluationService.UpdatePCEEvaluation(base.GetCurrentUserId(), Id, Dto);
                    return RedirectToAction("PCEDetail", "MOPCECase", new { PCEId = pceEvaluation.PCEId });   
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error updating production valuation for ID {Dto.Id}", Dto.Id);
                    ModelState.AddModelError("", "An error occurred while updating the production valuation.");
                }
            }
            return View(Dto);
            // return NoContent();
        }

        [HttpGet]
        public async Task<ActionResult> Delete(Guid Id)
        {
            var userId = base.GetCurrentUserId();
            try
            {
                var result = await _PCEEvaluationService.DeletePCEEvaluation(userId, Id);
                if (!result)
                {
                    return NotFound();
                }
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting production valuation for ID {Id}; User: {userId}", Id, userId);
                // var error = new { message = ex.Message };
                return Json(new { success = false, error = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> Evaluate(Guid Id)
        {
            var userId = base.GetCurrentUserId();
            try
            {
                await _PCEEvaluationService.EvaluatePCEEvaluation(userId, Id);
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error Sending production valuation of ID: {Id} to RM for review; User: {userId}", Id, userId);
                // var error = new { message = ex.Message };
                return Json(new { success = false, error = ex.Message });
                // return Json(new { success = false, message = ex.Message });
            }
        }
        
        [HttpGet]
        public async Task<IActionResult> Reestimate(Guid Id)
        {
            var userId = base.GetCurrentUserId();
            try
            {
                await _PCEEvaluationService.EvaluatePCEEvaluation(userId, Id);
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error Resending production valuation of ID: {Id} to RM for review; User: {userId}", Id, userId);
                // var error = new { message = ex.Message };
                return Json(new { success = false, error = ex.Message });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reject(PCERejectPostDto Dto)
        {
            var userId = base.GetCurrentUserId();
            try
            {
                await _PCEEvaluationService.RejectPCEEvaluation(userId, Dto);
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error Rejecting production valuation for user {userId}", userId);
                // var error = new { message = ex.Message };
                return Json(new { success = false, error = ex.Message });
            }
        }

        // [HttpGet]
        // public async Task<IActionResult> Rework(Guid Id)
        // {
        //    var userId = base.GetCurrentUserId();
        //     try
        //     {
        //         await _PCEEvaluationService.ReworkPCEEvaluation(userId, Id);
        //         return Json(new { success = true });
        //     }
        //     catch (Exception ex)
        //     {
        //         _logger.LogError(ex, "Error Returning production valuation of ID: {Id} back to MO for rework; User: {userId}", Id, userId);
        //         // var error = new { message = ex.Message };
        //         return Json(new { success = false, error = ex.Message });
        //     }
        // }

        [HttpGet]
        // [HttpPost]
        // [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reevaluate(Guid Id)
        {                       
            var userId = base.GetCurrentUserId();
            var pceEvaluation = await _PCEEvaluationService.GetPCEEvaluation(userId, Id);
            // var pceEvaluation = await _PCEEvaluationService.GetPCEEvaluationByPCEId(userId, PCEId);
            var pce = await _ProductionCapacityService.GetProduction(userId, Id);

            // var comments = await _ProductionCapacityService.GetComments(Id);
            // ViewData["Comments"] = comments;
            // var relatedFiles = await _uploadFileService.GetUploadFileByCollateralId(Id); 
            // ViewData["RelatedFiles"] = RelatedFiles;
            return View(_mapper.Map<PCEEvaluationUpdateDto>(pceEvaluation));          
            // string jsonData = JsonConvert.SerializeObject(pce, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
            // return Content(jsonData, "application/json");            
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemarkRelease(Guid Id, String Remark, Guid EvaluatorId)
        {
            var userId = base.GetCurrentUserId();
            try
            {
                var pceEvaluation = await _PCEEvaluationService.RemarkReleasePCEEvaluation(userId, Id, Remark, EvaluatorId);
                
                await _MailService.SendEmail(new MailPostDto
                {
                    SenderEmail = " getnetadane1@cbe.com.et",
                    SenderPassword = "Gechlove@1234",
                    RecipantEmail = "yohannessintayhu@cbe.com.et",
                    Subject = "Remark Release Update" ,
                    Body = "Dear! </br> Remark release Update for Applicant:-" + pceEvaluation.PCE.PropertyOwner + "</br></br> For further Detail please check Production Valuation System",
                });

                return RedirectToAction("RemarkPCECases", "MOPCECase");
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error releasing remark of production valuation for user {userId}", userId);
                // var error = new { message = ex.Message };
                return Json(new { success = false, error = ex.Message });
            }
        }  

        [HttpGet]
        public async Task<IActionResult> GetPCESummary(Guid PCECaseId)
        {
            var pceEvaluations = await _PCEEvaluationService.GetPCEEvaluationsByPCECaseId(base.GetCurrentUserId(), PCECaseId);
            string jsonData = JsonConvert.SerializeObject(pceEvaluations, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
            return Content(jsonData, "application/json");
        } 
        
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
    }
}