using AutoMapper;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;

using mechanical.Data;
using mechanical.Models.Dto.CaseTimeLineDto;
using mechanical.Models.Entities;
using mechanical.Models.PCE.Dto.PCECaseTimeLineDto;
using mechanical.Models.PCE.Entities;
using mechanical.Services.PCE.PCECaseTimeLineService;

namespace mechanical.Services.PCE.PCECaseTimeLineService
{
    public class PCECaseTimeLineService : IPCECaseTimeLineService
    {

        private readonly CbeContext _cbeContext;
        private readonly IMapper _mapper;
        private readonly ILogger<PCECaseTimeLineService> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;


        public PCECaseTimeLineService(CbeContext cbeContext, IMapper mapper, ILogger<PCECaseTimeLineService> logger, IHttpContextAccessor httpContextAccessor)
        {
            _cbeContext = cbeContext;
            _mapper = mapper;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<PCECaseTimeLinePostDto> PCECaseTimeLine(PCECaseTimeLinePostDto pCECaseTimeLinePostDto)
        {
            // using var transaction = await _cbeContext.Database.BeginTransactionAsync();
            try
            { 
                var httpContext = _httpContextAccessor.HttpContext;
                var caseTimeline = _mapper.Map<PCECaseTimeLine>(pCECaseTimeLinePostDto);
                if (caseTimeline.UserId == Guid.Empty)
                    caseTimeline.UserId = Guid.Parse(httpContext.Session.GetString("userId"));
                caseTimeline.CreatedAt = DateTime.Now;

                await _cbeContext.PCECaseTimeLines.AddAsync(caseTimeline);
                await _cbeContext.SaveChangesAsync(); 
                // await transaction.CommitAsync();

                return _mapper.Map<PCECaseTimeLinePostDto>(caseTimeline);  
            }

            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating case timeline");
                // await transaction.RollbackAsync();
                throw new ApplicationException("An error occurred while creating case timeline.");
            }
        }

        public async Task<IEnumerable<PCECaseTimeLineReturnDto>> GetPCECaseTimeLines(Guid CaseId)
        {
            var caseTimelines = await _cbeContext.PCECaseTimeLines.Where(a=>a.CaseId == CaseId).Include(res => res.User).ThenInclude(res => res.Role).OrderBy(res => res.CreatedAt).ToListAsync(); 

            return _mapper.Map<IEnumerable<PCECaseTimeLineReturnDto>> (caseTimelines); 
        }
    }
}