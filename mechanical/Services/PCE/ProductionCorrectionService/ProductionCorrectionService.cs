using AutoMapper;
using mechanical.Data;
using mechanical.Models.Dto.CaseTimeLineDto;
using mechanical.Models.Dto.Correction;
using mechanical.Models.PCE.Dto.ProductionCapcityCorrectionDto;

using mechanical.Models.Entities;
using mechanical.Services.CaseTimeLineService;
using Microsoft.EntityFrameworkCore;
using mechanical.Services.PCE.PCECaseTimeLineService;
using mechanical.Models.PCE.Entities;
using mechanical.Models.PCE.Dto.PCECaseTimeLineDto;

namespace mechanical.Services.PCE.ProductionCorrectionService
{
    public class ProductionCorrectionService : IProductionCorrectionService
    {
        private readonly CbeContext _cbeContext;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IPCECaseTimeLineService _IPCECaseTimeLineService;
        public ProductionCorrectionService(CbeContext cbeContext, IMapper mapper, IHttpContextAccessor httpContextAccessor, IPCECaseTimeLineService IPCECaseTimeLineService)
        {
            _cbeContext = cbeContext;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
            _IPCECaseTimeLineService = _IPCECaseTimeLineService;

        }
        public async Task<ProductionCapcityCorrection> CreateProductionCorrection(ProductionCapcityCorrectionPostDto createCorrectionDto)
        {
            var httpContext = _httpContextAccessor.HttpContext;
            var loanCase = _mapper.Map<ProductionCapcityCorrection>(createCorrectionDto);
            var getcaseId = _cbeContext.Collaterals.Where(c => c.Id == createCorrectionDto.ProductionCapacityId).Select(c => c.CaseId).FirstOrDefault();
            loanCase.ProductionCapacityId = getcaseId;
            loanCase.CommentedByUserId = Guid.Parse(httpContext.Session.GetString("userId"));
            loanCase.CreationDate = DateTime.Now;
            await _cbeContext.ProductionCapcityCorrections.AddAsync(loanCase);
            await _cbeContext.SaveChangesAsync();

            //await _IPCECaseTimeLineService.GetPCECaseTimeLines(new PCECaseTimeLinePostDto
            //{
            //    CaseId = loanCase.PCECaseId,
            //    Activity = $"<strong>A case with ID {loanCase.PCECaseId} out of the collaterals list one collateral wiht ID {createCorrectionDto.ProductionCapacityId} has been Returned to Maker  For correction</strong>",
            //    CurrentStage = "Relation Manager"
            //});

            return loanCase;
        }

        public async Task<ProductionCapcityCorrectionPostDto> GetProductionCorrection(Guid Id)
        {
            var loanCase = await _cbeContext.ProductionCapcityCorrections.Include(res => res.ProductionCapacityId).FirstOrDefaultAsync(c => c.Id == Id);
            return _mapper.Map<ProductionCapcityCorrectionPostDto>(loanCase);
        }


    }
}

