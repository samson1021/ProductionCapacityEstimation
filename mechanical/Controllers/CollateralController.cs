using mechanical.Models.Entities;
using mechanical.Models.Dto.CollateralDto;
using mechanical.Services.CaseServices;
using mechanical.Services.CollateralService;
using mechanical.Services.UploadFileService;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using mechanical.Models.Enum;
using mechanical.Services.MotorVehicleService;
using mechanical.Services.ConstMngAgrMachineryService;
using mechanical.Services.IndBldgF;
using mechanical.Data;
using Microsoft.EntityFrameworkCore;
using mechanical.Models;
using Microsoft.AspNetCore.Http;
using mechanical.Models.Dto.UploadFileDto;

namespace mechanical.Controllers
{
    public class CollateralController : BaseController
    {
        private readonly ICaseService _caseService;
        private readonly ICollateralService _collateralService;
        private readonly IUploadFileService _uploadFileService;
        private readonly IMotorVehicleService _motorVehicleService;
        private readonly IConstMngAgrMachineryService _constMngAgrMachineryService;
        private readonly IIndBldgFacilityEquipmentService _indBldgFacilityEquipmentService;
        private readonly CbeContext _cbeContext;
        public CollateralController(ICaseService caseService,IConstMngAgrMachineryService constMngAgrMachineryService, IIndBldgFacilityEquipmentService indBldgFacilityEquipmentService, ICollateralService collateralService, CbeContext cbeContext, IUploadFileService uploadFileService,IMotorVehicleService motorVehicleService)
        {
            _caseService = caseService;
            _collateralService = collateralService;
            _uploadFileService = uploadFileService;
            _motorVehicleService = motorVehicleService;
            _constMngAgrMachineryService = constMngAgrMachineryService;
            _indBldgFacilityEquipmentService = indBldgFacilityEquipmentService;
            _cbeContext = cbeContext;
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
     //   [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Guid caseId, CollateralPostDto collateralDto)
        {
            if (ModelState.IsValid)
            {
                await _collateralService.CreateCollateral(base.GetCurrentUserId(), caseId,collateralDto);
                var response = new { message = "Collateral created successfully" };
                return Ok(response);
            }
            return BadRequest();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateMoDocument(Guid CaseId, string DocumentType, IEnumerable<IFormFile>? Document)
        {
            try
            {
                await _collateralService.CreateMOFile(base.GetCurrentUserId(), CaseId, DocumentType, Document);
                var response = new { message = "Document created successfully" };
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateCivil(Guid caseId, CivilCollateralPostDto civilCollateralDto)
        {
            if (ModelState.IsValid)
            {
                await _collateralService.CreateCivilCollateral(base.GetCurrentUserId(), caseId, civilCollateralDto);
                var response = new { message = "Collateral created successfully" };
                return Ok(response);
            }
            return BadRequest();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateAgriculture(Guid caseId, AgricultureCollateralPostDto agricultureCollateralDto)
        {
            if (ModelState.IsValid)
            {
                await _collateralService.CreateAgricultureCollateral(base.GetCurrentUserId(), caseId, agricultureCollateralDto);
                var response = new { message = "Collateral created successfully" };
                return Ok(response);
            }
            return BadRequest();
        }
        [HttpGet]
        public async Task<IActionResult> GetCollaterals(Guid CaseId)
        {
            var collaterals = await _collateralService.GetCollaterals(CaseId);
            string jsonData = JsonConvert.SerializeObject(collaterals);
            return Content(jsonData, "application/json");
        }
        [HttpGet]
        public async Task<IActionResult> GetRejectCollaterals(Guid CaseId)
        {
            var collaterals = await _collateralService.GetRejectedCollaterals(CaseId);
            string jsonData = JsonConvert.SerializeObject(collaterals);
            return Content(jsonData, "application/json");
        }
        public async Task<IActionResult> GetPendCollaterals(Guid CaseId)
        {
            var collaterals = await _collateralService.GetPendCollaterals(CaseId);
            string jsonData = JsonConvert.SerializeObject(collaterals);
            return Content(jsonData, "application/json");
        }
        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            var response = await _collateralService.GetCollateral(base.GetCurrentUserId(), id);
            var file = await _uploadFileService.GetUploadFileByCollateralId(id);
            ViewData["collateralFiles"] = file;
            if (response == null) { return RedirectToAction("NewCases"); }
            return View(response);
        }
        [HttpPost]
        public async Task<IActionResult> handleRemark(Guid CollateralId, Guid EvaluatorUserID ,String RemarkType,CreateFileDto uploadFile, Guid CheckerUserID)
        {
            var caseAssignment = await _cbeContext.CaseAssignments.Where(res => res.CollateralId == CollateralId && res.UserId == EvaluatorUserID).FirstOrDefaultAsync();
            caseAssignment.Status = "Remark";
            _cbeContext.Update(caseAssignment);

            var collateral = await _cbeContext.Collaterals.Where(res => res.Id == CollateralId).FirstOrDefaultAsync();
            if(RemarkType == "Verfication")
            {
                collateral.CurrentStatus = "Remark Verfication";
            }
            else
            {
                collateral.CurrentStatus = "Remark Justfication";
            }
            if (uploadFile.File != null)
            {
                uploadFile.CaseId = collateral.CaseId;
                await _uploadFileService.CreateUploadFile(base.GetCurrentUserId(), uploadFile);
            }
            collateral.CurrentStage = "Maker Officer";
            _cbeContext.Update(collateral);
            _cbeContext.SaveChanges();

            return RedirectToAction("MyCompleteCases", "Case");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, CollateralPostDto collateralPostDto)
        {
            if (ModelState.IsValid)
            {
                var collateral = await _collateralService.EditCollateral(base.GetCurrentUserId(), id, collateralPostDto);
                if(collateral.CurrentStatus == "Reject")
                {
                    return RedirectToAction("RejectedDetail", "Case", new { Id = collateral.CaseId });
                }
                return RedirectToAction("Detail","Case", new { Id = collateral.CaseId });
            }
            return View();
        }


        [HttpPost]
        public async Task<ActionResult> DeleteCollateralFile(Guid Id)
        {
            if (await _collateralService.DeleteCollateralFile(base.GetCurrentUserId(), Id))
            {
                return Ok();
            }
            return BadRequest();
        }

        [HttpPost]
        public async Task<ActionResult> UploadCollateralFile(IFormFile BussinessLicence, Guid caseId, string DocumentCategory)
        {
            if (await _collateralService.UploadCollateralFile(base.GetCurrentUserId(), BussinessLicence, caseId, DocumentCategory))
            {
                return Ok();
            }
            return BadRequest();
        }

        [HttpGet]
        public async Task<IActionResult> Detail(Guid id)
        {
            var response = await _collateralService.GetCollateral(base.GetCurrentUserId(), id);
            var loanCase = await _caseService.GetCaseDetail(id);
           
            var restimation = await _cbeContext.CollateralReestimations.Where(res=> res.CollateralId == id).FirstOrDefaultAsync();
            if(restimation != null)
            {
                ViewData["restimation"] = restimation;
            }

            if (response == null) { return RedirectToAction("NewCases"); }
            var file = await _uploadFileService.GetUploadFileByCollateralId(id);
            var rejectedCollateral = await _cbeContext.Rejects.Where(res => res.CollateralId == id).FirstOrDefaultAsync();
            var remarkTypeCollateral = await _cbeContext.Collaterals.Where(res => res.Id == id).FirstAsync();

            if (rejectedCollateral!=null)
            {
                var user = await _cbeContext.Users.Include(res => res.Role).FirstOrDefaultAsync(rea => rea.Id == rejectedCollateral.RejectedBy);
                ViewData["user"] = user;

            }
            if (response.Category == EnumHelper.GetEnumDisplayName(MechanicalCollateralCategory.MOV))
            {
                var motorVehicleDto = await _motorVehicleService.GetMotorVehicleByCollateralId(id);
                ViewData["movaluation"] = motorVehicleDto;
            }
            else if (response.Category == EnumHelper.GetEnumDisplayName(MechanicalCollateralCategory.CMAMachinery))
            {
                var constMng = await _constMngAgrMachineryService.GetConstMngAgrMachineryByCollateralId(id);
                ViewData["Cavaluation"] = constMng;
            }
            else if (response.Category == EnumHelper.GetEnumDisplayName(MechanicalCollateralCategory.IBFEqupment))
            {
                var indBldgFacilityEquipment = await _indBldgFacilityEquipmentService.GetIndBldgFacilityEquipmentByCollateralId(id);
                ViewData["Ibvaluation"] = indBldgFacilityEquipment;
            }
            ViewData["case"] = loanCase;
            ViewData["collateralFiles"] = file;
            ViewData["rejectedCollateral"] = rejectedCollateral;
            ViewData["CurrentUserId"] = base.GetCurrentUserId();
            var userId = base.GetCurrentUserId();
            var UserForRole = await _cbeContext.Users.Where(res => res.Id==userId).FirstOrDefaultAsync();
            var role = await _cbeContext.Roles.Where(res => res.Id == UserForRole.RoleId).FirstOrDefaultAsync();
            ViewData["loggedRole"] = role;
            ViewData["remarkTypeCollateral"] = remarkTypeCollateral;
            return View(response);
        }

        [HttpGet]
        public async Task<IActionResult> GetRMRemarkCollaterals(Guid CaseId)
        {
            var collaterals = await _collateralService.GetRmRemarkCollaterals(base.GetCurrentUserId(), CaseId);
            string jsonData = JsonConvert.SerializeObject(collaterals);
            return Content(jsonData, "application/json");
        }
        [HttpGet]
        public async Task<IActionResult> GetRemarkCollaterals(Guid CaseId)
        {
            var collaterals = await _collateralService.GetRemarkCollaterals(base.GetCurrentUserId(), CaseId);
            string jsonData = JsonConvert.SerializeObject(collaterals);
            return Content(jsonData, "application/json");
        }
        [HttpGet]
        public async Task<IActionResult> GetMMCollaterals(Guid CaseId)
        {
            var collaterals = await _collateralService.GetMMCollaterals(base.GetCurrentUserId(),CaseId);
            string jsonData = JsonConvert.SerializeObject(collaterals);
            return Content(jsonData, "application/json");
        }
        [HttpGet]
        public async Task<IActionResult> GetCMCollaterals(Guid CaseId)
        {
            var collaterals = await _collateralService.GetCMCollaterals(base.GetCurrentUserId(), CaseId);
            string jsonData = JsonConvert.SerializeObject(collaterals);
            return Content(jsonData, "application/json");
        }

        [HttpGet]
        public async Task<IActionResult> GetMMCompleteCollaterals(Guid CaseId)
        {
            var collaterals = await _collateralService.GetMMCompleteCollaterals(base.GetCurrentUserId(), CaseId);
            string jsonData = JsonConvert.SerializeObject(collaterals);
            return Content(jsonData, "application/json");
        }
        [HttpGet]

        public async Task<IActionResult> GetMyAssigmentCollateral(Guid CaseId)
        {
            var collaterals = await _collateralService.GetMyAssignmentCollateral(base.GetCurrentUserId(), CaseId);
            string jsonData = JsonConvert.SerializeObject(collaterals);
            return Content(jsonData, "application/json");
        }

        [HttpGet]
        public async Task<IActionResult> GetMMPendCollaterals(Guid CaseId)
        {
            var collaterals = await _collateralService.GetMMPendCollaterals(base.GetCurrentUserId(), CaseId);
            string jsonData = JsonConvert.SerializeObject(collaterals);
            return Content(jsonData, "application/json");
        }

        [HttpPost]
        public async Task<IActionResult> DeleteCocllateral(Guid id)
        {
          
            if (await _collateralService.DeleteCocllateral(base.GetCurrentUserId(), id))

            {
                return Ok();
            }
            return BadRequest();
        }
        [HttpGet]
        public async Task<IActionResult> GetRMCompleteCollaterals(Guid CaseId)
        {
            var collaterals = await _collateralService.GetRmComCollaterals(CaseId);
            string jsonData = JsonConvert.SerializeObject(collaterals);
            return Content(jsonData, "application/json");
        }

        [HttpGet]
        public async Task<IActionResult> CheckCategory(Guid CaseId)
        {
            var collaterals = await _collateralService.GetCollaterals(CaseId);
            string jsonData = JsonConvert.SerializeObject(collaterals);
            return Content(jsonData, "application/json");
        }
        //[HttpGet]
        //public async Task<IActionResult> GetCOCollaterals(Guid CaseId)
        //{
        //    var collaterals = await _collateralService.GetCOCollaterals(CaseId);
        //    string jsonData = JsonConvert.SerializeObject(collaterals);
        //    return Content(jsonData, "application/json");
        //}


        //[HttpGet]
        //public async Task<IActionResult> GetMTLCollaterals(Guid CaseId)
        //{
        //    var collaterals = await _collateralService.GetMTLCollaterals(CaseId);
        //    string jsonData = JsonConvert.SerializeObject(collaterals);
        //    return Content(jsonData, "application/json");
        //}
        //[HttpGet]
        //public async Task<IActionResult> GetCTLCollaterals(Guid CaseId)
        //{
        //    var collaterals = await _collateralService.GetCTLCollaterals(CaseId);
        //    string jsonData = JsonConvert.SerializeObject(collaterals);
        //    return Content(jsonData, "application/json");
        //}
        [HttpPost]
        public async Task<IActionResult> changeCollateralStatus(Guid Id, string status)
        {
            try
            {
                var collaterals = await _collateralService.ChangeStatus(base.GetCurrentUserId(), Id, status);
                if (collaterals)
                {
                    return Json(new { success = true, message = "Status changed successfully.", data = collaterals });
                }
                else
                {
                 
                    return Json(new { success = false, message = "unexpected error occur please contact the Systme Admin" });
                }
            }
            catch (Exception ex)
            {
                if(ex.Message!=null && ex.Message != string.Empty)
                {
                    if (ex.Message == "correction")
                    {
                        return Json(new { success = false, message = "add comment before send to correction" });
                    }
                    else if (ex.Message == "Complete")
                    {
                        return Json(new { success = false, message = "Remove all comment before complete and  send to Relation Manager" });
                    }
                    else
                    {
                        return Json(new { success = false, message = "unexpected error occur please contact the Systme Admin" });
                    }
                }
                else
                {
                    return Json(new { success = false, message = "unexpected error occur please contact the Systme Admin" });
                }
                
            }
              
         
                
        }
        //[HttpGet]
        //public async Task<IActionResult> GetCMCollaterals(Guid CaseId)
        //{
        //    var collaterals = await _collateralService.GetCMCollaterals(CaseId);
        //    string jsonData = JsonConvert.SerializeObject(collaterals);
        //    return Content(jsonData, "application/json");
        //}

        public async Task<JsonResult> MyReturnedCollaterals()
        {
            var collaterals =await _collateralService.MyReturnedCollaterals(base.GetCurrentUserId());
            return Json(collaterals);
        }
        public async Task<IActionResult> MyReturnedCollateral(Guid CollateralId)
        {
            var collaterals = await _collateralService.MyReturnedCollateral(CollateralId, base.GetCurrentUserId());
            return View(collaterals);
        }
        public async Task<IActionResult> MyResubmitedCollaterals()
        {
            var collaterals =await _collateralService.MyResubmitedCollaterals(base.GetCurrentUserId());

            return View(collaterals);

        }
        public async Task<IActionResult> MyResubmitedCollateral(Guid CollateralId)
        {
            var collaterals = await _collateralService.MyResubmitedCollateral(CollateralId, base.GetCurrentUserId());
            return View(collaterals);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddCollateralByMO(Guid caseId, CollateralPostDto collateralDto)
        {
            if (ModelState.IsValid)
            {
               var collaterals= await _collateralService.CreateCollateral(base.GetCurrentUserId(), caseId, collateralDto);
                CaseAssignment ass=new CaseAssignment { 
                    UserId = base.GetCurrentUserId(), 
                    CollateralId= collaterals.Id,
                    AssignmentDate=DateTime.Now,
                    Status="New"
                };
                await _cbeContext.CaseAssignments.AddAsync(ass);
                await _cbeContext.SaveChangesAsync();

                var response = new { message = "Collateral created successfully" };
                return Redirect($"/MOCase/MyCase?Id={collaterals.CaseId}");
            }
            return Redirect($"/MOCase/MyCase?Id={caseId}");
        }
    }
}
