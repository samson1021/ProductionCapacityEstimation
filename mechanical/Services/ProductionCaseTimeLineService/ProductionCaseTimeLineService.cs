using AutoMapper;
using mechanical.Data;
using mechanical.Models.Dto.CaseTimeLineDto;
using mechanical.Models.Dto.ProductionCaseTimeLineDto;
using mechanical.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace mechanical.Services.ProductionCaseTimeLineService
{
    public class ProductionCaseTimeLineService : IProductionCaseTimeLineService
    {
        private readonly CbeContext _cbeContext;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public ProductionCaseTimeLineService(CbeContext cbeContext, IMapper mapper, IHttpContextAccessor httpContextAccessor)
        {
            _cbeContext = cbeContext;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
    
        }
        public async Task<ProductionCaseTimeLineDto> CreateProductionCaseTimeLine(ProductionCaseTimeLinePostDto caseTimeLinePostDto)
        {
            var httpContext = _httpContextAccessor.HttpContext;

            var caseTimeline = _mapper.Map<ProductionCaseTimeLine>(caseTimeLinePostDto);
            if (caseTimeline.UserId == Guid.Empty) caseTimeline.UserId = Guid.Parse(httpContext.Session.GetString("userId"));
            caseTimeline.CreatedAt = DateTime.Now;

            await _cbeContext.ProductionCaseTimeLines.AddAsync(caseTimeline);
            await _cbeContext.SaveChangesAsync();

            return _mapper.Map<ProductionCaseTimeLineDto>(caseTimeline);
        }

        public async Task<IEnumerable<ProductionCaseTimeLineReturnDto>> GetProductionCaseTimeLines(Guid CaseId)
        {
            var caseTimelines = await _cbeContext.ProductionCaseTimeLines.Where(res => res.ProductionCaseId == CaseId).Include(res => res.User).ThenInclude(res => res.Role).OrderBy(res => res.CreatedAt).ToListAsync();
            return _mapper.Map<IEnumerable<ProductionCaseTimeLineReturnDto>>(caseTimelines);
        }
    }
}
