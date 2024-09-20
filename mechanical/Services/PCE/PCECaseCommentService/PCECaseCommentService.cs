using AutoMapper;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;

using mechanical.Data;
using mechanical.Models.Entities;
using mechanical.Models.PCE.Entities;
using mechanical.Models.PCE.Dto.PCECaseCommentDto;

namespace mechanical.Services.PCE.PCECaseCommentService
{
    public class PCECaseCommentService:IPCECaseCommentService
    {
        private readonly CbeContext _cbeContext;
        private readonly IMapper _mapper;
        private readonly ILogger<PCECaseCommentService> _logger;

        public PCECaseCommentService(CbeContext cbeContext, IMapper mapper, ILogger<PCECaseCommentService> logger)
        {
            _cbeContext = cbeContext;
            _mapper = mapper;
            _logger = logger;
        }
        public async Task<PCECaseCommentReturnDto> CreateCaseComment(Guid userId, PCECaseCommentPostDto caseCommentPostDto)
        {
            using var transaction = await _cbeContext.Database.BeginTransactionAsync();
            try
            {    
                var caseComment = _mapper.Map<PCECaseComment>(caseCommentPostDto);
                caseComment.AuthorId = userId;
                caseComment.CreatedAt = DateTime.Now;

                await _cbeContext.PCECaseComments.AddAsync(caseComment);
                await _cbeContext.SaveChangesAsync();
                await transaction.CommitAsync();

                return _mapper.Map<PCECaseCommentReturnDto>(caseComment);               
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating case comment");
                await transaction.RollbackAsync();
                throw new ApplicationException("An error occurred while creating case comment.");
            }
        }

        public async Task<IEnumerable<PCECaseCommentReturnDto>> GetCaseComments(Guid PCECaseId)
        {
            var caseComment = await _cbeContext.PCECaseComments.Include(res => res.Author).Where(res => res.PCECaseId == PCECaseId).OrderBy(res => res.CreatedAt).ToListAsync();
            return _mapper.Map<IEnumerable<PCECaseCommentReturnDto>>(caseComment);
        }
    }
}

    
