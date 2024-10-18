using AutoMapper;
using mechanical.Data;
using mechanical.Models.Dto.CaseDto;
using mechanical.Models.Dto.CaseTimeLineDto;
using mechanical.Models.Dto.Correction;
using mechanical.Models.Entities;
using mechanical.Services.CaseTimeLineService;
using Microsoft.EntityFrameworkCore;

namespace mechanical.Services.CorrectionServices
{
    public class CorrectionService:ICorrectionService
    {

        private readonly CbeContext _cbeContext;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ICaseTimeLineService _caseTimeLineService;
        public CorrectionService(CbeContext cbeContext, IMapper mapper, IHttpContextAccessor httpContextAccessor, ICaseTimeLineService caseTimeLineService)
        {
            _cbeContext = cbeContext;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
            _caseTimeLineService = caseTimeLineService;
        }
        public async Task<Correction>CreateCorrection(CorrectionPostDto createCorrectionDto)
        {
            var httpContext = _httpContextAccessor.HttpContext;  
            var loanCase = _mapper.Map<Correction>(createCorrectionDto);

            var correction = await _cbeContext.Corrections.FirstOrDefaultAsync(res => res.CollateralID == loanCase.CollateralID && res.CommentedAttribute == loanCase.CommentedAttribute);
            if(correction != null)
            {
                if(loanCase.Comment == "" || loanCase.Comment == null)
                {
                     _cbeContext.Corrections.Remove(correction);
                    await _cbeContext.SaveChangesAsync();
                }
                else
                {
                    correction.Comment = loanCase.Comment;
                    correction.CommentedByUserId = Guid.Parse(httpContext.Session.GetString("userId"));
                    correction.CreationDate = DateTime.Now;
                    _cbeContext.Corrections.Update(correction);
                    await _cbeContext.SaveChangesAsync();
                }
            

                return correction;
            }
            else
            {
                var getcaseId = _cbeContext.Collaterals.Where(c => c.Id == createCorrectionDto.CollateralID).Select(c => c.CaseId).FirstOrDefault();
                loanCase.CaseId = getcaseId;
                loanCase.CommentedByUserId = Guid.Parse(httpContext.Session.GetString("userId"));
                loanCase.CreationDate = DateTime.Now;
                await _cbeContext.Corrections.AddAsync(loanCase);
                await _cbeContext.SaveChangesAsync();
                await _caseTimeLineService.CreateCaseTimeLine(new CaseTimeLinePostDto
                {
                    CaseId = loanCase.CaseId,
                    Activity = $"<strong>A case with ID {loanCase.CaseId} out of the collaterals list one collateral wiht ID {createCorrectionDto.CollateralID} has been Returned to Maker  For correction</strong>",
                    CurrentStage = "Relation Manager"
                });

                return loanCase;
            }
          

           
        }
        public async Task<CorrectionPostDto>GetCorrection(Guid Id)
        {
            var loanCase = await _cbeContext.Corrections.Include(res => res.CollateralID).FirstOrDefaultAsync(c => c.Id == Id);
            return _mapper.Map<CorrectionPostDto>(loanCase);
        }


    }
}
