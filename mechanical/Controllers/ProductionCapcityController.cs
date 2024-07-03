using mechanical.Controllers.PCE;
using mechanical.Data;
using mechanical.Models.Enum;
using mechanical.Models;
using mechanical.Models.PCE.Entities;
using mechanical.Services.CaseServices;
using mechanical.Services.PCE.PCECaseService;
using mechanical.Services.ProductionCaseService;
using mechanical.Services.UploadFileService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using mechanical.Services.ProductionCapacityServices;
using mechanical.Models.Dto.CollateralDto;
using mechanical.Models.Dto.ProductionCapacityDto;
using mechanical.Models.Enum.ProductionCapcityEstimation;
using mechanical.Models.Dto.UploadFileDto;
using mechanical.Models.Entities;
using mechanical.Services.ProductionUploadFileService;
using mechanical.Models.Dto.ProductionUploadFileDto;

namespace mechanical.Controllers
{
    public class ProductionCapcityController :BaseController
    {
        private readonly IProductionCaseService _productionCaseService;
        private readonly IProductionCapacityServices _productionCapacityServices;
        private readonly ILogger<ProductionCapcityController> _logger;
        private readonly CbeContext _cbeContext;
        private readonly IProductionUploadFileService _productionUploadFileService;

        public ProductionCapcityController(CbeContext cbeContext, IProductionCaseService productionCaseService, IProductionCapacityServices productionCapacityServices, IProductionUploadFileService productionUploadFileService)
        {
            _cbeContext = cbeContext;
            _productionCapacityServices = productionCapacityServices;
            _productionCaseService = productionCaseService;
            _productionUploadFileService = productionUploadFileService;
            

        }
  
        public IActionResult Index()
        {
            return View();
        }


        [HttpPost]
       [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Guid ProductionCaseId, ProductionPostDto productionDto)
        {
            if (ModelState.IsValid)
            {
                await _productionCapacityServices.CreateProductionCapacity(base.GetCurrentUserId(), ProductionCaseId, productionDto);
                var response = new { message = "PCE created successfully" };
                return Ok(response);
            }
            return BadRequest();
           
        }
       
        [HttpGet]
        public async Task<IActionResult> GetProductions(Guid ProductionCaseId)
        {
            var products = await _productionCapacityServices.GetProductions(ProductionCaseId);
            string jsonData = JsonConvert.SerializeObject(products);
            return Content(jsonData, "application/json");

        }

        [HttpGet]
        public async Task<IActionResult> Detail(Guid id)
        {
            var response = await _productionCapacityServices.GetProduction(base.GetCurrentUserId(), id);
            var loanCase = await _productionCaseService.GetProductionCaseDetail(id);

            var restimation = await _cbeContext.ProductionReestimations.Where(res => res.ProductionCapacityId == id).FirstOrDefaultAsync();
            if (restimation != null)
            {
                ViewData["restimation"] = restimation;
            }

            if (response == null) { return RedirectToAction("PNewCases"); }
            var file = await _productionUploadFileService.GetUploadFileByProductionCapacityId(id);
            var rejectedProduction = await _cbeContext.ProductionRejects.Where(res => res.ProductionCapacityId == id).FirstOrDefaultAsync();
            var remarkTypeProduction = await _cbeContext.ProductionCapacities.Where(res => res.Id == id).FirstAsync();
            var productionById= await _productionCapacityServices.GetProductionCapacityById(id);
            
            if (rejectedProduction != null)
            {
                var user = await _cbeContext.CreateUsers.Include(res => res.Role).FirstOrDefaultAsync(rea => rea.Id == rejectedProduction.RejectedBy);
                ViewData["user"] = user;

            }
           
            ViewData["Prvaluation"] = productionById;
            ViewData["case"] = loanCase;
            ViewData["productionFiles"] = file;
            ViewData["rejectedCollateral"] = rejectedProduction;
            ViewData["CurrentUserId"] = base.GetCurrentUserId();
            var userId = base.GetCurrentUserId();
            var UserForRole = await _cbeContext.CreateUsers.Where(res => res.Id == userId).FirstOrDefaultAsync();
            var role = await _cbeContext.CreateRoles.Where(res => res.Id == UserForRole.RoleId).FirstOrDefaultAsync();
            ViewData["loggedRole"] = role;
            ViewData["remarkTypeCollateral"] = remarkTypeProduction;
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
                return RedirectToAction("Detail", "ProductionCase", new { Id = collateral.ProductionCaseId });
            }
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            var response = await _productionCapacityServices.GetProduction(base.GetCurrentUserId(), id);
            var file = await _productionUploadFileService.GetUploadFileByProductionCapacityId(id);
            ViewData["productionFiles"] = file;
            if (response == null) { return RedirectToAction("PNewCases"); }
            return View(response);
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
        public async Task<ActionResult> UploadProductionFile(IFormFile ProductionBussinessLicence, Guid ProductionCaseId, string DocumentCatagory)
        {
            if (await _productionCapacityServices.UploadProductionFile(base.GetCurrentUserId(), ProductionBussinessLicence, ProductionCaseId, DocumentCatagory))
            {
                return Ok();
            }
            return BadRequest();
        }



        [HttpPost]
        public async Task<IActionResult> handleProductionRemark(Guid ProductionCapacityId, Guid EvaluatorUserID, String RemarkType, CreateProductionFileDto uploadFile, Guid CheckerUserID)
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
                uploadFile.ProductionCaseId = Production.ProductionCaseId;
                await _productionUploadFileService.CreateProductionUploadFile(base.GetCurrentUserId(), uploadFile);
            }
            Production.CurrentStage = "Maker Officer";
            _cbeContext.Update(Production);
            _cbeContext.SaveChanges();

            //return RedirectToAction("MyCompleteCases", "Case");
                //just for opration 
            return RedirectToAction("PNewCases", "ProductionCase");
        }

        [HttpGet]
        public async Task<IActionResult> GetRemarkProducts(Guid CaseId)
        {
            var collaterals = await _productionCapacityServices.GetRemarkProducts(base.GetCurrentUserId(), CaseId);
            string jsonData = JsonConvert.SerializeObject(collaterals);
            return Content(jsonData, "application/json");
        }




    }
}
