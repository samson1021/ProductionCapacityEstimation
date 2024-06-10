using AutoMapper;
using mechanical.Data;
using mechanical.Models.PCE.Dto.PCECaseTimeLineDto;
using mechanical.Models.PCE.Entities;
using mechanical.Services.PCE.PCECaseTimeLineService;

public class PCECaseTimeLineService : IPCECaseTimeLineService
{

    private readonly CbeContext _cbeContext;
    private readonly IMapper _mapper;
    private readonly IHttpContextAccessor _httpContextAccessor;


    public PCECaseTimeLineService(CbeContext cbeContext, IMapper mapper, IHttpContextAccessor httpContextAccessor)
    {
        _cbeContext = cbeContext;
        _mapper = mapper;
        _httpContextAccessor = httpContextAccessor;
    }

    async Task<PCECaseTimeLinePostDto> IPCECaseTimeLineService.PCECaseTimeLine(PCECaseTimeLinePostDto pCECaseTimeLinePostDto)
    {
        var httpContext = _httpContextAccessor.HttpContext;
        var caseTimeline = _mapper.Map<PCECaseTimeLine>(pCECaseTimeLinePostDto);
        if (caseTimeline.UserId == Guid.Empty)
            caseTimeline.UserId = Guid.Parse(httpContext.Session.GetString("userId"));
        caseTimeline.CreatedAt = DateTime.Now;

        await _cbeContext.PCECaseTimeLines.AddAsync(caseTimeline);
        await _cbeContext.SaveChangesAsync();
        return _mapper.Map<PCECaseTimeLinePostDto>(caseTimeline);
    }
}


