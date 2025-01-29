using mechanical.Data;
using mechanical.Models.Dto.CaseDto;
using mechanical.Models.Dto.IndBldgFacilityEquipmentDto;
using mechanical.Models.Dto.MailDto;
using mechanical.Models.Dto.MotorVehicleDto;
using mechanical.Models.Entities;
using mechanical.Services.CaseScheduleService;
using mechanical.Services.CaseServices;
using mechanical.Services.CollateralService;
using mechanical.Services.IndBldgF;
using mechanical.Services.MailService;
using mechanical.Services.MotorVehicleService;
using mechanical.Services.UploadFileService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace mechanical.Controllers
{
    public class IndBldgFacilityEquipmentController : BaseController
    {
        private readonly ICollateralService _collateralService;
        private readonly IIndBldgFacilityEquipmentService _indBldgFacilityEquipment;
        private readonly IUploadFileService _uploadFileService;
        private readonly CbeContext _cbeContext;
        private readonly IMailService _mailService;
        private readonly ICaseScheduleService _caseScheduleService;
        public IndBldgFacilityEquipmentController(IUploadFileService uploadFileService, ICaseScheduleService caseScheduleService, IMailService mailService, ICollateralService collateralService,IIndBldgFacilityEquipmentService indBldgFacilityEquipment , IMotorVehicleService motorVehicleService,CbeContext cbeContext)
        {
            _collateralService = collateralService;
            _indBldgFacilityEquipment = indBldgFacilityEquipment;
            _cbeContext = cbeContext;
            _uploadFileService = uploadFileService;
            _mailService = mailService;
            _caseScheduleService = caseScheduleService;
        }


        [HttpGet]
        public async Task<IActionResult> Create(Guid Id)
        {
            var collateral = await _collateralService.GetCollateral(base.GetCurrentUserId(),Id);
            var scheduledDate = await _caseScheduleService.GetApprovedCaseSchedule(collateral.CaseId);

            if (scheduledDate == null)
            {
                return Json(new { success = false, message = "Please first set a schedule date befor making evaluation." });
            }
            else if (scheduledDate.ScheduleDate > DateTime.Now)
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
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CheckAssessment(Guid Id, IndBldgFacilityEquipmentPostDto indBldgFacilityEquipment)
        {
            var indBldgFacility = await _indBldgFacilityEquipment.CheckIndBldgFacilityEquipment(base.GetCurrentUserId(), Id, indBldgFacilityEquipment);
            var redirectUrl = Url.Action("GetCheckIndBldgFacilityEquipment", "IndBldgFacilityEquipment", indBldgFacility);
            return Json(new { redirectUrl });
        }
        [HttpGet]
        public async Task<IActionResult> GetCheckIndBldgFacilityEquipment(IndBldgFacilityEquipmentReturnDto indBldgFacilityEquipmentReturnDto)
        {
            //var motorVehicleDto = await _motorVehicleService.GetMotorVehicle(Id);
            return View(indBldgFacilityEquipmentReturnDto);
        }

        [HttpPost]
        public async Task<IActionResult> RemarkRelease(Guid Id, Guid CollateralId, String Remark, Guid EvaluatorUserID)
        {
            var Mov = await _cbeContext.IndBldgFacilityEquipment.FindAsync(Id);
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
        public async Task<IActionResult> Create(IndBldgFacilityEquipmentPostDto indBldgFacilityEquipment)
        {
            var userid = base.GetCurrentUserId();
            if (ModelState.IsValid)
            {
                var indBldgFacility = await _indBldgFacilityEquipment.CreateIndBldgFacilityEquipment(base.GetCurrentUserId(),indBldgFacilityEquipment);

                var collateral = await _cbeContext.Collaterals.FindAsync(indBldgFacility.CollateralId);
                collateral.CurrentStatus = "Pending";

                _cbeContext.Update(collateral);

                var caseAssignment = await _cbeContext.CaseAssignments.FirstOrDefaultAsync(a => a.CollateralId == collateral.Id && a.UserId == userid);
                caseAssignment.Status = "Pending";
                _cbeContext.Update(caseAssignment);
                await _cbeContext.SaveChangesAsync();
                return RedirectToAction("GetIndBldgFacilityEquipment", "IndBldgFacilityEquipment", new { Id = indBldgFacility.Id });
            }
            return View(indBldgFacilityEquipment);
        }
        [HttpGet]
        public async Task<IActionResult> GetIndBldgFacilityEquipment(Guid Id)
        {
            var indBldgFacilityEquipment = await _indBldgFacilityEquipment.GetIndBldgFacilityEquipment(Id);
            return View(indBldgFacilityEquipment);
        }
        [HttpGet]
        public async Task<IActionResult> GetIndBldgFacilityEquipmentSummary(Guid CaseId)
        {
            var IndBldgFacilityEquipments = await _cbeContext.IndBldgFacilityEquipment.Where(res => res.Collateral.CaseId == CaseId).ToListAsync();
            string jsonData = JsonConvert.SerializeObject(IndBldgFacilityEquipments);
            return Content(jsonData, "application/json");
        }
        public async Task<IActionResult> GetEvaluatedIndBldgFacilityEquipment(Guid Id)
        {
            var indBldgFacilityEquipment = await _indBldgFacilityEquipment.GetEvaluatedIndBldgFacilityEquipment(Id);
            var comments = await _indBldgFacilityEquipment.GetCollateralComment(Id);
            ViewData["comments"] = comments;
            ViewData["indBldgFacilityEquipmentReturnDto"]  = await _cbeContext.IndBldgFacilityEquipment.FirstOrDefaultAsync(res => res.CollateralId == Id);
            return View(indBldgFacilityEquipment);
        }

        [HttpGet]
        public async Task<IActionResult> EditIndBldgFacilityEquipment(Guid id)
        {

            var indBldgFacilityEquipmentReturnDto = await _cbeContext.IndBldgFacilityEquipment.FirstOrDefaultAsync(res => res.CollateralId == id);
            //ViewData["EvaluatedMOV"] = motorVehicleDto;

            return View(indBldgFacilityEquipmentReturnDto);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, IndBldgFacilityEquipmentPostDto indBldgFacilityEquipmentPostDto)
        {
            await _indBldgFacilityEquipment.EditIndBldgFacilityEquipment(id, indBldgFacilityEquipmentPostDto);
            return RedirectToAction("GetIndBldgFacilityEquipment", "IndBldgFacilityEquipment", new { Id = id });
        }

        public async Task<IActionResult> GetReturnedEvaluatedIndBldgFacilityEquipment(Guid Id)
        {
            var indBldgFacilityEquipment = await _indBldgFacilityEquipment.GetReturnedEvaluatedIndBldgFacilityEquipment(Id);
            var comments = await _indBldgFacilityEquipment.GetCollateralComment(Id);
            ViewData["comments"] = comments;
            ViewData["collateralFile"] = await _uploadFileService.GetUploadFileByCollateralId(Id);
            return View(indBldgFacilityEquipment);
        }
        //edit the collateral based on the corrction comment 
        public async Task<IActionResult> EditIndBldgFacilityEquipment(Guid Id, IndBldgFacilityEquipmentPostDto indBldgFacilityEquipment)
        {
            
                //var motrvechel = await _motorVehicleService.EditMotorVehicle(Id, createMotorVehicleDto);
                var motrvechel = await _indBldgFacilityEquipment.EditIndBldgFacilityEquipment(Id, indBldgFacilityEquipment);

                var motorVechelAssesment = await _cbeContext.IndBldgFacilityEquipment.FirstOrDefaultAsync(res => res.Id == Id);
                var collateral = await _cbeContext.Collaterals.FindAsync(indBldgFacilityEquipment.CollateralId);
                Guid? checkerID = Guid.Empty;
                if (motorVechelAssesment.CheckerUserID == null)
                {
                    var correction = await _cbeContext.Corrections.FirstOrDefaultAsync(res => res.CollateralID == motrvechel.CollateralId);

                    //var correction = await _cbeContext.Corrections.FirstOrDefaultAsync(res => res.CollateralID == motrvechel.CollateralId);
                    checkerID = correction.CommentedByUserId;
                }
                else
                {
                    checkerID = motorVechelAssesment.CheckerUserID;
                }
            //var caseAssignment = await _cbeContext.CaseAssignments.Where(res => res.CollateralId == indBldgFacilityEquipment.CollateralId && res.UserId == correction.CommentedByUserId).FirstOrDefaultAsync();
            var userid = base.GetCurrentUserId();
            var caseAssignmentChange = await _cbeContext.CaseAssignments.Where(res => res.UserId == userid && res.CollateralId == indBldgFacilityEquipment.CollateralId).FirstOrDefaultAsync();
                    caseAssignmentChange.Status = "Pending";
            var caseAssignment = await _cbeContext.CaseAssignments.Where(res => res.CollateralId == indBldgFacilityEquipment.CollateralId && res.UserId == checkerID).FirstOrDefaultAsync();
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

                caseAssignment.AssignmentDate = DateTime.Now;
                _cbeContext.Update(caseAssignment);
                _cbeContext.Update(caseAssignmentChange);

            collateral.CurrentStage = "Checker Officer";

                _cbeContext.Update(collateral);
                await _cbeContext.SaveChangesAsync();
               
                return RedirectToAction("MyReturnedCollaterals", "MoCase");
            
        }
        
    }
}
