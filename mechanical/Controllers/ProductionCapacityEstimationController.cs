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

using mechanical.Data;
using mechanical.Models;
using mechanical.Models.Dto.ProductionCapacityDto;
using mechanical.Models.Dto.ProductionCapacityDto.FileUploadDto;
using mechanical.Models.Entities.ProductionCapacity;
using mechanical.Services.ProductionCapacityService;
// using mechanical.Services.UploadFileService;

namespace mechanical.Controllers
{
    public class ProductionCapacityEstimationController : BaseController
    {
        private readonly IProductionCapacityEstimationService _productionCapacityEstimationService;
        private readonly ILogger<ProductionCapacityEstimationController> _logger;
        private readonly IMapper _mapper;
        
        private readonly IFileUploadService _fileUploadService;

        // private readonly IUploadFileService _uploadFileService;

        // private readonly IPCECaseService _PCEcaseService;
        // private readonly IPCECaseScheduleService _PCEcaseScheduleService;
        // private readonly IPCECaseTerminateService _PCEcaseTermnateService;


        public ProductionCapacityEstimationController(IMapper mapper, IProductionCapacityEstimationService productionCapacityEstimationService, ILogger<ProductionCapacityEstimationController> logger, IFileUploadService fileUploadService) 
            // IPCECaseService PCEcaseService, IPCECaseTerminateService PCEcaseTermnateService, IPCECaseScheduleService PCEcaseScheduleService)
       
{
            _productionCapacityEstimationService = productionCapacityEstimationService;
            _mapper = mapper;
            _logger = logger;

            _fileUploadService = fileUploadService;
            // _uploadFileService = uploadFileService;

            // _PCEcaseService = PCEcaseService;
            // _PCEcaseScheduleService = PCEcaseScheduleService;
            // _PCEcaseTermnateService = PCEcaseTermnateService;
        }

        [HttpGet]
        public IActionResult Create()
        {
            ViewData["EmployeeId"] = HttpContext.Session.GetString("EmployeeId") ?? null;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Guid PCEcaseId, ProductionCapacityEstimationPostDto dto)

        {
            if (ModelState.IsValid)
            {
                try
                {

                    // Debugging: Check if files are received
                    if (dto.SupportingEvidences != null)
                    {
                        Console.WriteLine($"SupportingEvidences count: {dto.SupportingEvidences.Count}");
                        foreach (var file in dto.SupportingEvidences)
                        {
                            Console.WriteLine($"Received file: {file.File.FileName}, size: {file.File.Length}");
                        }
                    }
                    else
                    {
                        Console.WriteLine("No SupportingEvidences files received");
                    }

                    if (dto.ProductionProcessFlowDiagrams != null)
                    {
                        Console.WriteLine($"ProductionProcessFlowDiagrams count: {dto.ProductionProcessFlowDiagrams.Count}");
                        foreach (var file in dto.ProductionProcessFlowDiagrams)
                        {
                            Console.WriteLine($"Received file: {file.File.FileName}, size: {file.File.Length}");
                        }
                    }
                    else
                    {
                        Console.WriteLine("No ProductionProcessFlowDiagrams files received");
                    }


                    var productionCapacityEstimation = await _productionCapacityEstimationService.CreateProductionCapacityEstimation(base.GetCurrentUserId(), PCEcaseId, dto);
                    /////
                    // productionCapacityEstimation.PerShiftProduction = ProductionCapacityCalculationUtility.CalculatePerShiftProduction(productionCapacityEstimation.EffectiveProductionHourPerShift, productionCapacityEstimation.ProductionPerHour);
                    // productionCapacityEstimation.PerDayProduction = ProductionCapacityCalculationUtility.CalculatePerDayProduction(productionCapacityEstimation.ShiftsPerDay, productionCapacityEstimation.PerShiftProduction);
                    // productionCapacityEstimation.PerMonthProduction = ProductionCapacityCalculationUtility.CalculatePerMonthProduction(productionCapacityEstimation.WorkingDaysPerMonth, productionCapacityEstimation.PerDayProduction);
                    // productionCapacityEstimation.PerYearProduction = ProductionCapacityCalculationUtility.CalculatePerYearProduction(productionCapacityEstimation.PerMonthProduction);
                    //////
                    return RedirectToAction("NewEstimations");
                    return RedirectToAction("Detail", new { id = productionCapacityEstimation.Id });
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error creating Production Capacity Estimation for user {UserId}", base.GetCurrentUserId());
                    ModelState.AddModelError("", "An error occurred while creating the production capacity estimation.");
                }
            }
            return View(dto);
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
                _logger.LogError(ex, "Error fetching Production Capacity Estimation for editing, ID {Id}", id);
                return View("Error", new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, ProductionCapacityEstimationPostDto dto)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var productionCapacityEstimation = await _productionCapacityEstimationService.EditProductionCapacityEstimation(base.GetCurrentUserId(), id, dto);
                    
                    //////
                    // productionCapacityEstimation.PerShiftProduction = ProductionCapacityCalculationUtility.CalculatePerShiftProduction(productionCapacityEstimation.EffectiveProductionHourPerShift, productionCapacityEstimation.ProductionPerHour);
                    // productionCapacityEstimation.PerDayProduction = ProductionCapacityCalculationUtility.CalculatePerDayProduction(productionCapacityEstimation.ShiftsPerDay, productionCapacityEstimation.PerShiftProduction);
                    // productionCapacityEstimation.PerMonthProduction = ProductionCapacityCalculationUtility.CalculatePerMonthProduction(productionCapacityEstimation.WorkingDaysPerMonth, productionCapacityEstimation.PerDayProduction);
                    // productionCapacityEstimation.PerYearProduction = ProductionCapacityCalculationUtility.CalculatePerYearProduction(productionCapacityEstimation.PerMonthProduction);
                    ///////

                    return RedirectToAction("NewEstimations");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error editing Production Capacity Estimation for ID {Id}", id);
                    ModelState.AddModelError("", "An error occurred while editing the production capacity estimation.");
                }
            }
            return View(dto);
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
                // ViewData["productionCapacityEstimation"] = productionCapacityEstimation;
                // return View();

                // productionCapacityEstimation.SupportingEvidences = _mapper.Map<ICollection<FileReturnDto>>(productionCapacityEstimation.SupportingEvidences);
                // productionCapacityEstimation.ProductionProcessFlowDiagrams = _mapper.Map<ICollection<FileCreateDto>>(productionCapacityEstimation.ProductionProcessFlowDiagrams);
        
                return View(productionCapacityEstimation);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching Production Capacity Estimation details for ID {Id}", id);
                return View("Error", new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
            }
        }

        // public IActionResult NewEstimations()
        // {
        //     return View();
        // }

        [HttpGet]
        public async Task<IActionResult> NewEstimations()
        {
            try
            {
                var newEstimations = await _productionCapacityEstimationService.GetNewEstimations(base.GetCurrentUserId());
                return View(newEstimations);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching new estimations for user {UserId}", base.GetCurrentUserId());
                return View("Error", new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
            }
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
                return View("Error", new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
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
                return View("Error", new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
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
                return View("Error", new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
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
                _logger.LogError(ex, "Error fetching pending details for Production Capacity Estimation, ID {Id}", id);
                return View("Error", new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
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
                return View("Error", new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
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
                _logger.LogError(ex, "Error sending estimations for approval for user {UserId}", base.GetCurrentUserId());
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
                _logger.LogError(ex, "Error rejecting estimation for ID {Id}", id);
                var error = new { message = ex.Message };
                return BadRequest(error);
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
                return View("Error", new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
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
                return View("Error", new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
            }
        }
    
        public async Task<ActionResult> Index()
        {
            try
            {
                var estimations = await _productionCapacityEstimationService.GetAllProductionCapacityEstimations(base.GetCurrentUserId());
                return View(estimations);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching all production capacity estimations for user {UserId}", base.GetCurrentUserId());
                return View("Error", new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
            }
        }
        
        [HttpGet]
        public IActionResult MyPCECases()
        {
            return View();
        }
        
        // [HttpGet]
        // public async Task<IActionResult> MyPCECase(Guid Id)
        // {
        //     var PCEcase = await _PCEcaseService.GetPCECaseDetail(Id);
        //     var PCEcaseSchedule = await _PCEcaseScheduleService.GetPCECaseSchedules(Id);
        //     var PCEcaseTerminate = await _PCEcaseTermnateService.GetPCECaseTerminates(Id);
        //     ViewData["PCEcaseTerminate"] = PCEcaseTerminate;
        //     if (PCEcase == null) { return RedirectToAction("NewPCECases"); }
        //     ViewData["PCEcase"] = PCEcase;
        //     ViewData["PCECaseSchedule"] = PCEcaseSchedule;
        //     ViewData["Id"]=base.GetCurrentUserId();
        //     return View();
        // }

        // public IActionResult RemarkPCECases()
        // {
        //     return View();
        // }

        // [HttpGet]
        // public async Task<IActionResult> GetRemarkedCases()
        // {
        //     var myPCECase = await _mOCaseService.GetMoRemarkedCases(GetCurrentUserId());
        //     if (myPCECase == null) { return BadRequest("Unable to load case"); }
        //     string jsonData = JsonConvert.SerializeObject(myPCECase);
        //     return Content(jsonData, "application/json");
        // }
        // [HttpGet]
        // public async Task<IActionResult> GetMyPCECases()
        // {
        //     var myPCECase = await _mOCaseService.GetMMNewCases(GetCurrentUserId());
        //     if (myPCECase == null) { return BadRequest("Unable to load case"); }
        //     string jsonData = JsonConvert.SerializeObject(myPCECase);
        //     return Content(jsonData, "application/json");
        // }
        // [HttpGet]
        // public IActionResult MypendingCase()
        // {

        //     return View();
        // }
        // [HttpGet]
        // public async Task<IActionResult> GetMyPendingCases()
        // {
        //     var myPCECase = await _mOCaseService.GetMMPendingCases(GetCurrentUserId());
        //     if (myPCECase == null) { return BadRequest("Unable to load case"); }
        //     string jsonData = JsonConvert.SerializeObject(myPCECase);
        //     return Content(jsonData, "application/json");
        // }
        
        [HttpGet]
        public async Task<IActionResult> Evaluation(Guid id)
        {
            try
            {
                var productionCapacityEstimation = await _productionCapacityEstimationService.GetProductionCapacityEstimation(base.GetCurrentUserId(), id);
                
                // return RedirectToAction("Create", "PCE", new { Id = Id });
                
            
                string jsonData = JsonConvert.SerializeObject(productionCapacityEstimation);

                return Content(jsonData, "application/json");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching Production Capacity Estimation for editing, ID {Id}", id);
                return View("Error", new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
            }
        }


        // public async Task<IActionResult>ReEvaluation(Guid Id)
        // {
        //     var collateral = await _productionCapacityEstimationService.GetCollateral(base.GetCurrentUserId(), Id);
        //     if (collateral.Category == EnumHelper.GetEnumDisplayName(MechanicalCollateralCategory.MOV))
        //     {
        //         return RedirectToAction("GetReturnedEvaluatedPCE", "MotorVehicle", new { Id = Id });
        //     }
        
        //     string jsonData = JsonConvert.SerializeObject(collateral);
        //     return Content(jsonData, "application/json");
        // }
        // [HttpGet]
        // public async Task<IActionResult> MyPendDetail(Guid Id)
        // {
        //     var PCEcase = await _caseService.GetPCECaseDetail(Id);
        //     var caseSchedule = await _caseScheduleService.GetPCECaseSchedules(Id);
        //     if (PCEcase == null) { return RedirectToAction("NewCases"); }
        //     ViewData["case"] = PCEcase;
        //     ViewData["CaseSchedule"] = caseSchedule;
        //     ViewData["Id"] = base.GetCurrentUserId();
        //     return View();
        // }
 
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
                _logger.LogError(ex, "Error fetching production capacity estimation for deletion, ID {Id}", id);
                return View("Error", new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
            }
        }

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
                _logger.LogError(ex, "Error deleting production capacity estimation for ID {Id}", id);
                return View("Error", new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
            }
        }

        // [HttpPost]
        // public async Task<IActionResult> UploadSupportingEvidence(IFormFile supportingEvidence, Guid estimationId)
        // {
        //     try
        //     {
        //         if (await _productionCapacityEstimationService.UploadSupportingEvidence(base.GetCurrentUserId(), supportingEvidence, estimationId))
        //         {
        //             return Ok();
        //         }
        //         return BadRequest();
        //     }
        //     catch (Exception ex)
        //     {
        //         _logger.LogError(ex, "Error uploading supporting evidence for estimation ID {EstimationId}", estimationId);
        //         return View("Error", new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        //     }
        // }

        // [HttpPost]
        // public async Task<IActionResult> UploadProcessFlowDiagram(IFormFile processFlowDiagram, Guid estimationId)
        // {
        //     try
        //     {
        //         if (await _productionCapacityEstimationService.UploadProcessFlowDiagram(base.GetCurrentUserId(), processFlowDiagram, estimationId))
        //         {
        //             return Ok();
        //         }
        //         return BadRequest();
        //     }
        //     catch (Exception ex)
        //     {
        //         _logger.LogError(ex, "Error uploading process flow diagram for estimation ID {EstimationId}", estimationId);
        //         return View("Error", new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        //     }
        // }
    }
}
