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

                if (PCEEvaluation != null)
                {
                    return RedirectToAction("Detail", "PCEEvaluation", new { Id = PCEEvaluation.Id });
                }

                var pce = await _productionCapacityService.GetProduction(base.GetCurrentUserId(), PCEId);

                if (pce == null)
                {
                    return RedirectToAction("MyNewPCECases", "PCEEvaluation");
                }

                var pCECase = await _PCEEvaluationService.GetPCECase(GetCurrentUserId(), pce.PCECaseId);
                ViewData["PCECase"] = pCECase;
                // ViewData["PCECase"] = pce.PCECase;
                ViewData["PCE"] = pce;
                
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
                    
                    return RedirectToAction("Detail", "PCEEvaluation", new { Id = PCEEvaluation.Id });
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
                    return RedirectToAction("NewPCEEvaluations");
                }
                
                var pce = await _productionCapacityService.GetProduction(base.GetCurrentUserId(), PCEEvaluation.PCEId);
                var pCECase = await _PCEEvaluationService.GetPCECase(GetCurrentUserId(), pce.PCECaseId);

                ViewData["PCE"] = pce;
                ViewData["PCECase"] = pCECase;

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
                try
                {             
                    await _PCEEvaluationService.UpdatePCEEvaluation(base.GetCurrentUserId(), Id, Dto);
                    return RedirectToAction("Detail", "PCEEvaluation", new { Id = Id });
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error updating PCEEvaluation for ID {Id}", Id);
                    ModelState.AddModelError("", "An error occurred while updating the PCEEvaluation.");
                }
            }
            return View(Dto);
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

        [HttpGet]
        public async Task<IActionResult> Detail(Guid Id)
        {
            try
            {
                var PCEEvaluation = await _PCEEvaluationService.GetPCEEvaluation(base.GetCurrentUserId(), Id);
                if (PCEEvaluation == null)
                {
                    return RedirectToAction("NewPCEEvaluations");
                }            
                var pce = await _productionCapacityService.GetProduction(base.GetCurrentUserId(), PCEEvaluation.PCEId);
                var pCECase = await _PCEEvaluationService.GetPCECase(GetCurrentUserId(), pce.PCECaseId);

                ViewData["PCE"] = pce;
                ViewData["PCECase"] = pCECase;
                ViewData["CurrentStatus"] = pce.CurrentStatus;

                return View(PCEEvaluation);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching PCEEvaluation details for ID {Id}", Id);
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
        public async Task<IActionResult> Reevaluate(Guid Id)
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
        
        [HttpGet]
        public async Task<IActionResult> Rework(Guid Id)
        {
            try
            {
                await _PCEEvaluationService.ReworkPCEEvaluation(base.GetCurrentUserId(), Id);
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error Returning PCE Evaluation of ID: {Id} back to MO for rework; User: {UserId}", Id, base.GetCurrentUserId());
                // var error = new { message = ex.Message };
                return Json(new { success = false, error = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Reject(PCERejectPostDto Dto)
        {
            try
            {
                await _PCEEvaluationService.RejectPCEEvaluation(base.GetCurrentUserId(), Dto);
                return Json(new { success = true });
                return RedirectToAction("AllRejected");
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
        public async Task<IActionResult> PCECaseDetail(Guid Id)
        {
            var pCECase = await _PCEEvaluationService.GetPCECase(GetCurrentUserId(), Id);
            if (pCECase == null) 
            {
                return RedirectToAction("MyNewPCECases"); 
            }
            ViewData["PCECase"] = pCECase;
            ViewData["Title"] = "PCE Case Detail";
            return View();
        }
        
        [HttpGet]
        public IActionResult MyNewPCECases()
        {
            ViewData["Title"] = "New PCE Cases";
            ViewBag.Url = "/PCEEvaluation/GetMyNewPCECases";
            return View("PCECases");
        }

        [HttpGet]
        public IActionResult MyPendingPCECases()
        {

            ViewData["Title"] = "Pending PCE Cases";
            ViewBag.Url = "/PCEEvaluation/GetMyPendingPCECases";
            return View("PCECases");
        }

        [HttpGet]
        public IActionResult MyCompletedPCECases()
        {
            ViewData["Title"] = "Completed PCE Cases";
            ViewBag.Url = "/PCEEvaluation/GetMyCompletedPCECases";
            return View("PCECases");
        }

        [HttpGet]
        public IActionResult MyTotalPCECases()
        {
            ViewBag.Url = "/PCEEvaluation/GetMyTotalPCECases";
            ViewData["Title"] = "Total PCE Cases";
            return View("PCECases");
        }
     
        [HttpGet]
        public async Task<IActionResult> GetMyPCECase(Guid Id)
        {
            var myPCECase = await _PCEEvaluationService.GetPCECase(GetCurrentUserId(), Id);
            if (myPCECase == null) 
            {
                return BadRequest("Unable to load PCE Cases"); 
            }
            string jsonData = JsonConvert.SerializeObject(myPCECase);
            return Content(jsonData, "application/json");
        }

        [HttpGet]
        public async Task<IActionResult> GetMyNewPCECases()
        {
            var status = "New";
            var myPCECase = await _PCEEvaluationService.GetPCECasesWithStatus(GetCurrentUserId(), status);
            if (myPCECase == null) 
            {
                return BadRequest("Unable to load {status} PCE Cases");
            }
            string jsonData = JsonConvert.SerializeObject(myPCECase);
            return Content(jsonData, "application/json");
        }

        [HttpGet]
        public async Task<IActionResult> GetMyPendingPCECases()
        {
            var status = "Pending";
            var myPCECase = await _PCEEvaluationService.GetPCECasesWithStatus(GetCurrentUserId(), status);
            if (myPCECase == null) 
            {
                return BadRequest("Unable to load {status} PCE Cases");
            }
            string jsonData = JsonConvert.SerializeObject(myPCECase);
            return Content(jsonData, "application/json");
        }

        [HttpGet]
        public async Task<IActionResult> GetMyCompletedPCECases()
        {
            var status = "Completed";
            var myPCECase = await _PCEEvaluationService.GetPCECasesWithStatus(GetCurrentUserId(), status);
            if (myPCECase == null) 
            {
                return BadRequest("Unable to load {status} PCE Cases"); 
            }
            string jsonData = JsonConvert.SerializeObject(myPCECase);
            return Content(jsonData, "application/json");
        }

        [HttpGet]
        public async Task<IActionResult> GetMyTotalPCECases()
        {
            var myPCECase = await _PCEEvaluationService.GetTotalPCECases(GetCurrentUserId());
            if (myPCECase == null) 
            {
                return BadRequest("Unable to load PCE Cases"); 
            }
            string jsonData = JsonConvert.SerializeObject(myPCECase);
            return Content(jsonData, "application/json");
        }

        [HttpGet]
        public async Task<IActionResult> GetMyDashboardPCECaseCount()
        {
            var myPCECase = await _PCEEvaluationService.GetDashboardPCECaseCount(base.GetCurrentUserId());
            string jsonData = JsonConvert.SerializeObject(myPCECase);
            return Content(jsonData, "application/json");
        }

        ///// PCEs ///

        [HttpGet]
        public async Task<IActionResult> GetMyNewPCEs(Guid PCECaseId)
        {
            var status = "New";
            var productions = await _PCEEvaluationService.GetProductionCapacitiesWithStatus(PCECaseId, status);
            
            if (productions == null) 
            {
                return BadRequest("Unable to load {status} PCEs"); 
            }
            
            string jsonData = JsonConvert.SerializeObject(productions);

            return Content(jsonData, "application/json");
        }

        [HttpGet]
        public async Task<IActionResult> GetMyPendingPCEs(Guid PCECaseId)
        {
            var status = "Pending";
            var productions = await _PCEEvaluationService.GetProductionCapacitiesWithStatus(PCECaseId, status);
            
            if (productions == null) 
            {
                return BadRequest("Unable to load {status} PCEs"); 
            }
            
            string jsonData = JsonConvert.SerializeObject(productions);

            return Content(jsonData, "application/json");
        }

        [HttpGet]
        public async Task<IActionResult> GetMyReturnedPCEs(Guid PCECaseId)
        {
            var status = "Rejected";
            var productions = await _PCEEvaluationService.GetProductionCapacitiesWithStatus(PCECaseId, status);
            
            if (productions == null) 
            {
                return BadRequest("Unable to load {status} PCEs"); 
            }
            
            string jsonData = JsonConvert.SerializeObject(productions);

            return Content(jsonData, "application/json");
        }

        [HttpGet]
        public async Task<IActionResult> GetMyCompletedPCEs(Guid PCECaseId)
        {
            var status = "Completed";
            var productions = await _PCEEvaluationService.GetProductionCapacitiesWithStatus(PCECaseId, status);
            
            if (productions == null) 
            {
                return BadRequest("Unable to load {status} PCEs"); 
            }
            
            string jsonData = JsonConvert.SerializeObject(productions);

            return Content(jsonData, "application/json");
        }

        [HttpGet]
        public async Task<IActionResult> GetAllMyPCEs(Guid PCECaseId)
        {
            var productions = await _PCEEvaluationService.GetProductionCapacities(PCECaseId);
            
            if (productions == null) 
            {
                return BadRequest("Unable to load PCEs"); 
            }

            string jsonData = JsonConvert.SerializeObject(productions);

            return Content(jsonData, "application/json");
        }

        [HttpGet]
        public async Task<IActionResult> MyRejectedPCEs()
        {
            ViewBag.Url = "/PCEEvaluation/GetMyRejectedPCEs";
            ViewData["Title"] = "Returned PCE Cases";
            return View("PCECases");
        }

        public async Task<IActionResult> MyResubmittedPCEs()
        {
            ViewBag.Url = "/PCEEvaluation/GetMyResubmittedPCEs";
            ViewData["Title"] = "Resubmitted PCE Cases";
            return View("PCECases");
        }
        [HttpGet]
        public async Task<IActionResult> GetMyRejectedPCEs()
        {
            var status = "Rejected";
            var myPCECase = await _PCEEvaluationService.GetPCECasesWithStatus(GetCurrentUserId(), status);
            if (myPCECase == null) 
            {
                return BadRequest("Unable to load {status} PCE Cases"); 
            }
            string jsonData = JsonConvert.SerializeObject(myPCECase);
            return Content(jsonData, "application/json");
        }

        [HttpGet]
        public async Task<IActionResult> GetMyResubmittedPCEs()
        {
            var status = "Resubmitted";
            var myPCECase = await _PCEEvaluationService.GetPCECasesWithStatus(GetCurrentUserId(), status);
            if (myPCECase == null) 
            {
                return BadRequest("Unable to load {status} PCE Cases");
            }
            string jsonData = JsonConvert.SerializeObject(myPCECase);
            return Content(jsonData, "application/json");
        }
    }
}
