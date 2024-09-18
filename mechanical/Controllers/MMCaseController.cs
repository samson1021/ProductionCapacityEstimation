using mechanical.Services.CaseAssignmentService;
using mechanical.Services.CaseScheduleService;
using mechanical.Services.CaseServices;
using mechanical.Services.CaseTerminateService;
using mechanical.Services.MMCaseService;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using mechanical.Services.PCE.PCECaseService;
using mechanical.Services.PCE.ProductionCapacityServices;
using mechanical.Services.PCE.ProductionCaseScheduleService;
using mechanical.Services.PCE.ProductionCaseAssignmentServices;
using mechanical.Services.PCE.MOPCECaseService;
using mechanical.Services.MailService;
using mechanical.Services.PCE.PCECaseScheduleService;
using mechanical.Services.PCE.PCECaseTerminateService;
using mechanical.Models.PCE.Dto.PCECaseCommentDto;
using mechanical.Services.PCE.PCECaseCommentService;

namespace mechanical.Controllers
{
    public class MMCaseController : BaseController
    {
        private readonly ICaseService _caseService;
        private readonly IMMCaseService _mMCaseService;
        private readonly ICaseAssignmentService _caseAssignmentService;
        private readonly ICaseScheduleService _caseScheduleService;
        private readonly ICaseTerminateService _caseTermnateService;
        private readonly IPCECaseService _PCECaseService;
        private readonly IProductionCaseScheduleService _ProductionCaseScheduleService;
        private readonly IProductionCaseAssignmentServices _productionCaseAssignmentServices;
        private readonly IMOPCECaseService _MOPCECaseService;       
        private readonly IPCECaseTerminateService _PCECaseTerminateService;
        private readonly IPCECaseCommentService _PCEcaseCommentService;




        public MMCaseController(ICaseService caseService, IPCECaseService PCECaseService, IProductionCaseScheduleService ProductionCaseScheduleService, IProductionCaseAssignmentServices productionCaseAssignmentServices, IMOPCECaseService MOPCECaseService, ICaseTerminateService caseTerminateService,ICaseScheduleService caseScheduleService,IMMCaseService mMCaseService , ICaseAssignmentService caseAssignment, IPCECaseTerminateService PCECaseTerminateService, IPCECaseCommentService PCEcaseCommentService)
        {
            _caseService = caseService;
            _caseAssignmentService = caseAssignment;
            _mMCaseService = mMCaseService; 
            _caseScheduleService = caseScheduleService;
            _caseTermnateService = caseTerminateService;
            _PCECaseService = PCECaseService;
            _ProductionCaseScheduleService = ProductionCaseScheduleService;
            _productionCaseAssignmentServices = productionCaseAssignmentServices;
            _MOPCECaseService = MOPCECaseService;
            _PCECaseTerminateService = PCECaseTerminateService;
            _PCEcaseCommentService = PCEcaseCommentService;
        }

        [HttpGet]
        public IActionResult MyCases()
        {

            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetMyCases()
        {
            var myCase = await _mMCaseService.GetMMNewCases(GetCurrentUserId());
            if (myCase == null) { return BadRequest("Unable to load case"); }
            string jsonData = JsonConvert.SerializeObject(myCase);
            return Content(jsonData, "application/json");
        }
      
        [HttpGet]
        public async Task<IActionResult> GetMyAssignmentCases()
        {
            var myCase = await _mMCaseService.GetMyAssignmentCases(GetCurrentUserId());
            if (myCase == null) { return BadRequest("Unable to load case"); }
            string jsonData = JsonConvert.SerializeObject(myCase);
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
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AssignTeamleader(string selectedCollateralIds, string employeeId)
        {
            await _caseAssignmentService.AssignMakerTeamleader(base.GetCurrentUserId(),selectedCollateralIds, employeeId);
            var response = new { message = "Collaterals assigned successfully" };
            return Ok(response); 
        }
        
        [HttpPost]
        public async Task<IActionResult> ReAssignTeamleader(string selectedCollateralIds, string employeeId)
        {
            await _caseAssignmentService.ReAssignMakerTeamleader(base.GetCurrentUserId(), selectedCollateralIds, employeeId);
            var response = new { message = "Collaterals re-assigned successfully" };
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
        //    var myCase = await _mMCaseService.GetMmPendingCases();
        //    string jsonData = JsonConvert.SerializeObject(myCase);
        //    return Content(jsonData, "application/json");
        //}
        //[HttpGet]
        //public async Task<IActionResult> GetDashboardCaseCount()
        //{
        //    var myCase = await _mMCaseService.GetDashboardCaseCount();
        //    string jsonData = JsonConvert.SerializeObject(myCase);
        //    return Content(jsonData, "application/json");
        //}

        //// PCE Cases /////////

        [HttpPost]
        public async Task<IActionResult> PCEAssignTeamleader(string selectedPCEIds, string employeeId)
        {
            await _productionCaseAssignmentServices.AssignProductionMakerTeamleader(base.GetCurrentUserId(), selectedPCEIds, employeeId);
            var response = new { message = "Productions assigned to MTL successfully" };
            return Ok(response);
        }
        
        [HttpPost]
        public async Task<IActionResult> PCEReAssignTeamleader(string selectedPCEIds, string employeeId)
        {
            await _productionCaseAssignmentServices.ReAssignProductionMakerTeamleader(base.GetCurrentUserId(), selectedPCEIds, employeeId);
            var response = new { message = "Productions re-assigned to MTL successfully" };
            return Ok(response);
        }

        [HttpGet]
        public IActionResult MyPCECases(string Status = "New")
        {
            ViewData["Title"] = Status + " PCE Cases";
            ViewBag.Url = "/MMCase/GetMyPCECases";
            ViewBag.Status = Status;
            return View("PCECases");
        }

        [HttpGet]
        public async Task<IActionResult> GetMyPCECases(string Status)
        {
            var pceCases = await _MOPCECaseService.GetPCECases(base.GetCurrentUserId(), Status);
            if (pceCases == null)
            {
                return BadRequest("Unable to load {Status} PCE Cases");
            }
            string jsonData = JsonConvert.SerializeObject(pceCases);
            return Content(jsonData, "application/json");
        }

        [HttpGet]
        public async Task<IActionResult> GetPCEs(Guid PCECaseId, string Status)
        {
            var productions = await _MOPCECaseService.GetPCEs(base.GetCurrentUserId(), PCECaseId, Status: Status);

            if (productions == null)
            {
                return BadRequest("Unable to load {Status} PCEs with PCECase ID: {PCECaseId}");
            }

            string jsonData = JsonConvert.SerializeObject(productions);

            return Content(jsonData, "application/json");
        }

        [HttpGet]
        public async Task<IActionResult> GetMyDashboardPCECasesCount()
        {
            var pceCasesCount = await _MOPCECaseService.GetDashboardPCECasesCount(base.GetCurrentUserId());
            string jsonData = JsonConvert.SerializeObject(pceCasesCount);
            return Content(jsonData, "application/json");
        }
        
        [HttpGet]
        public async Task<IActionResult> PCECaseDetail(Guid Id, string Status)
        {

            var pceCase = await _MOPCECaseService.GetPCECase(base.GetCurrentUserId(), Id);
            if (pceCase == null)
            {
                return RedirectToAction("MyPCECases");
            }
            // var PCECaseTerminate = await _PCECaseTerminateService.GetCaseTerminates(Id);
            var ProductionCaseSchedule = await _ProductionCaseScheduleService.GetProductionCaseSchedules(Id);
            
            ViewData["CurrentUser"] = await _MOPCECaseService.GetUser(base.GetCurrentUserId());
            // ViewData["CurrentUserRole"] = pceDetail.CurrentUserRole;
            ViewData["PCECaseId"] = pceCase.Id;
            ViewData["PCECase"] = pceCase;
            // ViewData["PCECaseTerminate"] = PCECaseTerminate;
            ViewData["ProductionCaseSchedule"] = ProductionCaseSchedule;
            ViewData["Title"] = Status + " PCE Case Details";             
            ViewBag.Status = Status;

            return View();
        }


        
        [HttpGet]
        public async Task<IActionResult> GetMyAssignmentPCECases()
        {
            var myPCECases = await _PCECaseService.GetMyAssignmentPCECases(base.GetCurrentUserId());
            if (myPCECases == null) { return BadRequest("Unable to load PCEcase"); }
            string jsonData = JsonConvert.SerializeObject(myPCECases);
            return Content(jsonData, "application/json");
        }

        public IActionResult MyPCEAssignments()
        {
            return View();
        }

        public async Task<IActionResult> MyPCEAssignment(Guid Id)
        {            
            var pceCase = await _MOPCECaseService.GetPCECase(base.GetCurrentUserId(), Id);
            if (pceCase == null)
            {
                return RedirectToAction("MyPCECases");
            }
            ViewData["PCECaseId"] = Id;

            return View();
        }
        
        [HttpGet]
        public async Task<IActionResult> GetMyPreviousValuations(Guid PCEId)
        {
            var valuationHistory = await _MOPCECaseService.GetValuationHistory(base.GetCurrentUserId(), PCEId);    
            string jsonData = JsonConvert.SerializeObject(valuationHistory.PreviousEvaluations, new JsonSerializerSettings{ReferenceLoopHandling = ReferenceLoopHandling.Ignore});
            return Content(jsonData, "application/json");
        }
        [HttpGet]
        public async Task<IActionResult> MyPCECase(Guid Id)
        {

            var loanCase = await _PCECaseService.GetCaseDetail(Id);
            var caseSchedule = await _ProductionCaseScheduleService.GetProductionCaseSchedules(Id);
            var caseTerminate = await _PCECaseTerminateService.GetCaseTerminates(Id);
            ViewData["PCEcaseTerminate"] = caseTerminate;
            if (loanCase == null) { return RedirectToAction("NewCases"); }
            ViewData["PCEcase"] = loanCase;
            ViewData["ProductionCaseSchedule"] = caseSchedule;
            return View();
        }        
    }
}
