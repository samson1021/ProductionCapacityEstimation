using AutoMapper;
using mechanical.Data;
using mechanical.Models.Dto.CaseDto;
using mechanical.Models.Dto.DashboardDto;
using mechanical.Services.CaseTimeLineService;
using mechanical.Services.UploadFileService;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace mechanical.Services.MMCaseService
{
    public class CMCaseService:ICMCaseService
    {
        private readonly CbeContext _cbeContext;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ICaseTimeLineService _caseTimeLineService;
        private readonly IUploadFileService _uploadFileService;
        
        public async Task<IEnumerable<CaseDto>> GetCoRemarkedCases(Guid userId)
        {

            var caseAssignments = await _cbeContext.CaseAssignments.Include(res => res.Collateral).ThenInclude(res => res.Case).Where(Ca => Ca.UserId == userId && Ca.Status.Contains("Remark")).ToListAsync();
            var uniqueCases = caseAssignments.Select(ca => ca.Collateral.Case).DistinctBy(c => c.Id).ToList();
            var caseDtos = _mapper.Map<IEnumerable<CaseDto>>(uniqueCases);
            foreach (var caseDto in caseDtos)
            {
                caseDto.TotalNoOfCollateral = await _cbeContext.Collaterals.CountAsync(res => res.CaseId == caseDto.Id);
            }
            return caseDtos;
        }
        public CMCaseService(CbeContext cbeContext, IMapper mapper, IHttpContextAccessor httpContextAccessor, IUploadFileService uploadFileService, ICaseTimeLineService caseTimeLineService)
        {
            _cbeContext = cbeContext;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
            _caseTimeLineService = caseTimeLineService;
            _uploadFileService = uploadFileService;
        }
        public async Task<IEnumerable<RMCaseDto>> GetMmPendingCases()
        {
            var httpContext = _httpContextAccessor.HttpContext;
            var user = await _cbeContext.Users.FindAsync(Guid.Parse(httpContext.Session.GetString("userId")));
            var cases = await _cbeContext.Cases.Include(ca => ca.District).Include(ca => ca.Collaterals.Where(ca => ca.CurrentStatus != "New" && ca.CurrentStatus != "Complete"))
                .Where(Ca => Ca.DistrictId == user.DistrictId && Ca.Status == "Maker" && Ca.Collaterals.Any(ca => ca.CurrentStatus != "New" && ca.CurrentStatus != "Complete")).ToListAsync();

            return _mapper.Map<IEnumerable<RMCaseDto>>(cases);
        }
        public async Task<IEnumerable<MMNewCaseDto>> GetCMNewCases()
        {
            var httpContext = _httpContextAccessor.HttpContext;
            var user = await _cbeContext.Users.FirstOrDefaultAsync(ca => ca.Id == Guid.Parse(httpContext.Session.GetString("userId")) && ca.Role.Name == "Checker Manager");
            if (user == null)
            {
                return null;
            }
            var cases = await _cbeContext.Cases.Include(ca => ca.District).Include(ca => ca.Collaterals.Where(ca => ca.CurrentStatus == "Checker"))
               .Where(Ca => Ca.DistrictId == user.DistrictId && Ca.Collaterals.Any(ca => ca.CurrentStatus == "Checker")).ToListAsync();

            return _mapper.Map<IEnumerable<MMNewCaseDto>>(cases);
        }
        public async Task<IEnumerable<MMCaseDto>> GetMmLatestCases()
        {
            var httpContext = _httpContextAccessor.HttpContext;
            var user = await _cbeContext.Users.FirstOrDefaultAsync(ca => ca.Id == Guid.Parse(httpContext.Session.GetString("userId")) && ca.Role.Name == "Maker Manager");
            if (user == null)
            {
                return null;
            }
            var cases = await _cbeContext.Cases
                    .Include(x => x.Collaterals)
                    .Include(x => x.District)
                    .Where(res => res.DistrictId == user.DistrictId && res.Status == "Maker")
                    .OrderByDescending(res => res.CreationAt).Take(7).ToListAsync();
            return _mapper.Map<IEnumerable<MMCaseDto>>(cases);
        }
        public async Task<CaseCountDto> GetDashboardCaseCount()
        {
            var httpContext = _httpContextAccessor.HttpContext;
            var user = await _cbeContext.Users.FirstOrDefaultAsync(ca => ca.Id == Guid.Parse(httpContext.Session.GetString("userId")) && ca.Role.Name == "Maker Manager");
            if (user == null)
            {
                return null;
            }
            return new CaseCountDto()
            {
                NewCaseCount = await _cbeContext.Cases.Where(res => res.DistrictId == user.DistrictId &&res.Status == "New").CountAsync(),
                PendingCaseCount = await _cbeContext.Cases.Where(res => res.DistrictId == user.DistrictId &&  res.Status == "Pending").CountAsync(),
                CompletedCaseCount = await _cbeContext.Cases.Where(res => res.DistrictId == user.DistrictId &&  res.Status == "Complete").CountAsync()
            };
        }
    }
}
