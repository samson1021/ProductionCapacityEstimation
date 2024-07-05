using mechanical.Models.Dto.CaseCommentDto;
using mechanical.Services.CaseCommentService;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace mechanical.Controllers
{
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
            var response = new { 
                userId = base.GetCurrentUserId(),
                caseComments =await _caseCommentService.GetCaseComments(caseId) 
            };
            return Content(JsonConvert.SerializeObject(response), "application/json");
        }
    }
}
