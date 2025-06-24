using mechanical.Data;
using mechanical.Models.Dto.Correction;
using mechanical.Models.Entities;
using mechanical.Services.CaseAssignmentService;
using mechanical.Services.CaseServices;
using mechanical.Services.CorrectionServices;
using Microsoft.AspNetCore.Mvc;

namespace mechanical.Controllers
{
    public class CorrectionController : Controller
    {

        private readonly ICaseService _caseService;
        private readonly ICorrectionService _CorrectionService;
        private readonly CbeContext _cbeContext;
        public CorrectionController(ICaseService caseService, CbeContext cbeContext, ICorrectionService correctionService)
        {
            _caseService = caseService;
            _cbeContext = cbeContext;
            _CorrectionService = correctionService;
        }


        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public async Task< IActionResult> Create(CorrectionPostDto correctionDto, string Controller, string Action)
        {
            var collateralId= correctionDto.CollateralID;

            if (ModelState.IsValid)
            {
               var correction = await _CorrectionService.CreateCorrection(correctionDto);
                return Ok();
            }
            
            return RedirectToAction(Action, new { Id = collateralId });
        }

        //public async Task<IActionResult> GetComment(Guid CollateralId)
        //{

        //    if (ModelState.IsValid)
        //    {
        //        var correction = await _CorrectionService.CreateCorrection(correctionDto);
        //        return Ok();
        //    }

        //    return RedirectToAction(Action, new { Id = collateralId });
        //}
        [HttpPost]
        public async Task<IActionResult> ReplayForCheckerCorrections(CorrectionPostDto correctionDto, string Controller, string Action)
        {
            var collateralId = correctionDto.CollateralID;

            if (ModelState.IsValid)
            {
                var correction = await _CorrectionService.CreateCorrection(correctionDto);
                return Ok();
            }

            return RedirectToAction(Action, new { Id = collateralId });
        }

    }
}
