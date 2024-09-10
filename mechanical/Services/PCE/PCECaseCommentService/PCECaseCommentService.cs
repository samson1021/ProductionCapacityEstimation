using AutoMapper;
using mechanical.Data;
using mechanical.Models.Dto.CaseCommentDto;
using mechanical.Models.Entities;
using mechanical.Models.PCE.Dto.PCECaseCommentDto;
using mechanical.Models.PCE.Entities;
using Microsoft.EntityFrameworkCore;

namespace mechanical.Services.PCE.PCECaseCommentService
{
    public class PCECaseCommentService:IPCECaseCommentService
    {
        private readonly CbeContext _cbeContext;
        private readonly IMapper _mapper;
        public PCECaseCommentService(CbeContext cbeContext, IMapper mapper)
        {
            _cbeContext = cbeContext;
            _mapper = mapper;
        }
        public async Task<PCECaseCommentReturnDto> CreateCaseComment(Guid userId, PCECaseCommentPostDto caseCommentPostDto)
        {
            var caseComment = _mapper.Map<PCECaseComment>(caseCommentPostDto);
            caseComment.AuthorId = userId;
            caseComment.CreatedAt = DateTime.Now;
            await _cbeContext.PCECaseComments.AddAsync(caseComment);
            await _cbeContext.SaveChangesAsync();
            return _mapper.Map<PCECaseCommentReturnDto>(caseComment);
        }

        public async Task<IEnumerable<PCECaseCommentReturnDto>> GetCaseComments(Guid PCECaseId)
        {
            var caseComment = await _cbeContext.PCECaseComments.Include(res => res.Author).Where(res => res.PCECaseId == PCECaseId).OrderBy(res => res.CreatedAt).ToListAsync();
            return _mapper.Map<IEnumerable<PCECaseCommentReturnDto>>(caseComment);
        }
    }
}

    
