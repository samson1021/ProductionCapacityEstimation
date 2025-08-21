using AutoMapper;
using mechanical.Data;
using mechanical.Models.Dto.CaseCommentDto;
using mechanical.Models.Dto.CaseDto;
using mechanical.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace mechanical.Services.CaseCommentService
{
    public class CaseCommentService : ICaseCommentService
    {
        private readonly CbeContext _cbeContext;
        private readonly IMapper _mapper;
        public CaseCommentService(CbeContext cbeContext, IMapper mapper)
        {
            _cbeContext = cbeContext;
            _mapper = mapper;
        }
        public async Task<CaseCommentReturnDto> CreateCaseComment(Guid userId, CaseCommentPostDto caseCommentPostDto)
        {
            var caseComment = _mapper.Map<CaseComment>(caseCommentPostDto);
            caseComment.AuthorId = userId;
            caseComment.CreatedAt = DateTime.UtcNow;
            await _cbeContext.CaseComments.AddAsync(caseComment);
            await _cbeContext.SaveChangesAsync();
            return _mapper.Map<CaseCommentReturnDto>(caseComment);
        }
        public async Task<IEnumerable<CaseCommentReturnDto>> GetCaseComments(Guid caseId)
        {
            var caseComment = await _cbeContext.CaseComments.Include(res => res.Author).Where(res => res.CaseId == caseId).OrderBy(res => res.CreatedAt).ToListAsync();
            return _mapper.Map<IEnumerable<CaseCommentReturnDto>>(caseComment);
        }

        public async Task<IEnumerable<CaseCorrectionHistoryRetunDto>> GetCaseCorrectionHistory(Guid caseId)
        {
            var caseComment = await _cbeContext.CommentHistorys
                                    //.Include(res => res.CommentByUserId) // Adjust according to your navigation property name
                                    .Include(res => res.Case) // Adjust according to your navigation property name
                                    .Where(res => res.CaseId == caseId)
                                    .OrderBy(res => res.CreatedAt)
                                    .ToListAsync();

            string datas3 = "ga";
            string datas4 = "ga";

            return _mapper.Map<IEnumerable<CaseCorrectionHistoryRetunDto>>(caseComment);
        }
    }
}
