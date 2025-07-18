using mechanical.Models.Dto.CaseCommentDto;
using mechanical.Models.Dto.CaseDto;

namespace mechanical.Services.CaseCommentService
{
    public interface ICaseCommentService
    {
        Task<CaseCommentReturnDto> CreateCaseComment(Guid userId, CaseCommentPostDto caseCommentPostDto);
        Task<IEnumerable<CaseCommentReturnDto>> GetCaseComments(Guid caseId);
        Task<IEnumerable<CaseCorrectionHistoryRetunDto>> GetCaseCorrectionHistory(Guid caseId);
    }
}
