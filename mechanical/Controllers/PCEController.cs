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
using mechanical.Models.PCE.Entities;
using mechanical.Models.PCE.Dto.PCEDto;
using mechanical.Models.PCE.Dto.FileUploadDto;
using mechanical.Services.PCE.PCEService;
// using mechanical.Services.UploadFileService;

namespace mechanical.Controllers
{
    public class PCEController : BaseController
    {
        private readonly IPCEService _PCEService;
        private readonly ILogger<PCEController> _logger;
        private readonly IMapper _mapper;
        
        private readonly IFileUploadService _fileUploadService;

        // private readonly IUploadFileService _uploadFileService;

        // private readonly IPCECaseService _PCEcaseService;
        // private readonly IPCECaseScheduleService _PCEcaseScheduleService;
        // private readonly IPCECaseTerminateService _PCEcaseTermnateService;


        public PCEController(IMapper mapper, IPCEService PCEService, ILogger<PCEController> logger, IFileUploadService fileUploadService) 
            // IPCECaseService PCEcaseService, IPCECaseTerminateService PCEcaseTermnateService, IPCECaseScheduleService PCEcaseScheduleService)
       
{
            _PCEService = PCEService;
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
        public async Task<IActionResult> Create(Guid PCEcaseId, PCEPostDto dto)

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


                    var PCE = await _PCEService.CreatePCE(base.GetCurrentUserId(), PCEcaseId, dto);
                    /////
                    // PCE.PerShiftProduction = PCECalculationUtility.CalculatePerShiftProduction(PCE.EffectiveProductionHourPerShift, PCE.ProductionPerHour);
                    // PCE.PerDayProduction = PCECalculationUtility.CalculatePerDayProduction(PCE.ShiftsPerDay, PCE.PerShiftProduction);
                    // PCE.PerMonthProduction = PCECalculationUtility.CalculatePerMonthProduction(PCE.WorkingDaysPerMonth, PCE.PerDayProduction);
                    // PCE.PerYearProduction = PCECalculationUtility.CalculatePerYearProduction(PCE.PerMonthProduction);
                    //////
                    return RedirectToAction("NewPCEs");
                    return RedirectToAction("Detail", new { id = PCE.Id });
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error creating PCE for user {UserId}", base.GetCurrentUserId());
                    ModelState.AddModelError("", "An error occurred while creating the PCE.");
                }
            }
            return View(dto);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            try
            {
                var PCE = await _PCEService.GetPCE(base.GetCurrentUserId(), id);
                if (PCE == null)
                {
                    return RedirectToAction("NewPCEs");
                }
                return View(PCE);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching PCE for editing, ID {Id}", id);
                return View("Error", new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, PCEPostDto dto)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var PCE = await _PCEService.EditPCE(base.GetCurrentUserId(), id, dto);
                    
                    //////
                    // PCE.PerShiftProduction = PCECalculationUtility.CalculatePerShiftProduction(PCE.EffectiveProductionHourPerShift, PCE.ProductionPerHour);
                    // PCE.PerDayProduction = PCECalculationUtility.CalculatePerDayProduction(PCE.ShiftsPerDay, PCE.PerShiftProduction);
                    // PCE.PerMonthProduction = PCECalculationUtility.CalculatePerMonthProduction(PCE.WorkingDaysPerMonth, PCE.PerDayProduction);
                    // PCE.PerYearProduction = PCECalculationUtility.CalculatePerYearProduction(PCE.PerMonthProduction);
                    ///////

                    return RedirectToAction("NewPCEs");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error editing PCE for ID {Id}", id);
                    ModelState.AddModelError("", "An error occurred while editing the PCE.");
                }
            }
            return View(dto);
        }

        [HttpGet]
        public async Task<IActionResult> Detail(Guid id)
        {
            try
            {
                var PCE = await _PCEService.GetPCE(base.GetCurrentUserId(), id);
                if (PCE == null)
                {
                    return RedirectToAction("NewPCEs");
                }
                // ViewData["PCE"] = PCE;
                // return View();

                // PCE.SupportingEvidences = _mapper.Map<ICollection<FileReturnDto>>(PCE.SupportingEvidences);
                // PCE.ProductionProcessFlowDiagrams = _mapper.Map<ICollection<FileCreateDto>>(PCE.ProductionProcessFlowDiagrams);
        
                return View(PCE);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching PCE details for ID {Id}", id);
                return View("Error", new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
            }
        }

        // public IActionResult NewPCEs()
        // {
        //     return View();
        // }

        [HttpGet]
        public async Task<IActionResult> NewPCEs()
        {
            try
            {
                var newPCEs = await _PCEService.GetNewPCEs(base.GetCurrentUserId());
                return View(newPCEs);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching new PCEs for user {UserId}", base.GetCurrentUserId());
                return View("Error", new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
            }
        }

        [HttpGet]
        public IActionResult TerminatedPCEs()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetTerminatedPCEs()
        {
            try
            {
                var terminatedPCEs = await _PCEService.GetTerminatedPCEs(base.GetCurrentUserId());
                return Content(JsonConvert.SerializeObject(terminatedPCEs), "application/json");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching terminated PCEs for user {UserId}", base.GetCurrentUserId());
                return View("Error", new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
            }
        }

        [HttpGet]
        public IActionResult RejectedPCEs()
        {
            return View();
        }

        [HttpGet]
        public IActionResult MyPendingPCEs()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetNewPCEs()
        {
            try
            {
                var newPCEs = await _PCEService.GetNewPCEs(base.GetCurrentUserId());
                return Content(JsonConvert.SerializeObject(newPCEs), "application/json");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching new PCEs for user {UserId}", base.GetCurrentUserId());
                return View("Error", new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetRejectedPCEs()
        {
            try
            {
                var rejectedPCEs = await _PCEService.GetRejectedPCEs(base.GetCurrentUserId());
                return Content(JsonConvert.SerializeObject(rejectedPCEs), "application/json");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching rejected PCEs for user {UserId}", base.GetCurrentUserId());
                return View("Error", new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
            }
        }

        [HttpGet]
        public async Task<IActionResult> PendDetail(Guid id)
        {
            try
            {
                var PCE = await _PCEService.GetPCE(base.GetCurrentUserId(), id);
                if (PCE == null)
                {
                    return RedirectToAction("NewPCEs");
                }
                ViewData["PCE"] = PCE;
                ViewData["Id"] = base.GetCurrentUserId();
                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching pending details for PCE, ID {Id}", id);
                return View("Error", new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetPendingPCEs()
        {
            try
            {
                var pendingPCEs = await _PCEService.GetPendingPCEs(base.GetCurrentUserId());
                return Content(JsonConvert.SerializeObject(pendingPCEs), "application/json");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching pending PCEs for user {UserId}", base.GetCurrentUserId());
                return View("Error", new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
            }
        }

        [HttpPost]
        public async Task<IActionResult> SendForApproval(string selectedPCEIds, string CenterId)
        {
            try
            {
                await _PCEService.SendForApproval(selectedPCEIds, CenterId);
                var response = new { message = "PCEs sent for approval successfully" };
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending PCEs for approval for user {UserId}", base.GetCurrentUserId());
                var error = new { message = ex.Message };
                return BadRequest(error);
            }
        }

        [HttpPost]
        public async Task<IActionResult> RejectPCE(Guid id, string rejectionReason)
        {
            try
            {
                await _PCEService.RejectPCE(id, rejectionReason);
                return RedirectToAction("RejectedPCEs");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error rejecting PCE for ID {Id}", id);
                var error = new { message = ex.Message };
                return BadRequest(error);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetDashboardPCECount()
        {
            try
            {
                var PCECount = await _PCEService.GetDashboardPCECount(base.GetCurrentUserId());
                return Content(JsonConvert.SerializeObject(PCECount), "application/json");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching dashboard PCE count for user {UserId}", base.GetCurrentUserId());
                return View("Error", new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetMyDashboardPCECount()
        {
            try
            {
                var myPCECount = await _PCEService.GetMyDashboardPCECount(base.GetCurrentUserId());
                return Content(JsonConvert.SerializeObject(myPCECount), "application/json");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching my dashboard PCE count for user {UserId}", base.GetCurrentUserId());
                return View("Error", new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
            }
        }
    
        public async Task<ActionResult> Index()
        {
            try
            {
                var PCEs = await _PCEService.GetAllPCEs(base.GetCurrentUserId());
                return View(PCEs);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching all PCEs for user {UserId}", base.GetCurrentUserId());
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
                var PCE = await _PCEService.GetPCE(base.GetCurrentUserId(), id);
                
                // return RedirectToAction("Create", "PCE", new { Id = Id });
                
            
                string jsonData = JsonConvert.SerializeObject(PCE);

                return Content(jsonData, "application/json");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching PCE for editing, ID {Id}", id);
                return View("Error", new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
            }
        }


        // public async Task<IActionResult>ReEvaluation(Guid Id)
        // {
        //     var collateral = await _PCEService.GetCollateral(base.GetCurrentUserId(), Id);
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
                var PCE = await _PCEService.GetPCE(base.GetCurrentUserId(), id);
                if (PCE == null)
                {
                    return NotFound();
                }
                return View(PCE);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching PCE for deletion, ID {Id}", id);
                return View("Error", new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(Guid id, IFormCollection collection)
        {
            try
            {
                var result = await _PCEService.DeletePCE(base.GetCurrentUserId(), id);
                if (!result)
                {
                    return NotFound();
                }
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting PCE for ID {Id}", id);
                return View("Error", new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
            }
        }

        // [HttpPost]
        // public async Task<IActionResult> UploadSupportingEvidence(IFormFile supportingEvidence, Guid PCEId)
        // {
        //     try
        //     {
        //         if (await _PCEService.UploadSupportingEvidence(base.GetCurrentUserId(), supportingEvidence, PCEId))
        //         {
        //             return Ok();
        //         }
        //         return BadRequest();
        //     }
        //     catch (Exception ex)
        //     {
        //         _logger.LogError(ex, "Error uploading supporting evidence for PCE ID {PCEId}", PCEId);
        //         return View("Error", new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        //     }
        // }

        // [HttpPost]
        // public async Task<IActionResult> UploadProcessFlowDiagram(IFormFile processFlowDiagram, Guid PCEId)
        // {
        //     try
        //     {
        //         if (await _PCEService.UploadProcessFlowDiagram(base.GetCurrentUserId(), processFlowDiagram, PCEId))
        //         {
        //             return Ok();
        //         }
        //         return BadRequest();
        //     }
        //     catch (Exception ex)
        //     {
        //         _logger.LogError(ex, "Error uploading process flow diagram for PCE ID {PCEId}", PCEId);
        //         return View("Error", new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        //     }
        // }
    }
}
