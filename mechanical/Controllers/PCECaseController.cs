using Humanizer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Newtonsoft.Json;
using System.Threading.Tasks;

using mechanical.Data;
using mechanical.Models.Entities;
using mechanical.Models.Dto.MailDto;
using mechanical.Services.MailService;
using mechanical.Services.UserService;
using mechanical.Services.UploadFileService;

using mechanical.Models.PCE.Entities;
using mechanical.Models.PCE.Dto.PCECaseDto;
using mechanical.Models.PCE.Dto.PCECaseScheduleDto;
using mechanical.Models.PCE.Dto.PCECaseTerminateDto;
using mechanical.Services.PCE.PCECaseService;
using mechanical.Services.PCE.PCEEvaluationService;
using mechanical.Services.PCE.PCECaseScheduleService;
using mechanical.Services.PCE.PCECaseTerminateService;
using mechanical.Services.CaseServices;
using mechanical.Services.PCE.PCEEvaluationService;

namespace mechanical.Controllers.PCE
{
    [Authorize]
    public class PCECaseController : BaseController
    {

        private readonly CbeContext _cbeContext;
        private readonly IUserService _UserService;
        private readonly IMailService _mailService;
        private readonly IPCECaseService _PCECaseService;
        private readonly ILogger<PCECaseController> _logger;
        private readonly IUploadFileService _UploadFileService;
        private readonly IPCECaseScheduleService _PCECaseScheduleService;
        private readonly IPCECaseTerminateService _PCECaseTerminateService;
        private readonly IPCEEvaluationService _PCEEvaluationService;

        private readonly ICaseService _caseService;

        public PCECaseController(CbeContext cbeContext, IPCEEvaluationService PCEEvaluationService, ICaseService caseService, IUserService UserService, IPCECaseService PCECaseService, IPCECaseScheduleService PCECaseScheduleService, IPCECaseTerminateService PCECaseTerminateService, IUploadFileService UploadFileService, IMailService mailService)
        {
            _cbeContext = cbeContext;
            _caseService = caseService;
            _mailService = mailService;
            _PCEEvaluationService = PCEEvaluationService;
            _UserService = UserService;
            _PCECaseService = PCECaseService;
            _UploadFileService = UploadFileService;
            _PCECaseScheduleService = PCECaseScheduleService;
            _PCECaseTerminateService = PCECaseTerminateService;
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            ViewData["EmployeeId"] = HttpContext.Session.GetString("EmployeeId") ?? null;
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> Create(PCECaseDto pceCaseDto)
        {
            if (ModelState.IsValid)
            {
                var pceCase = await _PCECaseService.PCECase(base.GetCurrentUserId(), pceCaseDto);
                // var pceCases = await _PCECaseService.PCECase(pceCaseDto);
                return RedirectToAction("Detail", new { Id = pceCase.Id, Status = "New" });
            }
            return View();
        }

        public async Task<IActionResult> Edit(Guid Id)
        {
            var pceCase = await _PCECaseService.GetPCECase(base.GetCurrentUserId(), Id);
            
            if (pceCase == null) 
            { 
                return RedirectToAction("PCECases");
            }

            return View(pceCase);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(PCECaseReturnDto pceCaseDto)
        {
            if (ModelState.IsValid)
            {
                var pceCase = await _PCECaseService.Edit(pceCaseDto.Id, pceCaseDto);
                return RedirectToAction("Detail", new { Id = pceCase.Id, Status = "New" });
            }
            return View(pceCaseDto);
        }

        [HttpGet]
        public async Task<IActionResult> Detail(Guid Id, string Status = "All")
        {
            var allowedStatuses = new[] { "", "All", "New", "Pending", "Completed", "Returned", "Terminated", "Remarked", "Reestimate" };
            
            if (!allowedStatuses.Any(s => s.Equals(Status, StringComparison.OrdinalIgnoreCase))) {
                // Error page
                return BadRequest("Invalid status.");
                // return NotFound("Resource not found.");
                // return Unauthorized("Authentication required.");
                // return StatusCode(500, "An unexpected error occurred.");
                // return Forbid("You do not have permission to access this resource.");
            }

            var userId = base.GetCurrentUserId();
            var pceCase = await _PCECaseService.GetPCECase(userId, Id);

            if (pceCase == null)
            {
                return RedirectToAction("PCECases");
            }

            var PCECaseTerminate = await _PCECaseTerminateService.GetCaseTerminates(Id);
            var pceCaseSchedule = await _PCECaseScheduleService.GetSchedules(Id);
            var latestPCECaseSchedule = await _PCECaseScheduleService.GetLatestSchedule(Id);

            ViewData["CurrentUser"] = await _UserService.GetUserById(userId);
            ViewData["PCECase"] = pceCase;
            ViewData["PCECaseTerminate"] = PCECaseTerminate;
            ViewData["PCECaseSchedule"] = pceCaseSchedule;
            ViewData["LatestPCECaseSchedule"] = latestPCECaseSchedule;
            ViewData["Title"] = "PCE Case Details";             
            ViewBag.Status = Status;

            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetByApplicantName(string applicantName)
        {
            if (applicantName != null)
            {
                var pceCase = _cbeContext.PCECases
                .Where(c => c.ApplicantName.ToLower().Contains(applicantName.ToLower()))
                .Select(c => new
                {
                    c.Id,
                    c.CaseNo,
                    c.ApplicantName,
                    c.CustomerId,
                    c.CustomerEmail
                })
            .ToList();
                return Json(pceCase);

            }
            else
            {
                var pceCases = _cbeContext.PCECases
                .Select(c => new
                {
                    c.Id,
                    c.CaseNo,
                    c.ApplicantName,
                    c.CustomerId,
                    c.CustomerEmail
                })
            .ToList();

                return Json(pceCases);
            }

        }     
        
        [HttpGet]
        public async Task<IActionResult> PCECases(string Status = "All")
        {   
            var allowedStatuses = new[] { "", "All", "New", "Pending", "Completed", "Returned", "Terminated", "Remarked", "Reestimate" };         
            
            if (!allowedStatuses.Any(s => s.Equals(Status, StringComparison.OrdinalIgnoreCase))) { 
                return BadRequest("Invalid status.");
            }

            if (Status == "Reestimate")
            {
                Status = "Completed";
            }

            ViewBag.Status = Status;
            ViewData["Title"] = Status + " PCE Cases";
            ViewBag.Url = "/PCECase/GetPCECases";
            ViewData["CurrentUser"] = await _UserService.GetUserById(base.GetCurrentUserId());

            return View("PCECases");
        }
        
        public async Task<IActionResult> RemarkPCECases()
        {
            ViewData["CurrentUser"] = await _UserService.GetUserById(base.GetCurrentUserId());
            return View();
        }
        
        public async Task<IActionResult> RemarkPCECase(Guid Id)
        {
            var userId = base.GetCurrentUserId();
            var pceCase = await _PCECaseService.GetPCECase(userId, Id);
            var pceCaseSchedule = await _PCECaseScheduleService.GetSchedules(Id);
            
            if (pceCase == null) 
            { 
                return RedirectToAction("PCECases"); 
            }

            ViewData["CurrentUser"] = await _UserService.GetUserById(userId);
            ViewData["PCECase"] = pceCase;
            ViewData["PCECaseSchedule"] = pceCaseSchedule;
            ViewData["Id"] = userId;
            return View();
        }
   
        [HttpGet]
        public async Task<IActionResult> GetPCECase(Guid Id)
        {
            var pceCase = await _PCECaseService.GetPCECase(base.GetCurrentUserId(), Id);
            
            if (pceCase == null)
            {
                return BadRequest("Unable to load the PCE Case is ID: {Id}");
            }

            string jsonData = JsonConvert.SerializeObject(pceCase, new JsonSerializerSettings{ReferenceLoopHandling = ReferenceLoopHandling.Ignore});
            return Content(jsonData, "application/json");
        }

        [HttpGet]
        public async Task<IActionResult> GetPCECases(string Status = "All", int? Limit = null)
        {
            
            var allowedStatuses = new[] { "", "All", "New", "Pending", "Completed", "Returned", "Terminated", "Remarked", "Reestimate" };         
            
            if (!allowedStatuses.Any(s => s.Equals(Status, StringComparison.OrdinalIgnoreCase))) {
                return BadRequest("Invalid status.");
            }

            IEnumerable<PCECaseReturnDto> pceCases = null;

            var userId = base.GetCurrentUserId();
        
            if (Status == "Reestimate")
            {
                pceCases = await _PCECaseService.GetPCECases(userId, "Completed", Limit: Limit);
            }
            else
            {                
                pceCases = await _PCECaseService.GetPCECases(userId, Status, Limit: Limit);
            }            
            
            if (pceCases == null)
            {
                return BadRequest("Unable to load {Status} PCE Cases");
            }

            string jsonData = JsonConvert.SerializeObject(pceCases, new JsonSerializerSettings{ReferenceLoopHandling = ReferenceLoopHandling.Ignore});
            return Content(jsonData, "application/json");
        }      

        [HttpGet]
        public async Task<IActionResult> GetRemarkedPCECases()
        {
            var pceCases = await _PCECaseService.GetRemarkedPCECases(GetCurrentUserId());
            
            if (pceCases == null)
            {
                return BadRequest("Unable to load Remarked PCE Cases.");
            }
            
            string jsonData = JsonConvert.SerializeObject(pceCases, new JsonSerializerSettings{ReferenceLoopHandling = ReferenceLoopHandling.Ignore});
            return Content(jsonData, "application/json");
        }

        [HttpGet]
        public async Task<IActionResult> GetAssignedPCECases()
        {
            var myPCECases = await _PCECaseService.GetAssignedPCECases(base.GetCurrentUserId());
            
            if (myPCECases == null) 
            { 
                return BadRequest("Unable to load assigned PCE Cases"); 
            }
            
            string jsonData = JsonConvert.SerializeObject(myPCECases, new JsonSerializerSettings{ReferenceLoopHandling = ReferenceLoopHandling.Ignore});
            return Content(jsonData, "application/json");
        }     
   
        [HttpGet]
        public async Task<IActionResult> GetDashboardPCECaseCount()
        {
            var pceCaseCount = await _PCECaseService.GetDashboardPCECaseCount(base.GetCurrentUserId());
            string jsonData = JsonConvert.SerializeObject(pceCaseCount, new JsonSerializerSettings{ReferenceLoopHandling = ReferenceLoopHandling.Ignore});
            return Content(jsonData, "application/json");
        }

        [HttpGet]
        public async Task<IActionResult> PCECasesReport()
        {
            ViewData["CurrentUser"] = await _UserService.GetUserById(base.GetCurrentUserId());
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> GetPCECasesReport()
        {
            var newCases = await _PCECaseService.GetPCECasesReport(base.GetCurrentUserId());
            return Content(JsonConvert.SerializeObject(newCases, new JsonSerializerSettings{ReferenceLoopHandling = ReferenceLoopHandling.Ignore}), "application/json");
        }

        public async Task<IActionResult> PCECaseDetailReport(Guid id)
        {
            var pceCaseDto = await _PCECaseService.GetPCECaseDetailReport(base.GetCurrentUserId(), id);
            return View(pceCaseDto);
        }

        [HttpGet]
        public async Task<IActionResult> PCEReport(Guid Id)
        {
            var pceReportData = await _PCECaseService.GetPCEReportData(Id);
            var file = await _UploadFileService.GetUploadFileByCollateralId(Id);
            ViewData["ProductionFiles"] = file;


            double customerId = Convert.ToDouble(pceReportData.PCECases.CustomerId);

            var customerinfo = await _caseService.GetCustomerName(customerId);

            if (customerinfo == null || customerinfo== "err") { 
            
            ViewData["customerinfo"] = "Unable Customer Name";

            }
            else
            {
                ViewData["customerinfo"] = customerinfo;

            }

            if ((pceReportData.PCEEvaluations != null && pceReportData.PCEEvaluations.Count() != 0) || pceReportData.PCEEvaluations.Any())
            {
                var userIdss = _cbeContext.Users.Where(c => c.Id == pceReportData.PCEEvaluations[0].EvaluatorId).Select(c => c.emp_ID).FirstOrDefault();
                var EvaluatorNames = _cbeContext.Users.Include(res => res.Signatures).ThenInclude(res => res.SignatureFile).Where(c => c.Id == pceReportData.PCEEvaluations[0].EvaluatorId).FirstOrDefault();
                var EvaluatorName = EvaluatorNames.Name;
                var signaturefilename = _cbeContext.Signatures.Where(c => c.Emp_Id == userIdss).Select(c => c.SignatureFileId).FirstOrDefault();
                ViewData["signiture"] = EvaluatorNames;

                var evaluatorReportDto = new EvaluatorReportDto
                {
                    EvaluatorId = pceReportData.PCEEvaluations[0].EvaluatorId,
                    CreatedAt = pceReportData.PCEEvaluations[0].CreatedAt,
                    EvaluatorName = EvaluatorName,
                    PCEvaluationId = pceReportData.PCEEvaluations[0].Id,
                    SignatureImageId = signaturefilename
                };
                ViewData["EvaluatorReport"] = evaluatorReportDto;

            }
            else
            {
                var evaluatorReportDto = new EvaluatorReportDto();
                ViewData["EvaluatorReport"] = evaluatorReportDto;
            }

            ViewData["PCECase"] = pceReportData.PCECases;
            ViewData["Productions"] = pceReportData.Productions;
            ViewData["PCEEvaluations"] = pceReportData.PCEEvaluations;
            ViewData["PCECaseSchedule"] = pceReportData.PCECaseSchedule;
            ViewData["CurrentUser"] = await _UserService.GetUserById(base.GetCurrentUserId());
            ViewData["ProductionLines"] = pceReportData.ProductionLines;

            return View();
        }

        [HttpGet]
        public async Task<IActionResult> PCEAllReport(Guid Id)
        {
            var pceReportData = await _PCECaseService.GetPCEAllReportData(Id);
            var file = await _UploadFileService.GetAllUploadFileByCaseId(Id);
            

            double customerId = Convert.ToDouble(pceReportData.PCECases.CustomerId);

            //var customerinfo = await _caseService.GetCustomerName(customerId);
            var customerinfo = "err";


            if (customerinfo == null) { return BadRequest("Unable Customer Name"); }
            ViewData["customerinfo"]  = customerinfo;
            //string jsonData = JsonConvert.SerializeObject(myCase);
            // return Content(jsonData, "application/json");



            if (pceReportData.PCEEvaluations != null && pceReportData.PCEEvaluations.Any())
            {
                var evaluatorReports = new List<EvaluatorReportDto>();

                foreach (var evaluation in pceReportData.PCEEvaluations)
                {
                    var userIdss = _cbeContext.Users.Where(c => c.Id == evaluation.EvaluatorId).Select(c => c.emp_ID).FirstOrDefault();
                    var evaluatorName = _cbeContext.Users.Where(c => c.Id == evaluation.EvaluatorId).Select(c => c.Name).FirstOrDefault();
                    var signatureFilename = _cbeContext.Signatures.Where(c => c.Emp_Id == userIdss).Select(c => c.SignatureFileId).FirstOrDefault();

                    var evaluatorReportDto = new EvaluatorReportDto
                    {
                        EvaluatorId = evaluation.EvaluatorId,
                        CreatedAt = evaluation.CreatedAt,
                        EvaluatorName = evaluatorName,
                        PCEvaluationId = evaluation.Id,
                        SignatureImageId = signatureFilename
                    };

                    evaluatorReports.Add(evaluatorReportDto);
                }

                ViewData["EvaluatorReports"] = evaluatorReports;
            }
            ViewData["ProductionFiles"] = file;
            ViewData["PCECase"] = pceReportData.PCECases;
            ViewData["Productions"] = pceReportData.Productions;
            ViewData["PCEEvaluations"] = pceReportData.PCEEvaluations;
            ViewData["PCECaseSchedule"] = pceReportData.PCECaseSchedule;
            ViewData["CurrentUser"] = await _UserService.GetUserById(base.GetCurrentUserId());

            return View();
        }

        [HttpGet]
        public async Task<IActionResult> PCESummary(Guid Id)
        {
            var pceCase = await _PCECaseService.GetPCECase(base.GetCurrentUserId(), Id);

            //var pceEvaluations = await _PCEEvaluationService.GetValuationsByPCECaseId(base.GetCurrentUserId(), Id);

            //ViewData["pceEvaluations"] = pceEvaluations;
            ViewData["PCECase"] = pceCase;
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetPCESummary(Guid PCECaseId)
        {
            var pceEvaluations = await _PCEEvaluationService.GetValuationsSummaryByPCECaseId(base.GetCurrentUserId(), PCECaseId);
            string jsonData = JsonConvert.SerializeObject(pceEvaluations, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
            return Content(jsonData, "application/json");
        } 

        // PCE Terminate Cases
        [HttpGet]
        public async Task<IActionResult> PCETerminatedCases()
        {
            ViewData["CurrentUser"] = await _UserService.GetUserById(base.GetCurrentUserId());
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetPCETerminatedCases()
        {
            var newCases = await _PCECaseTerminateService.GetPCECaseTerminates(base.GetCurrentUserId());
            return Content(JsonConvert.SerializeObject(newCases, new JsonSerializerSettings{ReferenceLoopHandling = ReferenceLoopHandling.Ignore}), "application/json");
        }
       
        [HttpPost]
        public async Task<IActionResult> CreateTermination(PCECaseTerminatePostDto pceCaseTerminatePostDto)
        {
            var userId = base.GetCurrentUserId();            
            var pceCaseTerminate = await _PCECaseTerminateService.CreateCaseTerminate(userId, pceCaseTerminatePostDto);
            
            if (pceCaseTerminate == null) 
            { 
                return BadRequest("Unable to Create case Schdule"); 
            }

            var CaseInfo = await _PCECaseService.GetPCECase(userId, pceCaseTerminate.PCECaseId);

            // var recipientEmail = await _cbeContext.Users.Where(u => u.Id == CaseInfo.ApplicantId).Select(u => u.Email).FirstOrDefaultAsync();
            var recipientEmail = "yohannessintayhu@cbe.com.et";
            await _mailService.SendEmail(
                recipientEmail: recipientEmail,
                subject: "Valuation Schedule for Case Number " + CaseInfo.CaseNo,
                body: "Dear! Case Termination request  For Applicant:-" + CaseInfo.ApplicantName + " Is " + pceCaseTerminate.Reason + " For further Detail please check PCE Valuation System"
            );

            string jsonData = JsonConvert.SerializeObject(pceCaseTerminate);
            return Ok(pceCaseTerminate);
        }
        [HttpPost]
        public async Task<IActionResult> UpdateTermination(Guid Id, PCECaseTerminatePostDto pceCaseTerminatePostDto)
        {
            var pceCaseTerminate = await _PCECaseTerminateService.UpdateCaseTerminate(base.GetCurrentUserId(), Id, pceCaseTerminatePostDto);
            
            if (pceCaseTerminate == null) 
            { 
                return BadRequest("Unable to update case Termination"); 
            }
            return Ok();
        }
        [HttpPost]
        public async Task<IActionResult> ApproveCaseTermination(Guid Id)
        {
            var caseSchedule = await _PCECaseTerminateService.ApproveCaseTermination(Id);
            
            if (caseSchedule == null) 
            { 
                return BadRequest("Unable to update PCE case Schdule"); 
            }
            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> PCEReestimationCases()
        {
            ViewData["CurrentUser"] = await _UserService.GetUserById(base.GetCurrentUserId());
            return View();
        }
       
        public async Task<IActionResult> PCEReestimationCase(Guid Id)
        {
            var pceCase = await _PCECaseService.GetPCECase(base.GetCurrentUserId(), Id);
            
            if (pceCase == null) 
            { 
                return RedirectToAction("PCECases"); 
            }

            var pceCaseSchedule = await _PCECaseScheduleService.GetSchedules(Id);
            
            ViewData["PCECase"] = pceCase;
            ViewData["PCECaseSchedule"] = pceCaseSchedule;
            ViewData["CurrentUser"] =  await _UserService.GetUserById(base.GetCurrentUserId());

            return View();
        }
        // HO
        [HttpGet]
        public async Task<IActionResult> HOPCECases(string Status = "All")
        {
            var allowedStatuses = new[] { "", "All", "New", "Pending", "Completed", "Returned", "Terminated", "Remarked", "Reestimate" };

            if (!allowedStatuses.Any(s => s.Equals(Status, StringComparison.OrdinalIgnoreCase)))
            {
                return BadRequest("Invalid status.");
            }

            if (Status == "Reestimate")
            {
                Status = "Completed";
            }

            ViewBag.Status = Status;
            ViewData["Title"] = Status + " PCE Cases";
            ViewBag.Url = "/PCECase/GetHOPCECases";
            ViewData["CurrentUser"] = await _UserService.GetUserById(base.GetCurrentUserId());

            return View("HOPCECases");
        }

        [HttpGet]
        public async Task<IActionResult> GetHOPCECases(string Status = "All", int? Limit = null)
        {

            var allowedStatuses = new[] { "", "All", "New", "Pending", "Completed", "Returned", "Terminated", "Remarked", "Reestimate" };

            if (!allowedStatuses.Any(s => s.Equals(Status, StringComparison.OrdinalIgnoreCase)))
            {
                return BadRequest("Invalid status.");
            }

            IEnumerable<PCECaseReturnDto> pceCases = null;

            // var userId = base.GetCurrentUserId();

            if (Status == "Reestimate")
            {
                pceCases = await _PCECaseService.GetHOPCECases("Completed", Limit: Limit);
            }
            else
            {
                pceCases = await _PCECaseService.GetHOPCECases(Status, Limit: Limit);
            }

            if (pceCases == null)
            {
                return BadRequest("Unable to load {Status} PCE Cases");
            }

            string jsonData = JsonConvert.SerializeObject(pceCases, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
            return Content(jsonData, "application/json");
        }
        [HttpGet]
        public async Task<IActionResult> GetHODashboardPCECaseCount()
        {
            var pceCaseCount = await _PCECaseService.GetHODashboardPCECaseCount();
            string jsonData = JsonConvert.SerializeObject(pceCaseCount, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
            return Content(jsonData, "application/json");
        }
      
        //[HttpGet]
        //public async Task<IActionResult> HODetail(Guid Id, string Status = "All")
        //{
        //    var allowedStatuses = new[] { "", "All", "New", "Pending", "Completed", "Returned", "Terminated", "Remarked", "Reestimate" };

        //    if (!allowedStatuses.Any(s => s.Equals(Status, StringComparison.OrdinalIgnoreCase)))
        //    {
        //        // Error page
        //        return BadRequest("Invalid status.");
        //        // return NotFound("Resource not found.");
        //        // return Unauthorized("Authentication required.");
        //        // return StatusCode(500, "An unexpected error occurred.");
        //        // return Forbid("You do not have permission to access this resource.");
        //    }

        //    var userId = base.GetCurrentUserId();
        //    var pceCase = await _PCECaseService.GetHOPCECase(Id);

        //    if (pceCase == null)
        //    {
        //        return RedirectToAction("HOPCECases");
        //    }

        //    var PCECaseTerminate = await _PCECaseTerminateService.GetCaseTerminates(Id);
        //    var pceCaseSchedule = await _PCECaseScheduleService.GetSchedules(Id);
        //    var latestPCECaseSchedule = await _PCECaseScheduleService.GetLatestSchedule(Id);

        //    ViewData["CurrentUser"] = await _UserService.GetUserById(userId);
        //    ViewData["PCECase"] = pceCase;
        //    ViewData["PCECaseTerminate"] = PCECaseTerminate;
        //    ViewData["PCECaseSchedule"] = pceCaseSchedule;
        //    ViewData["LatestPCECaseSchedule"] = latestPCECaseSchedule;
        //    ViewData["Title"] = "PCE Case Details";
        //    ViewBag.Status = Status;

        //    return View();
        //}

        [HttpGet]
        public async Task<IActionResult> HODetail(Guid Id, string Status = "All")
        {
            var allowedStatuses = new[] { "", "All", "New", "Pending", "Completed", "Returned", "Terminated", "Remarked", "Reestimate" };

            if (!allowedStatuses.Any(s => s.Equals(Status, StringComparison.OrdinalIgnoreCase)))
            {
                // Error page
                return BadRequest("Invalid status.");
                // return NotFound("Resource not found.");
                // return Unauthorized("Authentication required.");
                // return StatusCode(500, "An unexpected error occurred.");
                // return Forbid("You do not have permission to access this resource.");
            }

            var userId = base.GetCurrentUserId();
            var pceCase = await _PCECaseService.GetHOPCECase(Id);

            if (pceCase == null)
            {
                return RedirectToAction("PCECases");
            }

            var PCECaseTerminate = await _PCECaseTerminateService.GetCaseTerminates(Id);
            var pceCaseSchedule = await _PCECaseScheduleService.GetSchedules(Id);
            var latestPCECaseSchedule = await _PCECaseScheduleService.GetLatestSchedule(Id);

            ViewData["CurrentUser"] = await _UserService.GetUserById(userId);
            ViewData["PCECase"] = pceCase;
            ViewData["PCECaseTerminate"] = PCECaseTerminate;
            ViewData["PCECaseSchedule"] = pceCaseSchedule;
            ViewData["LatestPCECaseSchedule"] = latestPCECaseSchedule;
            ViewData["Title"] = "PCE Case Details";
            ViewBag.Status = Status;

            return View();
        }
        public async Task<IActionResult> HORemarkPCECases()
        {
            ViewData["CurrentUser"] = await _UserService.GetUserById(base.GetCurrentUserId());
            return View();
        }

        public async Task<IActionResult> HOemarkPCECase(Guid Id)
        {
            var userId = base.GetCurrentUserId();
            var pceCase = await _PCECaseService.GetHOPCECase(Id);
            var pceCaseSchedule = await _PCECaseScheduleService.GetSchedules(Id);

            if (pceCase == null)
            {
                return RedirectToAction("PCECases");
            }

            ViewData["CurrentUser"] = await _UserService.GetUserById(userId);
            ViewData["PCECase"] = pceCase;
            ViewData["PCECaseSchedule"] = pceCaseSchedule;
            ViewData["Id"] = userId;
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> GetHORemarkedPCECases()
        {
            var pceCases = await _PCECaseService.GetHORemarkedPCECases();

            if (pceCases == null)
            {
                return BadRequest("Unable to load Remarked PCE Cases.");
            }

            string jsonData = JsonConvert.SerializeObject(pceCases, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
            return Content(jsonData, "application/json");
        }
        // PCE Terminate Cases
        [HttpGet]
        public async Task<IActionResult> HOPCETerminatedCases()
        {
            ViewData["CurrentUser"] = await _UserService.GetUserById(base.GetCurrentUserId());
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetHOPCETerminatedCases()
        {
            var newCases = await _PCECaseTerminateService.GetHOPCECaseTerminates();
            return Content(JsonConvert.SerializeObject(newCases, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), "application/json");
        }
        [HttpGet]
        public async Task<IActionResult> HOPCESummary(Guid Id)
        {
            var pceCase = await _PCECaseService.GetHOPCECase(Id);

            //var pceEvaluations = await _PCEEvaluationService.GetValuationsByPCECaseId(base.GetCurrentUserId(), Id);

            //ViewData["pceEvaluations"] = pceEvaluations;
            ViewData["PCECase"] = pceCase;
            return View();
        }
    }
}