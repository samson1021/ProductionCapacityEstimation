using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using System;
using System.Threading.Tasks;
using System.Collections.Generic;

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
                var collateralEstimationFees = await _collateralEstimationFeeService.GetAllCollateralEstimationFees(base.GetCurrentUserId());
                return View(collateralEstimationFees);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching collateral estimation fees for user {UserId}", base.GetCurrentUserId());
                return View("Error", new { message = "An error occurred while fetching collateral estimation fees." });
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
                _logger.LogError(ex, "Error fetching details for collateral estimation fee with ID {Id}", id);
                return View("Error", new { message = "An error occurred while fetching the collateral estimation fee details." });
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
                    await _collateralEstimationFeeService.CreateCollateralEstimationFee(base.GetCurrentUserId(), dto);
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error creating collateral estimation fee for user {UserId}", base.GetCurrentUserId());
                    ModelState.AddModelError(string.Empty, "An error occurred while creating the collateral estimation fee.");
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
                _logger.LogError(ex, "Error fetching collateral estimation fee for editing with ID {Id}", id);
                return View("Error", new { message = "An error occurred while fetching the collateral estimation fee for editing." });
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
                    _logger.LogError(ex, "Error editing collateral estimation fee with ID {Id} for user {UserId}", id, base.GetCurrentUserId());
                    ModelState.AddModelError(string.Empty, "An error occurred while editing the collateral estimation fee.");
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
                _logger.LogError(ex, "Error fetching collateral estimation fee for deletion with ID {Id}", id);
                return View("Error", new { message = "An error occurred while fetching the collateral estimation fee for deletion." });
            }
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            try
            {
                await _collateralEstimationFeeService.DeleteCollateralEstimationFee(id);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting collateral estimation fee with ID {Id}", id);
                return View("Error", new { message = "An error occurred while deleting the collateral estimation fee." });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ValidateFees(Guid caseId)
        {
            try
            {
                var result = await _collateralEstimationFeeService.ValidateFees(caseId);
                if (result)
                {
                    return RedirectToAction(nameof(Index));
                }
                return BadRequest("Validation failed");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validating fees for case ID {CaseId}", caseId);
                return View("Error", new { message = "An error occurred while validating the fees." });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CommitFees(Guid caseId)
        {
            try
            {
                var result = await _collateralEstimationFeeService.CommitFees(caseId);
                if (result)
                {
                    return RedirectToAction(nameof(Index));
                }
                return BadRequest("Commitment failed");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error committing fees for case ID {CaseId}", caseId);
                return View("Error", new { message = "An error occurred while committing the fees." });
            }
        }
    }
}
