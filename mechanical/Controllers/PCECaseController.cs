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
                var pceCases = await _PCECaseService.PCECase(base.GetCurrentUserId(), pceCaseDto);
                // var pceCases = await _PCECaseService.PCECase(pceCaseDto);
                return RedirectToAction("Detail", new { Id = pceCases.Id, Status = "New" });
            }
            return View();
        }

        public async Task<IActionResult> Edit(Guid Id)
        {
            var editCase = await _PCECaseService.GetPCECase(base.GetCurrentUserId(), Id);
            
            if (editCase == null) 
            { 
                return RedirectToAction("PCECases");
            }

            return View(editCase);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(PCECaseReturnDto pceCaseDto)
        {
            if (ModelState.IsValid)
            {
                var pceCases = await _PCECaseService.Edit(pceCaseDto.Id, pceCaseDto);
                return RedirectToAction("PCECases");
            }
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Detail(Guid Id, string Status = "All")
        {
            var userId = base.GetCurrentUserId();
            var pceCase = await _PCECaseService.GetPCECase(userId, Id);

            if (pceCase == null)
            {
                return RedirectToAction("PCECases");
            }

            var PCECaseTerminate = await _PCECaseTerminateService.GetCaseTerminates(Id);
            var pceCaseSchedule = await _PCECaseScheduleService.GetPCECaseSchedules(Id);

            ViewData["CurrentUser"] = await _UserService.GetUserById(userId);
            ViewData["PCECase"] = pceCase;
            ViewData["PCECaseTerminate"] = PCECaseTerminate;
            ViewData["PCECaseSchedule"] = pceCaseSchedule;
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
        
        [HttpGet]
        public async Task<IActionResult> PCECases(string Status = "All")
        {            
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
            var pceCaseSchedule = await _PCECaseScheduleService.GetPCECaseSchedules(Id);
            
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
            var myCase = await _PCECaseService.GetDashboardPCECaseCount(base.GetCurrentUserId());
            string jsonData = JsonConvert.SerializeObject(myCase, new JsonSerializerSettings{ReferenceLoopHandling = ReferenceLoopHandling.Ignore});
            return Content(jsonData, "application/json");
        }
   
        [HttpGet]
        public async Task<IActionResult> GetDashboardPCECasesCount()
        {
            var pceCasesCount = await _PCECaseService.GetDashboardPCECasesCount(base.GetCurrentUserId());
            string jsonData = JsonConvert.SerializeObject(pceCasesCount, new JsonSerializerSettings{ReferenceLoopHandling = ReferenceLoopHandling.Ignore});
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
            var pcepceCaseDto = _PCECaseService.GetPCECaseDetailReport(base.GetCurrentUserId(), id);
            return View(pcepceCaseDto);
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

        [HttpGet]
        public async Task<IActionResult> PCESummary(Guid Id)
        {
            var pceCase = _PCECaseService.GetPCECase(base.GetCurrentUserId(), Id);
            ViewData["PCECase"] = pceCase;
            return View();
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
            await _mailService.SendEmail(new MailPostDto
            {
                SenderEmail = "getnetadane1@cbe.com.et",
                SenderPassword = "Gechlove@1234",
                RecipantEmail = "yohannessintayhu@cbe.com.et",
                Subject = "Valuation Schedule for Case Number " + CaseInfo.CaseNo,
                Body = "Dear! Case Termination request  For Applicant:-" + CaseInfo.ApplicantName + " Is " + pceCaseTerminate.Reason + " For further Detail please check PCE Valuation System",
            });
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

            var pceCaseSchedule = await _PCECaseScheduleService.GetPCECaseSchedules(Id);
            
            ViewData["PCECase"] = pceCase;
            ViewData["PCECaseSchedule"] = pceCaseSchedule;
            ViewData["CurrentUser"] =  await _UserService.GetUserById(base.GetCurrentUserId());

            return View();
        }
        

        
        // [HttpGet]
        // public async Task<IActionResult> PCENewCases()
        // {
        //     ViewData["CurrentUser"] = await _UserService.GetUserById(base.GetCurrentUserId());
        //     return View();
        // }
        // [HttpGet]
        // public async Task<IActionResult> GetPCENewCases()
        // {
        //     var newCases = await _PCECaseService.GetPCENewCases(base.GetCurrentUserId());
        //     return Content(JsonConvert.SerializeObject(newCases, new JsonSerializerSettings{ReferenceLoopHandling = ReferenceLoopHandling.Ignore}), "application/json");
        // }

        // [HttpGet]
        // public async Task<IActionResult> PCEPendingCases()
        // {
        //     ViewData["CurrentUser"] = await _UserService.GetUserById(base.GetCurrentUserId());
        //     return View();
        // }
        // [HttpGet]
        // public async Task<IActionResult> GetPCEPendingCases()
        // {
        //     var newCases = await _PCECaseService.GetPCEPendingCases(base.GetCurrentUserId());
        //     return Content(JsonConvert.SerializeObject(newCases, new JsonSerializerSettings{ReferenceLoopHandling = ReferenceLoopHandling.Ignore}), "application/json");
        // }

        // [HttpGet]
        // public async Task<IActionResult> PCECompleteCases()
        // {
        //     ViewData["CurrentUser"] = await _UserService.GetUserById(base.GetCurrentUserId());
        //     return View();
        // }

        // ////////
        // [HttpGet]
        // public async Task<IActionResult> PCERejectedCases()
        // {
        //     ViewData["CurrentUser"] = await _UserService.GetUserById(base.GetCurrentUserId());
        //     return View();
        // }
        // [HttpGet]
        // public async Task<IActionResult> GetPCERejectedCases()
        // {
        //     var newCases = await _PCECaseService.GetPCERejectedCases(base.GetCurrentUserId());
        //     return Content(JsonConvert.SerializeObject(newCases, new JsonSerializerSettings{ReferenceLoopHandling = ReferenceLoopHandling.Ignore}), "application/json");
        // }

        // [HttpGet]
        // public async Task<IActionResult> PCERejectedDetail(Guid id)
        // {
        //     var pcepceCaseDto = _PCECaseService.GetPCECase(base.GetCurrentUserId(), id);
        //     ViewData["PCECase"] = pcepceCaseDto;
        //     return View();
        // }
        /////



        // [HttpGet]
        // public async Task<IActionResult> PCETotalCases()
        // {
        //     ViewData["CurrentUser"] = await _UserService.GetUserById(base.GetCurrentUserId());
        //     return View();
        // }
        // [HttpGet]
        // public async Task<IActionResult> GetPCETotalCases()
        // {
        //     var newCases = await _PCECaseService.GetPCETotalCases(base.GetCurrentUserId());
        //     return Content(JsonConvert.SerializeObject(newCases, new JsonSerializerSettings{ReferenceLoopHandling = ReferenceLoopHandling.Ignore}), "application/json");
        // }

        // // abdu start
        // [HttpGet]
        // public async Task<IActionResult> GetPCECompleteCases()
        // {
        //     var myCase = await _PCECaseService.GetPCECompleteCases(base.GetCurrentUserId());
        //     string jsonData = JsonConvert.SerializeObject(myCase, new JsonSerializerSettings{ReferenceLoopHandling = ReferenceLoopHandling.Ignore});
        //     return Content(jsonData, "application/json");
        // }

        // [HttpGet]
        // public async Task<IActionResult> PCECompleteCase(Guid id)
        // {
        //     ViewData["CurrentUser"] = await _UserService.GetUserById(base.GetCurrentUserId());
        //     ViewData["Id"] = base.GetCurrentUserId();


        //     var pceCaseSchedule = await _PCECaseScheduleService.GetPCECaseSchedules(id);
        //     ViewData["PCECaseSchedule"] = pceCaseSchedule; // Updated key 


        //     var pceCase = await _PCECaseService.GetCase(base.GetCurrentUserId(), id);
        //     if (pceCase == null) { return RedirectToAction("GetPCECompleteCases"); }
        //     ViewData["PCECase"] = pceCase;

        //     var production = await _cbeContext.ProductionCapacities.ToListAsync();
        //     List<ProductionCapacity> productions = null;
        //     try { productions = await _cbeContext.ProductionCapacities.ToListAsync(); }
        //     catch (Exception ex) { Console.WriteLine($"An error occurred while retrieving productions vehicles: {ex.Message}"); }
        //     ViewData["Production"] = production;


        //     var pcepceCaseDto = _PCECaseService.GetPCECase(base.GetCurrentUserId(), id);
        //     if (pcepceCaseDto == null) { return RedirectToAction("PCENewCases"); }
        //     ViewData["PCECase"] = pcepceCaseDto;

        //     ViewData["PCECaseId"] = pcepceCaseDto.Id;

        //     return View();
        // }

        // [HttpGet]
        // public async Task<IActionResult> PCEPendingDetail(Guid id)
        // {
        //     var pcepceCaseDto = await _PCECaseService.GetPCECase(base.GetCurrentUserId(), id);
        //     var pceCaseTerminate = await _PCECaseTerminateService.GetCaseTerminates(id);
        //     var caseSchedule = await _PCECaseScheduleService.GetPCECaseSchedules(id);
        //     ViewData["PCECase"] = pcepceCaseDto;
        //     ViewData["PCECaseTerminate"] = pceCaseTerminate;
        //     ViewData["PCECaseSchedule"] = caseSchedule;
        //     ViewData["Id"] = base.GetCurrentUserId();
        //     ViewData["CurrentUser"] = await _UserService.GetUserById(base.GetCurrentUserId());
        //     ViewData["PCECaseId"] = pcepceCaseDto.Id;

        //     return View();
        // }
    }
}