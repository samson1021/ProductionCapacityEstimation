using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using AutoMapper;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;
using Microsoft.CodeAnalysis.Operations;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using mechanical.Data;
using mechanical.Services.ProductionCapacityService;
using mechanical.Models.Dto.ProductionCapacityDto;
using mechanical.Models.Entities;
using mechanical.Models.Entities.ProductionCapacity;
using mechanical.Services.MailService;
using mechanical.Models.Dto.MailDto;

namespace mechanical.Controllers
{
    public class ProductionCapacityEstimationController : BaseController
    {
        private readonly IProductionCapacityEstimationService _productionCapacityEstimationService;
        private readonly CbeContext _cbeContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMailService _mailService;
        private readonly IMapper _mapper;
        private readonly ILogger<ProductionCapacityEstimationController> _logger;

        public ProductionCapacityEstimationController(IMapper mapper, IProductionCapacityEstimationService productionCapacityEstimationService, CbeContext cbeContext, IHttpContextAccessor httpContextAccessor, IMailService mailService, ILogger<ProductionCapacityEstimationController> logger)
        {
            _productionCapacityEstimationService = productionCapacityEstimationService;
            _cbeContext = cbeContext;
            _httpContextAccessor = httpContextAccessor;
            _mailService = mailService;
            _mapper = mapper;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Create()
        {
            ViewData["EmployeeId"] = HttpContext.Session.GetString("EmployeeId") ?? null;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductionCapacityEstimationDto productionCapacityEstimationDto)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var productionCapacityEstimation = await _productionCapacityEstimationService.CreateProductionCapacityEstimation(base.GetCurrentUserId(), productionCapacityEstimationDto);
                    return RedirectToAction("Detail", new { id = productionCapacityEstimation.Id });
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error creating production capacity estimation for user {UserId}", base.GetCurrentUserId());
                    ModelState.AddModelError(string.Empty, "An error occurred while creating the production capacity estimation.");
                }
            }
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            try
            {
                var productionCapacityEstimation = await _productionCapacityEstimationService.GetProductionCapacityEstimation(base.GetCurrentUserId(), id);
                if (productionCapacityEstimation == null)
                {
                    return RedirectToAction("NewEstimations");
                }
                return View(productionCapacityEstimation);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching production capacity estimation for editing with ID {Id}", id);
                return View("Error", new { message = "An error occurred while fetching the production capacity estimation for editing." });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, ProductionCapacityEstimationDto productionCapacityEstimationDto)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var productionCapacityEstimation = await _productionCapacityEstimationService.EditProductionCapacityEstimation(base.GetCurrentUserId(), id, productionCapacityEstimationDto);
                    return RedirectToAction("NewEstimations");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error editing production capacity estimation with ID {Id} for user {UserId}", id, base.GetCurrentUserId());
                    ModelState.AddModelError(string.Empty, "An error occurred while editing the production capacity estimation.");
                }
            }
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Detail(Guid id)
        {
            try
            {
                var productionCapacityEstimation = await _productionCapacityEstimationService.GetProductionCapacityEstimation(base.GetCurrentUserId(), id);
                if (productionCapacityEstimation == null)
                {
                    return RedirectToAction("NewEstimations");
                }
                ViewData["productionCapacityEstimation"] = productionCapacityEstimation;
                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching production capacity estimation details with ID {Id}", id);
                return View("Error", new { message = "An error occurred while fetching the production capacity estimation details." });
            }
        }

        [HttpGet]
        public IActionResult NewEstimations()
        {
            return View();
        }

        [HttpGet]
        public IActionResult TerminatedEstimations()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetTerminatedEstimations()
        {
            try
            {
                var terminatedEstimations = await _productionCapacityEstimationService.GetTerminatedEstimations(base.GetCurrentUserId());
                return Content(JsonConvert.SerializeObject(terminatedEstimations), "application/json");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching terminated estimations for user {UserId}", base.GetCurrentUserId());
                return View("Error", new { message = "An error occurred while fetching the terminated estimations." });
            }
        }

        [HttpGet]
        public IActionResult RejectedEstimations()
        {
            return View();
        }

        [HttpGet]
        public IActionResult MyPendingEstimations()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetNewEstimations()
        {
            try
            {
                var newEstimations = await _productionCapacityEstimationService.GetNewEstimations(base.GetCurrentUserId());
                return Content(JsonConvert.SerializeObject(newEstimations), "application/json");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching new estimations for user {UserId}", base.GetCurrentUserId());
                return View("Error", new { message = "An error occurred while fetching the new estimations." });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetRejectedEstimations()
        {
            try
            {
                var rejectedEstimations = await _productionCapacityEstimationService.GetRejectedEstimations(base.GetCurrentUserId());
                return Content(JsonConvert.SerializeObject(rejectedEstimations), "application/json");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching rejected estimations for user {UserId}", base.GetCurrentUserId());
                return View("Error", new { message = "An error occurred while fetching the rejected estimations." });
            }
        }

        [HttpGet]
        public async Task<IActionResult> PendDetail(Guid id)
        {
            try
            {
                var productionCapacityEstimation = await _productionCapacityEstimationService.GetProductionCapacityEstimation(base.GetCurrentUserId(), id);
                if (productionCapacityEstimation == null)
                {
                    return RedirectToAction("NewEstimations");
                }
                ViewData["productionCapacityEstimation"] = productionCapacityEstimation;
                ViewData["Id"] = base.GetCurrentUserId();
                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching pending detail for production capacity estimation with ID {Id}", id);
                return View("Error", new { message = "An error occurred while fetching the pending detail for production capacity estimation." });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetPendingEstimations()
        {
            try
            {
                var pendingEstimations = await _productionCapacityEstimationService.GetPendingEstimations(base.GetCurrentUserId());
                return Content(JsonConvert.SerializeObject(pendingEstimations), "application/json");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching pending estimations for user {UserId}", base.GetCurrentUserId());
                return View("Error", new { message = "An error occurred while fetching the pending estimations." });
            }
        }

        [HttpPost]
        public async Task<IActionResult> SendForApproval(string selectedEstimationIds, string CenterId)
        {
            try
            {
                await _productionCapacityEstimationService.SendForApproval(selectedEstimationIds, CenterId);
                var response = new { message = "Estimations sent for approval successfully" };
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending estimations for approval with IDs {EstimationIds} and Center ID {CenterId}", selectedEstimationIds, CenterId);
                var error = new { message = ex.Message };
                return BadRequest(error);
            }
        }

        [HttpPost]
        public async Task<IActionResult> RejectEstimation(Guid id, string rejectionReason)
        {
            try
            {
                await _productionCapacityEstimationService.RejectEstimation(id, rejectionReason);
                return RedirectToAction("RejectedEstimations");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error rejecting estimation with ID {Id} and reason {Reason}", id, rejectionReason);
                var error = new { message = ex.Message };
                return BadRequest(error);
            }
        }

        [HttpPost]
        public async Task<IActionResult> UploadSupportingEvidence(IFormFile supportingEvidence, Guid estimationId)
        {
            try
            {
                if (await _productionCapacityEstimationService.UploadSupportingEvidence(base.GetCurrentUserId(), supportingEvidence, estimationId))
                {
                    return Ok();
                }
                return BadRequest();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading supporting evidence for estimation ID {EstimationId}", estimationId);
                return BadRequest();
            }
        }

        [HttpPost]
        public async Task<IActionResult> UploadProcessFlowDiagram(IFormFile processFlowDiagram, Guid estimationId)
        {
            try
            {
                if (await _productionCapacityEstimationService.UploadProcessFlowDiagram(base.GetCurrentUserId(), processFlowDiagram, estimationId))
                {
                    return Ok();
                }
                return BadRequest();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading process flow diagram for estimation ID {EstimationId}", estimationId);
                return BadRequest();
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetDashboardEstimationCount()
        {
            try
            {
                var estimationCount = await _productionCapacityEstimationService.GetDashboardEstimationCount(base.GetCurrentUserId());
                return Content(JsonConvert.SerializeObject(estimationCount), "application/json");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching dashboard estimation count for user {UserId}", base.GetCurrentUserId());
                return View("Error", new { message = "An error occurred while fetching the dashboard estimation count." });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetMyDashboardEstimationCount()
        {
            try
            {
                var myEstimationCount = await _productionCapacityEstimationService.GetMyDashboardEstimationCount(base.GetCurrentUserId());
                return Content(JsonConvert.SerializeObject(myEstimationCount), "application/json");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching my dashboard estimation count for user {UserId}", base.GetCurrentUserId());
                return View("Error", new { message = "An error occurred while fetching my dashboard estimation count." });
            }
        }

        // GET: ProductionCapacityEstimation
        public async Task<ActionResult> Index()
        {
            try
            {
                var estimations = await _productionCapacityEstimationService.GetAllProductionCapacityEstimations();
                return View(estimations);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching all production capacity estimations for user {UserId}", base.GetCurrentUserId());
                return View("Error", new { message = "An error occurred while fetching all production capacity estimations." });
            }
        }

        // GET: ProductionCapacityEstimation/Delete/5
        public async Task<ActionResult> Delete(Guid id)
        {
            try
            {
                var estimation = await _productionCapacityEstimationService.GetProductionCapacityEstimation(base.GetCurrentUserId(), id);
                if (estimation == null)
                {
                    return NotFound();
                }
                return View(estimation);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching production capacity estimation for deletion with ID {Id}", id);
                return View("Error", new { message = "An error occurred while fetching the production capacity estimation for deletion." });
            }
        }

        // POST: ProductionCapacityEstimation/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(Guid id, IFormCollection collection)
        {
            try
            {
                var result = await _productionCapacityEstimationService.DeleteProductionCapacityEstimation(base.GetCurrentUserId(), id);
                if (!result)
                {
                    return NotFound();
                }
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting production capacity estimation with ID {Id}", id);
                return View("Error", new { message = "An error occurred while deleting the production capacity estimation." });
            }
        }
    }
}
