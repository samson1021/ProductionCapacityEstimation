using Newtonsoft.Json;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DocumentFormat.OpenXml.Bibliography;

using mechanical.Data;
using mechanical.Models;
using mechanical.Models.Enum;
using mechanical.Models.Dto.UploadFileDto;
using mechanical.Services.UserService;
using mechanical.Services.UploadFileService;

using mechanical.Models.PCE.Entities;
using mechanical.Models.PCE.Enum.ProductionCapacity;
using mechanical.Models.PCE.Dto.ProductionCapacityDto;
using mechanical.Services.PCE.PCECaseScheduleService;
using mechanical.Services.PCE.ProductionCapacityService;

namespace mechanical.Controllers
{

    // [Authorize(Roles = "Maker Manager,District Valuation Manager ,Maker Officer, Maker TeamLeader, Relation Manager,Checker Manager, Checker TeamLeader, Checker Officer")]

    public class ProductionCapacityController : BaseController
    {
        private readonly IUserService _UserService;
        private readonly IUploadFileService _UploadFileService;
        private readonly ILogger<ProductionCapacityController> _logger;
        private readonly IPCECaseScheduleService _PCECaseScheduleService;
        private readonly IProductionCapacityService _ProductionCapacityService;

        public ProductionCapacityController(IUserService UserService, IProductionCapacityService ProductionCapacityService, IPCECaseScheduleService PCECaseScheduleService, IUploadFileService UploadFileService)
        {
            _UserService = UserService;
            _UploadFileService = UploadFileService;
            _PCECaseScheduleService = PCECaseScheduleService;
            _ProductionCapacityService = ProductionCapacityService;
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
        // [ValidateAntiForgeryToken]
        public async Task<IActionResult> PlantCreate(Guid caseId, PlantPostDto PlantDto)
        {
            if (ModelState.IsValid)
            {

                if (PlantDto.PlantName == "Others, please specify")
                {
                    PlantDto.PlantName = PlantDto.OtherPlantName;
                }

                await _ProductionCapacityService.CreatePlantProduction(base.GetCurrentUserId(), caseId, PlantDto);
                var response = new { message = "Plant PCE created successfully" };
                return Ok(response);
            }
            return BadRequest();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, ProductionPostDto ProductionDto)
        {
            if (ModelState.IsValid)
            {
                var production = await _ProductionCapacityService.EditProduction(base.GetCurrentUserId(), id, ProductionDto);
                return RedirectToAction("Detail", "PCECase", new { Id = production.PCECaseId, Status = "New" });
            }
            return View();
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


        [HttpGet]
        public async Task<IActionResult> PlantEdit(Guid id)
        {
            var response = await _ProductionCapacityService.GetPlantProduction(base.GetCurrentUserId(), id);
            
            if (response == null) 
            { 
                return RedirectToAction("PCECases", "PCECase"); 
            }

            var file = await _UploadFileService.GetUploadFileByCollateralId(id);
            ViewData["ProductionFiles"] = file;
            return View(response);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PlantEdit(Guid id, PlantEditPostDto PlantDto)
        {
            if (ModelState.IsValid)
            {
                if (PlantDto.PlantName == "Others, please specify")
                {
                    PlantDto.PlantName = PlantDto.OtherPlantName;
                }
                var plant = await _ProductionCapacityService.EditPlantProduction(base.GetCurrentUserId(), id, PlantDto);
                return RedirectToAction("Detail", "PCECase", new { Id = plant.PCECaseId, Status = "New" });
            }
            return View();
        }

        // [HttpGet("{Id}")]
        [HttpGet]
        public async Task<IActionResult> Detail(Guid Id)
        {
            try{
                    
                var userId = base.GetCurrentUserId();
                var pceDetail = await _ProductionCapacityService.GetPCEDetails(userId, Id);

                if (pceDetail.ProductionCapacity == null)
                {
                    if (pceDetail.PCECase == null)
                    {
                        return RedirectToAction("PCECases", "PCECase");
                    }
                    return RedirectToAction("Detail", "PCECase", new { Id = pceDetail.PCECase.Id, Status = "New" });   
                }
                
                ViewData["CurrentUser"] = await _UserService.GetUserById(userId);
                ViewData["LatestPCECaseSchedule"] = await _PCECaseScheduleService.GetLatestSchedule(pceDetail.PCECase.Id);
                ViewData["Reestimation"] = pceDetail.Reestimation;
                ViewData["PCE"] = pceDetail.ProductionCapacity;
                ViewData["LatestEvaluation"] = pceDetail.PCEValuationHistory.LatestEvaluation;
                ViewData["PreviousEvaluations"] = pceDetail.PCEValuationHistory.PreviousEvaluations;
                ViewData["PCECase"] = pceDetail.PCECase;
                ViewData["ProductionFiles"] = pceDetail.RelatedFiles;
                ViewData["RejectedProduction"] = pceDetail.RejectedProduction;
                ViewData["RejectedBy"] = pceDetail.RejectedBy;
                ViewData["Title"] = "Production Detail";
                
                return View(pceDetail.ProductionCapacity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching Production capacity details for ID: {Id}", Id);
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
        public async Task<ActionResult> UploadProductionFile(IFormFile BussinessLicence, Guid caseId, string DocumentCatagory)
        {


            if (await _ProductionCapacityService.UploadProductionFile(base.GetCurrentUserId(), BussinessLicence, caseId, DocumentCatagory))
            {
                return Ok();
            }
            return BadRequest();

        }


        [HttpGet]
        public async Task<IActionResult> Productions(string Status = "All")
        { 
            var allowedStatuses = new[] { "", "All", "New", "Pending", "Completed", "Rejected", "Terminated", "Remarked", "Reestimate" };         
            
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
            var allowedStatuses = new[] { "", "All", "New", "Pending", "Completed", "Rejected", "Terminated", "Remarked", "Reestimate" };         
            
            if (!allowedStatuses.Any(s => s.Equals(Status, StringComparison.OrdinalIgnoreCase))) { 
                return BadRequest("Invalid status.");
            }

            IEnumerable<ReturnProductionDto> productions = null;
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
    }
}