using AutoMapper;
using mechanical.Data;
using mechanical.Models.Dto.CaseCommentDto;
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
            caseComment.CreatedAt = DateTime.Now;
            await  _cbeContext.CaseComments.AddAsync(caseComment);
            await _cbeContext.SaveChangesAsync();
            return _mapper.Map<CaseCommentReturnDto>(caseComment);
        }

        public async  Task<IEnumerable<CaseCommentReturnDto>> GetCaseComments(Guid caseId)
        {
            var caseComment = await _cbeContext.CaseComments.Include(res => res.Author).Where(res => res.CaseId == caseId).OrderBy(res => res.CreatedAt).ToListAsync();
            return _mapper.Map<IEnumerable<CaseCommentReturnDto>>(caseComment);
        }
    }
}
