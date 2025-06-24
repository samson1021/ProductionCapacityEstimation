using Newtonsoft.Json;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using DocumentFormat.OpenXml.Bibliography;

using mechanical.Data;
using mechanical.Models;
using mechanical.Models.Enum;
using mechanical.Models.ViewModels;
using mechanical.Models.Dto.UploadFileDto;
using mechanical.Services.UserService;
using mechanical.Services.UploadFileService;

using mechanical.Models.PCE.Entities;
using mechanical.Models.PCE.Enum.ProductionCapacity;
using mechanical.Models.PCE.Dto.ProductionCapacityDto;
using mechanical.Services.PCE.PCECaseScheduleService;
using mechanical.Services.PCE.ProductionCapacityService;
using mechanical.Models.Entities;

namespace mechanical.Controllers
{

    // [Authorize(Roles = "Maker Manager,District Valuation Manager ,Maker Officer, Maker TeamLeader, Relation Manager,Checker Manager, Checker TeamLeader, Checker Officer")]
    [Authorize]
    public class ProductionCapacityController : BaseController
    {
        private readonly IUserService _UserService;
        private readonly IUploadFileService _UploadFileService;
        private readonly ILogger<ProductionCapacityController> _logger;
        private readonly IPCECaseScheduleService _PCECaseScheduleService;
        private readonly IProductionCapacityService _ProductionCapacityService;

        public ProductionCapacityController(IUserService UserService, IProductionCapacityService ProductionCapacityService, IPCECaseScheduleService PCECaseScheduleService, IUploadFileService UploadFileService, ILogger<ProductionCapacityController> logger)
        {
            _UserService = UserService;
            _UploadFileService = UploadFileService;
            _PCECaseScheduleService = PCECaseScheduleService;
            _ProductionCapacityService = ProductionCapacityService;
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Guid PCECaseId, ProductionPostDto productionDto)
        {
            if (ModelState.IsValid)
            {
                await _ProductionCapacityService.CreateProductionCapacity(base.GetCurrentUserId(), PCECaseId, productionDto);
                var response = new { message = "Manufacturing PCE created successfully" };
                return Ok(response);
            }
            return BadRequest();

        }       

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, ProductionEditDto ProductionDto)
        {
            if (ModelState.IsValid)
            {
                var production = await _ProductionCapacityService.EditProduction(base.GetCurrentUserId(), id, ProductionDto);
               // return RedirectToAction("Detail", "ProductionCapacity", new { Id = production.Id });
                return RedirectToAction("Detail", "PCECase", new { Id = production.PCECaseId });
            }
            var response = await _ProductionCapacityService.GetProduction(base.GetCurrentUserId(), id);
            var file = await _UploadFileService.GetUploadFileByCollateralId(id);
            ViewData["ProductionFiles"] = file;
            return View(response);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            var response = await _ProductionCapacityService.GetProduction(base.GetCurrentUserId(), id);
            
            if (response == null) 
            { 
                return RedirectToAction("PCECases", "PCECase");
            }

            var file = await _UploadFileService.GetUploadFileByCollateralId(id);
            ViewData["ProductionFiles"] = file;

            return View(response);
        }

        // [HttpGet("{Id}")]
        [HttpGet]
        public async Task<IActionResult> Detail(Guid Id)
        {
            try{
                    
                var userId = base.GetCurrentUserId();
                var productionDetail = await _ProductionCapacityService.GetProductionDetails(userId, Id);

                if (productionDetail.ProductionCapacity == null)
                {
                    if (productionDetail.PCECase == null)
                    {
                        return RedirectToAction("Detail", "PCECase", new { Id = productionDetail.PCECase.Id, Status = "New" });   
                    }
                    return RedirectToAction("PCECases", "PCECase");
                }
                
                ViewData["CurrentUser"] = await _UserService.GetUserById(userId);
                ViewData["LatestPCECaseSchedule"] = await _PCECaseScheduleService.GetLatestSchedule(productionDetail.PCECase.Id);
                ViewData["Reestimation"] = productionDetail.Reestimation;
                ViewData["Production"] = productionDetail.ProductionCapacity;
                ViewData["LatestEvaluation"] = productionDetail.PCEValuationHistory.LatestEvaluation;
                ViewData["PreviousEvaluations"] = productionDetail.PCEValuationHistory.PreviousEvaluations;
                ViewData["PCECase"] = productionDetail.PCECase;
                ViewData["ProductionFiles"] = productionDetail.RelatedFiles;
                ViewData["ReturnedProductions"] = productionDetail.ReturnedProductions;
                ViewData["Title"] = "Production Detail";
                
                return View(productionDetail.ProductionCapacity);
            }
            catch (Exception ex)
            {
                // _logger.LogError(ex, "Error fetching Production capacity details for ID: {Id}", Id);
                return View("Error", new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
            }
        }

        [HttpPost]
        public async Task<IActionResult> DeleteProduction(Guid id)
        {
            if (await _ProductionCapacityService.DeleteProduction(base.GetCurrentUserId(), id))

            {
                return Ok();
            }
            return BadRequest();
        }


        [HttpGet]
        public async Task<IActionResult> CheckCategory(Guid CaseId)
        {
            var productions = await _ProductionCapacityService.GetProductions(CaseId);
            string jsonData = JsonConvert.SerializeObject(productions, new JsonSerializerSettings{ReferenceLoopHandling = ReferenceLoopHandling.Ignore});
            return Content(jsonData, "application/json");
        }


        [HttpPost]
        public async Task<ActionResult> DeleteProductionFile(Guid Id)
        {
            if (await _ProductionCapacityService.DeleteProductionFile(base.GetCurrentUserId(), Id))
            {
                return Ok();
            }
            return BadRequest();
        }

        [HttpPost]
        public async Task<ActionResult> UploadProductionFile(IFormFile File, Guid ProductionId, string DocumentCategory)
        {

            if (await _ProductionCapacityService.UploadProductionFile(base.GetCurrentUserId(), File, ProductionId, DocumentCategory))
            {
                return Ok();
            }
            return BadRequest();

        }


        [HttpGet]
        public async Task<IActionResult> Productions(string Status = "All")
        { 
            var allowedStatuses = new[] { "", "All", "New", "Pending", "Completed", "Returned", "Terminated", "Remarked", "Reestimate" };         
            
            if (!allowedStatuses.Any(s => s.Equals(Status, StringComparison.OrdinalIgnoreCase))) { 
                return BadRequest("Invalid status.");
            }
            
            ViewData["CurrentUser"] = await _UserService.GetUserById(base.GetCurrentUserId());
            ViewData["Title"] = Status + " Productions";
            ViewBag.Status = Status;
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetProductions(Guid? PCECaseId = null, string Status = "All")
        { 
            var allowedStatuses = new[] { "", "All", "New", "Pending", "Completed", "Returned", "Terminated", "Remarked", "Reestimate" };         
            
            if (!allowedStatuses.Any(s => s.Equals(Status, StringComparison.OrdinalIgnoreCase))) { 
                return BadRequest("Invalid status.");
            }

            IEnumerable<ProductionReturnDto> productions = null;
            if (PCECaseId == null)
            {
                productions = await _ProductionCapacityService.GetProductions(base.GetCurrentUserId(), Status: Status);
            
                if (productions == null)
                {
                    return BadRequest("Unable to load {Status} Productions");
                }
            }
            else
            {
                productions = await _ProductionCapacityService.GetProductions(base.GetCurrentUserId(), PCECaseId, Status: Status);
            
                if (productions == null)
                {
                    return BadRequest("Unable to load {Status} Productions with PCECase ID: {PCECaseId}");
                }
            }

            string jsonData = JsonConvert.SerializeObject(productions, new JsonSerializerSettings{ReferenceLoopHandling = ReferenceLoopHandling.Ignore});
            return Content(jsonData, "application/json");
        }


        [HttpGet]
        public async Task<IActionResult> GetRemarkProductions(Guid PCECaseId)
        {
            var productions = await _ProductionCapacityService.GetRemarkProductions(base.GetCurrentUserId(), PCECaseId);
            string jsonData = JsonConvert.SerializeObject(productions, new JsonSerializerSettings{ReferenceLoopHandling = ReferenceLoopHandling.Ignore});
            return Content(jsonData, "application/json");
        }

        [HttpGet]
        public async Task<IActionResult> GetAssignedProduction(Guid PCECaseId)
        {
            var productions = await _ProductionCapacityService.GetAssignedProductions(base.GetCurrentUserId(), PCECaseId);
            string jsonData = JsonConvert.SerializeObject(productions, new JsonSerializerSettings{ReferenceLoopHandling = ReferenceLoopHandling.Ignore});
            return Content(jsonData, "application/json");
        }

        // [HttpGet]
        // public async Task<IActionResult> GetMyDashboardPCECount()
        // {
        //     var productions = await _ProductionCapacityService.GetDashboardPCECount(base.GetCurrentUserId());
        // string jsonData = JsonConvert.SerializeObject(productions, new JsonSerializerSettings{ReferenceLoopHandling = ReferenceLoopHandling.Ignore});
        //     return Content(jsonData, "application/json");
        // }


        // HO
        [HttpGet]
        public async Task<IActionResult> HOProductions(string Status = "All")
        {
            var allowedStatuses = new[] { "", "All", "New", "Pending", "Completed", "Returned", "Terminated", "Remarked", "Reestimate" };

            if (!allowedStatuses.Any(s => s.Equals(Status, StringComparison.OrdinalIgnoreCase)))
            {
                return BadRequest("Invalid status.");
            }

            ViewData["CurrentUser"] = await _UserService.GetUserById(base.GetCurrentUserId());
            ViewData["Title"] = Status + " Productions";
            ViewBag.Status = Status;
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> GetHOProductions(Guid? PCECaseId = null, string Status = "All")
        {
            var allowedStatuses = new[] { "", "All", "New", "Pending", "Completed", "Returned", "Terminated", "Remarked", "Reestimate" };

            if (!allowedStatuses.Any(s => s.Equals(Status, StringComparison.OrdinalIgnoreCase)))
            {
                return BadRequest("Invalid status.");
            }

            var productions = await _ProductionCapacityService.GetHOProductions(PCECaseId, Status);

            if (productions == null)
            {
                var caseIdMessage = PCECaseId.HasValue
                    ? $" with PCECase ID: {PCECaseId}"
                    : string.Empty;

                return BadRequest($"Unable to load {Status} Productions{caseIdMessage}");
            }

            string jsonData = JsonConvert.SerializeObject(productions, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
            return Content(jsonData, "application/json");
        }
        [HttpGet]
        public async Task<IActionResult> HODetail(Guid Id)
        {
            try
            {

                var userId = base.GetCurrentUserId();
                var productionDetail = await _ProductionCapacityService.GetHOProductionDetails(Id);

                if (productionDetail.ProductionCapacity == null)
                {
                    if (productionDetail.PCECase == null)
                    {
                        return RedirectToAction("HODetail", "PCECase", new { Id = productionDetail.PCECase.Id, Status = "New" });
                    }
                    return RedirectToAction("HOPCECases", "PCECase");
                }

                ViewData["CurrentUser"] = await _UserService.GetUserById(userId);
                ViewData["LatestPCECaseSchedule"] = await _PCECaseScheduleService.GetLatestSchedule(productionDetail.PCECase.Id);
                ViewData["Reestimation"] = productionDetail.Reestimation;
                ViewData["Production"] = productionDetail.ProductionCapacity;
                ViewData["LatestEvaluation"] = productionDetail.PCEValuationHistory.LatestEvaluation;
                ViewData["PreviousEvaluations"] = productionDetail.PCEValuationHistory.PreviousEvaluations;
                ViewData["PCECase"] = productionDetail.PCECase;
                ViewData["ProductionFiles"] = productionDetail.RelatedFiles;
                ViewData["ReturnedProductions"] = productionDetail.ReturnedProductions;
                ViewData["Title"] = "Production Detail";

                return View(productionDetail.ProductionCapacity);
            }
            catch (Exception ex)
            {
                // _logger.LogError(ex, "Error fetching Production capacity details for ID: {Id}", Id);
                return View("Error", new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
            }
        }

    }
}