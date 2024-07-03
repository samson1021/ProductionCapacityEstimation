﻿using mechanical.Services.CaseServices;
using mechanical.Services.CollateralService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Data;
using mechanical.Models.Enum;
using mechanical.Models;
using mechanical.Models.Entities;
using mechanical.Services.MOCaseService;
using mechanical.Services.MMCaseService;
using mechanical.Services.CaseScheduleService;
using mechanical.Models.Dto.CaseScheduleDto;
using System.Net.Mail;
using mechanical.Services.MailService;
using mechanical.Models.Dto.MailDto;
using mechanical.Models.Dto.MotorVehicleDto;
using mechanical.Services.CaseTerminateService;

namespace mechanical.Controllers
{
    [Authorize(Roles = "Maker Manager,District Valuation Manager ,Maker Officer, Maker TeamLeader, Relation Manager,Checker Manager, Checker TeamLeader, Checker Officer")]
    public class MOCaseController : BaseController
    {
        private readonly ICaseService _caseService;
        private readonly IMMCaseService _mOCaseService;
        private readonly ICOCaseService _cOCaseService;
        private readonly ICollateralService _collateralService;
        private readonly ICaseScheduleService _caseScheduleService;
        private readonly IMailService _mailService;
        private readonly ICaseTerminateService _caseTermnateService;
        public MOCaseController(ICaseService caseService, ICaseTerminateService caseTermnateService, ICollateralService collateralService,ICaseScheduleService caseScheduleService, IMMCaseService mOCaseService,ICOCaseService coCaseService, IMailService mailService)
        {
            _caseService = caseService;
            _collateralService = collateralService;
            _mOCaseService = mOCaseService;
            _cOCaseService = coCaseService;
            _caseScheduleService = caseScheduleService;
            _mailService = mailService;
            _caseTermnateService = caseTermnateService;
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
            return View();
        }
        //[HttpGet]
        //public async Task<IActionResult> ReAssignment(Guid Id)
        //{   


        //}
        [HttpGet]
        public async Task<IActionResult> GetRemarkedCases()
        {
            var myCase = await _mOCaseService.GetMoRemarkedCases(GetCurrentUserId());
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
        public IActionResult MypendingCase()
        {

            return View();
        }
        [HttpGet]
        public async Task<IActionResult> GetMyPendingCases()
        {
            var myCase = await _mOCaseService.GetMMPendingCases(GetCurrentUserId());
            if (myCase == null) { return BadRequest("Unable to load case"); }
            string jsonData = JsonConvert.SerializeObject(myCase);
            return Content(jsonData, "application/json");
        }
        [HttpGet]
        public async Task<IActionResult> Evaluation(Guid Id)
        {
            var collateral = await _collateralService.GetCollateral(base.GetCurrentUserId(),Id);
            if (collateral.Category == EnumHelper.GetEnumDisplayName(MechanicalCollateralCategory.MOV))
            {
                return RedirectToAction("Create", "MotorVehicle", new { Id = Id });
            }
            else if (collateral.Category == EnumHelper.GetEnumDisplayName(MechanicalCollateralCategory.CMAMachinery))
            {
                return RedirectToAction("Create", "ConstMngAgrMachinery", new { Id = Id });
            }
            else if (collateral.Category == EnumHelper.GetEnumDisplayName(MechanicalCollateralCategory.IBFEqupment))
            {
                return RedirectToAction("Create", "IndBldgFacilityEquipment", new { Id = Id });
            }
            string jsonData = JsonConvert.SerializeObject(collateral);
            return Content(jsonData, "application/json");
        }
        public async Task<IActionResult>ReEvaluation(Guid Id)
        {
            var collateral = await _collateralService.GetCollateral(base.GetCurrentUserId(), Id);
            if (collateral.Category == EnumHelper.GetEnumDisplayName(MechanicalCollateralCategory.MOV))
            {
                return RedirectToAction("GetReturnedEvaluatedMoterVehicle", "MotorVehicle", new { Id = Id });
            }
            else if (collateral.Category == EnumHelper.GetEnumDisplayName(MechanicalCollateralCategory.CMAMachinery))
            {
                return RedirectToAction("GetReturnedEvaluatedConstMngAgrMachinery", "ConstMngAgrMachinery", new { Id = Id });
            }
            else if (collateral.Category == EnumHelper.GetEnumDisplayName(MechanicalCollateralCategory.IBFEqupment))
            {
                return RedirectToAction("GetReturnedEvaluatedIndBldgFacilityEquipment", "IndBldgFacilityEquipment", new { Id = Id });
            }
            string jsonData = JsonConvert.SerializeObject(collateral);
            return Content(jsonData, "application/json");
        }
        [HttpGet]
        public async Task<IActionResult> MyPendDetail(Guid Id)
        {
            var loanCase = await _caseService.GetCaseDetail(Id);
            var caseSchedule = await _caseScheduleService.GetCaseSchedules(Id);
            if (loanCase == null) { return RedirectToAction("NewCases"); }
            ViewData["case"] = loanCase;
            ViewData["CaseSchedule"] = caseSchedule;
            ViewData["Id"] = base.GetCurrentUserId();
            return View();
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
            ViewData["Id"]=base.GetCurrentUserId();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateSchedule(CaseSchedulePostDto CaseSchedulePostDto)
        {
            var caseSchedule = await _caseScheduleService.CreateCaseSchedule(base.GetCurrentUserId(), CaseSchedulePostDto);
            if (caseSchedule == null) { return BadRequest("Unable to Create case Schdule"); }
            var CaseInfo= await _caseService.GetCaseDetail(caseSchedule.CaseId);
            await _mailService.SendEmail(new MailPostDto
            {
                SenderEmail = "getnetadane1@cbe.com.et",
                SenderPassword = "Gechlove@1234",
                RecipantEmail = "yohannessintayhu@cbe.com.et",
                Subject = "Valuation Schedule for Case Number "+CaseInfo.CaseNo,
                Body  = "Dear! Valuation Schedule  For Applicant:-"+ CaseInfo.ApplicantName+" Is "+ caseSchedule.ScheduleDate + " For further Detail please check Collateral Valuation System",
            });
            string jsonData = JsonConvert.SerializeObject(caseSchedule);
            return Ok(caseSchedule);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateSchedule(Guid Id,CaseSchedulePostDto CaseSchedulePostDto)
        {
            var caseSchedule = await _caseScheduleService.UpdateCaseSchedule(base.GetCurrentUserId(),Id, CaseSchedulePostDto);
            if (caseSchedule == null) { return BadRequest("Unable to update case Schdule"); }
            var CaseInfo = await _caseService.GetCaseDetail(caseSchedule.CaseId);
            await _mailService.SendEmail(new MailPostDto
            {
                SenderEmail = "getnetadane1@cbe.com.et",
                SenderPassword = "Gechlove@1234",
                RecipantEmail = "yohannessintayhu@cbe.com.et",
                Subject = "Valuation Schedule Update for Case Number " + CaseInfo.CaseNo,
                Body = "Dear! Valuation Schedule Update  For Applicant:-" + CaseInfo.ApplicantName + " Is " + caseSchedule.ScheduleDate + " For further Detail please check Collateral Valuation System",
            });
            string jsonData = JsonConvert.SerializeObject(caseSchedule);
            return Ok(caseSchedule);
        }
        [HttpPost]
        public async Task<IActionResult> ApproveSchedule(Guid Id)
        {
            var caseSchedule = await _caseScheduleService.ApproveCaseSchedule(Id);
            if (caseSchedule == null) { return BadRequest("Unable to update case Schdule"); }
            string jsonData = JsonConvert.SerializeObject(caseSchedule);
            return Ok(caseSchedule);
        }
        //public async Task<IActionResult> MyReturnedCases()
        //{
        //    var collaterals = _caseService.MyReturnedCases();
        //    return View(collaterals);


        //}MyReturnedCollaterals
        public async Task<IActionResult> MyReturnedCollaterals(Guid CollateralId)
        {
            var collaterals = await _collateralService.MyReturnedCollaterals(base.GetCurrentUserId());
            return View(collaterals);
        }

        public async Task<IActionResult> MyReturnedCollateral(Guid CollateralId)
        {
            var collaterals =await _collateralService.MyReturnedCollateral( base.GetCurrentUserId(), CollateralId);
            return View(collaterals);
        }
        //public async Task<IActionResult> MyResubmitedCases()
        //{
        //    var collaterals = _caseService.MyResubmitedCases();
        //    return View(collaterals);


        //}
        //public async Task<IActionResult> MyResubmitedCase(Guid CollateralId)
        //{
        //    var collaterals = _caseService.MyReturnedCase(CollateralId);
        //    return View(collaterals);
        //}


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SendCheking(Guid CollateralId)
        {
            if (!await _cOCaseService.SendCheking(base.GetCurrentUserId(), CollateralId))
            {
                return RedirectToAction("Index", "Case");
            }
            return RedirectToAction("MypendingCase", "MOCase");
        }



    }
}
