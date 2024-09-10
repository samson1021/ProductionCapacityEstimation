using AutoMapper;
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
using mechanical.Models.PCE.Entities;
using mechanical.Models.PCE.Dto.PCEEvaluationDto;
using mechanical.Services.PCE.PCEEvaluationService;
using mechanical.Services.PCE.ProductionCapacityServices;
using Humanizer;

namespace mechanical.Controllers
{
    // [Authorize(Roles = "Maker Manager,District Valuation Manager ,Maker Officer, Maker TeamLeader, Relation Manager,Checker Manager, Checker TeamLeader, Checker Officer")]

    public class PCEEvaluationController : BaseController
    {
        private readonly IMapper _mapper;
        private readonly ILogger<PCEEvaluationController> _logger;
        private readonly IPCEEvaluationService _PCEEvaluationService;
        private readonly IProductionCapacityServices _ProductionCapacityService;

        public PCEEvaluationController(IMapper mapper, IPCEEvaluationService PCEEvaluationService, ILogger<PCEEvaluationController> logger, IProductionCapacityServices ProductionCapacityService)
        {
            _mapper = mapper;
            _logger = logger;
            _PCEEvaluationService = PCEEvaluationService;
            _ProductionCapacityService = ProductionCapacityService;            
        }

        [HttpGet]
        public async Task<IActionResult> Create(Guid PCEId)
        {
            try
            {
                var PCEEvaluation = await _PCEEvaluationService.GetPCEEvaluationByPCEId(base.GetCurrentUserId(), PCEId);

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

                var pce = await _ProductionCapacityService.GetProduction(base.GetCurrentUserId(), PCEEvaluation.PCEId);
                var pceCase = await _PCEEvaluationService.GetPCECase(base.GetCurrentUserId(), pce.PCECaseId);

                ViewData["PCE"] = pce;
                ViewData["PCECase"] = pceCase;

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


        [HttpGet]
        public async Task<IActionResult> GetPCESummary(Guid PCECaseId)
        {
            var pceEvaluations = await _PCEEvaluationService.GetPCEEvaluationsByPCECaseId(base.GetCurrentUserId(), PCECaseId);
            string jsonData = JsonConvert.SerializeObject(pceEvaluations, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
            return Content(jsonData, "application/json");
        }
    }
}