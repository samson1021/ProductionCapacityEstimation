﻿using mechanical.Controllers.PCE;
using mechanical.Data;
using mechanical.Models.Enum;
using mechanical.Models;
using mechanical.Models.PCE.Entities;
using mechanical.Services.CaseServices;
using mechanical.Services.PCE.PCECaseService;
//using mechanical.Services.ProductionCaseService;
using mechanical.Services.UploadFileService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using mechanical.Services.PCE.ProductionCapacityServices;
using mechanical.Models.PCE.Dto.ProductionCapacityDto;
using mechanical.Models.Dto.UploadFileDto;
using mechanical.Models.Dto.CollateralDto;
using mechanical.Models.PCE.Enum.ProductionCapacity;
using mechanical.Services.PCE.PCEEvaluationService;
using DocumentFormat.OpenXml.Bibliography;


namespace mechanical.Controllers
{
    public class ProductionCapacityController : BaseController
    {
        private readonly IPCECaseService _pCECaseService;
        private readonly IProductionCapacityServices _productionCapacityServices;
        private readonly ILogger<ProductionCapacityController> _logger;
        private readonly CbeContext _cbeContext;

        private readonly IUploadFileService _uploadFileService;
        private readonly IPCEEvaluationService _PCEEvaluationService;


        public ProductionCapacityController(CbeContext cbeContext, IPCECaseService pCECaseService, IPCEEvaluationService PCEEvaluationService, IProductionCapacityServices productionCapacityServices, IUploadFileService uploadFileService)
        {
            _cbeContext = cbeContext;
            _productionCapacityServices = productionCapacityServices;
            _PCEEvaluationService = PCEEvaluationService;
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
            //PCECaseId = Guid.Parse("C847C43F-958C-456A-B46F-043A6E22DD5B");
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
            var response = await _productionCapacityServices.GetProduction(base.GetCurrentUserId(), id);
            var loanCase = await _pCECaseService.GetProductionCaseDetail(response.PCECaseId);

            var restimation = await _cbeContext.ProductionReestimations.Where(res => res.ProductionCapacityId == id).FirstOrDefaultAsync();
            if (restimation != null)
            {
                ViewData["restimation"] = restimation;
            }

            if (response == null) { return RedirectToAction("PCENewCases"); }
            var file = await _uploadFileService.GetUploadFileByCollateralId(id);
            var rejectedProduction = await _cbeContext.ProductionRejects.Where(res => res.PCEId == id).FirstOrDefaultAsync();
            var remarkTypeProduction = await _cbeContext.ProductionCapacities.Where(res => res.Id == id).FirstAsync();
            var productionById = await _productionCapacityServices.GetProductionCapacityById(id);
            var PcevalutionDto = await _productionCapacityServices.GetValuationById(id);

            if (rejectedProduction != null)
            {
                var user = await _cbeContext.CreateUsers.Include(res => res.Role).FirstOrDefaultAsync(rea => rea.Id == rejectedProduction.RejectedBy);
                ViewData["user"] = user;

            }


            if (PcevalutionDto != null)
            {
                ViewData["PcevalutionDto"] = PcevalutionDto;
            }
            if (response.ProductionType == "Manufacturing")
            {
                var Production = await _productionCapacityServices.GetManufuctringProductionCapacityEvalutionById(id);
                ViewData["Mavaluation"] = Production;
            }
            else if (response.ProductionType == "Plant")
            {
                var Production = await _productionCapacityServices.GetPlantProductionCapacityEvalutionById(id);
                ViewData["Pavaluation"] = Production;
            }


            // ViewData["Prvaluation"] = productionById;
            ViewData["pcecaseDtos"] = loanCase;
            ViewData["productionFiles"] = file;
            ViewData["rejectedCollateral"] = rejectedProduction;
            ViewData["CurrentUserId"] = base.GetCurrentUserId();
            var userId = base.GetCurrentUserId();
            var UserForRole = await _cbeContext.CreateUsers.Where(res => res.Id == userId).FirstOrDefaultAsync();
            var role = await _cbeContext.CreateRoles.Where(res => res.Id == UserForRole.RoleId).FirstOrDefaultAsync();
            ViewData["loggedRole"] = role;
            ViewData["remarkTypeCollateral"] = remarkTypeProduction;

            var pceDetail = await _PCEEvaluationService.GetPCEDetails(base.GetCurrentUserId(), id);
            var currentUser = await _PCEEvaluationService.GetUser(base.GetCurrentUserId());
            ViewData["CurrentUser"] = currentUser;
            ViewData["PCE"] = pceDetail.ProductionCapacity;
            ViewData["LatestEvaluation"] = pceDetail.PCEValuationHistory.LatestEvaluation;


            return View(response);

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
            ViewData["productionFiles"] = file;
            if (response == null) { return RedirectToAction("PCENewCases"); }
            return View(response);
        }


        [HttpGet]
        public async Task<IActionResult> PlantEdit(Guid id)
        {
            var response = await _productionCapacityServices.GetPlantProduction(base.GetCurrentUserId(), id);
            var file = await _uploadFileService.GetUploadFileByCollateralId(id);
            ViewData["productionFiles"] = file;
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
        public async Task<IActionResult> handleProductionRemark(Guid ProductionCapacityId, Guid EvaluatorUserID, String RemarkType, CreateFileDto uploadFile, Guid CheckerUserID)
        {
            var ProductioncaseAssignment = await _cbeContext.ProductionCaseAssignments.Where(res => res.ProductionCapacityId == ProductionCapacityId && res.UserId == EvaluatorUserID).FirstOrDefaultAsync();
            ProductioncaseAssignment.Status = "Remark";
            _cbeContext.Update(ProductioncaseAssignment);

            var Production = await _cbeContext.ProductionCapacities.Where(res => res.Id == ProductionCapacityId).FirstOrDefaultAsync();
            if (RemarkType == "Verfication")
            {
                Production.CurrentStatus = "Remark Verfication";
            }
            else
            {
                Production.CurrentStatus = "Remark Justfication";
            }
            if (uploadFile.File != null)
            {
                uploadFile.CaseId = Production.PCECaseId;
                await _uploadFileService.CreateUploadFile(base.GetCurrentUserId(), uploadFile);
            }
            Production.CurrentStage = "Maker Officer";
            _cbeContext.Update(Production);
            _cbeContext.SaveChanges();

            //return RedirectToAction("MyCompleteCases", "Case");
            //just for opration 
            return RedirectToAction("PCENewCases", "PCECase");
        }

        [HttpGet]
        public async Task<IActionResult> GetRemarkProducts(Guid CaseId)
        {
            var collaterals = await _productionCapacityServices.GetRemarkProducts(base.GetCurrentUserId(), CaseId);
            string jsonData = JsonConvert.SerializeObject(collaterals);
            return Content(jsonData, "application/json");
        }


        [HttpGet]
        public async Task<IActionResult> GetRMCompleteCollaterals(Guid PCECaseId)
        {
           /// Guid PCECaseId = Guid.Parse("ced1b3c4-7ee3-4219-a62a-72cb7475f304");
            var collaterals = await _productionCapacityServices.GetRmComCollaterals(PCECaseId);
            string jsonData = JsonConvert.SerializeObject(collaterals);
            return Content(jsonData, "application/json");
        }

    }
}
