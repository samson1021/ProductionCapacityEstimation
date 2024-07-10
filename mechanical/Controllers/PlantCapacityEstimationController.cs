
using mechanical.Models.PCE.Dto.PlantCapacityEstimationDto;

using mechanical.Services.PCE.PCECollateralService;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using mechanical.Services.PCE.UploadFileService;
using mechanical.Models.Dto.CollateralDto;
using mechanical.Models.PCE.Dto.ProductionCapacityDto;


namespace mechanical.Controllers
{
    public class PlantCapacityEstimationController : BaseController
    {
        private readonly IPCECollateralService _pCECollateralService;
        private readonly IPCEUploadFileService _uploadFileService;

        public PlantCapacityEstimationController(IPCECollateralService pCECollateralService, IPCEUploadFileService pCEUploadFileService)
        {
            _pCECollateralService = pCECollateralService;
            _uploadFileService= pCEUploadFileService;
        }

        [HttpPost]
        public async Task<IActionResult> Create(PlantCapacityEstimationPostDto pceDto)
        {
            if (ModelState.IsValid)
            {
                pceDto.CreatedById = base.GetCurrentUserId();
                await _pCECollateralService.CreatePCECollateral(pceDto);
                var response = new { message = "Plant PCE Created Successfully" };
                return Ok(response);
            }
            return BadRequest();
        }

        [HttpPost]
        public async Task<IActionResult> PlantCreate(PlantPostDto pceDto)
        {
            if (ModelState.IsValid)
            {
                pceDto.CreatedById = base.GetCurrentUserId();
                //await _pCECollateralService.CreatePCECollateral(pceDto);
                var response = new { message = "Plant PCE Created Successfully" };
                return Ok(response);
            }
            return BadRequest();
        }


        [HttpGet]
        public async Task<IActionResult> GetPCECollaterals(Guid CaseId)
        {
            var collaterals = await _pCECollateralService.GetCollaterals(CaseId);
            string jsonData = JsonConvert.SerializeObject(collaterals);
            return Content(jsonData, "application/json");
        }

        [HttpGet]
        public async Task<IActionResult> PCEDetail(Guid id )
        {
            var response = await _pCECollateralService.GetCollateral(id);
            var file = await _uploadFileService.GetUploadFileByCollateralId(id);
            ViewData["collateralFiles"] = file;
            return View(response);
        }


        [HttpGet]
        public async Task<IActionResult> PCEEdit(Guid id)
        {
            var response = await _pCECollateralService.GetCollateral(id);
            var file = await _uploadFileService.GetUploadFileByCollateralId(id);
            ViewData["collateralFiles"] = file;
            if (response == null) { return RedirectToAction("PCENewCases"); }
            return View(response);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PCEEdit(Guid id, PlantCapacityEstimationEditPostDto collateralPostDto)
        {
            if (!ModelState.IsValid)
            {
                // Add the validation errors to the ViewData so they can be displayed in the view
                ViewData["ValidationErrors"] = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                return View();
            }

            var collateral = await _pCECollateralService.EditCollateral(base.GetCurrentUserId(), id, collateralPostDto);
            return RedirectToAction("PCEDetail", "PCECase", new { Id = collateral.PCECaseId });
        }


        [HttpPost]
        public async Task<ActionResult> DeleteCollateralFile(Guid Id)
        {
            if (await _pCECollateralService.DeleteCollateralFile(base.GetCurrentUserId(), Id))
            {
                return Ok();
            }
            return BadRequest();
        }


        [HttpPost]
        public async Task<IActionResult> DeleteCocllateral(Guid id)
        {

            if (await _pCECollateralService.DeleteCocllateral(base.GetCurrentUserId(), id))

            {
                return Ok();
            }
            return BadRequest();
        }



        [HttpPost]
        public async Task<ActionResult> UploadCollateralFile(IFormFile BussinessLicence, Guid pcecaseId,string DocumentCatagory, Guid caseId)
        {
            if (await _pCECollateralService.UploadCollateralFile(base.GetCurrentUserId(), BussinessLicence, pcecaseId, DocumentCatagory, caseId))
            {
                return Ok();
            }
            return BadRequest();
        }

    }
}
