using Newtonsoft.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DocumentFormat.OpenXml.Bibliography;

using mechanical.Data;
using mechanical.Models;
using mechanical.Models.Enum;
using mechanical.Models.Dto.UploadFileDto;
using mechanical.Services.UploadFileService;

using mechanical.Models.PCE.Entities;
using mechanical.Models.PCE.Enum.ProductionCapacity;
using mechanical.Models.PCE.Dto.ProductionCapacityDto;
// using mechanical.Controllers.PCE;
using mechanical.Services.PCE.PCECaseService;
using mechanical.Services.PCE.MOPCECaseService;
using mechanical.Services.PCE.PCEEvaluationService;
using mechanical.Services.PCE.ProductionCapacityServices;


namespace mechanical.Controllers
{
    public class ProductionCapacityController : BaseController
    {
        private readonly IPCECaseService _pCECaseService;
        private readonly IProductionCapacityServices _productionCapacityServices;
        private readonly ILogger<ProductionCapacityController> _logger;
        private readonly CbeContext _cbeContext;

        private readonly IUploadFileService _uploadFileService;
        private readonly IMOPCECaseService _MOPCECaseService;


        public ProductionCapacityController(CbeContext cbeContext, IPCECaseService pCECaseService, IMOPCECaseService MOPCECaseService, IProductionCapacityServices productionCapacityServices, IUploadFileService uploadFileService)
        {
            _cbeContext = cbeContext;
            _productionCapacityServices = productionCapacityServices;
            _MOPCECaseService = MOPCECaseService;
            _pCECaseService = pCECaseService;
            _uploadFileService = uploadFileService;
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
                await _productionCapacityServices.CreateProductionCapacity(base.GetCurrentUserId(), PCECaseId, productionDto);
                var response = new { message = "Manufacturing PCE created successfully" };
                return Ok(response);
            }
            return BadRequest();

        }

        [HttpPost]
        // [ValidateAntiForgeryToken]
        public async Task<IActionResult> PlantCreate(Guid caseId, PlantPostDto PlantCollateralDto)
        {
            if (ModelState.IsValid)
            {

                if (PlantCollateralDto.PlantName == "Others, please specify")
                {
                    PlantCollateralDto.PlantName = PlantCollateralDto.OtherPlantName;
                }

                await _productionCapacityServices.CreatePlantProduction(base.GetCurrentUserId(), caseId, PlantCollateralDto);
                var response = new { message = "Plant PCE created successfully" };
                return Ok(response);
            }
            return BadRequest();
        }
        [HttpGet]
        public async Task<IActionResult> GetProductions(Guid PCECaseId)
        {
            var products = await _productionCapacityServices.GetProductions(PCECaseId);
            string jsonData = JsonConvert.SerializeObject(products);
            return Content(jsonData, "application/json");

        }
        [HttpGet]
        public async Task<IActionResult> GetPendingProductions(Guid PCECaseId)
        {
            //PCECaseId = Guid.Parse("C847C43F-958C-456A-B46F-043A6E22DD5B");
            var products = await _productionCapacityServices.GetPendingProductions(PCECaseId);
            string jsonData = JsonConvert.SerializeObject(products);
            return Content(jsonData, "application/json");

        }

        [HttpGet]
        public async Task<IActionResult> Detail(Guid id)
        {
            var userId = base.GetCurrentUserId();
            var pceDetail = await _MOPCECaseService.GetPCEDetails(userId, id);

            if (pceDetail.ProductionCapacity == null)
            {
                if (pceDetail.PCECase != null)
                {
                    return RedirectToAction("PCECaseDetail", "MOPCECase", new { Id = pceDetail.PCECase.Id });   
                }
                return RedirectToAction("MyPCECases");
            }
            
            ViewData["CurrentUser"] = await _MOPCECaseService.GetUser(userId);
            ViewData["Reestimation"] = pceDetail.Reestimation;
            ViewData["PCE"] = pceDetail.ProductionCapacity;
            ViewData["LatestEvaluation"] = pceDetail.PCEValuationHistory.LatestEvaluation;
            ViewData["PreviousEvaluations"] = pceDetail.PCEValuationHistory.PreviousEvaluations;
            ViewData["PCECase"] = pceDetail.PCECase;
            ViewData["ProductionFiles"] = pceDetail.RelatedFiles;
            ViewData["RejectedProduction"] = pceDetail.RejectedProduction;
            ViewData["RejectedBy"] = pceDetail.RejectedBy;

            return View(pceDetail.ProductionCapacity);

        }


        [HttpGet]
        public async Task<IActionResult> GetRejectProducts(Guid CaseId)
        {
            var products = await _productionCapacityServices.GetRejectedProductions(CaseId);
            string jsonData = JsonConvert.SerializeObject(products);
            return Content(jsonData, "application/json");
        }
        public async Task<IActionResult> GetPendProductions(Guid CaseId)
        {
            var products = await _productionCapacityServices.GetPendProductions(CaseId);
            string jsonData = JsonConvert.SerializeObject(products);
            return Content(jsonData, "application/json");
        }

        [HttpPost]
        public async Task<IActionResult> DeleteProduction(Guid id)
        {

            if (await _productionCapacityServices.DeleteProduction(base.GetCurrentUserId(), id))

            {
                return Ok();
            }
            return BadRequest();
        }
        [HttpGet]
        public async Task<IActionResult> GetRMCompleteProduction(Guid CaseId)
        {
            var production = await _productionCapacityServices.GetRmComProductions(CaseId);
            string jsonData = JsonConvert.SerializeObject(production);
            return Content(jsonData, "application/json");
        }

        //////////
        [HttpGet]
        public async Task<IActionResult> GetRmRejectedProductions(Guid PCECaseId)
        {
            var production = await _productionCapacityServices.GetRmRejectedProductions(base.GetCurrentUserId(), PCECaseId);
            string jsonData = JsonConvert.SerializeObject(production);
            return Content(jsonData, "application/json");
        }
        ///////////

        [HttpGet]
        public async Task<IActionResult> CheckCategory(Guid CaseId)
        {
            var production = await _productionCapacityServices.GetProductions(CaseId);
            string jsonData = JsonConvert.SerializeObject(production);
            return Content(jsonData, "application/json");
        }



        [HttpGet]
        public async Task<IActionResult> GetMyAssigmentProduction(Guid CaseId)
        {
            var Production = await _productionCapacityServices.GetMyAssignmentProductions(base.GetCurrentUserId(), CaseId);
            string jsonData = JsonConvert.SerializeObject(Production);
            return Content(jsonData, "application/json");
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, ProductionPostDto collateralPostDto)
        {
            if (ModelState.IsValid)
            {
                var collateral = await _productionCapacityServices.EditProduction(base.GetCurrentUserId(), id, collateralPostDto);
                return RedirectToAction("PCEDetail", "PCECase", new { Id = collateral.PCECaseId });
            }
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            var response = await _productionCapacityServices.GetProduction(base.GetCurrentUserId(), id);
            var file = await _uploadFileService.GetUploadFileByCollateralId(id);
            ViewData["ProductionFiles"] = file;
            if (response == null) { return RedirectToAction("PCENewCases"); }
            return View(response);
        }


        [HttpGet]
        public async Task<IActionResult> PlantEdit(Guid id)
        {
            var response = await _productionCapacityServices.GetPlantProduction(base.GetCurrentUserId(), id);
            var file = await _uploadFileService.GetUploadFileByCollateralId(id);
            ViewData["ProductionFiles"] = file;
            if (response == null) { return RedirectToAction("PCENewCases"); }
            return View(response);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PlantEdit(Guid id, PlantEditPostDto collateralPostDto)
        {
            if (ModelState.IsValid)
            {
                if (collateralPostDto.PlantName == "Others, please specify")
                {
                    collateralPostDto.PlantName = collateralPostDto.OtherPlantName;
                }
                var collateral = await _productionCapacityServices.EditPlantProduction(base.GetCurrentUserId(), id, collateralPostDto);
                return RedirectToAction("PCEDetail", "PCECase", new { Id = collateral.PCECaseId });
            }
            return View();
        }



        [HttpPost]
        public async Task<ActionResult> DeleteProductionFile(Guid Id)
        {
            if (await _productionCapacityServices.DeleteProductionFile(base.GetCurrentUserId(), Id))
            {
                return Ok();
            }
            return BadRequest();
        }

        [HttpPost]
        public async Task<ActionResult> UploadProductionFile(IFormFile BussinessLicence, Guid caseId, string DocumentCatagory)
        {


            if (await _productionCapacityServices.UploadProductionFile(base.GetCurrentUserId(), BussinessLicence, caseId, DocumentCatagory))
            {
                return Ok();
            }
            return BadRequest();

        }

        [HttpPost]
        public async Task<IActionResult> HandleRemark(Guid ProductionCapacityId, Guid EvaluatorId, String RemarkType, CreateFileDto UploadFile)
        {
            var pceCaseAssignment = await _cbeContext.PCECaseAssignments.Where(res => res.ProductionCapacityId == ProductionCapacityId && res.UserId == EvaluatorId).FirstOrDefaultAsync();
            pceCaseAssignment.Status = "Remark";
            _cbeContext.Update(pceCaseAssignment);

            var production = await _cbeContext.ProductionCapacities.Where(res => res.Id == ProductionCapacityId).FirstOrDefaultAsync();
            if (RemarkType == "Verfication")
            {
                production.CurrentStatus = "Remark Verfication";
            }
            else
            {
                production.CurrentStatus = "Remark Justfication";
            }
            if (UploadFile.File != null)
            {
                UploadFile.CaseId = production.PCECaseId;
                await _uploadFileService.CreateUploadFile(base.GetCurrentUserId(), UploadFile);
            }

            production.CurrentStage = "Maker Officer";

            _cbeContext.Update(production);
            _cbeContext.SaveChanges();

            return RedirectToAction("PCENewCases", "PCECase");
        }

        [HttpGet]
        public async Task<IActionResult> GetRemarkProductions(Guid PCECaseId)
        {
            var collaterals = await _productionCapacityServices.GetRemarkProductions(base.GetCurrentUserId(), PCECaseId);
            string jsonData = JsonConvert.SerializeObject(collaterals);
            return Content(jsonData, "application/json");
        }


        [HttpGet]
        public async Task<IActionResult> GetRMCompleteCollaterals(Guid PCECaseId)
        {
            var collaterals = await _productionCapacityServices.GetRmComCollaterals(PCECaseId);
            string jsonData = JsonConvert.SerializeObject(collaterals);
            return Content(jsonData, "application/json");
        }

        [HttpGet]
        public async Task<IActionResult> GetPCEs(Guid? PCECaseId = null, string Status = "All")
        {
            IEnumerable<ReturnProductionDto> productions = null;
            if (PCECaseId == null)
            {
                productions = await _MOPCECaseService.GetPCEs(base.GetCurrentUserId(), Status: Status);
            
                if (productions == null)
                {
                    return BadRequest("Unable to load {Status} PCEs");
                }
            }
            else
            {
                productions = await _MOPCECaseService.GetPCEs(base.GetCurrentUserId(), PCECaseId, Status: Status);
            
                if (productions == null)
                {
                    return BadRequest("Unable to load {Status} PCEs with PCECase ID: {PCECaseId}");
                }
            }

            string jsonData = JsonConvert.SerializeObject(productions);
            return Content(jsonData, "application/json");
        }
    }
}
