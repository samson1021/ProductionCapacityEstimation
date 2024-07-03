using mechanical.Services.CaseAssignmentService;
using mechanical.Services.CaseServices;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace mechanical.Controllers
{
    public class CheckerController : Controller
    {
        private readonly ICaseService _caseService;
        private readonly ICaseAssignmentService _caseAssignmentService;
        public CheckerController(ICaseService caseService, ICaseAssignmentService caseAssignment)
        {
            _caseService = caseService;
            _caseAssignmentService = caseAssignment;
        }

        [HttpGet]
        public IActionResult MyCases()
        {

            return View();
        }

        //[HttpGet]
        //public async Task<IActionResult> GetMyCases()
        //{
        //    var myCase = await _caseService.GetCheckerNewCases();
        //    if (myCase == null) { return BadRequest("Unable to load case"); }
        //    string jsonData = JsonConvert.SerializeObject(myCase);
        //    return Content(jsonData, "application/json");
        //}
        //[HttpGet]
        //public async Task<IActionResult> MyCase(Guid Id)
        //{
        //    var myCase = await _caseService.GetRmNewCase(Id);

        //    if (MyCase == null)
        //    {
        //        return RedirectToAction("MyCase");
        //    }
        //    ViewData["case"] = myCase;
        //    return View();
        //}
        //[HttpPost]
        //public async Task<IActionResult> AssignTeamleader(string selectedCollateralIds, string employeeId)
        //{
        //    await _caseAssignmentService.AssignMakerTeamleader(selectedCollateralIds, employeeId);
        //    var response = new { message = "Collaterals assigned successfully" };
        //    return Ok(response);
        //}

        //[HttpGet]
        //public IActionResult MyPendingCases()
        //{
        //    return View();
        //}

        //[HttpGet]
        //public async Task<IActionResult> GetMyPendingCases()
        //{
        //    var myCase = await _caseService.GetMmPendingCases();
        //    string jsonData = JsonConvert.SerializeObject(myCase);
        //    return Content(jsonData, "application/json");
        //}
    }
}
