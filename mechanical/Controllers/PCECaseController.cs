﻿using mechanical.Data;
using mechanical.Models.Entities;
using mechanical.Models.PCE.Dto.PCECaseDto;
using mechanical.Models.PCE.Entities;
using mechanical.Services.PCE.PCECaseService;
using mechanical.Services.PCE.ProductionCaseScheduleService;
using mechanical.Services.PCE.ProductionCaseAssignmentServices;
using mechanical.Services.PCE.PCEEvaluationService;
using mechanical.Services.UploadFileService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace mechanical.Controllers.PCE
{
    public class PCECaseController : BaseController
    {


        private readonly CbeContext _cbeContext;
        private readonly IPCECaseService _PCECaseService;
        private readonly ILogger<PCECaseController> _logger;
        private readonly IProductionCaseScheduleService _productionCaseScheduleService;
        private readonly IProductionCaseAssignmentServices _productionCaseAssignmentService;
        private readonly IPCEEvaluationService _PCEEvaluationService;
        private readonly IUploadFileService _uploadFileService;


        public PCECaseController(CbeContext cbeContext, IPCECaseService PCECaseService, IPCEEvaluationService PCEEvaluationService, IProductionCaseScheduleService ProductionCaseScheduleService, IProductionCaseAssignmentServices ProductionCaseAssignmentService , IUploadFileService uploadFileService)
        {
            _cbeContext = cbeContext;
            _PCECaseService = PCECaseService;
            _productionCaseScheduleService = ProductionCaseScheduleService;
            _productionCaseAssignmentService = ProductionCaseAssignmentService;
            _PCEEvaluationService = PCEEvaluationService;
            _uploadFileService = uploadFileService;
        }



        [HttpGet]
        public IActionResult PCECreate()
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
                return RedirectToAction("PCEDetail", new { id = cases.Id });
            }
            return View();
        }

        [HttpGet]
        public IActionResult PCENewCases()
        {
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> GetPCENewCases()
        {
            var newCases = await _PCECaseService.GetPCENewCases(base.GetCurrentUserId());
            return Content(JsonConvert.SerializeObject(newCases), "application/json");
        }

        [HttpGet]
        public IActionResult PCECasesReport()
        {
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> GetPCECasesReport()
        {
            var newCases = await _PCECaseService.GetPCECasesReport(base.GetCurrentUserId());
            return Content(JsonConvert.SerializeObject(newCases), "application/json");
        }

        public async Task<IActionResult> PCECaseDetailReport(Guid id)
        {
            var pcecaseDto = _PCECaseService.GetPCECaseDetailReport(base.GetCurrentUserId(), id);
            //ViewData["pcecaseDtos"] = pcecaseDto;
            return View(pcecaseDto);
        }

        [HttpGet]
        public IActionResult PCEPendingCases()
        {
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> GetPCEPendingCases()
        {
            var newCases = await _PCECaseService.GetPCEPendingCases(base.GetCurrentUserId());
            return Content(JsonConvert.SerializeObject(newCases), "application/json");
        }


        [HttpGet]
        public IActionResult PCECompleteCases()
        {
            return View();
        }

        ////////
        [HttpGet]
        public IActionResult PCERejectedCases()
        {
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> GetPCERejectedCases()
        {
            var newCases = await _PCECaseService.GetPCERejectedCases(base.GetCurrentUserId());
            return Content(JsonConvert.SerializeObject(newCases), "application/json");
        }

        [HttpGet]
        public async Task<IActionResult> PCERejectedDetail(Guid id)
        {
            var pcecaseDto = _PCECaseService.GetPCECase(base.GetCurrentUserId(), id);      
            ViewData["pcecaseDtos"] = pcecaseDto;
            return View();
        }
        /////



        [HttpGet]
        public IActionResult PCETotalCases()
        {
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> GetPCETotalCases()
        {
            var newCases = await _PCECaseService.GetPCETotalCases(base.GetCurrentUserId());
            return Content(JsonConvert.SerializeObject(newCases), "application/json");
        }



        [HttpGet]
        public async Task<IActionResult> GetDashboardPCECaseCount()
        {
            var myCase = await _PCECaseService.GetDashboardPCECaseCount(base.GetCurrentUserId());
            string jsonData = JsonConvert.SerializeObject(myCase);
            return Content(jsonData, "application/json");
        }
        [HttpGet]
        public async Task<IActionResult> GetMyDashboardPCECaseCount()
        {
            var myCase = await _PCECaseService.GetMyDashboardCaseCount();
            string jsonData = JsonConvert.SerializeObject(myCase);
            return Content(jsonData, "application/json");
        }

        [HttpGet]
        public async Task<IActionResult> PCEDetail(Guid id)
        {
            var pcecaseDto =  _PCECaseService.GetPCECase(base.GetCurrentUserId(), id);
            ViewData["pcecaseDtos"] = pcecaseDto;
            return View();
        } 

      
        public async Task<IActionResult> PCEEdit(Guid Id)
        {
            var editCase =  _PCECaseService.GetPCECase(base.GetCurrentUserId(), Id);
            if (editCase == null) { return RedirectToAction("PCENewCases"); }
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
        public IActionResult GetByApplicantName(string applicantName)
        {
            if (applicantName!=null)
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
            //var newCases = await _PCECaseService.GetPCECompleteCases(base.GetCurrentUserId());
            //return Content(JsonConvert.SerializeObject(newCases), "application/json");

            var myCase = await _PCECaseService.GetPCECompleteCases(base.GetCurrentUserId());
            string jsonData = JsonConvert.SerializeObject(myCase);
            return Content(jsonData, "application/json");
        }

        [HttpGet]
        public async Task<IActionResult> PCECompleteCase(Guid id)
        {

            var loanCase = await _PCECaseService.GetCase(base.GetCurrentUserId(), id);
            //var caseSchedule = await _caseScheduleService.GetCaseSchedules(id);
            var production = await _cbeContext.ProductionCapacities.ToListAsync();
            if (loanCase == null) { return RedirectToAction("GetCompleteCases"); }
            ViewData["case"] = loanCase;
          //  ViewData["CaseSchedule"] = caseSchedule;
            ViewData["Id"] = base.GetCurrentUserId();
            List<ProductionCapacity> productions = null;
            try
            {
                productions = await _cbeContext.ProductionCapacities.ToListAsync();
            }
            catch (Exception ex)
            {
                // Handle the exception (e.g., log the error, display a message, etc.)
                Console.WriteLine($"An error occurred while retrieving productions vehicles: {ex.Message}");
            }
           
            ViewData["production"] = production;
            //ViewData["indBldgFacEq"] = indBldgFacEq;
            //ViewData["conMngAgr"] = conMngAgr;
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> MOVSummary(Guid CaseId)
        {
            var cases = await _cbeContext.PCECases.FindAsync(CaseId);
            ViewData["cases"] = cases;
            var collaterals = await _cbeContext.ProductionCapacities.Where(res => res.PCECaseId == CaseId && res.CurrentStatus == "Complete" && res.CurrentStage == "Checker Officer").ToListAsync();
            ViewData["collaterals"] = collaterals;
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> MOVReport(Guid CaseId)
        {
            var cases = await _cbeContext.PCECases.FindAsync(CaseId);
            var MotorVehicles = await _cbeContext.ProductionCapacities.Include(res => res.EvaluatorUserID).Include(res => res.CheckerUserID).ToListAsync();
            var collaterals = await _cbeContext.ProductionCapacities.Where(res => res.PCECaseId == CaseId && res.CurrentStatus == "Complete" && res.CurrentStage == "Checker Officer").ToListAsync();
            var caseSchedule = await _cbeContext.ProductionCaseSchedules.Where(res => res.PCECaseId == CaseId && res.Status == "Approved").FirstOrDefaultAsync();
            ViewData["cases"] = cases;
            ViewData["collaterals"] = collaterals;
            ViewData["MotorVehicles"] = MotorVehicles;
            ViewData["caseSchedule"] = caseSchedule;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SendForValuation(string selectedCollateralIds, string CenterId)
        {

            var userId = base.GetCurrentUserId();
            try
            {
                await _productionCaseAssignmentService.SendProductionForValuation(selectedCollateralIds, CenterId);
                var response = new { message = "PCE assigned successfully" };
                return Ok(response);
            }
            catch (Exception ex)
            {
                var error = new { message = ex.Message };
                return BadRequest(error);
            }
        }

        [HttpPost]
        public async Task<IActionResult> SendProductionForReestimation(string ReestimationReason, string selectedCollateralIds, string CenterId)
        {
            var userId = base.GetCurrentUserId();
            try
            {
                await _productionCaseAssignmentService.SendProductionForReestimation(ReestimationReason, selectedCollateralIds, CenterId);
                var response = new { message = "PCE Reestimation assigned successfully" };
                return Ok(response);
            }
            catch (Exception ex)
            {
                var error = new { message = ex.Message };
                return BadRequest(error);
            }
        }

        [HttpGet]
        public async Task<IActionResult> PCEPendingDetail(Guid id)
        {
            //var loanCase = await _PCECaseService.GetCase(base.GetCurrentUserId(), id);
            //var caseSchedule = await _caseScheduleService.GetCaseSchedules(id);
            //var caseTerminate = await _caseTermnateService.GetCaseTerminates(id);
            //if (loanCase == null) { return RedirectToAction("NewCases"); }
            //ViewData["case"] = loanCase;
            //ViewData["CaseSchedule"] = caseSchedule;
            //ViewData["caseTerminate"] = caseTerminate;
            //ViewData["Id"] = base.GetCurrentUserId();
            //return View();

            var pcecaseDto = _PCECaseService.GetPCECase(base.GetCurrentUserId(), id);
            ViewData["pcecaseDtos"] = pcecaseDto;
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> PCEReport(Guid Id)
        {
            var pceReportData = await _PCECaseService.GetPCEReportData(Id);
            var file = await _uploadFileService.GetUploadFileByCollateralId(Id);
            ViewData["productionFiles"] = file;
            
           
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


            ViewData["pceCase"] = pceReportData.PCESCase;
            ViewData["productions"] = pceReportData.Productions;
            ViewData["pceEvaluations"] = pceReportData.PCEEvaluations;
            ViewData["pceCaseSchedule"] = pceReportData.PCECaseSchedule;

            return View();
        }

        [HttpGet]
        public async Task<IActionResult> PCEAllReport(Guid Id)
        {
            var pceReportData = await _PCECaseService.GetPCEAllReportData(Id);
            var file = await _uploadFileService.GetAllUploadFileByCaseId(Id);


            //if (pceReportData.PCEEvaluations != null || pceReportData.PCEEvaluations.Any())
            //{
            //    var userIdss = _cbeContext.CreateUsers.Where(c => c.Id == pceReportData.PCEEvaluations[0].EvaluatorId).Select(c => c.emp_ID).FirstOrDefault();
            //    var EvaluatorName = _cbeContext.CreateUsers.Where(c => c.Id == pceReportData.PCEEvaluations[0].EvaluatorId).Select(c => c.Name).FirstOrDefault();
            //    var signaturefilename = _cbeContext.Signatures.Where(c => c.Emp_Id == userIdss).Select(c => c.SignatureFileId).FirstOrDefault();
            //    var evaluatorReportDto = new EvaluatorReportDto
            //    {
            //        EvaluatorId = pceReportData.PCEEvaluations[0].EvaluatorId,
            //        CreatedAt = pceReportData.PCEEvaluations[0].CreatedAt,
            //        EvaluatorName = EvaluatorName,
            //        PCEvaluationId = pceReportData.PCEEvaluations[0].Id,
            //        SignatureImageId = signaturefilename
            //    };
            //    ViewData["EvaluatorReport"] = evaluatorReportDto;

            //}
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
            ViewData["productionFiles"] = file;             
            ViewData["pceCase"] = pceReportData.PCESCase;
            ViewData["productions"] = pceReportData.Productions;
            ViewData["pceEvaluations"] = pceReportData.PCEEvaluations;
            ViewData["pceCaseSchedule"] = pceReportData.PCECaseSchedule;

            return View();
        }
    
        /////////////////// PCE /////////////
        [HttpGet]
        public IActionResult PCECases(string Status = "New")
        {
            ViewData["Title"] = Status + " PCE Cases";
            ViewBag.Url = "/PCECase/GetPCECases";
            ViewBag.Status = Status;
            return View("PCECases");
        }

        [HttpGet]
        public async Task<IActionResult> GetPCECases(string Status)
        {
            var pceCases = await _PCEEvaluationService.GetPCECases(base.GetCurrentUserId(), Status);
            if (pceCases == null)
            {
                return BadRequest("Unable to load {Status} PCE Cases");
            }
            string jsonData = JsonConvert.SerializeObject(pceCases);
            return Content(jsonData, "application/json");
        }
        
        public IActionResult RemarkPCECases()
        {
            return View();
        }
        public async Task<IActionResult> RemarkPCECase(Guid Id)
        {
            var pceCase = _PCECaseService.GetPCECase(base.GetCurrentUserId(), Id);
            // var pceCase = await _PCECaseService.GetPCECase(base.GetCurrentUserId(), Id);
            var PCECaseSchedule = await _productionCaseScheduleService.GetProductionCaseSchedules(Id);
            if (pceCase == null) 
            { 
                return RedirectToAction("NewPCECases"); 
            }
            ViewData["pcecaseDtos"] = pceCase;
            ViewData["PCECaseSchedule"] = PCECaseSchedule;
            ViewData["Id"] = base.GetCurrentUserId();
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> GetRemarkedPCECases()
        {
            var pceCases = await _PCECaseService.GetRemarkedPCECases(GetCurrentUserId());
            if (pceCases == null) 
            { 
                return BadRequest("Unable to load PCE Case"); 
            }
            string jsonData = JsonConvert.SerializeObject(pceCases);
            return Content(jsonData, "application/json");
        }
    }
}