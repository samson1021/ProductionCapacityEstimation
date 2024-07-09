using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc.Rendering;

using System;
using System.Linq;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Collections.Generic;

using mechanical.Models;
using mechanical.Models.PCE.Enum.Collateral;
using mechanical.Models.PCE.Dto.CollateralEstimationFeeDto;
using mechanical.Services.PCE.CollateralEstimationFeeService;

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

            // Populate ViewBag with enum values for CollateralClass
            ViewBag.CollateralClassSelectList = Enum.GetValues(typeof(CollateralClass))
                                                      .Cast<CollateralClass>()
                                                      .Select(c => new SelectListItem
                                                      {
                                                          Value = c.ToString(),
                                                          Text = EnumHelper.GetEnumDisplayName(c)
                                                      })
                                                      .ToList();

            // Initialize empty lists for CollateralCategory and UnitOfMeasure
            ViewBag.CollateralCategorySelectList = new List<SelectListItem>();
            ViewBag.UnitOfMeasureSelectList = new List<SelectListItem>();
            // ViewBag.CollateralCategorySelectList = Enum.GetValues(typeof(CollateralCategory))
            //                                            .Cast<CollateralCategory>()
            //                                            .Select(c => new SelectListItem
            //                                            {
            //                                                Value = c.ToString(),
            //                                                Text = EnumHelper.GetEnumDisplayName(c)
            //                                            })
            //                                            .ToList();

            // ViewBag.UnitOfMeasureSelectList = Enum.GetValues(typeof(UnitOfMeasure))
            //                                       .Cast<UnitOfMeasure>()
            //                                       .Select(u => new SelectListItem
            //                                       {
            //                                           Value = u.ToString(),
            //                                           Text = EnumHelper.GetEnumDisplayName(u)
            //                                       })
            //                                       .ToList();


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
                    var pce = await _collateralEstimationFeeService.CreateCollateralEstimationFee(base.GetCurrentUserId(), dto);
                    return RedirectToAction("Detail", new { id = pce.Id });
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

        [HttpGet]
        public IActionResult LoadCategories(string collateralClass)
        {
            if (Enum.TryParse(collateralClass, out CollateralClass selectedClass))
            {
                var categoryList = CollateralMapping.ClassToCategoryMap[selectedClass]
                                                    .Select(cat => new SelectListItem
                                                    {
                                                        Value = cat.ToString(),
                                                        Text = EnumHelper.GetEnumDisplayName(cat) 
                                                    }).ToList();
                // var categoryList = Enum.GetValues(typeof(CollateralCategory))
                //                      .Cast<CollateralCategory>()
                //                      .Where(c => CollateralMapping.ClassToCategoryMap[selectedClass].Contains(c))
                //                      .Select(c => c.ToString())
                //                      .ToList();
                return Json(categoryList);
            }

            return BadRequest("Invalid collateral class.");
        }

        [HttpGet]
        public IActionResult LoadUnits(string collateralCategory)
        {
            if (Enum.TryParse(collateralCategory, out CollateralCategory selectedCategory))
            {
                var unitList =  CollateralMapping.CategoryToUnitMap[selectedCategory]
                                                .Select(unit => new SelectListItem
                                                {
                                                    Value = unit.ToString(),
                                                    Text = EnumHelper.GetEnumDisplayName(unit) 
                                                }).ToList();

                // var unitList = Enum.GetValues(typeof(UnitOfMeasure))
                //                 .Cast<UnitOfMeasure>()
                //                 .Where(u => CollateralMapping.CategoryToUnitMap[selectedCategory].Contains(u))
                //                 .Select(u => u.ToString())
                //                 .ToList();
                return Json(unitList);
            }

            return BadRequest("Invalid collateral category.");
        }
    }
}