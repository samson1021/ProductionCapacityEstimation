using mechanical.Models.Entities;
using mechanical.Services.PCE.PCECaseTimeLineService;
using Microsoft.AspNetCore.Mvc;

namespace mechanical.Controllers
{
    public class PCECaseTimeLineController : Controller
    {
        private readonly IPCECaseTimeLineService _PCECaseTimeLineService;

        public PCECaseTimeLineController(IPCECaseTimeLineService pCECaseTimeLineService)
        {
            _PCECaseTimeLineService = pCECaseTimeLineService;
        }

        //public IActionResult Index(Guid CaseId)
        //{
        //    return View();
        //}

        public async Task<IActionResult> Index(Guid CaseId)
        {
            var caseTimeline = await _PCECaseTimeLineService.GetPCECaseTimeLines(CaseId);
            return View(caseTimeline);
        }

    }
}
