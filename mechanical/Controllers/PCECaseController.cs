using Humanizer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
using mechanical.Services.PCE.PCECaseScheduleService;
using mechanical.Services.PCE.PCECaseTerminateService;

namespace mechanical.Controllers.PCE
{
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

        public PCECaseController(CbeContext cbeContext, IUserService UserService, IPCECaseService PCECaseService, IPCECaseScheduleService PCECaseScheduleService, IPCECaseTerminateService PCECaseTerminateService, IUploadFileService UploadFileService, IMailService mailService)
        {
            _cbeContext = cbeContext;
            _mailService = mailService;
            _UserService = UserService;
            _PCECaseService = PCECaseService;
            _UploadFileService = UploadFileService;
            _PCECaseScheduleService = PCECaseScheduleService;
            _PCECaseTerminateService = PCECaseTerminateService;
        }

        [HttpGet]
        public async Task<IActionResult> PCECreate()
        {
            ViewData["EmployeeId"] = HttpContext.Session.GetString("EmployeeId") ?? null;
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> PCECreate(PCECaseDto caseDto)
        {
            if (ModelState.IsValid)
            {
                var cases = await _PCECaseService.PCECase(base.GetCurrentUserId(), caseDto);
                return RedirectToAction("Detail", new { Id = cases.Id, Status = "New" });
            }
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> PCENewCases()
        {
            ViewData["CurrentUser"] = await _UserService.GetUserById(base.GetCurrentUserId());
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> GetPCENewCases()
        {
            var newCases = await _PCECaseService.GetPCENewCases(base.GetCurrentUserId());
            return Content(JsonConvert.SerializeObject(newCases, new JsonSerializerSettings{ReferenceLoopHandling = ReferenceLoopHandling.Ignore}), "application/json");
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
            var pcecaseDto = _PCECaseService.GetPCECaseDetailReport(base.GetCurrentUserId(), id);
            return View(pcecaseDto);
        }

        [HttpGet]
        public async Task<IActionResult> PCEPendingCases()
        {
            ViewData["CurrentUser"] = await _UserService.GetUserById(base.GetCurrentUserId());
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> GetPCEPendingCases()
        {
            var newCases = await _PCECaseService.GetPCEPendingCases(base.GetCurrentUserId());
            return Content(JsonConvert.SerializeObject(newCases, new JsonSerializerSettings{ReferenceLoopHandling = ReferenceLoopHandling.Ignore}), "application/json");
        }


        [HttpGet]
        public async Task<IActionResult> PCECompleteCases()
        {
            ViewData["CurrentUser"] = await _UserService.GetUserById(base.GetCurrentUserId());
            return View();
        }

        ////////
        [HttpGet]
        public async Task<IActionResult> PCERejectedCases()
        {
            ViewData["CurrentUser"] = await _UserService.GetUserById(base.GetCurrentUserId());
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> GetPCERejectedCases()
        {
            var newCases = await _PCECaseService.GetPCERejectedCases(base.GetCurrentUserId());
            return Content(JsonConvert.SerializeObject(newCases, new JsonSerializerSettings{ReferenceLoopHandling = ReferenceLoopHandling.Ignore}), "application/json");
        }

        [HttpGet]
        public async Task<IActionResult> PCERejectedDetail(Guid id)
        {
            var pcecaseDto = _PCECaseService.GetPCECase(base.GetCurrentUserId(), id);
            ViewData["PCECase"] = pcecaseDto;
            return View();
        }
        /////



        [HttpGet]
        public async Task<IActionResult> PCETotalCases()
        {
            ViewData["CurrentUser"] = await _UserService.GetUserById(base.GetCurrentUserId());
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> GetPCETotalCases()
        {
            var newCases = await _PCECaseService.GetPCETotalCases(base.GetCurrentUserId());
            return Content(JsonConvert.SerializeObject(newCases, new JsonSerializerSettings{ReferenceLoopHandling = ReferenceLoopHandling.Ignore}), "application/json");
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
        public async Task<IActionResult> CreateTermination(PCECaseTerminatePostDto caseTerminatePostDto)
        {
            var caseTerminate = await _PCECaseTerminateService.CreateCaseTerminate(base.GetCurrentUserId(), caseTerminatePostDto);
            if (caseTerminate == null) { return BadRequest("Unable to Create case Schdule"); }
            var CaseInfo = await _PCECaseService.GetCaseDetail(caseTerminate.PCECaseId);
            await _mailService.SendEmail(new MailPostDto
            {
                SenderEmail = "getnetadane1@cbe.com.et",
                SenderPassword = "Gechlove@1234",
                RecipantEmail = "yohannessintayhu@cbe.com.et",
                Subject = "Valuation Schedule for Case Number " + CaseInfo.CaseNo,
                Body = "Dear! Case Termination request  For Applicant:-" + CaseInfo.ApplicantName + " Is " + caseTerminate.Reason + " For further Detail please check PCE Valuation System",
            });
            string jsonData = JsonConvert.SerializeObject(caseTerminate);
            return Ok(caseTerminate);
        }
        [HttpPost]
        public async Task<IActionResult> UpdateTermination(Guid Id, PCECaseTerminatePostDto caseTerminatePostDto)
        {
            var caseTerminate = await _PCECaseTerminateService.UpdateCaseTerminate(base.GetCurrentUserId(), Id, caseTerminatePostDto);
            if (caseTerminate == null) { return BadRequest("Unable to update case Termination"); }
            return Ok();
        }
        [HttpPost]
        public async Task<IActionResult> ApproveCaseTermination(Guid Id)
        {
            var caseSchedule = await _PCECaseService.ApproveCaseTermination(Id);
            if (caseSchedule == null) { return BadRequest("Unable to update PCE case Schdule"); }
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
            var pceCase = await _PCECaseService.GetCase(base.GetCurrentUserId(), Id);
            if (pceCase == null) { return RedirectToAction("GetPCECompleteCases"); }
            ViewData["PCECase"] = pceCase;
            var pcecaseDto = _PCECaseService.GetPCECase(base.GetCurrentUserId(), Id);
            if (pcecaseDto == null) { return RedirectToAction("PCENewCases"); }
            var pceCaseSchedule = await _PCECaseScheduleService.GetPCECaseSchedules(Id);
            ViewData["PCECaseSchedule"] = pceCaseSchedule; // Updated key
            ViewData["Id"] = base.GetCurrentUserId();
            ViewData["CurrentUser"] =  await _UserService.GetUserById(base.GetCurrentUserId());
            ViewData["PCECaseId"] = pcecaseDto.Id;
            ViewData["PCECase"] = pcecaseDto;
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> GetDashboardPCECaseCount()
        {
            var myCase = await _PCECaseService.GetDashboardPCECaseCount(base.GetCurrentUserId());
            string jsonData = JsonConvert.SerializeObject(myCase, new JsonSerializerSettings{ReferenceLoopHandling = ReferenceLoopHandling.Ignore});
            return Content(jsonData, "application/json");
        }
        // [HttpGet]
        // public async Task<IActionResult> GetMyDashboardPCECaseCount()
        // {
        //     var myCase = await _PCECaseService.GetMyDashboardCaseCount();
        //     string jsonData = JsonConvert.SerializeObject(myCase, new JsonSerializerSettings{ReferenceLoopHandling = ReferenceLoopHandling.Ignore});
        //     return Content(jsonData, "application/json");
        // }
        
        [HttpGet]
        public async Task<IActionResult> PCEDetail(Guid id)
        {
            var pcecaseDto = await _PCECaseService.GetPCECase(base.GetCurrentUserId(), id);
            
            if (pcecaseDto == null) 
            { 
                return RedirectToAction("PCENewCases"); 
            }

            var pceCaseSchedule = await _PCECaseScheduleService.GetPCECaseSchedules(id);
            ViewData["PCECaseSchedule"] = pceCaseSchedule; 
            ViewData["Id"] = base.GetCurrentUserId();
            ViewData["CurrentUser"] = await _UserService.GetUserById(base.GetCurrentUserId());
            ViewData["PCECaseId"] = pcecaseDto.Id;
            ViewData["PCECase"] = pcecaseDto;

            return View();
        }

        public async Task<IActionResult> PCEEdit(Guid Id)
        {
            var editCase = await _PCECaseService.GetPCECase(base.GetCurrentUserId(), Id);
            
            if (editCase == null) 
            { 
                return RedirectToAction("PCENewCases"); 
            }

            return View(editCase);
        }

        [HttpPost]
        public async Task<IActionResult> PCEEdit(PCECaseReturntDto caseDto)
        {


            if (ModelState.IsValid)
            {
                var cases = await _PCECaseService.PCEEdit(caseDto.Id, caseDto);
                return RedirectToAction("PCENewCases");
            }
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
                    c.CustomerUserId,
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
                    c.CustomerUserId,
                    c.CustomerEmail
                })
            .ToList();

                return Json(pceCases);
            }

        }
        // abdu start
        [HttpGet]
        public async Task<IActionResult> GetPCECompleteCases()
        {
            var myCase = await _PCECaseService.GetPCECompleteCases(base.GetCurrentUserId());
            string jsonData = JsonConvert.SerializeObject(myCase, new JsonSerializerSettings{ReferenceLoopHandling = ReferenceLoopHandling.Ignore});
            return Content(jsonData, "application/json");
        }

        [HttpGet]
        public async Task<IActionResult> PCECompleteCase(Guid id)
        {
            ViewData["CurrentUser"] = await _UserService.GetUserById(base.GetCurrentUserId());
            ViewData["Id"] = base.GetCurrentUserId();


            var pceCaseSchedule = await _PCECaseScheduleService.GetPCECaseSchedules(id);
            ViewData["PCECaseSchedule"] = pceCaseSchedule; // Updated key 


            var pceCase = await _PCECaseService.GetCase(base.GetCurrentUserId(), id);
            if (pceCase == null) { return RedirectToAction("GetPCECompleteCases"); }
            ViewData["PCECase"] = pceCase;

            var production = await _cbeContext.ProductionCapacities.ToListAsync();
            List<ProductionCapacity> productions = null;
            try { productions = await _cbeContext.ProductionCapacities.ToListAsync(); }
            catch (Exception ex) { Console.WriteLine($"An error occurred while retrieving productions vehicles: {ex.Message}"); }
            ViewData["Production"] = production;


            var pcecaseDto = _PCECaseService.GetPCECase(base.GetCurrentUserId(), id);
            if (pcecaseDto == null) { return RedirectToAction("PCENewCases"); }
            ViewData["PCECase"] = pcecaseDto;

            ViewData["PCECaseId"] = pcecaseDto.Id;

            return View();
        }

        [HttpGet]
        public async Task<IActionResult> PCEPendingDetail(Guid id)
        {
            var pcecaseDto = await _PCECaseService.GetPCECase(base.GetCurrentUserId(), id);
            var caseTerminate = await _PCECaseTerminateService.GetCaseTerminates(id);
            var caseSchedule = await _PCECaseScheduleService.GetPCECaseSchedules(id);
            ViewData["PCECase"] = pcecaseDto;
            ViewData["PCECaseTerminate"] = caseTerminate;
            ViewData["PCECaseSchedule"] = caseSchedule;
            ViewData["Id"] = base.GetCurrentUserId();
            ViewData["CurrentUser"] = await _UserService.GetUserById(base.GetCurrentUserId());
            ViewData["PCECaseId"] = pcecaseDto.Id;

            return View();
        }

        [HttpGet]
        public async Task<IActionResult> PCEReport(Guid Id)
        {
            var pceReportData = await _PCECaseService.GetPCEReportData(Id);
            var file = await _UploadFileService.GetUploadFileByCollateralId(Id);
            ViewData["ProductionFiles"] = file;


            if (pceReportData.PCEEvaluations != null || pceReportData.PCEEvaluations.Any())
            {
                var userIdss = _cbeContext.CreateUsers.Where(c => c.Id == pceReportData.PCEEvaluations[0].EvaluatorId).Select(c => c.emp_ID).FirstOrDefault();
                var EvaluatorName = _cbeContext.CreateUsers.Where(c => c.Id == pceReportData.PCEEvaluations[0].EvaluatorId).Select(c => c.Name).FirstOrDefault();
                var signaturefilename = _cbeContext.Signatures.Where(c => c.Emp_Id == userIdss).Select(c => c.SignatureFileId).FirstOrDefault();


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


            ViewData["PCECase"] = pceReportData.PCESCase;
            ViewData["Productions"] = pceReportData.Productions;
            ViewData["PCEEvaluations"] = pceReportData.PCEEvaluations;
            ViewData["PCECaseSchedule"] = pceReportData.PCECaseSchedule;
            ViewData["CurrentUser"] = await _UserService.GetUserById(base.GetCurrentUserId());

            return View();
        }

        [HttpGet]
        public async Task<IActionResult> PCEAllReport(Guid Id)
        {
            var pceReportData = await _PCECaseService.GetPCEAllReportData(Id);
            var file = await _UploadFileService.GetAllUploadFileByCaseId(Id);

            if (pceReportData.PCEEvaluations != null && pceReportData.PCEEvaluations.Any())
            {
                var evaluatorReports = new List<EvaluatorReportDto>();

                foreach (var evaluation in pceReportData.PCEEvaluations)
                {
                    var userIdss = _cbeContext.CreateUsers.Where(c => c.Id == evaluation.EvaluatorId).Select(c => c.emp_ID).FirstOrDefault();
                    var evaluatorName = _cbeContext.CreateUsers.Where(c => c.Id == evaluation.EvaluatorId).Select(c => c.Name).FirstOrDefault();
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
            ViewData["PCECase"] = pceReportData.PCESCase;
            ViewData["Productions"] = pceReportData.Productions;
            ViewData["PCEEvaluations"] = pceReportData.PCEEvaluations;
            ViewData["PCECaseSchedule"] = pceReportData.PCECaseSchedule;
            ViewData["CurrentUser"] = await _UserService.GetUserById(base.GetCurrentUserId());

            return View();
        }

        /////////////////// PCE /////////////

        [HttpGet]
        public async Task<IActionResult> GetPCECases(string Status)
        {
            var pceCases = await _PCECaseService.GetPCECases(base.GetCurrentUserId(), Status);
            
            if (pceCases == null)
            {
                return BadRequest("Unable to load {Status} PCE Cases");
            }

            string jsonData = JsonConvert.SerializeObject(pceCases, new JsonSerializerSettings{ReferenceLoopHandling = ReferenceLoopHandling.Ignore});
            return Content(jsonData, "application/json");
        }
        
        public async Task<IActionResult> RemarkPCECases()
        {
            ViewData["CurrentUser"] = await _UserService.GetUserById(base.GetCurrentUserId());
            return View();
        }
        
        public async Task<IActionResult> RemarkPCECase(Guid Id)
        {
            var userId = base.GetCurrentUserId();
            var pceCase = await _PCECaseService.GetPCECase(Id);
            // var pceCase = await _PCECaseService.GetPCECase(base.GetCurrentUserId(), Id);
            var pceCaseSchedule = await _PCECaseScheduleService.GetPCECaseSchedules(Id);
            
            if (pceCase == null) 
            { 
                return RedirectToAction("MyPCECases"); 
            }

            ViewData["CurrentUser"] = await _UserService.GetUserById(userId);
            ViewData["PCECase"] = pceCase;
            ViewData["PCECaseSchedule"] = pceCaseSchedule;
            ViewData["Id"] = userId;
            return View();
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
        public async Task<IActionResult> GetMyAssignmentPCECases()
        {
            var myPCECases = await _PCECaseService.GetMyAssignmentPCECases(base.GetCurrentUserId());
            
            if (myPCECases == null) 
            { 
                return BadRequest("Unable to load assigned PCEcases"); 
            }
            
            string jsonData = JsonConvert.SerializeObject(myPCECases, new JsonSerializerSettings{ReferenceLoopHandling = ReferenceLoopHandling.Ignore});
            return Content(jsonData, "application/json");
        }

        [HttpGet]
        public async Task<IActionResult> PCESummary(Guid Id)
        {
            var pceCase = _PCECaseService.GetPCECase(base.GetCurrentUserId(), Id);
            ViewData["PCECase"] = pceCase;
            return View();
        }

        
        [HttpGet]
        public async Task<IActionResult> MyPCECases(string Status = "All")
        {
            ViewData["Title"] = Status + " PCE Cases";
            ViewBag.Url = "/PCECase/GetMyPCECases";
            ViewBag.Status = Status;
            ViewData["CurrentUser"] = await _UserService.GetUserById(base.GetCurrentUserId());
            return View("PCECases");
        }

        [HttpGet]
        public async Task<IActionResult> PCECases(string Status = "All")
        {
            ViewData["Title"] = Status + " PCE Cases";
            ViewBag.Url = "/PCECase/GetPCECases";
            ViewBag.Status = Status;
            ViewData["CurrentUser"] = await _UserService.GetUserById(base.GetCurrentUserId());
            return View("PCECases");
        }
         
        [HttpGet]
        public async Task<IActionResult> MyPCECase(Guid Id)
        {
            var pceCase = await _PCECaseService.GetCaseDetail(Id);
            var pceCaseSchedule = await _PCECaseScheduleService.GetPCECaseSchedules(Id);
            var pceCaseTerminate = await _PCECaseTerminateService.GetCaseTerminates(Id);
            
            if (pceCase == null) 
            { 
                return RedirectToAction("MyPCECases"); 
            }
            
            ViewData["PCECase"] = pceCase;
            ViewData["PCECaseSchedule"] = pceCaseSchedule;
            ViewData["PCECaseTerminate"] = pceCaseTerminate;
            ViewData["CurrentUser"] = await _UserService.GetUserById(base.GetCurrentUserId());

            return View();
        }
        
        
        [HttpGet]
        public async Task<IActionResult> Detail(Guid Id, string Status = "All")
        {
            var userId = base.GetCurrentUserId();
            var pceCase = await _PCECaseService.GetPCECase(Id);

            if (pceCase == null)
            {
                return RedirectToAction("MyPCECases");
            }

            var PCECaseTerminate = await _PCECaseTerminateService.GetCaseTerminates(Id);
            var pceCaseSchedule = await _PCECaseScheduleService.GetPCECaseSchedules(Id);

            ViewData["CurrentUser"] = await _UserService.GetUserById(userId);
            ViewData["PCECase"] = pceCase;
            ViewData["PCECaseTerminate"] = PCECaseTerminate;
            ViewData["PCECaseSchedule"] = pceCaseSchedule;
            ViewData["Title"] = Status + " PCE Case Details";             
            ViewBag.Status = Status;

            return View();
        }

        
        [HttpGet]
        public async Task<IActionResult> GetPCECase(Guid Id)
        {
            var pceCase = await _PCECaseService.GetPCECase(Id);
            
            if (pceCase == null)
            {
                return BadRequest("Unable to load the PCE Case is ID: {Id}");
            }

            string jsonData = JsonConvert.SerializeObject(pceCase, new JsonSerializerSettings{ReferenceLoopHandling = ReferenceLoopHandling.Ignore});
            return Content(jsonData, "application/json");
        }

        [HttpGet]
        public async Task<IActionResult> GetMyPCECases(string Status = "All")
        {
            var pceCases = await _PCECaseService.GetPCECases(base.GetCurrentUserId(), Status);
            
            if (pceCases == null)
            {
                return BadRequest("Unable to load {Status} PCE Cases");
            }
            
            string jsonData = JsonConvert.SerializeObject(pceCases, new JsonSerializerSettings{ReferenceLoopHandling = ReferenceLoopHandling.Ignore});
            return Content(jsonData, "application/json");
        }

        [HttpGet]
        public async Task<IActionResult> GetMyLatestPCECases()
        {
            var limit = 5;
            var pceCases = await _PCECaseService.GetPCECases(base.GetCurrentUserId(), Limit: limit);
            
            if (pceCases == null)
            {
                return BadRequest("Unable to load {Status} PCE Cases");
            }
            
            string jsonData = JsonConvert.SerializeObject(pceCases, new JsonSerializerSettings{ReferenceLoopHandling = ReferenceLoopHandling.Ignore});
            return Content(jsonData, "application/json");
        }

        [HttpGet]
        public async Task<IActionResult> GetMyDashboardPCECasesCount()
        {
            var pceCasesCount = await _PCECaseService.GetMyDashboardPCECasesCount(base.GetCurrentUserId());
            string jsonData = JsonConvert.SerializeObject(pceCasesCount, new JsonSerializerSettings{ReferenceLoopHandling = ReferenceLoopHandling.Ignore});
            return Content(jsonData, "application/json");
        }
        // [HttpGet]
        // public async Task<IActionResult> GetMyDashboardPCECasesCount()
        // {
        //     var pceCasesCount = await _PCECaseService.GetDashboardPCECasesCount(base.GetCurrentUserId());
        //     string jsonData = JsonConvert.SerializeObject(pceCasesCount, new JsonSerializerSettings{ReferenceLoopHandling = ReferenceLoopHandling.Ignore});
        //     return Content(jsonData, "application/json");
        // }  
    }
}