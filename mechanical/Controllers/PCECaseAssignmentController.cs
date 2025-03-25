using Newtonsoft.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using DocumentFormat.OpenXml.Bibliography;

using mechanical.Data;
using mechanical.Models;
using mechanical.Models.Enum;
using mechanical.Models.Dto.UploadFileDto;
using mechanical.Services.UserService;
using mechanical.Services.UploadFileService;

using mechanical.Models.PCE.Entities;
using mechanical.Services.PCE.PCECaseService;
using mechanical.Services.PCE.PCECaseAssignmentService;

namespace mechanical.Controllers
{

    // [Authorize(Roles = "Maker Manager,District Valuation Manager ,Maker Officer, Maker TeamLeader, Relation Manager,Checker Manager, Checker TeamLeader, Checker Officer")]
    [Authorize]
    public class PCECaseAssignmentController : BaseController
    {
        private readonly IUserService _UserService;
        private readonly IPCECaseService _PCECaseService;
        private readonly IPCECaseAssignmentService _PCECaseAssignmentService;

        public PCECaseAssignmentController(IUserService UserService, IPCECaseAssignmentService PCECaseAssignmentService, IPCECaseService PCECaseService)
        {
            _UserService = UserService;
            _PCECaseService = PCECaseService;
            _PCECaseAssignmentService = PCECaseAssignmentService;            
        }

        public async Task<IActionResult> MyAssignments()
        {
            ViewData["CurrentUser"] = await _UserService.GetUserById(base.GetCurrentUserId());
            return View();
        }     
        public async Task<IActionResult> MyAssignment(Guid Id)
        {            
            var userId = base.GetCurrentUserId();
            var pceCase = await _PCECaseService.GetPCECase(userId, Id);
            
            if (pceCase == null)
            {
                return RedirectToAction("PCECases");
            }

            ViewData["CurrentUser"] = await _UserService.GetUserById(userId);
            ViewData["PCECase"] = pceCase;

            return View();
        }         
        
        [HttpPost]
        public async Task<IActionResult> SendForValuation(string SelectedPCEIds, string AssignedId)
        {
            var userId = base.GetCurrentUserId();
            try
            {
                await _PCECaseAssignmentService.SendForValuation(base.GetCurrentUserId(), SelectedPCEIds, AssignedId);
                var response = new { message = "Production is assigned for estimation successfully" };
                return Ok(response);
            }
            catch (Exception ex)
            {
                var error = new { message = ex.Message };
                return BadRequest(error);
            }
        }

        [HttpPost]
        public async Task<IActionResult> SendForReestimation(string ReestimationReason, string SelectedPCEIds, string AssignedId)
        {
            var userId = base.GetCurrentUserId();
            try
            {
                await _PCECaseAssignmentService.SendForReestimation(base.GetCurrentUserId(), ReestimationReason, SelectedPCEIds, AssignedId);
                var response = new { message = "Production is assigned for reestimation successfully" };
                return Ok(response);
            }
            catch (Exception ex)
            {
                var error = new { message = ex.Message };
                return BadRequest(error);
            }
        }

        [HttpPost]
        public async Task<IActionResult> PCEAssignMakerTeamleader(string SelectedPCEIds, string AssignedId)
        {
            await _PCECaseAssignmentService.AssignProduction(base.GetCurrentUserId(), SelectedPCEIds, AssignedId);
            var response = new { message = "Productions assigned to Maker Team Leader successfully" };
            return Ok(response);
        }
        
        [HttpPost]
        public async Task<IActionResult> PCEReAssignMakerTeamleader(string SelectedPCEIds, string AssignedId)
        {
            await _PCECaseAssignmentService.ReAssignProduction(base.GetCurrentUserId(), SelectedPCEIds, AssignedId);
            var response = new { message = "Productions re-assigned to Maker Team Leader successfully" };
            return Ok(response);
        }
        
        [HttpPost]
        public async Task<IActionResult> PCEAssignMakerOfficer(string SelectedPCEIds, string AssignedId)
        {
            
            await _PCECaseAssignmentService.AssignProduction(base.GetCurrentUserId(), SelectedPCEIds, AssignedId);
            var response = new { message = "Productions assigned to Maker Officer successfully" };
            return Ok(response);
        }
            
        [HttpPost]
        public async Task<IActionResult> PCEReAssignMakerOfficer(string SelectedPCEIds, string AssignedId)
        {
            await _PCECaseAssignmentService.ReAssignProduction(base.GetCurrentUserId(), SelectedPCEIds, AssignedId);
            var response = new { message = "Productions re-assigned to Maker Officer successfully" };
            return Ok(response);
        }
    }
}