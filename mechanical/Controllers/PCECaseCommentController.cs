using mechanical.Models.Dto.CaseCommentDto;
using mechanical.Models.PCE.Dto.PCECaseCommentDto;
using mechanical.Services.CaseCommentService;
using mechanical.Services.PCE.PCECaseCommentService;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace mechanical.Controllers
{
    public class PCECaseCommentController : BaseController
    {
        private readonly IPCECaseCommentService _PCEcaseCommentService;
        
        public PCECaseCommentController(IPCECaseCommentService PCEcaseCommentService)
        {
            _PCEcaseCommentService = PCEcaseCommentService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateCaseComment([FromBody] PCECaseCommentPostDto caseCommentPostDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (caseCommentPostDto == null)
            {
                return BadRequest("Invalid data.");
            }
            await _PCEcaseCommentService.CreateCaseComment(base.GetCurrentUserId(), caseCommentPostDto);
            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> GetCaseComments(Guid PCECaseId)
        {
            var response = new
            {
                userId = base.GetCurrentUserId(),
                caseComments = await _PCEcaseCommentService.GetCaseComments(PCECaseId)
            };
            return Content(JsonConvert.SerializeObject(response), "application/json");
        }
    }
}
