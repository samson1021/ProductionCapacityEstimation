using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Diagnostics;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using mechanical.Data;
using mechanical.Models;
using mechanical.Models.PCE.Entities;
using mechanical.Models.Dto.UploadFileDto;
using mechanical.Models.PCE.Dto.PCEEvaluationDto;
using mechanical.Models.PCE.Enum.PCEEvaluation;
using mechanical.Services.UploadFileService;
using mechanical.Services.PCE.PCEEvaluationService;
using mechanical.Services.MailService;
using mechanical.Services.PCE.ProductionCapacityServices;

namespace mechanical.Controllers
{
    //    [Authorize(Roles = "Maker Manager,District Valuation Manager ,Maker Officer, Maker TeamLeader, Relation Manager,Checker Manager, Checker TeamLeader, Checker Officer")]

    public class PCEEvaluationController : BaseController
    {
        private readonly IPCEEvaluationService _PCEEvaluationService;
        private readonly ILogger<PCEEvaluationController> _logger;
        private readonly IMapper _mapper;
        private readonly IMailService _mailService;
        private readonly IUploadFileService _uploadFileService;
        private readonly IProductionCapacityServices _productionCapacityService;

        public PCEEvaluationController(IMapper mapper, IPCEEvaluationService PCEEvaluationService, IMailService mailService, ILogger<PCEEvaluationController> logger, IUploadFileService UploadFileService, IProductionCapacityServices ProductionCapacityService)
        {
            _PCEEvaluationService = PCEEvaluationService;
            _mapper = mapper;
            _logger = logger;
            _mailService = mailService;
            _uploadFileService = UploadFileService;
            _productionCapacityService = ProductionCapacityService;
        }

        [HttpGet]
        public async Task<IActionResult> Create(Guid PCEId)
        {
            try
            {
                var PCEEvaluation = await _PCEEvaluationService.GetPCEEvaluationsByPCEId(base.GetCurrentUserId(), PCEId);

                if (PCEEvaluation != null && PCEEvaluation.PCE.CurrentStatus != "Reestimate")
                {            
                    return RedirectToAction("PCEDetail", "PCEEvaluation", new { PCEId = PCEEvaluation.PCEId });
                }
                
                var pceDetail = await _PCEEvaluationService.GetPCEDetails(base.GetCurrentUserId(), PCEId);     

                if (pceDetail.ProductionCapacity == null)
                {
                    return RedirectToAction("MyPCEs");
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
                _logger.LogError(ex, "Error creating PCEEvaluation");
                return View("Error", new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PCEEvaluationPostDto Dto)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var PCEEvaluation = await _PCEEvaluationService.CreatePCEEvaluation(base.GetCurrentUserId(), Dto);

                    return RedirectToAction("PCEDetail", "PCEEvaluation", new { PCEId = PCEEvaluation.PCEId });

                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error creating PCEEvaluation for user {UserId}", base.GetCurrentUserId());
                    ModelState.AddModelError("", "An error occurred while creating the PCEEvaluation.");
                }
            }
            return View(Dto);
        }

        [HttpGet]
        public async Task<IActionResult> Update(Guid Id)
        {
            try
            {
                var PCEEvaluation = await _PCEEvaluationService.GetPCEEvaluation(base.GetCurrentUserId(), Id);

                if (PCEEvaluation == null)
                {
                    return RedirectToAction("MyPCEs");
                }

                var pce = await _productionCapacityService.GetProduction(base.GetCurrentUserId(), PCEEvaluation.PCEId);
                var pcecase = await _PCEEvaluationService.GetPCECase(base.GetCurrentUserId(), pce.PCECaseId);

                ViewData["PCE"] = pce;
                ViewData["PCECase"] = pcecase;

                return View(_mapper.Map<PCEEvaluationUpdateDto>(PCEEvaluation));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching PCEEvaluation for updating, ID {Id}", Id);
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
                    var PCEEvaluation = await _PCEEvaluationService.UpdatePCEEvaluation(base.GetCurrentUserId(), Id, Dto);
                    return RedirectToAction("PCEDetail", "PCEEvaluation", new { PCEId = PCEEvaluation.PCEId });
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error updating PCEEvaluation for ID {Dto.Id}", Dto.Id);
                    ModelState.AddModelError("", "An error occurred while updating the PCEEvaluation.");
                }
            }
            return View(Dto);
            // return NoContent();
        }

        [HttpGet]
        public async Task<ActionResult> Delete(Guid Id)
        {
            try
            {
                var result = await _PCEEvaluationService.DeletePCEEvaluation(base.GetCurrentUserId(), Id);
                if (!result)
                {
                    return NotFound();
                }
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting PCEEvaluation for ID {Id}; User: {UserId}", Id, base.GetCurrentUserId());
                // var error = new { message = ex.Message };
                return Json(new { success = false, error = ex.Message });
            }
        }

        // [HttpGet("{Id}")]
        [HttpGet]
        public async Task<IActionResult> PCEDetail(Guid PCEId)
        {
            try
            {
                var pceDetail = await _PCEEvaluationService.GetPCEDetails(base.GetCurrentUserId(), PCEId);
                if (pceDetail.ProductionCapacity == null)
                {
                    return RedirectToAction("MyPCEs");
                }
    
                ViewData["CurrentUser"] = pceDetail.CurrentUser;
                ViewData["Reestimation"] = pceDetail.Reestimation;
                ViewData["PCE"] = pceDetail.ProductionCapacity;
                ViewData["LatestEvaluation"] = pceDetail.PCEValuationHistory.LatestEvaluation;
                ViewData["PreviousEvaluations"] = pceDetail.PCEValuationHistory.PreviousEvaluations;
                ViewData["PCECase"] = pceDetail.PCECase;
                ViewData["ProductionFiles"] = pceDetail.RelatedFiles;
                ViewData["Remark"] = pceDetail.ProductionCapacity;

                return View(pceDetail.ProductionCapacity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching PCE details for ID {PCEId}", PCEId);
                return View("Error", new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
            }
        }

        [HttpGet]
        public async Task<IActionResult> Evaluate(Guid Id)
        {
            try
            {
                await _PCEEvaluationService.EvaluatePCEEvaluation(base.GetCurrentUserId(), Id);
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error Sending PCE Evaluation of ID: {Id} to RM for review; User: {UserId}", Id, base.GetCurrentUserId());
                // var error = new { message = ex.Message };
                return Json(new { success = false, error = ex.Message });
                // return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> Reestimate(Guid Id)
        {
            try
            {
                await _PCEEvaluationService.EvaluatePCEEvaluation(base.GetCurrentUserId(), Id);
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error Resending PCE Evaluation of ID: {Id} to RM for review; User: {UserId}", Id, base.GetCurrentUserId());
                // var error = new { message = ex.Message };
                return Json(new { success = false, error = ex.Message });
            }
        }

        // [HttpGet]
        // public async Task<IActionResult> Rework(Guid Id)
        // {
        //     try
        //     {
        //         await _PCEEvaluationService.ReworkPCEEvaluation(base.GetCurrentUserId(), Id);
        //         return Json(new { success = true });
        //     }
        //     catch (Exception ex)
        //     {
        //         _logger.LogError(ex, "Error Returning PCE Evaluation of ID: {Id} back to MO for rework; User: {UserId}", Id, base.GetCurrentUserId());
        //         // var error = new { message = ex.Message };
        //         return Json(new { success = false, error = ex.Message });
        //     }
        // }

        [HttpPost]
        public async Task<IActionResult> Reject(PCERejectPostDto Dto)
        {
            try
            {
                await _PCEEvaluationService.RejectPCEEvaluation(base.GetCurrentUserId(), Dto);
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error Rejecting PCE Evaluation for user {UserId}", base.GetCurrentUserId());
                // var error = new { message = ex.Message };
                return Json(new { success = false, error = ex.Message });
            }
        }

        //// PCE Cases /////////

        [HttpGet]
        public async Task<IActionResult> GetPCECase(Guid Id)
        {
            var pcecase = await _PCEEvaluationService.GetPCECase(base.GetCurrentUserId(), Id);
            if (pcecase == null)
            {
                return BadRequest("Unable to load PCE Cases");
            }
            string jsonData = JsonConvert.SerializeObject(pcecase);
            return Content(jsonData, "application/json");
        }

        [HttpGet]
        public async Task<IActionResult> PCECaseDetail(Guid Id, string Status)
        {

            var pcecase = await _PCEEvaluationService.GetPCECase(base.GetCurrentUserId(), Id);
            if (pcecase == null)
            {
                return RedirectToAction("MyPCECases");
            }
            ViewData["PCECaseId"] = Id;
            ViewData["PCECase"] = pcecase;
            ViewData["Title"] = Status + " PCE Case Details";
            ViewBag.Status = Status;

            return View();
        }

        [HttpGet]
        public IActionResult MyPCECases(string Status = "New")
        {
            ViewData["Title"] = Status + " PCE Cases";
            ViewBag.Url = "/PCEEvaluation/GetMyPCECases";
            ViewBag.Status = Status;
            return View("PCECases");
        }

        [HttpGet]
        public async Task<IActionResult> GetMyPCECases(string Status)
        {
            var pcecase = await _PCEEvaluationService.GetPCECases(base.GetCurrentUserId(), Status);
            if (pcecase == null)
            {
                return BadRequest("Unable to load {Status} PCE Cases");
            }
            string jsonData = JsonConvert.SerializeObject(pcecase);
            return Content(jsonData, "application/json");
        }

        // [HttpGet]
        // public async Task<IActionResult> GetMyDashboardPCECaseCount()
        // {
        //     var pcecase = await _PCEEvaluationService.GetDashboardPCECaseCount(base.GetCurrentUserId());
        //     string jsonData = JsonConvert.SerializeObject(pcecase);
        //     return Content(jsonData, "application/json");
        // }

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
            var Stage = string.Empty;
            var productions = await _PCEEvaluationService.GetPCEs(base.GetCurrentUserId(), PCECaseId, Stage, Status);

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
            Guid? PCECaseId = null;
            var Stage = string.Empty;

            var myPCEs = await _PCEEvaluationService.GetPCEs(base.GetCurrentUserId(), PCECaseId, Stage, Status);
            if (myPCEs == null)
            {
                return BadRequest("Unable to load {Status} PCEs");
            }
            string jsonData = JsonConvert.SerializeObject(myPCEs);
            return Content(jsonData, "application/json");
        }


        [HttpGet]
        public async Task<IActionResult> GetMyDashboardPCECount()
        {
            Guid? PCECaseId = null;
            var Stage = string.Empty;
            var myPCEs = await _PCEEvaluationService.GetDashboardPCECount(base.GetCurrentUserId(), PCECaseId, Stage);
            string jsonData = JsonConvert.SerializeObject(myPCEs);
            return Content(jsonData, "application/json");
        }

        [HttpGet]
        public async Task<IActionResult> GetMyLatestValuation(Guid PCEId)
        {
            var valuationHistory = await _PCEEvaluationService.GetValuationHistory(base.GetCurrentUserId(), PCEId);    
            string jsonData = JsonConvert.SerializeObject(valuationHistory.LatestEvaluation, new JsonSerializerSettings{ReferenceLoopHandling = ReferenceLoopHandling.Ignore});
            return Content(jsonData, "application/json");
        }

        [HttpGet]
        public async Task<IActionResult> GetMyPreviousValuations(Guid PCEId)
        {
            var valuationHistory = await _PCEEvaluationService.GetValuationHistory(base.GetCurrentUserId(), PCEId);    
            string jsonData = JsonConvert.SerializeObject(valuationHistory.PreviousEvaluations, new JsonSerializerSettings{ReferenceLoopHandling = ReferenceLoopHandling.Ignore});
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
        //     var myPCEs = await _PCEEvaluationService.GetReturnedPCEs(base.GetCurrentUserId());
        //     string jsonData = JsonConvert.SerializeObject(myPCEs);
        //     return Content(jsonData, "application/json");
        // }
    
    }
}