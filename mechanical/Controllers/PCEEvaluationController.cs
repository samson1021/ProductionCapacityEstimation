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
using mechanical.Models.Dto.UploadFileDto;
using mechanical.Models.PCE.Dto.PCEEvaluationDto;
using mechanical.Models.PCE.Enum.PCEEvaluation;
using mechanical.Services.UploadFileService;
using mechanical.Services.PCE.PCEEvaluationService;
using mechanical.Services.MailService;

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

        public PCEEvaluationController(IMapper mapper, IPCEEvaluationService PCEEvaluationService, IMailService mailService, ILogger<PCEEvaluationController> logger, IUploadFileService UploadFileService) 
            // IPCEEvaluationPCECaseService PCEEvaluationPCECaseService, IPCEEvaluationPCECaseTerminateService PCEEvaluationPCECaseTermnateService, IPCEEvaluationPCECaseScheduleService PCEEvaluationPCECaseScheduleService)
       
{
            _PCEEvaluationService = PCEEvaluationService;
            _mapper = mapper;
            _logger = logger;
            _mailService = mailService;
            _uploadFileService = UploadFileService;
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
        public async Task<IActionResult> Create(Guid PCEId)
        {
            try
            {
                // var PCE = await _PCEEvaluationService.GetPCE(base.GetCurrentUserId(), {PCEId);
                // if (PCE == null)
                // {
                //     return RedirectToAction("MyPCECase", "PCECase");
                // }
                // ViewData["PCE"] = PCE;
                // ViewData["EmployeeId"] = HttpContext.Session.GetString("EmployeeId") ?? null;
                // ViewData["PCEId"] = PCEId ?? Guid.Parse("E1BBBE4A-F804-439A-A8E6-539232CCC6F0");
                ViewData["PCEId"] = PCEId;
                
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
                {Console.WriteLine("hererergdfghfghfgdhfghfg");
                    var PCEEvaluation = await _PCEEvaluationService.CreatePCEEvaluation(base.GetCurrentUserId(), Dto);
                    
                    return RedirectToAction("Detail", "PCEEvaluation", new { Id = PCEEvaluation.Id });
                    // return View("Detail", PCEEvaluation);
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
        public async Task<IActionResult> Update(Guid PCEId, Guid Id)
        {    
            try
            {
                var PCEEvaluation = await _PCEEvaluationService.GetPCEEvaluation(base.GetCurrentUserId(), Id);
        
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
                _logger.LogError(ex, "Error fetching PCEEvaluation for updating, ID {Id}", Id);
                return View("Error", new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(Guid Id, PCEEvaluationPostDto Dto)
        {
            if (ModelState.IsValid)
            {
                try
                {
             
                    await _PCEEvaluationService.UpdatePCEEvaluation(base.GetCurrentUserId(), Id, Dto);
                    return RedirectToAction("Detail", "PCEEvaluation", new { Id = Id });
                    // return RedirectToAction("GetPCEEvaluation", "PCEEvaluation", new { Id = id });
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
                var PCEEvaluation = await _PCEEvaluationService.GetPCEEvaluation(base.GetCurrentUserId(), Id);
                if (PCEEvaluation == null)
                {
                    return NotFound();
                }
                // ViewData["PCEFile"] = await _uploadFileService.GetUploadFileByPCEId(PCEEvaluation.PCEId);
                return View(PCEEvaluation);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching PCEEvaluation for deletion, ID {Id}", Id);
                return View("Error", new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(Guid Id, IFormCollection collection)
        {
            try
            {
                var result = await _PCEEvaluationService.DeletePCEEvaluation(base.GetCurrentUserId(), Id);
                if (!result)
                {
                    return NotFound();
                }
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting PCEEvaluation for ID {Id}", Id);
                return View("Error", new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
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
            
                return View(PCEEvaluation);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching PCEEvaluation details for ID {Id}", Id);
                return View("Error", new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
            }
        }

        [HttpGet]
        public async Task<IActionResult> AllNew()
        {
            try
            {
                // var PCEEvaluations = await _PCEEvaluationService.GetNewPCEEvaluations(base.GetCurrentUserId());
                var PCEEvaluations = await _PCEEvaluationService.GetPCEEvaluationsWithStatus(base.GetCurrentUserId(), Status.New);
                ViewBag.Title = "New PCE Evaluations";
                return View("All", PCEEvaluations);
                // return View(PCEEvaluations);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching new PCEEvaluations for user {UserId}", base.GetCurrentUserId());
                return View("Error", new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
            }
        }
        
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Pend(Guid Id, PCEEvaluationPostDto Dto)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _PCEEvaluationService.PendPCEEvaluation(base.GetCurrentUserId(), Id, Dto);
                    return RedirectToAction("Pending", "PCEEvaluation");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error pending PCEEvaluation for ID {Id}", Id);
                    ModelState.AddModelError("", "An error occurred while pending the PCEEvaluation.");
                }
            }
            return View(Dto);
        }
        
        [HttpGet]
        public async Task<IActionResult> AllPending()
        {
            try
            {
                // var PCEEvaluations = await _PCEEvaluationService.GetPendingPCEEvaluations(base.GetCurrentUserId());
                var PCEEvaluations = await _PCEEvaluationService.GetPCEEvaluationsWithStatus(base.GetCurrentUserId(), Status.Pending);
                ViewBag.Title = "Pending PCE Evaluations";
                return View("All", PCEEvaluations);
                // return View(PCEEvaluations);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching Pending PCEEvaluations for user {UserId}", base.GetCurrentUserId());
                return View("Error", new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
            }
        }

        // [HttpGet]
        // public async Task<IActionResult> PendingDetail(Guid id)
        // {
        //     try
        //     {
        //         var PCEEvaluation = await _PCEEvaluationService.GetPCEEvaluation(base.GetCurrentUserId(), id);
        //         if (PCEEvaluation == null)
        //         {
        //             return RedirectToAction("NewPCEEvaluations");
        //         }
        //         ViewData["PCEEvaluation"] = PCEEvaluation;
        //         ViewData["Id"] = base.GetCurrentUserId();
        //         return View();
        //     }
        //     catch (Exception ex)
        //     {
        //         _logger.LogError(ex, "Error fetching pending details for PCEEvaluation, ID {Id}", id);
        //         return View("Error", new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        //     }
        // }
        
        [HttpPost]
        public async Task<IActionResult> Return(PCERejectPostDto Dto)
        // public async Task<IActionResult> Return(PCEEvaluationPostDto Dto)
        // public async Task<IActionResult> Return(PCEEvaluationPostDto Dto, string Reason)
        {
            try
            {
                await _PCEEvaluationService.ReturnPCEEvaluation(base.GetCurrentUserId(), Dto);
                // await _PCEEvaluationService.ReturnPCEEvaluation(base.GetCurrentUserId(), Dto, Reason);
                
                return Json(new { success = true });
                return RedirectToAction("ReturnedPCEEvaluations");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error Returning PCEEvaluation for user {UserId}", base.GetCurrentUserId());
                var error = new { message = ex.Message };
                return Json(new { success = false, error = ex.Message });
                // return BadRequest(error);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Returned(Guid Id)
        {
            try
            {
                var PCEEvaluation = await _PCEEvaluationService.GetPCEEvaluation(base.GetCurrentUserId(), Id);
                
                if (PCEEvaluation.Status == Status.Returned)
                {    
                    ViewData["Comments"] = await _PCEEvaluationService.GetComments(base.GetCurrentUserId(), Id);
                    // ViewData["PCEFile"] = await _uploadFileService.GetUploadFileByPCEId(Id);
                }
                // return Content(JsonConvert.SerializeObject(PCEEvaluation), "application/json");
                return View(PCEEvaluation);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching Returned PCEEvaluation for ID {Id}", Id);
                return View("Error", new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
            }
        }
        
        [HttpGet]
        public async Task<IActionResult> AllReturned()
        {
            try
            {
                // var PCEEvaluations = await _PCEEvaluationService.GetReturnedPCEEvaluations(base.GetCurrentUserId());
                var PCEEvaluations = await _PCEEvaluationService.GetPCEEvaluationsWithStatus(base.GetCurrentUserId(), Status.Returned);
                ViewBag.Title = "Returned PCE Evaluations";
                return View("All", PCEEvaluations);
                // return View(PCEEvaluations);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching Returned PCEEvaluations for user {UserId}", base.GetCurrentUserId());
                return View("Error", new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
            }
        }
        
        // public async Task<IActionResult>ReEvaluated(Guid Id)
        // {
        //     var PCE = await _PCEEvaluationService.GetPCE(base.GetCurrentUserId(), Id);
      
        //     string jsonData = JsonConvert.SerializeObject(PCE);
        //     return Content(jsonData, "application/json");
        // }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Complete(Guid PCEId, PCEEvaluationPostDto Dto)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _PCEEvaluationService.CompletePCEEvaluation(base.GetCurrentUserId(), PCEId, Dto);
                    return RedirectToAction("Completed", "PCEEvaluation");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error closing PCEEvaluation for ID {PCEId}", PCEId);
                    ModelState.AddModelError("", "An error occurred while closing the PCEEvaluation.");
                }
            }
            return View(Dto);
        }

        [HttpGet]
        public async Task<IActionResult> AllCompleted()
        {
            try
            {
                // var PCEEvaluations = await _PCEEvaluationService.GetCompletedPCEEvaluations(base.GetCurrentUserId());
                var PCEEvaluations = await _PCEEvaluationService.GetPCEEvaluationsWithStatus(base.GetCurrentUserId(), Status.Completed);
                ViewBag.Title = "Completed PCE Evaluations";
                return View("All", PCEEvaluations);
                // return View(PCEEvaluations);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching Completed PCEEvaluations for user {UserId}", base.GetCurrentUserId());
                return View("Error", new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
            }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SendToRM(Guid Id)
        {
            try
            {
                if (!await _PCEEvaluationService.SendToRM(base.GetCurrentUserId(), Id))
                {
                    return RedirectToAction("Index", "PCECase");
                }
                return RedirectToAction("MyPendingCase", "PCEEvaluation");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error Sending PCEEvaluation of ID: {Id} to RM", Id);
                return View("Error", new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
            }
    
        }
        

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SendToMO(Guid Id)
        {
           try
            {
                if (!await _PCEEvaluationService.SendToMO(base.GetCurrentUserId(), Id))
                {
                    return RedirectToAction("Index", "PCECase");
                }
                return RedirectToAction("MyPendingCase", "PCEEvaluation");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error Resending PCEEvaluation of ID: {Id} to MO", Id);
                return View("Error", new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
            }
    
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Evaluate(Guid Id, PCEEvaluationPostDto Dto)
        {
    
            // if (!await _PCEEvaluationService.Evaluate(base.GetCurrentUserId(), PCEId))
            // {
            //     return View();
            //     // return RedirectToAction("Index", "PCEEvaluation");
            // }
            // return RedirectToAction("Evaluated", "PCEEvaluation");
       
            if (ModelState.IsValid)
            {
                try
                {
                    await _PCEEvaluationService.EvaluatePCEEvaluation(base.GetCurrentUserId(), Id, Dto);
                    return RedirectToAction("Evaluate", "PCEEvaluation");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error Evaluate PCEEvaluation for ID {Id}", Id);
                    ModelState.AddModelError("", "An error occurred while Evaluate the PCEEvaluation.");
                }
            }
            return View(Dto);
        }
        
        [HttpGet]
        public async Task<IActionResult> AllEvaluated()
        {
            try
            {
                // var PCEEvaluations = await _PCEEvaluationService.GetEvaluatedPCEEvaluations(base.GetCurrentUserId());
                var PCEEvaluations = await _PCEEvaluationService.GetPCEEvaluationsWithStatus(base.GetCurrentUserId(), Status.Evaluated);
                ViewBag.Title = "Evaluated PCE Evaluations";
                return View("All", PCEEvaluations);
                // return View(PCEEvaluations);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching Evaluated Pending PCEEvaluations for user {UserId}", base.GetCurrentUserId());
                return View("Error", new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
            }
        }
                
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reevaluate(Guid PCEId, PCEEvaluationPostDto Dto)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _PCEEvaluationService.ReevaluatePCEEvaluation(base.GetCurrentUserId(), PCEId, Dto);
                    // return Content(JsonConvert.SerializeObject(PCEEvaluation), "application/json");
                    return RedirectToAction("Reevaluate", "PCEEvaluation");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error Reevaluating PCEEvaluation for ID {PCEId}", PCEId);
                    ModelState.AddModelError("", "An error occurred while reevaluating the PCEEvaluation.");
                }
            }
            return View(Dto);
        }
        

        [HttpGet]
        public async Task<IActionResult> AllReevaluated()
        {
            try
            {
                // var PCEEvaluations = await _PCEEvaluationService.GetReevaluatedPCEEvaluations(base.GetCurrentUserId());
                var PCEEvaluations = await _PCEEvaluationService.GetPCEEvaluationsWithStatus(base.GetCurrentUserId(), Status.Reevaluated);
                ViewBag.Title = "Reevaluated PCE Evaluations";
                return View("All", PCEEvaluations);
                // return View(PCEEvaluations);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching Reevaluated PCEEvaluations for user {UserId}", base.GetCurrentUserId());
                return View("Error", new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetMyDashboardPCECaseCount()
        {
            var myPCECase = await _PCEEvaluationService.GetMyDashboardPCECaseCount(base.GetCurrentUserId());
            string jsonData = JsonConvert.SerializeObject(myPCECase);
            return Content(jsonData, "application/json");
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
        public async Task<IActionResult> MyReturnedPCEs()
        {
            ViewBag.Url = "/PCEEvaluation/GetMyReturnedPCEs";
            ViewData["Title"] = "Returned PCE Cases";
            return View("PCECases");

            // var PCEs = await _PCEEvaluationService.MyReturnedPCEs(base.GetCurrentUserId());
            // return View("PCECases", PCEs);
        }

        public async Task<IActionResult> MyResubmitedPCEs()
        {
            ViewBag.Url = "/PCEEvaluation/GetMyResubmitedPCEs";
            ViewData["Title"] = "Resubmitted PCE Cases";
            return View("PCECases");

            // var PCEs = await _PCEEvaluationService.MyResubmitedPCEs(base.GetCurrentUserId());
            // return View("PCECases", PCEs);
        }


        [HttpGet]
        public IActionResult MyTotalPCECases()
        {
            ViewBag.Url = "/PCEEvaluation/GetMyTotalPCECases";
            ViewData["Title"] = "Total PCE Cases";
            return View("PCECases");
        }

        [HttpGet]
        public async Task<IActionResult> MyPCECase(Guid Id)
        {
            var loanPCECase = await _PCEEvaluationService.GetPCECaseDetail(GetCurrentUserId(), Id);
            // var PCECaseTerminate = await _PCECaseTermnateService.GetPCECaseTerminates(Id);
            // ViewData["PCECaseTerminate"] = PCECaseTerminate;
            if (loanPCECase == null) 
            {
                return RedirectToAction("MyNewPCECases"); 
            }
            ViewData["PCECase"] = loanPCECase;
            ViewData["Id"]=base.GetCurrentUserId();
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> MyPCECaseDetail(Guid Id)
        {
            var loanPCECase = await _PCEEvaluationService.GetPCECaseDetail(GetCurrentUserId(), Id);
            if (loanPCECase == null) 
            {
                return RedirectToAction("MyNewPCECases"); 
            }
            ViewData["PCECase"] = loanPCECase;
            ViewData["Id"] = base.GetCurrentUserId();
            return View();
        }

        public async Task<IActionResult> MyReturnedPCE(Guid PCEId)
        {
            var PCEs =await _PCEEvaluationService.MyReturnedPCE( base.GetCurrentUserId(), PCEId);
            return View(PCEs);
        }

        public async Task<IActionResult> MyResubmitedPCE(Guid PCEId)
        {
            var PCEs =await _PCEEvaluationService.MyResubmitedPCE( base.GetCurrentUserId(), PCEId);
            return View(PCEs);
        }
     
        [HttpGet]
        public async Task<IActionResult> GetMyPCECase(Guid Id)
        {
            var myPCECase = await _PCEEvaluationService.GetPCECase(GetCurrentUserId(), Id);
            if (myPCECase == null) 
            {
                return BadRequest("Unable to load PCECase"); 
            }
            string jsonData = JsonConvert.SerializeObject(myPCECase);
            return Content(jsonData, "application/json");
        }

        [HttpGet]
        public async Task<IActionResult> GetMyNewPCECases()
        {
            var myPCECase = await _PCEEvaluationService.GetNewPCECases(GetCurrentUserId());
            if (myPCECase == null) 
            {
                return BadRequest("Unable to load PCECase"); 
            }
            string jsonData = JsonConvert.SerializeObject(myPCECase);
            return Content(jsonData, "application/json");
        }

        [HttpGet]
        public async Task<IActionResult> GetMyPendingPCECases()
        {
            var myPCECase = await _PCEEvaluationService.GetPendingPCECases(GetCurrentUserId());
            if (myPCECase == null) 
            {
                return BadRequest("Unable to load PCECase"); 
            }
            string jsonData = JsonConvert.SerializeObject(myPCECase);
            return Content(jsonData, "application/json");
        }

        [HttpGet]
        public async Task<IActionResult> GetMyCompletedPCECases()
        {
            var myPCECase = await _PCEEvaluationService.GetCompletedPCECases(GetCurrentUserId());
            if (myPCECase == null) 
            {
                return BadRequest("Unable to load PCECase"); 
            }
            string jsonData = JsonConvert.SerializeObject(myPCECase);
            return Content(jsonData, "application/json");
        }

        [HttpGet]
        public async Task<IActionResult> GetMyReturnedPCEs()
        {
            var myPCECase = await _PCEEvaluationService.GetReturnedPCECases(GetCurrentUserId());
            if (myPCECase == null) 
            {
                return BadRequest("Unable to load PCECase"); 
            }
            string jsonData = JsonConvert.SerializeObject(myPCECase);
            return Content(jsonData, "application/json");
        }

        [HttpGet]
        public async Task<IActionResult> GetMyResubmitedPCEs()
        {
            var myPCECase = await _PCEEvaluationService.GetResubmitedPCECases(GetCurrentUserId());
            if (myPCECase == null) 
            {
                return BadRequest("Unable to load PCECase"); 
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
                return BadRequest("Unable to load PCECase"); 
            }
            string jsonData = JsonConvert.SerializeObject(myPCECase);
            return Content(jsonData, "application/json");
        }
    }
}
