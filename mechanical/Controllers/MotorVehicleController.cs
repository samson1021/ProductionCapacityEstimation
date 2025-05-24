using mechanical.Data;
using mechanical.Models.Dto.CaseDto;
using mechanical.Models.Dto.ConstMngAgrMachineryDto;
using mechanical.Models.Dto.MailDto;
using mechanical.Models.Dto.MotorVehicleDto;
using mechanical.Models.Entities;
using mechanical.Services.CaseScheduleService;
using mechanical.Services.CaseServices;
using mechanical.Services.CollateralService;
using mechanical.Services.ConstMngAgrMachineryService;
using mechanical.Services.MailService;
using mechanical.Services.MotorVehicleService;
using mechanical.Services.UploadFileService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Dynamic;

namespace mechanical.Controllers
{
    public class MotorVehicleController : BaseController
    {
        private readonly ICollateralService _collateralService;
        private readonly IMotorVehicleService _motorVehicleService;
        private readonly IConstMngAgrMachineryService _constMngAgriMachineryService;
        private readonly CbeContext _cbeContext;
        private readonly IUploadFileService _uploadFileService;
        private readonly IMailService _mailService;
        private readonly ICaseScheduleService _caseScheduleService;
        public MotorVehicleController(ICollateralService collateralService, ICaseScheduleService caseScheduleService, IConstMngAgrMachineryService constMngAgriMachineryService, IMailService mailService, IMotorVehicleService motorVehicleService, CbeContext cbeContext, IUploadFileService uploadFileService)
        {
            _collateralService = collateralService;
            _motorVehicleService = motorVehicleService;
            _cbeContext = cbeContext;
            _uploadFileService = uploadFileService;
            _mailService = mailService;
            _constMngAgriMachineryService = constMngAgriMachineryService;
            _caseScheduleService = caseScheduleService;
        }
        [HttpGet]
        public async Task<IActionResult> Create(Guid Id)
        {
            var collateral = await _collateralService.GetCollateral(base.GetCurrentUserId(), Id);
            var scheduledDate = await _caseScheduleService.GetApprovedCaseSchedule(collateral.CaseId);

            if (scheduledDate == null)
            {
                return Json(new { success = false, message = "Please first set a schedule date befor making evaluation." });
            }
            else if (scheduledDate.ScheduleDate > DateTime.UtcNow)
            {
                return Json(new { success = false, message = "Please you can't make evaluation before the approve date" });
            }

            if (collateral == null)
            {
                return RedirectToAction("MyCase", "MOCase");
            }
            ViewData["collateral"] = collateral;
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Currency([FromForm] string currency)
        {
            var currencyDate = DateTime.UtcNow;
            var exchangeRate = await _motorVehicleService.Currency(currency, currencyDate);
            return Json(new { exchangeRate });
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateMotorVehicleDto createMotorVehicleDto)
        {
            var userid = base.GetCurrentUserId();
            if (ModelState.IsValid)
            {
                var motorVehicle = await _motorVehicleService.CreateMotorVehicle(base.GetCurrentUserId(), createMotorVehicleDto);

                var collateral = await _cbeContext.Collaterals.FindAsync(createMotorVehicleDto.CollateralId);
                collateral.CurrentStatus = "Pending";

                _cbeContext.Update(collateral);

                var caseAssignment = await _cbeContext.CaseAssignments.FirstOrDefaultAsync(a => a.CollateralId == collateral.Id && a.UserId == userid);
                caseAssignment.Status = "Pending";
                _cbeContext.Update(caseAssignment);

                await _cbeContext.SaveChangesAsync();
                return RedirectToAction("GetMotorVehicle", "MotorVehicle", new { Id = motorVehicle.Id });
            }
            return View(createMotorVehicleDto);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CheckAssesment(Guid Id, CreateMotorVehicleDto createMotorVehicleDto)
        {
            var motorVehicle = await _motorVehicleService.CheckMotorVehicle(base.GetCurrentUserId(), Id, createMotorVehicleDto);
            var redirectUrl = Url.Action("GetCheckMotorVehicle", "MotorVehicle", motorVehicle);
            return Json(new { redirectUrl });
        }
        [HttpGet]
        public async Task<IActionResult> GetCheckMotorVehicle(ReturnMotorVehicleDto returnMotorVehicleDto)
        {
            //var motorVehicleDto = await _motorVehicleService.GetMotorVehicle(Id);
            return View(returnMotorVehicleDto);
        }
        [HttpGet]
        public async Task<IActionResult> GetMotorVehicle(Guid Id)
        {
            var motorVehicleDto = await _motorVehicleService.GetMotorVehicle(Id);
            return View(motorVehicleDto);
        }


        [HttpGet]
        public async Task<IActionResult> GetMOVSummary(Guid CaseId)
        {
            var motorvechiel = await _cbeContext.MotorVehicles.Where(res => res.Collateral.CaseId == CaseId).ToListAsync();
            string jsonData = JsonConvert.SerializeObject(motorvechiel);
            return Content(jsonData, "application/json");
        }
        public async Task<IActionResult> GetEvaluatedMoterVehicle(Guid Id)
        {
            var motorVehicleDto = await _motorVehicleService.GetEvaluatedMotorVehicle(Id);
            var comments = await _constMngAgriMachineryService.GetCollateralComment(Id);
            ViewData["comments"] = comments;
            ViewData["motorVehicleDto"] = await _cbeContext.MotorVehicles.FirstOrDefaultAsync(res => res.CollateralId == Id);
            return View(motorVehicleDto);
        }

        [HttpGet]
        public async Task<IActionResult> EditMoterVehicle(Guid id)
        {

            var motorVehicleDto = await _cbeContext.MotorVehicles.FirstOrDefaultAsync(res => res.CollateralId == id);
            //ViewData["EvaluatedMOV"] = motorVehicleDto;

            return View(motorVehicleDto);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, CreateMotorVehicleDto createMotorVehicleDto)
        {
            await _motorVehicleService.EditMotorVehicle(id, createMotorVehicleDto);
            return RedirectToAction("GetMotorVehicle", "MotorVehicle", new { Id = id });
        }

        public async Task<IActionResult> GetReturnedEvaluatedMoterVehicle(Guid Id)
        {
            var motorVehicleDto = await _motorVehicleService.GetEvaluatedMotorVehicle(Id);
            var comments = await _constMngAgriMachineryService.GetCollateralComment(Id);
            ViewData["comments"] = comments;
            ViewData["collateralFile"] = await _uploadFileService.GetUploadFileByCollateralId(Id);
            return View(motorVehicleDto);
        }
        [HttpPost]
        public async Task<IActionResult> RemarkRelease(Guid Id, Guid CollateralId, String Remark, Guid EvaluatorUserID)
        {
            var Mov = await _cbeContext.MotorVehicles.FindAsync(Id);
            Mov.Remark = Remark;
            _cbeContext.Update(Mov);
            var collateral = await _cbeContext.Collaterals.FindAsync(CollateralId);
            collateral.CurrentStage = "Checker Officer";
            collateral.CurrentStatus = "Complete";
            _cbeContext.Update(collateral);

            var caseAssignment = await _cbeContext.CaseAssignments.Where(res => res.UserId == EvaluatorUserID && res.CollateralId == CollateralId).FirstOrDefaultAsync();
            caseAssignment.Status = "Complete";
            _cbeContext.Update(caseAssignment);
            _cbeContext.SaveChanges();
            await _mailService.SendEmail(new MailPostDto
            {
                SenderEmail = " getnetadane1@cbe.com.et",
                SenderPassword = "Gechlove@1234",
                RecipantEmail = "yohannessintayhu@cbe.com.et",
                Subject = "Remark Release Update ",
                Body = "Dear! </br> Remark release Update  For Applicant:-" + collateral.PropertyOwner + "</br></br> For further Detail please check Collateral Valuation System",
            });
            return RedirectToAction("RemarkCases", "MoCase");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditMotorVehicle(Guid Id, CreateMotorVehicleDto createMotorVehicleDto)
        {
            var motrvechel = await _motorVehicleService.EditMotorVehicle(Id, createMotorVehicleDto);
            var motorVechelAssesment = await _cbeContext.MotorVehicles.FirstOrDefaultAsync(res => res.Id == Id);
            var collateral = await _cbeContext.Collaterals.FindAsync(createMotorVehicleDto.CollateralId);
            Guid? checkerID = Guid.Empty;
            if (motorVechelAssesment.CheckerUserID == null)
            {
                var correction = await _cbeContext.Corrections.FirstOrDefaultAsync(res => res.CollateralID == motrvechel.CollateralId);
                checkerID = correction.CommentedByUserId;
            }
            else
            {
                checkerID = motorVechelAssesment.CheckerUserID;
            }
            var userid = base.GetCurrentUserId();
            var caseAssignment = await _cbeContext.CaseAssignments.Where(res => res.CollateralId == createMotorVehicleDto.CollateralId && res.UserId == checkerID).FirstOrDefaultAsync();
            var collateralid = await _cbeContext.MotorVehicles.Where(res => res.Id == Id).FirstOrDefaultAsync();
            var caseAssignmentChange = await _cbeContext.CaseAssignments.Where(res => res.UserId == userid && res.CollateralId == collateralid.CollateralId).FirstOrDefaultAsync();
            caseAssignmentChange.Status = "Pending";
            if (collateral.CurrentStatus.Contains("Remark"))
            {
                collateral.CurrentStatus = "Remark Verfication";
                caseAssignment.Status = "Remark Verfication";
            }
            else
            {
                collateral.CurrentStatus = "New";
                caseAssignment.Status = "New";
            }

            caseAssignment.AssignmentDate = DateTime.UtcNow;
            _cbeContext.Update(caseAssignment);
            _cbeContext.Update(caseAssignmentChange);

            collateral.CurrentStage = "Checker Officer";

            _cbeContext.Update(collateral);
            await _cbeContext.SaveChangesAsync();
            return RedirectToAction("MyReturnedCollaterals", "MoCase");
        }

    }
}
