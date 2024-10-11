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

using mechanical.Data;
using mechanical.Models;
using mechanical.Models.Dto.MailDto;
using mechanical.Models.Dto.UploadFileDto;
using mechanical.Services.MailService;
using mechanical.Models.PCE.Entities;
using mechanical.Models.PCE.Dto.PCEEvaluationDto;
using mechanical.Services.PCE.PCEEvaluationService;
using mechanical.Services.PCE.ProductionCapacityService;

namespace mechanical.Controllers
{
    // [Authorize(Roles = "Maker Manager,District Valuation Manager ,Maker Officer, Maker TeamLeader, Relation Manager,Checker Manager, Checker TeamLeader, Checker Officer")]

    public class PCEEvaluationController : BaseController
    {
        private readonly IMapper _mapper;
        private readonly IMailService _MailService;
        private readonly ILogger<PCEEvaluationController> _logger;
        private readonly IPCEEvaluationService _PCEEvaluationService;
        private readonly IProductionCapacityService _ProductionCapacityService;

        public PCEEvaluationController(IMapper mapper, IMailService MailService, IPCEEvaluationService PCEEvaluationService, ILogger<PCEEvaluationController> logger, IProductionCapacityService ProductionCapacityService)
        {
            _mapper = mapper;
            _logger = logger;
            _MailService = MailService;   
            _PCEEvaluationService = PCEEvaluationService;
            _ProductionCapacityService = ProductionCapacityService;            
        }

        [HttpGet]
        public async Task<IActionResult> Create(Guid PCEId)
        {
            try
            {
                var userId = base.GetCurrentUserId();
                var pceEvaluation = await _PCEEvaluationService.GetValuationByPCEId(userId, PCEId);
                
                if (pceEvaluation != null && pceEvaluation.PCE.CurrentStatus != "Reestimate")
                {            
                    return RedirectToAction("Detail", "ProductionCapacity", new { Id = pceEvaluation.PCEId });
                }
                
                var pceDetail = await _ProductionCapacityService.GetPCEDetails(userId, PCEId);     

                if (pceDetail.ProductionCapacity == null)
                {
                    return RedirectToAction("PCECases", "PCECase");
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
                    var pceEvaluation = await _PCEEvaluationService.CreateValuation(userId, Dto);

                    return RedirectToAction("Detail", "ProductionCapacity", new { Id = pceEvaluation.PCEId });
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
                var pceEvaluation = await _PCEEvaluationService.GetValuation(userId, Id);

                if (pceEvaluation == null)
                {
                    return RedirectToAction("PCECases", "PCECase");
                }

                var pce = await _ProductionCapacityService.GetProduction(userId, pceEvaluation.PCEId);

                ViewData["PCE"] = pce;
                ViewData["PCECase"] = pce.PCECase;

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
                    var pceEvaluation = await _PCEEvaluationService.UpdateValuation(base.GetCurrentUserId(), Id, Dto);
                    return RedirectToAction("Detail", "ProductionCapacity", new { Id = pceEvaluation.PCEId });   
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
                var result = await _PCEEvaluationService.DeleteValuation(userId, Id);
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
        public async Task<IActionResult> Detail(Guid Id)
        {
            try{                    
                var pceValuation = await _PCEEvaluationService.GetValuation(base.GetCurrentUserId(), Id);

                if (pceValuation == null)
                {
                    return RedirectToAction("PCECases", "PCECase");
                }               

                return View(pceValuation);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching Production capacity valuation for ID: {Id}", Id);
                return View("Error", new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
            }
        }

        [HttpGet]
        public async Task<IActionResult> Evaluate(Guid Id)
        {
            var userId = base.GetCurrentUserId();
            try
            {
                await _PCEEvaluationService.CompleteValuation(userId, Id);
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reject(PCERejectPostDto Dto)
        {
            var userId = base.GetCurrentUserId();
            try
            {
                await _PCEEvaluationService.RejectValuation(userId, Dto);
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error Rejecting production valuation for user {userId}", userId);
                // var error = new { message = ex.Message };
                return Json(new { success = false, error = ex.Message });
            }
        }

        [HttpGet]
        // [HttpPost]
        // [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reevaluate(Guid Id)
        {                       
            var userId = base.GetCurrentUserId();
            var pceEvaluation = await _PCEEvaluationService.GetValuation(userId, Id);
            var pce = await _ProductionCapacityService.GetProduction(userId, Id);

            // var relatedFiles = await _uploadFileService.GetUploadFileByCollateralId(Id); 
            // ViewData["RelatedFiles"] = RelatedFiles;        
            // string jsonData = JsonConvert.SerializeObject(pce, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
            // return Content(jsonData, "application/json");  
            return View(_mapper.Map<PCEEvaluationUpdateDto>(pceEvaluation));            
        }

        [HttpPost]
        public async Task<IActionResult> HandleRemark(Guid PCEId, String RemarkType, CreateFileDto FileDto, Guid EvaluatorId)
        {            
            var userId = base.GetCurrentUserId();
            try
            {
                await _PCEEvaluationService.HandleRemark(userId, PCEId, RemarkType, FileDto, EvaluatorId);                
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error handling remark of production valuation for user {userId}", userId);
                return Json(new { success = false, error = ex.Message });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemarkRelease(Guid Id, String Remark, Guid EvaluatorId)
        {
            var userId = base.GetCurrentUserId();
            try
            {
                var pceEvaluation = await _PCEEvaluationService.ReleaseRemark(userId, Id, Remark, EvaluatorId);
                
                await _MailService.SendEmail(new MailPostDto
                {
                    SenderEmail = " getnetadane1@cbe.com.et",
                    SenderPassword = "Gechlove@1234",
                    RecipantEmail = "yohannessintayhu@cbe.com.et",
                    Subject = "Remark Release Update" ,
                    Body = "Dear! </br> Remark release Update for Applicant:-" + pceEvaluation.PCE.PropertyOwner + "</br></br> For further Detail please check Production Valuation System",
                });

                return RedirectToAction("RemarkPCECases", "PCECase");
                // return RedirectToAction("PCECases", "PCECase", new { Status = "Remark" });
                // return Json(new { success = true });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error releasing remark of production valuation for user {userId}", userId);
                // var error = new { message = ex.Message };
                return Json(new { success = false, error = ex.Message });
            }
        }  
        
        [HttpGet]
        public async Task<IActionResult> GetLatestValuation(Guid PCEId)
        {
            var valuationHistory = await _PCEEvaluationService.GetValuationHistory(base.GetCurrentUserId(), PCEId);    
            string jsonData = JsonConvert.SerializeObject(valuationHistory.LatestEvaluation, new JsonSerializerSettings{ReferenceLoopHandling = ReferenceLoopHandling.Ignore});
            return Content(jsonData, "application/json");
        }

        [HttpGet]
        public async Task<IActionResult> GetPreviousValuations(Guid PCEId)
        {
            var valuationHistory = await _PCEEvaluationService.GetValuationHistory(base.GetCurrentUserId(), PCEId);    
            string jsonData = JsonConvert.SerializeObject(valuationHistory.PreviousEvaluations, new JsonSerializerSettings{ReferenceLoopHandling = ReferenceLoopHandling.Ignore});
            return Content(jsonData, "application/json");
        }
    }
}