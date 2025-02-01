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
    public class MMCaseService:IMMCaseService
    {
        private readonly CbeContext _cbeContext;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ICaseTimeLineService _caseTimeLineService;
        private readonly IUploadFileService _uploadFileService;
        public MMCaseService(CbeContext cbeContext, IMapper mapper, IHttpContextAccessor httpContextAccessor, IUploadFileService uploadFileService, ICaseTimeLineService caseTimeLineService)
        {
            _cbeContext = cbeContext;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
            _caseTimeLineService = caseTimeLineService;
            _uploadFileService = uploadFileService;
        }
        //public async Task<IEnumerable<RMCaseDto>> GetMmPendingCases()
        //{
        //    var httpContext = _httpContextAccessor.HttpContext;
        //    var user = await _cbeContext.CreateUsers.FindAsync(Guid.Parse(httpContext.Session.GetString("userId")));
        //    var cases = await _cbeContext.Cases.Include(ca => ca.District).Include(ca => ca.Collaterals.Where(ca => ca.Status != "New" && ca.Status != "Complete"))
        //        .Where(Ca => Ca.DistrictId == user.DistrictId && Ca.CurrentStage == "Maker" && Ca.Collaterals.Any(ca => ca.Status != "New" && ca.Status != "Complete")).ToListAsync();

        //    return _mapper.Map<IEnumerable<RMCaseDto>>(cases);
        //}
        public async Task<IEnumerable<CaseDto>> GetMMNewCases(Guid userId)
        {

            var caseAssignments = await _cbeContext.CaseAssignments.Include(res=>res.Collateral).ThenInclude(res=>res.Case).ThenInclude(res=>res.CaseOriginator).Where(Ca => Ca.UserId == userId && Ca.Status=="New").ToListAsync();
            var uniqueCases = caseAssignments.Select(ca => ca.Collateral.Case) .DistinctBy(c => c.Id).ToList();
            var caseDtos = _mapper.Map<IEnumerable<CaseDto>>(uniqueCases);
            foreach (var caseDto in caseDtos)
            {
                caseDto.TotalNoOfCollateral = await _cbeContext.Collaterals.CountAsync(res => res.CaseId == caseDto.Id);
            }
            return caseDtos;
        }
        public async Task<IEnumerable<CaseDto>> GetMTLCompletedCases(Guid userId)
        {

            var caseAssignments = await _cbeContext.CaseAssignments.Include(res => res.Collateral).ThenInclude(res => res.Case).ThenInclude(res => res.CaseOriginator).Where(Ca => Ca.UserId == userId && Ca.Status == "Completed").ToListAsync();
            var uniqueCases = caseAssignments.Select(ca => ca.Collateral.Case).DistinctBy(c => c.Id).ToList();
            var caseDtos = _mapper.Map<IEnumerable<CaseDto>>(uniqueCases);
            foreach (var caseDto in caseDtos)
            {
                caseDto.TotalNoOfCollateral = await _cbeContext.Collaterals.CountAsync(res => res.CaseId == caseDto.Id);
            }
            return caseDtos;
        }
        public async Task<IEnumerable<CaseDto>> GetCMNewCases(Guid userId)
        {
            var user = await _cbeContext.CreateUsers.Include(res => res.District).FirstOrDefaultAsync(res=>res.Id == userId);
            
            if(user.District.Name == "Head Office")
            {
                var caseAssignments = await _cbeContext.CaseAssignments.Include(res => res.Collateral).ThenInclude(res => res.Case).ThenInclude(res => res.CaseOriginator).Where(Ca => Ca.UserId == userId && Ca.Status == "New").ToListAsync();
                var uniqueCases = caseAssignments.Select(ca => ca.Collateral.Case).DistinctBy(c => c.Id).ToList();
                var caseDtos = _mapper.Map<IEnumerable<CaseDto>>(uniqueCases);
                foreach (var caseDto in caseDtos)
                {
                    caseDto.TotalNoOfCollateral = await _cbeContext.Collaterals.CountAsync(res => res.CaseId == caseDto.Id);
                }
                return caseDtos;
            }
            else
            {
                var caseAssignments = await _cbeContext.CaseAssignments.Include(res => res.Collateral).ThenInclude(res => res.Case).ThenInclude(res => res.CaseOriginator).Where(Ca => Ca.UserId == userId && Ca.Status == "Checker New").ToListAsync();
                var uniqueCases = caseAssignments.Select(ca => ca.Collateral.Case).DistinctBy(c => c.Id).ToList();
                var caseDtos = _mapper.Map<IEnumerable<CaseDto>>(uniqueCases);
                foreach (var caseDto in caseDtos)
                {
                    caseDto.TotalNoOfCollateral = await _cbeContext.Collaterals.CountAsync(res => res.CaseId == caseDto.Id);
                }
                return caseDtos;
            }
           
           
           
        }
        public async Task<IEnumerable<CaseDto>> GetMyAssignmentCases(Guid userId)
        {

            var caseAssignments = await _cbeContext.CaseAssignments.Include(res => res.Collateral).ThenInclude(res => res.Case).Where(Ca => Ca.UserId == userId && Ca.Status != "Terminate").ToListAsync();
            var uniqueCases = caseAssignments.Select(ca => ca.Collateral.Case).DistinctBy(c => c.Id).ToList();
            var caseDtos = _mapper.Map<IEnumerable<CaseDto>>(uniqueCases);
            foreach (var caseDto in caseDtos)
            {
                caseDto.TotalNoOfCollateral = await _cbeContext.Collaterals.CountAsync(res => res.CaseId == caseDto.Id);
            }
            return caseDtos;
        }
        public async Task<IEnumerable<CaseDto>> GetMMPendingCases(Guid userId)
        {

            var caseAssignments = await _cbeContext.CaseAssignments.Include(res => res.Collateral).ThenInclude(res => res.Case).ThenInclude(res => res.CaseOriginator).Where(Ca => Ca.UserId == userId && Ca.Status == "Pending").ToListAsync();
            var uniqueCases = caseAssignments.Select(ca => ca.Collateral.Case).DistinctBy(c => c.Id).ToList();
            var caseDtos = _mapper.Map<IEnumerable<CaseDto>>(uniqueCases);
            foreach (var caseDto in caseDtos)
            {
                caseDto.TotalNoOfCollateral = await _cbeContext.Collaterals.CountAsync(res => res.CaseId == caseDto.Id);
            }
            return caseDtos;
        }
        public async Task<IEnumerable<CaseDto>> GetMoRemarkedCases(Guid userId)
        {

            var caseAssignments = await _cbeContext.CaseAssignments.Include(res => res.Collateral).ThenInclude(res => res.Case).ThenInclude(res => res.CaseOriginator).Where(Ca => Ca.UserId == userId && Ca.Status == "Remark").ToListAsync();
            var uniqueCases = caseAssignments.Select(ca => ca.Collateral.Case).DistinctBy(c => c.Id).ToList();
            var caseDtos = _mapper.Map<IEnumerable<CaseDto>>(uniqueCases);
            foreach (var caseDto in caseDtos)
            {
                caseDto.TotalNoOfCollateral = await _cbeContext.Collaterals.CountAsync(res => res.CaseId == caseDto.Id);
            }
            return caseDtos;
        }
        //public async Task<IEnumerable<MMCaseDto>> GetMmLatestCases()
        //{
        //    var httpContext = _httpContextAccessor.HttpContext;
        //    var user = await _cbeContext.CreateUsers.FirstOrDefaultAsync(ca => ca.Id == Guid.Parse(httpContext.Session.GetString("userId")) && ca.Role.Name == "Maker Manager");
        //    if (user == null)
        //    {
        //        return null;
        //    }
        //    var cases = await _cbeContext.Cases
        //            .Include(x => x.Collaterals)
        //            .Include(x => x.District)
        //            .Where(res => res.DistrictId == user.DistrictId && res.CurrentStage=="Maker")
        //            .OrderByDescending(res => res.CreationDate).Take(7).ToListAsync();
        //    return _mapper.Map<IEnumerable<MMCaseDto>>(cases);
        //}
        //public async Task<CaseCountDto> GetDashboardCaseCount()
        //{
        //    var httpContext = _httpContextAccessor.HttpContext;
        //    var user = await _cbeContext.CreateUsers.FirstOrDefaultAsync(ca => ca.Id == Guid.Parse(httpContext.Session.GetString("userId")) && ca.Role.Name == "Maker Manager");
        //    if (user == null)
        //    {
        //        return null;
        //    }
        //    return new CaseCountDto()
        //    {
        //        NewCaseCount = await _cbeContext.Cases.Where(res => res.DistrictId == user.DistrictId && res.CurrentStage == "Maker" && res.CurrentStatus == "New").CountAsync(),
        //        PendingCaseCount = await _cbeContext.Cases.Where(res => res.DistrictId == user.DistrictId && res.CurrentStage == "Maker" &&  res.CurrentStatus == "Pending").CountAsync(),
        //        CompletedCaseCount = await _cbeContext.Cases.Where(res => res.DistrictId == user.DistrictId && res.CurrentStage == "Checker Manager" && res.CurrentStatus == "Complete").CountAsync()
        //    };
        //}   

        // public async Task<IEnumerable<CaseDto>> GetMoRemarkedPCECases(Guid userId)
        // {

        //     var pceCaseAssignments = await _cbeContext.ProductionCaseAssignments.Include(res => res.ProductionCapacity).ThenInclude(res => res.PCECase).Where(Ca => Ca.UserId == userId && Ca.Status == "Remark").ToListAsync();
        //     var uniquePCECases = pceCaseAssignments.Select(ca => ca.ProductionCapacity.PCECase).DistinctBy(c => c.Id).ToList();
        //     var pceCaseDtos = _mapper.Map<IEnumerable<PCECaseDto>>(uniquePCECases);
        //     foreach (var pceCaseDto in pceCaseDtos)
        //     {
        //         pceCaseDto.TotalNoOfCollateral = await _cbeContext.ProductionCapacities.CountAsync(res => res.PCECaseId == pceCaseDto.Id);
        //     }
        //     return pceCaseDtos;
        // }    
    }
}
