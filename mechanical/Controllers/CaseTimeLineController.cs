using mechanical.Services.CaseTimeLineService;
using mechanical.Services.CollateralService;
using mechanical.Services.UploadFileService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace mechanical.Controllers
{
    [Authorize(Roles = "Maker Manager,District Valuation Manager ,Maker Officer, Maker TeamLeader, Relation Manager,Checker Manager, Checker TeamLeader, Checker Officer")]
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
