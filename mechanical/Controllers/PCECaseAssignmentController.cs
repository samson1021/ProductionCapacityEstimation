using Newtonsoft.Json;
using Microsoft.AspNetCore.Mvc;
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

    public class PCECaseAssignmentController : BaseController
    {
        private readonly IUserService _UserService;
        private readonly IPCECaseService _PCECaseService;
        private readonly ILogger<PCECaseAssignmentController> _logger;
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
            var pceCase = await _PCECaseService.GetPCECase(Id);
            
            if (pceCase == null)
            {
                return RedirectToAction("MyPCECases");
            }

            ViewData["CurrentUser"] = await _UserService.GetUserById(base.GetCurrentUserId());
            ViewData["PCECase"] = pceCase;

            return View();
        }         
        
        [HttpPost]
        public async Task<IActionResult> SendForValuation(string selectedPCEIds, string CenterId)
        {

            var userId = base.GetCurrentUserId();
            try
            {Console.WriteLine("hlkdhflkg");Console.WriteLine(selectedPCEIds);
                await _PCECaseAssignmentService.SendProductionForValuation(base.GetCurrentUserId(), selectedPCEIds, CenterId);
                var response = new { message = "PCE Estimation assigned successfully" };
                return Ok(response);
            }
            catch (Exception ex)
            {
                var error = new { message = ex.Message };
                return BadRequest(error);
            }
        }

        [HttpPost]
        public async Task<IActionResult> SendProductionForReestimation(string ReestimationReason, string selectedPCEIds, string CenterId)
        {
            var userId = base.GetCurrentUserId();
            try
            {
                await _PCECaseAssignmentService.SendProductionForReestimation(base.GetCurrentUserId(), ReestimationReason, selectedPCEIds, CenterId);
                var response = new { message = "PCE Reestimation assigned successfully" };
                return Ok(response);
            }
            catch (Exception ex)
            {
                var error = new { message = ex.Message };
                return BadRequest(error);
            }
        }

        [HttpPost]
        public async Task<IActionResult> PCEAssignTeamleader(string selectedPCEIds, string employeeId)
        {
            await _PCECaseAssignmentService.AssignProduction(base.GetCurrentUserId(), selectedPCEIds, employeeId);
            var response = new { message = "Productions assigned to Maker Team Leader successfully" };
            return Ok(response);
        }
        
        [HttpPost]
        public async Task<IActionResult> PCEReAssignTeamleader(string selectedPCEIds, string employeeId)
        {
            await _PCECaseAssignmentService.ReAssignProduction(base.GetCurrentUserId(), selectedPCEIds, employeeId);
            var response = new { message = "Productions re-assigned to Maker Team Leader successfully" };
            return Ok(response);
        }
        
        [HttpPost]
        public async Task<IActionResult> PCEAssignMakerOfficer(string selectedPCEIds, string employeeId)
        {
            
            await _PCECaseAssignmentService.AssignProduction(base.GetCurrentUserId(), selectedPCEIds, employeeId);
            var response = new { message = "Productions assigned to Maker Officer successfully" };
            return Ok(response);
        }
            
        [HttpPost]
        public async Task<IActionResult> PCEReAssignMakerOfficer(string selectedPCEIds, string employeeId)
        {
            await _PCECaseAssignmentService.ReAssignProduction(base.GetCurrentUserId(), selectedPCEIds, employeeId);
            var response = new { message = "Productions re-assigned to Maker Officer successfully" };
            return Ok(response);
        }
    }
}