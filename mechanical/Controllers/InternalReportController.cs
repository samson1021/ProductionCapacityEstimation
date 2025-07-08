
using mechanical.Data;
using mechanical.Services.InternalReportService;
using mechanical.Services.MailService;
using mechanical.Services.MMCaseService;
using mechanical.Services.UserService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace mechanical.Controllers
{
    [Authorize(Roles = "Maker Manager,District Valuation Manager ,Maker Officer, Maker TeamLeader, Relation Manager,Checker Manager, Checker TeamLeader, Checker Officer")]
    public class InternalReportController : BaseController
    {

        private readonly CbeContext _cbeContext;
        private readonly IUserService _UserService;
        private readonly IMailService _mailService;
        private readonly IInternalReportService _internalReportService;

        public InternalReportController(CbeContext cbeContext,  IUserService UserService, IInternalReportService internalReportService)
        {
            _cbeContext = cbeContext;
            _internalReportService = internalReportService;
            _UserService = UserService;
        }

        [HttpGet]
        public async Task<IActionResult> Detail()
        {
            ViewData["CurrentUser"] = await _UserService.GetUserById(base.GetCurrentUserId());
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> CaseDetail()
        {
            ViewData["CurrentUser"] = await _UserService.GetUserById(base.GetCurrentUserId());
            return View();
        }
        //[HttpGet]
        //public async Task<IActionResult> GetInternalPCECaseReport()
        //{
        //    var myCase = await _internalReportService.GetInternalPCECaseReport(GetCurrentUserId());
        //    //if (myCase == null) { return BadRequest("Unable to load case"); }
        //    string jsonData = JsonConvert.SerializeObject(myCase);
        //    return Content(jsonData, "application/json");
        //}
        [HttpGet]
        public async Task<IActionResult> GetCaseInternalReport()
        {
            var myCase = await _internalReportService.GetInternalCaseReport(GetCurrentUserId());
            if (myCase.DistinctCases == null && myCase.AllProductionCapacities == null)
            {
                return BadRequest("Unable to load case");
            }
            //return (DistinctCases: distinctCaseDtos, AllProductionCapacities: allCollateralDtos);

            var result = new
            {
                DistinctCases = myCase.DistinctCases,
                AllProductionCapacities = myCase.AllProductionCapacities
            };
            return Ok(result);
        }
        [HttpGet]
        public async Task<IActionResult> GetPCECaseInternalReport()
        {
            var myCase = await _internalReportService.GetInternalPCECaseReport(GetCurrentUserId());
            if (myCase.DistinctCases == null && myCase.AllProductionCapacities == null)
            {
                return BadRequest("Unable to load case");
            }
            var result = new
            {
                DistinctCases = myCase.DistinctCases,
                AllProductionCapacities = myCase.AllProductionCapacities
            };
            return Ok(result);
        }
    }
}
