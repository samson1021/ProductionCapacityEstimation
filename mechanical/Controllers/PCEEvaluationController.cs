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
using mechanical.Models.PCE.Dto.FileUploadDto;
using mechanical.Models.PCE.Dto.PCEEvaluationDto;
using mechanical.Services.PCE.FileUploadService;
using mechanical.Services.PCE.PCEEvaluationService;
using mechanical.Services.MailService;
    
namespace mechanical.Controllers
{
    public class PCEEvaluationController : BaseController
    {
        private readonly IPCEEvaluationService _PCEEvaluationService;
        private readonly ILogger<PCEEvaluationController> _logger;
        private readonly IMapper _mapper;
        private readonly IMailService _mailService;
        private readonly IFileUploadService _fileUploadService;

        // private readonly IPCEEvaluationPCECaseService _PCEEvaluationServicePCECaseService;
        // private readonly IPCEEvaluationPCECaseScheduleService _PCEEvaluationServicePCECaseScheduleService;
        // private readonly IPCEEvaluationPCECaseTerminateService _PCEEvaluationServicePCECaseTermnateService;


        public PCEEvaluationController(IMapper mapper, IPCEEvaluationService PCEEvaluationService, IMailService mailService, ILogger<PCEEvaluationController> logger, IFileUploadService fileUploadService) 
            // IPCEEvaluationPCECaseService PCEEvaluationPCECaseService, IPCEEvaluationPCECaseTerminateService PCEEvaluationPCECaseTermnateService, IPCEEvaluationPCECaseScheduleService PCEEvaluationPCECaseScheduleService)
       
{
            _PCEEvaluationService = PCEEvaluationService;
            _mapper = mapper;
            _logger = logger;
            _mailService = mailService;
            _fileUploadService = fileUploadService;

            // _PCEEvaluationServicePCECaseService = PCEEvaluationPCECaseService;
            // _PCEEvaluationServicePCECaseScheduleService = PCEEvaluationPCECaseScheduleService;
            // _PCEEvaluationServicePCECaseTermnateService = PCEEvaluationPCECaseTermnateService;
        }


        [HttpGet]
        public async Task<IActionResult> Create(Guid Id)
        {
            try
            {
                // var PCE = await _PCEEvaluationService.GetPCE(base.GetCurrentUserId(), Id);
                // if (PCE == null)
                // {
                //     return RedirectToAction("MyPCECase", "PCECase");
                // }
                // ViewData["PCE"] = PCE;
                // ViewData["EmployeeId"] = HttpContext.Session.GetString("EmployeeId") ?? null;
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
        public async Task<IActionResult> Create(PCEEvaluationPostDto dto)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var PCEEvaluation = await _PCEEvaluationService.CreatePCEEvaluation(base.GetCurrentUserId(), dto);
                    
                    return RedirectToAction("Detail", "PCEEvaluation", new { Id = PCEEvaluation.Id });
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error creating PCEEvaluation for user {UserId}", base.GetCurrentUserId());
                    ModelState.AddModelError("", "An error occurred while creating the PCEEvaluation.");
                }
            }
            return View(dto);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {    
            try
            {
                var PCEEvaluation = await _PCEEvaluationService.GetPCEEvaluation(base.GetCurrentUserId(), id);
        
                if (PCEEvaluation == null)
                {
                    return RedirectToAction("NewPCEEvaluations");
                }
        
                ViewData["PCEPost"] = PCEEvaluation;
                ViewData["PCEReturn"] = _mapper.Map<PCEEvaluationPostDto>(PCEEvaluation);
                
                return View(PCEEvaluation);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching PCEEvaluation for editing, ID {Id}", id);
                return View("Error", new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, PCEEvaluationPostDto dto)
        {
            if (ModelState.IsValid)
            {
                try
                {
             
                    await _PCEEvaluationService.EditPCEEvaluation(base.GetCurrentUserId(), id, dto);
                    return RedirectToAction("Detail", "PCEEvaluation", new { Id = id });
                    // return RedirectToAction("GetPCEEvaluation", "PCEEvaluation", new { Id = id });
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error editing PCEEvaluation for ID {Id}", id);
                    ModelState.AddModelError("", "An error occurred while editing the PCEEvaluation.");
                }
            }
            return View(dto);
        }

        [HttpGet]
        public async Task<IActionResult> Detail(Guid id)
        {
            try
            {
                var PCEEvaluation = await _PCEEvaluationService.GetPCEEvaluation(base.GetCurrentUserId(), id);
                if (PCEEvaluation == null)
                {
                    return RedirectToAction("NewPCEEvaluations");
                }
            
                return View(PCEEvaluation);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching PCEEvaluation details for ID {Id}", id);
                return View("Error", new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
            }
        }

        [HttpGet]
        public async Task<IActionResult> NewPCEEvaluations()
        {
            try
            {
                var newPCEEvaluations = await _PCEEvaluationService.GetNewPCEEvaluations(base.GetCurrentUserId());
                return View(newPCEEvaluations);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching new PCEEvaluations for user {UserId}", base.GetCurrentUserId());
                return View("Error", new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
            }
        }

        [HttpGet]
        public IActionResult TerminatedPCEEvaluations()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetTerminatedPCEEvaluations()
        {
            try
            {
                var terminatedPCEEvaluations = await _PCEEvaluationService.GetTerminatedPCEEvaluations(base.GetCurrentUserId());
                return Content(JsonConvert.SerializeObject(terminatedPCEEvaluations), "application/json");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching terminated PCEEvaluations for user {UserId}", base.GetCurrentUserId());
                return View("Error", new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
            }
        }

        [HttpGet]
        public IActionResult Rejected()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Pending()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetNew()
        {
            try
            {
                var newPCEEvaluations = await _PCEEvaluationService.GetNewPCEEvaluations(base.GetCurrentUserId());
                return Content(JsonConvert.SerializeObject(newPCEEvaluations), "application/json");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching new PCEEvaluations for user {UserId}", base.GetCurrentUserId());
                return View("Error", new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetRejected()
        {
            try
            {
                var rejectedPCEEvaluations = await _PCEEvaluationService.GetRejectedPCEEvaluations(base.GetCurrentUserId());
                return Content(JsonConvert.SerializeObject(rejectedPCEEvaluations), "application/json");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching rejected PCEEvaluations for user {UserId}", base.GetCurrentUserId());
                return View("Error", new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
            }
        }

        [HttpGet]
        public async Task<IActionResult> PendingDetail(Guid id)
        {
            try
            {
                var PCEEvaluation = await _PCEEvaluationService.GetPCEEvaluation(base.GetCurrentUserId(), id);
                if (PCEEvaluation == null)
                {
                    return RedirectToAction("NewPCEEvaluations");
                }
                ViewData["PCEEvaluation"] = PCEEvaluation;
                ViewData["Id"] = base.GetCurrentUserId();
                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching pending details for PCEEvaluation, ID {Id}", id);
                return View("Error", new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetPending()
        {
            try
            {
                var pendingPCEEvaluations = await _PCEEvaluationService.GetPendingPCEEvaluations(base.GetCurrentUserId());
                return Content(JsonConvert.SerializeObject(pendingPCEEvaluations), "application/json");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching pending PCEEvaluations for user {UserId}", base.GetCurrentUserId());
                return View("Error", new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
            }
        }

        [HttpPost]
        public async Task<IActionResult> SendForApproval(IEnumerable<Guid> SelectedIds, Guid CenterId)
        {
            try
            {
                await _PCEEvaluationService.SendForApproval(base.GetCurrentUserId(), SelectedIds, CenterId);
                var response = new { message = "PCEEvaluations sent for approval successfully" };
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending PCEEvaluations for approval for user {UserId}", base.GetCurrentUserId());
                var error = new { message = ex.Message };
                return BadRequest(error);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SendToRM(Guid PCEId)
        {
            if (!await _PCEEvaluationService.SendToRM(base.GetCurrentUserId(), PCEId))
            {
                return RedirectToAction("Index", "PCECase");
            }
            return RedirectToAction("MypendingCase", "PCECase");
        }

        [HttpPost]
        public async Task<IActionResult> Reject(Guid Id, string RejectionReason)
        {
            try
            {
                await _PCEEvaluationService.RejectPCEEvaluation(base.GetCurrentUserId(), Id, RejectionReason);
                return RedirectToAction("RejectedPCEEvaluations");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error rejecting PCEEvaluation for ID {Id}", Id);
                var error = new { message = ex.Message };
                return BadRequest(error);
            }
        }

    
        public async Task<ActionResult> Index()
        {
            try
            {
                var PCEEvaluations = await _PCEEvaluationService.GetAllPCEEvaluations(base.GetCurrentUserId());
                return View(PCEEvaluations);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching all PCEEvaluations for user {UserId}", base.GetCurrentUserId());
                return View("Error", new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
            }
        }
        
        [HttpGet]
        public IActionResult My()
        {
            return View();
        }
       
        [HttpGet]
        public async Task<IActionResult> Evaluation(Guid id)
        {
            try
            {
                var PCEEvaluation = await _PCEEvaluationService.GetPCEEvaluation(base.GetCurrentUserId(), id);
                
                // return RedirectToAction("Create", "PCEEvaluation", new { Id = Id });
                
            
                string jsonData = JsonConvert.SerializeObject(PCEEvaluation);

                return Content(jsonData, "application/json");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching PCEEvaluation for editing, ID {Id}", id);
                return View("Error", new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
            }
        }


        // public async Task<IActionResult>ReEvaluation(Guid Id)
        // {
        //     var PCE = await _PCEEvaluationService.GetPCE(base.GetCurrentUserId(), Id);
        //     if (PCE.Category == EnumHelper.GetEnumDisplayName(MechanicalPCECategory.MOV))
        //     {
        //         return RedirectToAction("GetReturnedEvaluatedPCEEvaluation", "MotorVehicle", new { Id = Id });
        //     }
        
        //     string jsonData = JsonConvert.SerializeObject(PCE);
        //     return Content(jsonData, "application/json");
        // }
     
 
        public async Task<ActionResult> Delete(Guid id)
        {
            try
            {
                var PCEEvaluation = await _PCEEvaluationService.GetPCEEvaluation(base.GetCurrentUserId(), id);
                if (PCEEvaluation == null)
                {
                    return NotFound();
                }
                return View(PCEEvaluation);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching PCEEvaluation for deletion, ID {Id}", id);
                return View("Error", new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(Guid id, IFormCollection collection)
        {
            try
            {
                var result = await _PCEEvaluationService.DeletePCEEvaluation(base.GetCurrentUserId(), id);
                if (!result)
                {
                    return NotFound();
                }
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting PCEEvaluation for ID {Id}", id);
                return View("Error", new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CheckAssessment(Guid Id, PCEEvaluationPostDto dto)
        {
            var PCEEvaluation = await _PCEEvaluationService.CheckPCEEvaluation(base.GetCurrentUserId(), Id, dto);
            return RedirectToAction("GetCheckPCEEvaluation", "PCEEvaluation", PCEEvaluation);
        }

        [HttpGet]
        public async Task<IActionResult> GetCheck(PCEEvaluationReturnDto dto)
        {
            return View(dto);
        }

        [HttpPost]
        public async Task<IActionResult> RemarkRelease(Guid Id, Guid PCEId, String Remark, Guid EvaluatorID)
        {
            await _PCEEvaluationService.RemarkRelease(base.GetCurrentUserId(), Id, PCEId, Remark, EvaluatorID);

            return RedirectToAction("RemarkPCECases", "MoPCECase");
        }
        
        public async Task<IActionResult> GetSummary(Guid Id)
        {
            var PCEEvaluationSummaries = await _PCEEvaluationService.GetPCEEvaluationSummary(base.GetCurrentUserId(), Id);
            return Content(PCEEvaluationSummaries, "application/json");
        }

        [HttpGet]
        public async Task<IActionResult> GetEvaluated(Guid Id)
        {
            var PCEEvaluation = await _PCEEvaluationService.GetEvaluatedPCEEvaluation(base.GetCurrentUserId(), Id);

            return View(PCEEvaluation);
        }

        public async Task<IActionResult> GetReturned(Guid Id)
        {
            var PCEEvaluation = await _PCEEvaluationService.GetReturnedPCEEvaluation(base.GetCurrentUserId(),Id);
            var com = await _PCEEvaluationService.GetComments(base.GetCurrentUserId(), Id);
            ViewData["Comments"] = com;
            // ViewData["PCEFile"] = await _uploadFileService.GetUploadFileByPCEId(Id);
            return View(PCEEvaluation);
        }
       

    }
}
