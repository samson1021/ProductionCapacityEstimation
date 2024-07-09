using mechanical.Services.CaseTimeLineService;
using mechanical.Services.CollateralService;
using mechanical.Services.UploadFileService;
using Microsoft.AspNetCore.Mvc;

namespace mechanical.Controllers
{
    public class CaseTimeLineController : Controller
    {
        private readonly ICaseTimeLineService _caseTimelineService;
        public CaseTimeLineController(ICaseTimeLineService collateralService)
        {
            _caseTimelineService = collateralService;
        }
        public async Task<IActionResult> Index(Guid CaseId)
        {
            var caseTimeline = await _caseTimelineService.GetCaseTimeLines(CaseId);
            return View(caseTimeline);
        }
    }
}
