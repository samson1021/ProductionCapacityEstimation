using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;

using mechanical.Models;
using mechanical.Models.Dto.ProductionCapacityDto;
using mechanical.Services.ProductionCapacityService;

namespace mechanical.Controllers
{
    public class CollateralEstimationFeeController : BaseController
    {
        private readonly ICollateralEstimationFeeService _collateralEstimationFeeService;
        private readonly IMapper _mapper;
        private readonly ILogger<CollateralEstimationFeeController> _logger;

        public CollateralEstimationFeeController(ICollateralEstimationFeeService estimationFeeService, IMapper mapper, ILogger<CollateralEstimationFeeController> logger)
        {
            _collateralEstimationFeeService = estimationFeeService;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            try
            {   
                var PCECaseId = Guid.Parse("E1BBBE4A-F804-439A-A8E6-539232CCC6F0");
                var collateralEstimationFees = await _collateralEstimationFeeService.GetAllCollateralEstimationFees(base.GetCurrentUserId(), PCECaseId);
                return View(collateralEstimationFees);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching Collateral Estimation Fees for user {UserId}", base.GetCurrentUserId());
                return View("Error", new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
            }
        }

        public async Task<IActionResult> Detail(Guid id)
        {
            try
            {
                var collateralEstimationFee = await _collateralEstimationFeeService.GetCollateralEstimationFee(base.GetCurrentUserId(), id);
                if (collateralEstimationFee == null)
                {
                    return NotFound();
                }
                return View(collateralEstimationFee);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching Collateral Estimation Fee details for ID {Id}", id);
                return View("Error", new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
            }
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CollateralEstimationFeeDto dto)
        {            
            if (ModelState.IsValid)
            {
                try
                {
                    var productionCapacityEstimation = await _collateralEstimationFeeService.CreateCollateralEstimationFee(base.GetCurrentUserId(), dto);
                    return RedirectToAction("Detail", new { id = productionCapacityEstimation.Id });
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error creating Collateral Estimation Fee for user {UserId}", base.GetCurrentUserId());
                    ModelState.AddModelError("", "An error occurred while creating the estimation fee.");
                }
            }
            
            return View(dto);
        }

        public async Task<IActionResult> Edit(Guid id)
        {
            try
            {
                var collateralEstimationFee = await _collateralEstimationFeeService.GetCollateralEstimationFee(base.GetCurrentUserId(), id);
                if (collateralEstimationFee == null)
                {
                    return NotFound();
                }
                return View(collateralEstimationFee);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching Collateral Estimation Fee details for editing, ID {Id}", id);
                return View("Error", new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, CollateralEstimationFeeDto dto)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _collateralEstimationFeeService.EditCollateralEstimationFee(base.GetCurrentUserId(), id, dto);
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error editing Collateral Estimation Fee for ID {Id}", id);
                    ModelState.AddModelError("", "An error occurred while editing the estimation fee.");
                }
            }
            return View(dto);
        }

        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                var collateralEstimationFee = await _collateralEstimationFeeService.GetCollateralEstimationFee(base.GetCurrentUserId(), id);
                if (collateralEstimationFee == null)
                {
                    return NotFound();
                }
                return View(collateralEstimationFee);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching Collateral Estimation Fee details for deletion, ID {Id}", id);
                return View("Error", new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
            }
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            try
            {
                await _collateralEstimationFeeService.DeleteCollateralEstimationFee(base.GetCurrentUserId(), id);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting Collateral Estimation Fee for ID {Id}", id);
                return View("Error", new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ValidateFees(Guid caseId)
        {
            try
            {
                var result = await _collateralEstimationFeeService.ValidateFees(base.GetCurrentUserId(), caseId);
                if (result)
                {
                    return RedirectToAction(nameof(Index));
                }
                return BadRequest("Validation failed");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validating fees for Case ID {CaseId}", caseId);
                return View("Error", new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CommitFees(Guid caseId)
        {
            try
            {
                var result = await _collateralEstimationFeeService.CommitFees(base.GetCurrentUserId(), caseId);
                if (result)
                {
                    return RedirectToAction(nameof(Index));
                }
                return BadRequest("Commitment failed");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error committing fees for Case ID {CaseId}", caseId);
                return View("Error", new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
            }
        }
    }
}
