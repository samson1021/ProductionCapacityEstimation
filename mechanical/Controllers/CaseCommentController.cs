using mechanical.Models.Dto.CaseCommentDto;
using mechanical.Services.CaseCommentService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace mechanical.Controllers
{
    [Authorize(Roles = "Maker Manager,District Valuation Manager ,Maker Officer, Maker TeamLeader, Relation Manager,Checker Manager, Checker TeamLeader, Checker Officer")]
    public class CaseCommentController : BaseController
    {
        private readonly ICaseCommentService _caseCommentService;
        public CaseCommentController(ICaseCommentService caseCommentService)
        {
            _caseCommentService = caseCommentService;
        }
        [HttpPost]
        public async Task<IActionResult> CreateCaseComment(string CaseId, CaseCommentPostDto caseCommentPostDto)
        {
            await _caseCommentService.CreateCaseComment(base.GetCurrentUserId(), caseCommentPostDto);
            return Ok();
        }
        [HttpGet]
        public async Task<IActionResult> GetCaseComments(Guid caseId)
        {
            var response = new
            {
                userId = base.GetCurrentUserId(),
                caseComments = await _caseCommentService.GetCaseComments(caseId)
            };
            return Content(JsonConvert.SerializeObject(response), "application/json");
        }
        [HttpGet]
        public async Task<IActionResult> GetCaseCorrectionHistory(Guid caseId)
        {
            Console.WriteLine("Starting to fetch case correction history...");

            try
            {
                var caseComments = await _caseCommentService.GetCaseCorrectionHistory(caseId);
                var response = new
                {
                    userId = base.GetCurrentUserId(),
                    caseComments = caseComments
                };

                return Content(JsonConvert.SerializeObject(response), "application/json");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
