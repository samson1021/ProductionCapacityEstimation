using AutoMapper;
using mechanical.Data;
using mechanical.Models.Dto.InternalReport;
using mechanical.Services.CaseTimeLineService;
using mechanical.Services.UploadFileService;
using Microsoft.EntityFrameworkCore;


namespace mechanical.Services.InternalReportService
{
    public class InternalReportService: IInternalReportService
    {
        private readonly CbeContext _cbeContext;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ICaseTimeLineService _caseTimeLineService;
        private readonly IUploadFileService _uploadFileService;
        public InternalReportService(CbeContext cbeContext, IMapper mapper, IHttpContextAccessor httpContextAccessor, IUploadFileService uploadFileService, ICaseTimeLineService caseTimeLineService)
        {
            _cbeContext = cbeContext;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
            _caseTimeLineService = caseTimeLineService;
            _uploadFileService = uploadFileService;
        }
 
        public async Task<IEnumerable<InternalCaseReportDto>> GetCaseReport(Guid userId)
        {

            var caseAssignments = await _cbeContext.CaseAssignments.Include(res => res.Collateral)
                                    .ThenInclude(res => res.Case)
                                    .ThenInclude(res => res.CaseOriginator)
                                    .Where(Ca => Ca.UserId == userId)
                                    .ToListAsync();

            var uniqueCases = caseAssignments.Select(ca => ca.Collateral.Case) 
                                    .DistinctBy(c => c.Id).ToList();

            var caseDtos = _mapper.Map<IEnumerable<InternalCaseReportDto>>(uniqueCases);
            foreach (var caseDto in caseDtos)
            {
                caseDto.TotalNoOfCollateral = await _cbeContext.Collaterals.CountAsync(res => res.CaseId == caseDto.Id);
            }
            return caseDtos;
        }

      
    }
}
