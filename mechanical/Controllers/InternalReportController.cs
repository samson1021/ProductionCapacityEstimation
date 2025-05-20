
using mechanical.Data;
using mechanical.Services.InternalReportService;
using mechanical.Services.MailService;
using mechanical.Services.MMCaseService;
using mechanical.Services.UserService;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace mechanical.Controllers
{
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
        public async Task<IActionResult> GetCaseReport()
        {
            var myCase = await _internalReportService.GetCaseReport(GetCurrentUserId());
            if (myCase == null) { return BadRequest("Unable to load case"); }
            string jsonData = JsonConvert.SerializeObject(myCase);
            return Content(jsonData, "application/json");
        }
    }
}
