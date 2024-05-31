using AutoMapper;
using mechanical.Data;
using mechanical.Models.Dto.CaseTimeLineDto;
using mechanical.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace mechanical.Services.CaseTimeLineService
{
    public class CaseTimeLineService : ICaseTimeLineService
    {
        private readonly CbeContext _cbeContext;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public CaseTimeLineService(CbeContext cbeContext, IMapper mapper, IHttpContextAccessor httpContextAccessor)
        {
            _cbeContext = cbeContext;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<CaseTimeLineDto> CreateCaseTimeLine(CaseTimeLinePostDto caseTimeLinePostDto)
        {
            var httpContext = _httpContextAccessor.HttpContext;

            var caseTimeline = _mapper.Map<CaseTimeLine>(caseTimeLinePostDto);
            if(caseTimeline.UserId == Guid.Empty) caseTimeline.UserId = Guid.Parse(httpContext.Session.GetString("userId"));
            caseTimeline.CreatedAt = DateTime.Now;

            await _cbeContext.CaseTimeLines.AddAsync(caseTimeline);
            await _cbeContext.SaveChangesAsync();

            return _mapper.Map<CaseTimeLineDto>(caseTimeline);
        }

        public async Task<IEnumerable<CaseTimeLineReturnDto>> GetCaseTimeLines(Guid CaseId)
        {
            var caseTimelines = await _cbeContext.CaseTimeLines.Where(res => res.CaseId == CaseId).Include(res => res.User).ThenInclude(res => res.Role).OrderBy(res => res.CreatedAt).ToListAsync();
            return _mapper.Map<IEnumerable<CaseTimeLineReturnDto>>(caseTimelines);
        }
    }
}
