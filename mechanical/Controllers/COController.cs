using mechanical.Models.Enum;
using mechanical.Models;
using mechanical.Services.CaseServices;
using mechanical.Services.CollateralService;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using mechanical.Services.MMCaseService;
using mechanical.Services.CaseTerminateService;
using mechanical.Services.CaseScheduleService;
using mechanical.Services.UploadFileService;

namespace mechanical.Controllers
{
    public class COController : BaseController
    {
        private readonly ICaseService _caseService;
        private readonly ICollateralService _collateralService;
        private readonly IMMCaseService _mOCaseService;
        private readonly ICMCaseService _CoCaseService;
        private readonly ICaseScheduleService _caseScheduleService;
        private readonly ICaseTerminateService _caseTermnateService;
        private readonly IUploadFileService _uploadFileService;

        public COController(ICaseService caseService, ICaseTerminateService caseTermnateService, IUploadFileService uploadFileService, ICaseScheduleService caseScheduleService, ICMCaseService CoCaseService, IMMCaseService mOCaseService, ICollateralService collateralService)
        {
            _caseService = caseService;
            _collateralService = collateralService;
            _mOCaseService = mOCaseService;
            _caseScheduleService = caseScheduleService;
            _CoCaseService = CoCaseService;
            _caseTermnateService = caseTermnateService;
            _uploadFileService = uploadFileService;
        }

        [HttpGet]
        public IActionResult MyCases()
        {

            return View();
        }

        public IActionResult RemarkCases()
        {
            return View();
        }

        public async Task<IActionResult> RemarkCase(Guid Id)
        {
            var loanCase = await _caseService.GetCaseDetail(Id);
            var caseSchedule = await _caseScheduleService.GetCaseSchedules(Id);
            if (loanCase == null) { return RedirectToAction("NewCases"); }
            ViewData["case"] = loanCase;
            ViewData["CaseSchedule"] = caseSchedule;
            ViewData["Id"] = base.GetCurrentUserId();
            var moFile = await _uploadFileService.GetMoUploadFile(Id);
            ViewData["moFile"] = moFile;
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> GetRemarkedCases()
        {
            var myCase = await _CoCaseService.GetCoRemarkedCases(GetCurrentUserId());
            if (myCase == null) { return BadRequest("Unable to load case"); }
            string jsonData = JsonConvert.SerializeObject(myCase);
            return Content(jsonData, "application/json");
        }
        [HttpGet]
        public async Task<IActionResult> GetMyCases()
        {
            var myCase = await _mOCaseService.GetMMNewCases(GetCurrentUserId());
            if (myCase == null) { return BadRequest("Unable to load case"); }
            string jsonData = JsonConvert.SerializeObject(myCase);
            return Content(jsonData, "application/json");
        }

        [HttpGet]
        public async Task<IActionResult> Evaluation(Guid Id)
        {
            var collateral = await _collateralService.GetCollateral(base.GetCurrentUserId(), Id);
            if (collateral.Category == EnumHelper.GetEnumDisplayName(MechanicalCollateralCategory.MOV))
            {
                return RedirectToAction("GetEvaluatedMoterVehicle", "MotorVehicle", new { Id = Id });
            }
            else if (collateral.Category == EnumHelper.GetEnumDisplayName(MechanicalCollateralCategory.CMAMachinery))
            {
                return RedirectToAction("GetEvaluatedconstMngAgriMachinery", "ConstMngAgrMachinery", new { Id = Id });
            }
            else if (collateral.Category == EnumHelper.GetEnumDisplayName(MechanicalCollateralCategory.IBFEqupment))
            {
                return RedirectToAction("GetEvaluatedIndBldgFacilityEquipment", "IndBldgFacilityEquipment", new { Id = Id });
            }
            string jsonData = JsonConvert.SerializeObject(collateral);
            return Content(jsonData, "application/json");
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


    }
}
