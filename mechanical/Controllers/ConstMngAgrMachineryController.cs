using mechanical.Data;
using mechanical.Models.Dto.CaseDto;
using mechanical.Models.Dto.ConstMngAgrMachineryDto;
using mechanical.Models.Dto.IndBldgFacilityEquipmentDto;
using mechanical.Models.Dto.MotorVehicleDto;
using mechanical.Models.Entities;
using mechanical.Services.CaseScheduleService;
using mechanical.Services.CaseServices;
using mechanical.Services.CollateralService;
using mechanical.Services.ConstMngAgrMachineryService;
using mechanical.Services.MailService;
using mechanical.Services.MotorVehicleService;
using mechanical.Services.UploadFileService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace mechanical.Controllers
{
    [Authorize(Roles = "Maker Manager,District Valuation Manager ,Maker Officer, Maker TeamLeader, Relation Manager,Checker Manager, Checker TeamLeader, Checker Officer")]
    public class ConstMngAgrMachineryController : BaseController
    {
        private readonly ICollateralService _collateralService;
        private readonly IConstMngAgrMachineryService _constMngAgriMachineryService;
        private readonly CbeContext _cbeContext;
        private readonly IUploadFileService _uploadFileService;
        private readonly IMailService _mailService;
        private readonly ICaseScheduleService _caseScheduleService;

        public ConstMngAgrMachineryController(ICollateralService collateralService, ICaseScheduleService caseScheduleService, IMailService mailService, IConstMngAgrMachineryService constMngAgrMachineryService, CbeContext cbeContext, IUploadFileService uploadFileService)
        {
            _collateralService = collateralService;
            _constMngAgriMachineryService = constMngAgrMachineryService;
            _cbeContext = cbeContext;
            _uploadFileService = uploadFileService;
            _mailService = mailService;
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
        public async Task<IActionResult> RemarkRelease(Guid Id, Guid CollateralId, String Remark, Guid EvaluatorUserID, String CurrentStatus)
        {
            var Mov = await _cbeContext.ConstMngAgrMachineries.FindAsync(Id);
            Mov.Remark = Remark;
            _cbeContext.Update(Mov);
            var collateral = await _cbeContext.Collaterals.FindAsync(CollateralId);

            if (CurrentStatus == "Remark")
            {

                collateral.CurrentStage = "Checker Officer";
                collateral.CurrentStatus = "Complete";
                _cbeContext.Update(collateral);

                var caseAssignment = await _cbeContext.CaseAssignments.Where(res => res.UserId == EvaluatorUserID && res.CollateralId == CollateralId).FirstOrDefaultAsync();
                caseAssignment.Status = "Complete";
                _cbeContext.Update(caseAssignment);
                _cbeContext.SaveChanges();
            }
            else
            {
                //collateral.CurrentStage = "Checker Officer";
                //collateral.CurrentStatus = "Complete";
                //_cbeContext.Update(collateral);

                //var caseAssignment = await _cbeContext.CaseAssignments.Where(res => res.UserId == EvaluatorUserID && res.CollateralId == CollaterialId).FirstOrDefaultAsync();
                //caseAssignment.Status = "Complete";
                //_cbeContext.Update(caseAssignment);
                //_cbeContext.SaveChanges();
            }
            return RedirectToAction("RemarkCases", "MoCase");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ConstMngAgrMachineryPostDto constMngAgrMachineryPostDto)
        {
            var userid = base.GetCurrentUserId();
            if (ModelState.IsValid)
            {
                var constMngAgrMachinery = await _constMngAgriMachineryService.CreateConstMngAgrMachinery(base.GetCurrentUserId(), constMngAgrMachineryPostDto);




                var collateral = await _cbeContext.Collaterals.FindAsync(constMngAgrMachinery.CollateralId);
                collateral.CurrentStatus = "Pending";

                _cbeContext.Update(collateral);

                var caseAssignment = _cbeContext.CaseAssignments.Where(a => a.UserId == userid).FirstOrDefault(a => a.CollateralId == collateral.Id);
                caseAssignment.Status = "Pending";
                _cbeContext.Update(caseAssignment);
                await _cbeContext.SaveChangesAsync();
                return RedirectToAction("GetConstMngAgrMachinery", "ConstMngAgrMachinery", new { Id = constMngAgrMachinery.Id });
            }
            return View(constMngAgrMachineryPostDto);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CheckAssessment(Guid Id, ConstMngAgrMachineryPostDto constMngAgrMachineryPostDto)
        {
            var constMngAgrMachinery = await _constMngAgriMachineryService.CheckConstMngAgrMachinery(base.GetCurrentUserId(), Id, constMngAgrMachineryPostDto);

            var redirectUrl = Url.Action("GetCheckConstMngAgrMachinery", "ConstMngAgrMachinery", constMngAgrMachinery);
            return Json(new { redirectUrl });
        }
        [HttpGet]
        public async Task<IActionResult> GetCheckConstMngAgrMachinery(ConstMngAgMachineryReturnDto constMngAgMachineryReturnDto)
        {
            //var motorVehicleDto = await _motorVehicleService.GetMotorVehicle(Id);
            return View(constMngAgMachineryReturnDto);
        }
        [HttpGet]
        public async Task<IActionResult> GetConstMngAgrMachinerySummary(Guid CaseId)
        {
            var ConstMngAgrMachinery = await _cbeContext.ConstMngAgrMachineries.Where(res => res.Collateral.CaseId == CaseId).ToListAsync();
            string jsonData = JsonConvert.SerializeObject(ConstMngAgrMachinery);
            return Content(jsonData, "application/json");
        }
        [HttpGet]
        public async Task<IActionResult> GetConstMngAgrMachinery(Guid Id)
        {
            var constMngAgMachinery = await _constMngAgriMachineryService.GetConstMngAgrMachinery(Id);
            return View(constMngAgMachinery);
        }
        public async Task<IActionResult> GetEvaluatedconstMngAgriMachinery(Guid Id)
        {

            var constMngAgMachinery = await _constMngAgriMachineryService.GetEvaluatedConstMngAgrMachinery(Id);
            var comments = await _constMngAgriMachineryService.GetCollateralComment(Id);
            ViewData["comments"] = comments;
            var chechConstruction = await _cbeContext.ConstMngAgrMachineries.FirstOrDefaultAsync(x => x.CollateralId == Id);
            ViewData["constMngAgMachinery"] = chechConstruction;
            return View(constMngAgMachinery);
        }
        public async Task<IActionResult> GetReturnedEvaluatedConstMngAgrMachinery(Guid Id)
        {
            var constMngAgMachinery = await _cbeContext.ConstMngAgrMachineries.FirstOrDefaultAsync(x => x.CollateralId == Id);
            var comments = await _constMngAgriMachineryService.GetCollateralComment(Id);
            ViewData["comments"] = comments;
            ViewData["collateralFile"] = await _uploadFileService.GetUploadFileByCollateralId(Id);
            return View(constMngAgMachinery);



        }
        [HttpGet]
        public async Task<IActionResult> EditConstMngAgrMachinery(Guid id)
        {

            var motorVehicleDto = await _cbeContext.ConstMngAgrMachineries.FirstOrDefaultAsync(x => x.CollateralId == id);
            //ViewData["EvaluatedMOV"] = motorVehicleDto;

            return View(motorVehicleDto);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, ConstMngAgrMachineryPostDto constMngAgrMachineryPostDto)
        {
            await _constMngAgriMachineryService.EditConstMngAgrMachinery(id, constMngAgrMachineryPostDto);
            return RedirectToAction("GetConstMngAgrMachinery", "ConstMngAgrMachinery", new { Id = id });
        }





        //edit the collateral based on the corrction comment 
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditConstMngAgrMachinery(Guid Id, ConstMngAgrMachineryPostDto constMngAgrMachineryPostDto)
        {
            var motrvechel = await _constMngAgriMachineryService.EditConstMngAgrMachinery(Id, constMngAgrMachineryPostDto);

            var motorVechelAssesment = await _cbeContext.ConstMngAgrMachineries.FirstOrDefaultAsync(res => res.Id == Id);
            var collateral = await _cbeContext.Collaterals.FindAsync(constMngAgrMachineryPostDto.CollateralId);
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
            var caseAssignmentChange = await _cbeContext.CaseAssignments.Where(res => res.UserId == userid && res.CollateralId == constMngAgrMachineryPostDto.CollateralId).FirstOrDefaultAsync();
            caseAssignmentChange.Status = "Pending";
            var caseAssignment = await _cbeContext.CaseAssignments.Where(res => res.CollateralId == constMngAgrMachineryPostDto.CollateralId && res.UserId == checkerID).FirstOrDefaultAsync();
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
