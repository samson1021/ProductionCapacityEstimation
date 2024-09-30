using Microsoft.AspNetCore.Mvc;

using mechanical.Models.Entities;
using mechanical.Services.PCE.PCECaseTimeLineService;

namespace mechanical.Controllers
{
    public class PCECaseTimeLineController : Controller
    {
        private readonly IPCECaseTimeLineService _PCECaseTimeLineService;

        public PCECaseTimeLineController(IPCECaseTimeLineService pCECaseTimeLineService)
        {
            _PCECaseTimeLineService = pCECaseTimeLineService;
        }

        public async Task<IActionResult> Index(Guid PCECaseId)
        {
            var caseTimeline = await _PCECaseTimeLineService.GetPCECaseTimeLines(PCECaseId);
            return View(caseTimeline);
        }
    }
}
