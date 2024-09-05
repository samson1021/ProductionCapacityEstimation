using mechanical.Models.Dto.CaseCommentDto;
using mechanical.Models.PCE.Dto.PCECaseCommentDto;

namespace mechanical.Services.PCE.PCECaseCommentService
{
    public interface IPCECaseCommentService
    {
        Task<PCECaseCommentReturnDto> CreateCaseComment(Guid userId, PCECaseCommentPostDto caseCommentPostDto);
        Task<IEnumerable<PCECaseCommentReturnDto>> GetCaseComments(Guid caseId);
    }
}
