
using mechanical.Models.Dto.CaseDto;
using mechanical.Services.CaseServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using mechanical.Models.Enum;
using System.ComponentModel.DataAnnotations;
using mechanical.Data;
using Microsoft.EntityFrameworkCore;
using mechanical.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using System.Data;
using mechanical.Services.CaseAssignmentService;
using mechanical.Models.Dto.CaseAssignmentDto;
using Microsoft.AspNetCore.Http;
using mechanical.Services.CaseScheduleService;
using System.Linq;
using System.Net.Http;
using mechanical.Models.Dto.CaseScheduleDto;
using mechanical.Models.Dto.MailDto;
using mechanical.Services.MailService;
using AutoMapper;
using mechanical.Models.Dto.CaseTerminateDto;
using mechanical.Services.CaseTerminateService;
using Microsoft.CodeAnalysis.Operations;
using mechanical.Services.UploadFileService;

namespace mechanical.Controllers
{
   
    public class CaseController : BaseController
    {
        private readonly ICaseService _caseService;
        private readonly ICaseAssignmentService _caseAssignmentService;
        private readonly CbeContext _cbeContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ICaseScheduleService   _caseScheduleService;
        private readonly ICaseTerminateService _caseTermnateService;
        private readonly IMailService _mailService;
        private readonly IMapper _mapper;
        private readonly IUploadFileService _uploadFileService;
        public CaseController(IMapper mapper,IUploadFileService uploadFileService, ICaseTerminateService caseTerminateService,ICaseService caseService,ICaseScheduleService caseScheduleService, CbeContext cbeContext, IHttpContextAccessor httpContextAccessor,ICaseAssignmentService caseAssignmentService, IMailService mailService)
        {
            _caseService = caseService;
            _cbeContext = cbeContext;
            _httpContextAccessor = httpContextAccessor;
            _caseAssignmentService = caseAssignmentService;
            _caseScheduleService = caseScheduleService;
            _caseTermnateService = caseTerminateService;
            _mailService = mailService;
            _mapper = mapper;
            _uploadFileService = uploadFileService;
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
        public async Task<IActionResult> GetRemarkedCases()
        {
            var myCase = await _caseService.GetRmRemarkedCases(GetCurrentUserId());
            if (myCase == null) { return BadRequest("Unable to load case"); }
            string jsonData = JsonConvert.SerializeObject(myCase);
            return Content(jsonData, "application/json");
        }



        [HttpGet]
        public  IActionResult Create()
        {
            ViewData["EmployeeId"] = HttpContext.Session.GetString("EmployeeId") ?? null;
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CasePostDto caseDto)
        {
            if (ModelState.IsValid)
            {
                var cases = await _caseService.CreateCase(base.GetCurrentUserId(), caseDto);
                return RedirectToAction("Detail", new { id = cases.Id });
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> GetCustomerName(double customerId)
        {
            //double cus= 9755012884;
            var myCase = await _caseService.GetCustomerName(customerId);
            if (myCase == null) { return BadRequest("Unable Customer Name"); }
            string jsonData = JsonConvert.SerializeObject(myCase);
            return Content(jsonData, "application/json");
        }



        [HttpGet]
        public IActionResult NewCases()
        {
            return View();
        }
        [HttpGet]
        public IActionResult TerminatedCases()
        {
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> GetTerminatedCases()
        {
            var newCases = await _caseService.GetTerminatedCases(base.GetCurrentUserId());
            return Content(JsonConvert.SerializeObject(newCases), "application/json");
        }
        public IActionResult RejectedCases()
        {
            return View();
        }

        [HttpGet]
        public IActionResult MyPendingCases()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetNewCases()
        {
            var newCases = await _caseService.GetNewCases(base.GetCurrentUserId());
            return Content(JsonConvert.SerializeObject(newCases), "application/json");
        }

        [HttpGet]
        public async Task<IActionResult> GetHOCases()
        {
            var cases = await _cbeContext.Cases.Include(res=>res.District).Include(res=>res.Collaterals).ToListAsync();
            var caseDtos = _mapper.Map<IEnumerable<CaseDto>>(cases);
            return Content(JsonConvert.SerializeObject(caseDtos), "application/json");
        }
        [HttpGet]
        public async Task<IActionResult> GetRejectedCases()
        {
            var rejectedCases = await _caseService.GetRejectedCases(base.GetCurrentUserId());
            return Content(JsonConvert.SerializeObject(rejectedCases), "application/json");
        }
        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            var loanCase = await _caseService.GetCase(base.GetCurrentUserId(), id);
            if (loanCase == null) { return RedirectToAction("NewCases"); }
            return View(loanCase);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, CasePostDto caseDto)
        {
            if (ModelState.IsValid)
            {
                var cases = await _caseService.EditCase(base.GetCurrentUserId(), id, caseDto);
                return RedirectToAction("NewCases");
            }
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> Detail(Guid id)
        {
            var loanCase = await _caseService.GetCase(base.GetCurrentUserId(), id);
            var caseSchedule = await _caseScheduleService.GetCaseSchedules(id);
            if (loanCase == null) { return RedirectToAction("NewCases"); }
            ViewData["case"] = loanCase;
            ViewData["CaseSchedule"] = caseSchedule;
            ViewData["Id"] = base.GetCurrentUserId();
            var moFile = await _uploadFileService.GetMoUploadFile(id);
            ViewData["moFile"] = moFile;
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> PendDetail(Guid id)
        {
            var loanCase = await _caseService.GetCase(base.GetCurrentUserId(), id);
            var caseSchedule = await _caseScheduleService.GetCaseSchedules(id);
            var caseTerminate = await _caseTermnateService.GetCaseTerminates(id);
            if (loanCase == null) { return RedirectToAction("NewCases"); }
            ViewData["case"] = loanCase;
            ViewData["CaseSchedule"] = caseSchedule;
            ViewData["caseTerminate"] = caseTerminate;
            ViewData["Id"] = base.GetCurrentUserId();
            var moFile = await _uploadFileService.GetMoUploadFile(id);
            ViewData["moFile"] = moFile;
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> RejectedDetail(Guid id)
        {
            var loanCase = await _caseService.GetCase(base.GetCurrentUserId(), id);
            if (loanCase == null) { return RedirectToAction("NewCases"); }
            ViewData["case"] = loanCase;
            var moFile = await _uploadFileService.GetMoUploadFile(id);
            ViewData["moFile"] = moFile;
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> SendForValuation(string selectedCollateralIds, string CenterId)
        {
            try
            {
                await _caseAssignmentService.SendForValuation(selectedCollateralIds, CenterId);
                var response = new { message = "Collaterals assigned successfully" };
                return Ok(response);
            }catch (Exception ex)
            {
                var error = new { message = ex.Message };
                return BadRequest(error);
            }
        }
        [HttpPost]
        public async Task<IActionResult> SendForReestimation(string ReestimationReason, string selectedCollateralIds, string CenterId)
        {
            try
            {
                await _caseAssignmentService.SendForReestimation(ReestimationReason,selectedCollateralIds, CenterId);
                var response = new { message = "Collaterals assigned successfully" };
                return Ok(response);
            }
            catch (Exception ex)
            {
                var error = new { message = ex.Message };
                return BadRequest(error);
            }
        }
        [HttpPost]
        public async Task<IActionResult> SendRejection(MoRejectCaseDto moRejectCaseDto)
        {

            if (!await _caseService.SendRejection(moRejectCaseDto))
            {
                return BadRequest();
            }
            return RedirectToAction("MyCases" , "MOCase");
            
        }
        [HttpPost]
        public async Task<IActionResult> RetrunToMaker(Guid Id)
        {
            bool success = await _caseService.RetrunToMaker(Id);
            if (!success)
            {
                return Json(new { success = false });
            }
            return Json(new { success = true });
        }
        [HttpGet]
        public async Task<IActionResult> GetDashboardCaseCount()
        {
            var myCase = await _caseService.GetDashboardCaseCount(base.GetCurrentUserId());
            string jsonData = JsonConvert.SerializeObject(myCase);
            return Content(jsonData, "application/json");
        }
        [HttpGet]
        public async Task<IActionResult> GetMyDashboardCaseCount()
        {
            var myCase = await _caseService.GetMyDashboardCaseCount(base.GetCurrentUserId());
            string jsonData = JsonConvert.SerializeObject(myCase);
            return Content(jsonData, "application/json");
        }
        [HttpPost]
        public async Task<IActionResult> CreateSchedule(CaseSchedulePostDto CaseSchedulePostDto)
        {
            var caseSche = await _cbeContext.CaseSchedules.FindAsync(CaseSchedulePostDto.Id);
            caseSche.Status = "Rejected";
            caseSche.Reason = CaseSchedulePostDto.Reason;
            _cbeContext.Update(caseSche);
            await _cbeContext.SaveChangesAsync();
            CaseSchedulePostDto.Reason = null;
            CaseSchedulePostDto.Id = Guid.Empty;
            var caseSchedule = await _caseScheduleService.CreateCaseSchedule(base.GetCurrentUserId(), CaseSchedulePostDto);
            if (caseSchedule == null) { return BadRequest("Unable to Create case Schdule"); }
            var CaseInfo = await _caseService.GetCaseDetail(caseSchedule.CaseId);
            await _mailService.SendEmail(new MailPostDto
            {
                SenderEmail = " getnetadane1@cbe.com.et",
                SenderPassword = "Gechlove@1234",
                RecipantEmail = "yohannessintayhu@cbe.com.et",
                Subject = "RM Proposed New Valuation Schedule for Case Number " + CaseInfo.CaseNo,
                Body = "Dear! </br> Valuation Schedule Update  For Applicant:-" + CaseInfo.ApplicantName + " Is " + caseSchedule.ScheduleDate + "</br></br> For further Detail please check Collateral Valuation System",
            });
            string jsonData = JsonConvert.SerializeObject(caseSchedule);
            return Ok(caseSchedule);
        }

        //[HttpGet]
        //public IActionResult MyCases()
        //{
        //    return View();
        //}
        [HttpPost]
        public async Task<ActionResult> DeleteBussinessLicence(Guid Id)
        {
            if (await _caseService.DeleteBussinessLicence(Id))
            {
                return Ok();
            }
            return BadRequest();
        }
        [HttpPost]
        public async Task<ActionResult> UploadBussinessLicence(IFormFile BussinessLicence, Guid caseId)
        {
            if (await _caseService.UploadBussinessLicence(base.GetCurrentUserId(),BussinessLicence, caseId))
            {
                return Ok();
            }
            return BadRequest();
        }
        [HttpGet]
        public async Task<IActionResult> GetMyPendingCases()
        {
            var myCase = await _caseService.GetRmPendingCases(base.GetCurrentUserId());
            string jsonData = JsonConvert.SerializeObject(myCase);
            return Content(jsonData, "application/json");
        }
        public async Task<IActionResult> SendReEvaluation(Guid Id)
        {
            var rejected = await _cbeContext.Rejects.FirstOrDefaultAsync(res => res.CollateralId == Id);
            var caseAssignment = await _cbeContext.CaseAssignments.Where(res => res.CollateralId == Id && res.UserId == rejected.RejectedBy).FirstOrDefaultAsync();
            caseAssignment.Status = "New";
            caseAssignment.AssignmentDate = DateTime.Now;
            _cbeContext.Update(caseAssignment);
            var collateral = await _cbeContext.Collaterals.FindAsync(Id);
            collateral.CurrentStage = "Maker Officer";
            collateral.CurrentStatus = "New";
            _cbeContext.Update(collateral);
            await _cbeContext.SaveChangesAsync();
            return RedirectToAction("RejectedCases", "Case");
        }
        [HttpGet]
        public async Task<IActionResult>  GetNextCaseNumber()
        {
            var userId = base.GetCurrentUserId();
            var userNumber = await _cbeContext.ConsecutiveNumbers.Where(res=>res.UserId == userId).FirstOrDefaultAsync();

            if (userNumber == null) { 
                userNumber = new ConsecutiveNumber { UserId = userId, NextNumber = 1 };
                await _cbeContext.ConsecutiveNumbers.AddAsync(userNumber);
            } else
            {
                userNumber.NextNumber++;
                _cbeContext.Update(userNumber);
            }
            await _cbeContext.SaveChangesAsync();

            return Ok(new { nextNumber = userNumber.NextNumber });
        }
        [HttpGet]
        public async Task<IActionResult> MOVSummary(Guid CaseId)
        {
            var cases = await _cbeContext.Cases.FindAsync(CaseId);
            var MotorVehicles = await _cbeContext.MotorVehicles
                              .Include(res => res.EvaluatorUser)
                                  .ThenInclude(res => res.Signatures).ThenInclude(res => res.SignatureFile)
                               .Include(res => res.CheckerUser)
                                  .ThenInclude(res => res.Signatures).ThenInclude(res => res.SignatureFile).Where(res => res.Collateral.CaseId == CaseId && res.Collateral.CurrentStatus == "Complete" && res.Collateral.CurrentStage == "Checker Officer").ToListAsync();

            var collaterals = await _cbeContext.Collaterals.Where(res => res.CaseId == CaseId && res.Category == MechanicalCollateralCategory.MOV && res.CurrentStatus == "Complete" && res.CurrentStage == "Checker Officer").ToListAsync();
            var caseSchedule = await _cbeContext.CaseSchedules.Where(res => res.CaseId == CaseId && res.Status == "Approved").FirstOrDefaultAsync();
            ViewData["cases"] = cases;
            ViewData["collaterals"] = collaterals;
            ViewData["MotorVehicles"] = MotorVehicles;
            ViewData["caseSchedule"] = caseSchedule;
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> MOVReport(Guid CaseId)
        {
            var cases = await _cbeContext.Cases.FindAsync(CaseId);
            var MotorVehicles = await _cbeContext.MotorVehicles
                              .Include(res => res.EvaluatorUser)
                                  .ThenInclude(res => res.Signatures).ThenInclude(res => res.SignatureFile)
                               .Include(res => res.CheckerUser)
                                  .ThenInclude(res => res.Signatures).ThenInclude(res => res.SignatureFile).Where(res => res.Collateral.CaseId == CaseId && res.Collateral.CurrentStatus == "Complete" && res.Collateral.CurrentStage == "Checker Officer").ToListAsync();

            var collaterals = await _cbeContext.Collaterals.Where(res => res.CaseId == CaseId && res.Category == MechanicalCollateralCategory.MOV && res.CurrentStatus == "Complete" && res.CurrentStage == "Checker Officer").ToListAsync();
            var caseSchedule = await _cbeContext.CaseSchedules.Where(res => res.CaseId == CaseId && res.Status == "Approved").FirstOrDefaultAsync();
            ViewData["cases"] = cases;
            ViewData["collaterals"] = collaterals;
            ViewData["MotorVehicles"] = MotorVehicles;
            ViewData["caseSchedule"] = caseSchedule;
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> ConstMngAgrMachinerySummary(Guid CaseId)
        {
            var cases = await _cbeContext.Cases.FindAsync(CaseId);
            var constMngAgrMachinery = await _cbeContext.ConstMngAgrMachineries
                                .Include(res => res.EvaluatorUser)
                                    .ThenInclude(res => res.Signatures).ThenInclude(res => res.SignatureFile)
                                 .Include(res => res.CheckerUser)
                                    .ThenInclude(res => res.Signatures).ThenInclude(res => res.SignatureFile).Where(res => res.Collateral.CaseId == CaseId && res.Collateral.CurrentStatus == "Complete" && res.Collateral.CurrentStage == "Checker Officer").ToListAsync();
            var collaterals = await _cbeContext.Collaterals.Where(res => res.CaseId == CaseId && res.Category == MechanicalCollateralCategory.CMAMachinery && res.CurrentStatus == "Complete" && res.CurrentStage == "Checker Officer").ToListAsync();
            var caseSchedule = await _cbeContext.CaseSchedules.Where(res => res.CaseId == CaseId && res.Status == "Approved").FirstOrDefaultAsync();
            ViewData["cases"] = cases;
            ViewData["collaterals"] = collaterals;
            ViewData["constMngAgrMachinery"] = constMngAgrMachinery;
            ViewData["caseSchedule"] = caseSchedule;
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> ConstMngAgrMachineryReport(Guid CaseId)
        {
            var cases = await _cbeContext.Cases.FindAsync(CaseId);
            var constMngAgrMachinery = await _cbeContext.ConstMngAgrMachineries
                                .Include(res => res.EvaluatorUser)
                                    .ThenInclude(res => res.Signatures).ThenInclude(res=>res.SignatureFile)
                                 .Include(res => res.CheckerUser)
                                    .ThenInclude(res => res.Signatures).ThenInclude(res => res.SignatureFile).Where(res => res.Collateral.CaseId == CaseId && res.Collateral.CurrentStatus == "Complete" && res.Collateral.CurrentStage == "Checker Officer").ToListAsync();
            var collaterals = await _cbeContext.Collaterals.Where(res => res.CaseId == CaseId && res.Category == MechanicalCollateralCategory.CMAMachinery &&  res.CurrentStatus == "Complete" && res.CurrentStage == "Checker Officer" ).ToListAsync();
            var caseSchedule =await _cbeContext.CaseSchedules.Where(res=>res.CaseId == CaseId && res.Status == "Approved").FirstOrDefaultAsync();
            ViewData["cases"] = cases;
            ViewData["collaterals"] = collaterals;
            ViewData["constMngAgrMachinery"] = constMngAgrMachinery;
            ViewData["caseSchedule"] = caseSchedule;
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> IndBldgFacilityEquipmentSummary(Guid CaseId)
        {
            var cases = await _cbeContext.Cases.FindAsync(CaseId);

            var IndBldgFacilityEquipment = await _cbeContext.IndBldgFacilityEquipment
                               .Include(res => res.EvaluatorUser)
                                   .ThenInclude(res => res.Signatures).ThenInclude(res => res.SignatureFile)
                                .Include(res => res.CheckerUser)
                                   .ThenInclude(res => res.Signatures).ThenInclude(res => res.SignatureFile).Where(res => res.Collateral.CaseId == CaseId && res.Collateral.CurrentStatus == "Complete" && res.Collateral.CurrentStage == "Checker Officer").ToListAsync();


            var collaterals = await _cbeContext.Collaterals.Where(res => res.CaseId == CaseId && res.Category == MechanicalCollateralCategory.IBFEqupment && res.CurrentStatus == "Complete" && res.CurrentStage == "Checker Officer").ToListAsync();
            var caseSchedule = await _cbeContext.CaseSchedules.Where(res => res.CaseId == CaseId && res.Status == "Approved").FirstOrDefaultAsync();
            ViewData["cases"] = cases;
            ViewData["collaterals"] = collaterals;
            ViewData["IndBldgFacilityEquipment"] = IndBldgFacilityEquipment;
            ViewData["caseSchedule"] = caseSchedule;
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> IndBldgFacilityEquipmentReport(Guid CaseId)
        {
            var cases = await _cbeContext.Cases.FindAsync(CaseId);
           
            var IndBldgFacilityEquipment = await _cbeContext.IndBldgFacilityEquipment
                               .Include(res => res.EvaluatorUser)
                                   .ThenInclude(res => res.Signatures).ThenInclude(res => res.SignatureFile)
                                .Include(res => res.CheckerUser)
                                   .ThenInclude(res => res.Signatures).ThenInclude(res => res.SignatureFile).Where(res => res.Collateral.CaseId == CaseId && res.Collateral.CurrentStatus == "Complete" && res.Collateral.CurrentStage == "Checker Officer").ToListAsync();


            var collaterals = await _cbeContext.Collaterals.Where(res => res.CaseId == CaseId && res.Category == MechanicalCollateralCategory.IBFEqupment && res.CurrentStatus == "Complete" && res.CurrentStage == "Checker Officer").ToListAsync();
            var caseSchedule = await _cbeContext.CaseSchedules.Where(res => res.CaseId == CaseId && res.Status == "Approved").FirstOrDefaultAsync();
            ViewData["cases"] = cases;
            ViewData["collaterals"] = collaterals;
            ViewData["IndBldgFacilityEquipment"] = IndBldgFacilityEquipment;
            ViewData["caseSchedule"] = caseSchedule;
            return View();
        }
        [HttpGet]
        public IActionResult MyCompleteCases()
        {
            
            return View();
        }
        [HttpGet]
        public IActionResult ReestimationCases()
        {
            return View();
        }
        [HttpGet]
        public IActionResult TotalCases()
        {
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> ReestimationCase(Guid id)
        {

            var loanCase = await _caseService.GetCase(base.GetCurrentUserId(), id);
            var caseSchedule = await _caseScheduleService.GetCaseSchedules(id);
            //var motorvechiel = await _cbeContext.MotorVehicles.Where(res => res.Collaterial.CaseId == CaseId).ToListAsync();
            if (loanCase == null) { return RedirectToAction("GetCompleteCases"); }
            ViewData["case"] = loanCase;
            ViewData["CaseSchedule"] = caseSchedule;
            ViewData["Id"] = base.GetCurrentUserId();
            var moFile = await _uploadFileService.GetMoUploadFile(id);
            ViewData["moFile"] = moFile;
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> MyCompleteCase(Guid id)
        {
            
            var loanCase = await _caseService.GetCase(base.GetCurrentUserId(), id);
            var caseSchedule = await _caseScheduleService.GetCaseSchedules(id);
            //var motorvechiel = await _cbeContext.MotorVehicles.Where(res => res.Collaterial.CaseId == CaseId).ToListAsync();
            if (loanCase == null) { return RedirectToAction("GetCompleteCases"); }
            ViewData["case"] = loanCase;
            ViewData["CaseSchedule"] = caseSchedule;
            ViewData["Id"] = base.GetCurrentUserId();
            List<MotorVehicle> motorVehicle = null;
            try
            {
                motorVehicle = await _cbeContext.MotorVehicles.Where(res => res.Collateral.CaseId == id && res.Collateral.CurrentStatus == "Complete" && res.Collateral.CurrentStage == "Checker Officer").ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while retrieving motor vehicles: {ex.Message}");
            }
            List<ConstMngAgrMachinery> conMngAgr = null;
            try
            {
                conMngAgr = await _cbeContext.ConstMngAgrMachineries.Where(res => res.Collateral.CaseId == id && res.Collateral.CurrentStatus == "Complete" && res.Collateral.CurrentStage == "Checker Officer").ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while retrieving motor vehicles: {ex.Message}");
            }
            List<IndBldgFacilityEquipment> indBldgFacEq = null;
            try
            {
                indBldgFacEq = await _cbeContext.IndBldgFacilityEquipment.Where(res => res.Collateral.CaseId == id && res.Collateral.CurrentStatus == "Complete" && res.Collateral.CurrentStage == "Checker Officer").ToListAsync();
            }
            catch (Exception ex)
            {
                // Handle the exception (e.g., log the error, display a message, etc.)
                Console.WriteLine($"An error occurred while retrieving motor vehicles: {ex.Message}");
            }
            ViewData["motorVehicle"] = motorVehicle;
            ViewData["indBldgFacEq"] = indBldgFacEq;
            ViewData["conMngAgr"] = conMngAgr;
            var moFile = await _uploadFileService.GetMoUploadFile(id);
            ViewData["moFile"] = moFile;
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> GetCompleteCases()
        {
            var myCase = await _caseService.GetRmCompleteCases(base.GetCurrentUserId());
            string jsonData = JsonConvert.SerializeObject(myCase);
            return Content(jsonData, "application/json");
        }
        [HttpGet]
        public async Task<IActionResult> GetTotalCases()
        {
            var id = base.GetCurrentUserId();
            var myCase= new object();
            var userRole= await _cbeContext.CreateUsers.Where(res=>res.Id == id).Include(res=>res.Role).FirstOrDefaultAsync();
            if (userRole.Role.Name == "Relation Manager")
            {
                 myCase = await _caseService.GetRmTotalCases(id);
            }
            else
            {
                 myCase = await _caseService.GetTotalCases(id);
            }
            string jsonData = JsonConvert.SerializeObject(myCase);
            return Content(jsonData, "application/json");
        }
        [HttpPost]
        public async Task<IActionResult> CreateTermination(CaseTerminatePostDto caseTerminatePostDto)
        {
            var caseTerminate = await _caseTermnateService.CreateCaseTerminate(base.GetCurrentUserId(), caseTerminatePostDto);
            if (caseTerminate == null) { return BadRequest("Unable to Create case Schdule"); }
            var CaseInfo = await _caseService.GetCaseDetail(caseTerminate.CaseId);
            await _mailService.SendEmail(new MailPostDto
            {
                SenderEmail = "getnetadane1@cbe.com.et",
                SenderPassword = "Gechlove@1234",
                RecipantEmail = "yohannessintayhu@cbe.com.et",
                Subject = "Valuation Schedule for Case Number " + CaseInfo.CaseNo,
                Body = "Dear! Case Termination request  For Applicant:-" + CaseInfo.ApplicantName + " Is " + caseTerminate.Reason + " For further Detail please check Collateral Valuation System",
            });
            string jsonData = JsonConvert.SerializeObject(caseTerminate);
            return Ok(caseTerminate);
        }
        [HttpPost]
        public async Task<IActionResult> UpdateTermination(Guid Id,CaseTerminatePostDto caseTerminatePostDto)
        {
            var caseTerminate = await _caseTermnateService.UpdateCaseTerminate(base.GetCurrentUserId(), Id, caseTerminatePostDto);
            if (caseTerminate == null) { return BadRequest("Unable to update case Termination"); }
            return Ok();
        }
        [HttpPost]
        public async Task<IActionResult> ApproveCaseTermination(Guid Id)
        {
            var caseSchedule = await _caseService.ApproveCaseTermination(Id);
            if (caseSchedule == null) { return BadRequest("Unable to update case Schdule"); }
            return Ok();
        }

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
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> SendEvaluation(Guid CaseId)
        //{
        //    if(!await _caseService.SendEvaluation(CaseId))
        //    {
        //        return RedirectToAction("Index","Case");
        //    }
        //    return RedirectToAction("MyCases", "Case");
        //}

        //[HttpPost]
        //public async Task<IActionResult> AssignCaseOfficer(CaseAssignmentDto caseAssignmentDto)
        //{
        //   await _assignmentService.CreateCaseAssignment(caseAssignmentDto);
        //    var cases = await _cbeContext.Cases.FindAsync(caseAssignmentDto.CaseId);
        //    cases.CurrentStatus = "Pending";
        //    _cbeContext.Cases.Update(cases);
        //    await _cbeContext.SaveChangesAsync();
        //   var response = new { message = "Case successfully assigned" };
        //   return Ok(response);
        //}

    }
}