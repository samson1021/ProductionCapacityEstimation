using Newtonsoft.Json;
using Microsoft.AspNetCore.Mvc;

using mechanical.Services.CaseServices;
using mechanical.Services.MMCaseService;
using mechanical.Services.CaseScheduleService;
using mechanical.Services.CaseTerminateService;
using mechanical.Services.CaseAssignmentService;
using mechanical.Services.UploadFileService;
using mechanical.Data;
using mechanical.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace mechanical.Controllers
{
    public class MTLCaseController : BaseController
    {
        private readonly ICaseService _caseService;
        private readonly IMMCaseService _mmCaseService;
        private readonly ICaseScheduleService _caseScheduleService;
        private readonly ICaseTerminateService _caseTermnateService;
        private readonly ICaseAssignmentService _caseAssignmentService;
        private readonly IUploadFileService _uploadFileService;
        private readonly CbeContext _cbeContext;

        public MTLCaseController(ICaseService caseService, IUploadFileService uploadFileService, ICaseTerminateService caseTermnateService, ICaseScheduleService caseScheduleService, ICaseAssignmentService caseAssignment, IMMCaseService mMCaseService, CbeContext cbeContext)
        {
            _caseService = caseService;
            _caseAssignmentService = caseAssignment;
            _mmCaseService = mMCaseService;
            _caseScheduleService = caseScheduleService;
            _caseTermnateService = caseTermnateService;
            _uploadFileService = uploadFileService;
            _cbeContext = cbeContext;
        }

        [HttpGet]
        public IActionResult MyCases()
        {

            return View();
        }
        [HttpGet]
        public IActionResult MyCompletedCases()
        {

            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetMyCases()
        {
            var myCase = await _mmCaseService.GetMMNewCases(GetCurrentUserId());
            if (myCase == null) { return BadRequest("Unable to load case"); }
            string jsonData = JsonConvert.SerializeObject(myCase);
            return Content(jsonData, "application/json");
        }
        [HttpGet]
        public async Task<IActionResult> GetMyCompletedCases()
        {
            var myCase = await _mmCaseService.GetMTLCompletedCases(GetCurrentUserId());
            if (myCase == null) { return BadRequest("Unable to load case"); }
            string jsonData = JsonConvert.SerializeObject(myCase);
            return Content(jsonData, "application/json");
        }
        [HttpGet]
        public async Task<IActionResult> GetMyAssignmentCases()
        {
            var myCase = await _mmCaseService.GetMyAssignmentCases(GetCurrentUserId());
            if (myCase == null) { return BadRequest("Unable to load case"); }
            string jsonData = JsonConvert.SerializeObject(myCase);
            return Content(jsonData, "application/json");
        }
        public IActionResult MyAssignments()
        {
            return View();
        }
        public async Task<IActionResult> MyAssignment(Guid Id)
        {
            var loanCase = await _caseService.GetCaseDetail(Id);
            if (loanCase == null) { return RedirectToAction("NewCases"); }
            ViewData["case"] = loanCase;
            var moFile = await _uploadFileService.GetMoUploadFile(Id);
            ViewData["moFile"] = moFile;
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> MyCase(Guid Id)
        {
            var loanCase = await _caseService.GetCaseDetail(Id);
            var caseSchedule = await _caseScheduleService.GetCaseSchedules(Id);
            var caseTerminate = await _caseTermnateService.GetCaseTerminates(Id);
            ViewData["caseTerminate"] = caseTerminate;
            if (loanCase == null) { return RedirectToAction("NewCases"); }
            ViewData["case"] = loanCase;
            ViewData["CaseSchedule"] = caseSchedule;
            var moFile = await _uploadFileService.GetMoUploadFile(Id);
            ViewData["moFile"] = moFile;
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> MyCompleteCase(Guid Id)
        {
            var loanCase = await _caseService.GetCaseDetail(Id);
            var caseSchedule = await _caseScheduleService.GetCaseSchedules(Id);
            //var motorvechiel = await _cbeContext.MotorVehicles.Where(res => res.Collaterial.CaseId == CaseId).ToListAsync();
            if (loanCase == null) { return RedirectToAction("GetCompleteCases"); }
            ViewData["case"] = loanCase;
            ViewData["CaseSchedule"] = caseSchedule;
            ViewData["Id"] = base.GetCurrentUserId();
            List<MotorVehicle> motorVehicle = null;
            try
            {
                motorVehicle = await _cbeContext.MotorVehicles.Where(res => res.Collateral.CaseId == Id && res.Collateral.CurrentStatus == "Complete" && res.Collateral.CurrentStage == "Checker Officer").ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while retrieving motor vehicles: {ex.Message}");
            }
            List<ConstMngAgrMachinery> conMngAgr = null;
            try
            {
                conMngAgr = await _cbeContext.ConstMngAgrMachineries.Where(res => res.Collateral.CaseId == Id && res.Collateral.CurrentStatus == "Complete" && res.Collateral.CurrentStage == "Checker Officer").ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while retrieving motor vehicles: {ex.Message}");
            }
            List<IndBldgFacilityEquipment> indBldgFacEq = null;
            try
            {
                indBldgFacEq = await _cbeContext.IndBldgFacilityEquipment.Where(res => res.Collateral.CaseId == Id && res.Collateral.CurrentStatus == "Complete" && res.Collateral.CurrentStage == "Checker Officer").ToListAsync();
            }
            catch (Exception ex)
            {
                // Handle the exception (e.g., log the error, display a message, etc.)
                Console.WriteLine($"An error occurred while retrieving motor vehicles: {ex.Message}");
            }
            ViewData["motorVehicle"] = motorVehicle;
            ViewData["indBldgFacEq"] = indBldgFacEq;
            ViewData["conMngAgr"] = conMngAgr;
            var moFile = await _uploadFileService.GetMoUploadFile(Id);
            ViewData["moFile"] = moFile;
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> AssignMakerOfficer(string selectedCollateralIds, string employeeId)
        {

            await _caseAssignmentService.AssignMakerTeamleader(base.GetCurrentUserId(), selectedCollateralIds, employeeId);
            var response = new { message = "Collaterals assigned successfully" };
            return Ok(response);
        }
        [HttpPost]
        public async Task<IActionResult> ReAssignMakerOfficer(string selectedCollateralIds, string employeeId)
        {
            await _caseAssignmentService.ReAssignMakerTeamleader(base.GetCurrentUserId(), selectedCollateralIds, employeeId);
            var response = new { message = "Collaterals assigned successfully" };
            return Ok(response);
        }



        //[HttpGet]
        //public IActionResult MyPendingCases()
        //{
        //    return View();
        //}

        //[HttpGet]
        //public async Task<IActionResult> GetMyPendingCases()
        //{
        //    var myCase = await _caseService.GetMTLPendingCases();
        //    string jsonData = JsonConvert.SerializeObject(myCase);
        //    return Content(jsonData, "application/json");
        //}
    }
}