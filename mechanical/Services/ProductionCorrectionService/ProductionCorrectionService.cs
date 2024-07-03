using AutoMapper;
using mechanical.Data;
using mechanical.Models.Dto.CaseTimeLineDto;
using mechanical.Models.Dto.Correction;
using mechanical.Models.Dto.ProductionCapcityCorrectionDto;
using mechanical.Models.Dto.ProductionCaseTimeLineDto;
using mechanical.Models.Entities;
using mechanical.Services.CaseTimeLineService;
using mechanical.Services.ProductionCaseTimeLineService;
using Microsoft.EntityFrameworkCore;

namespace mechanical.Services.ProductionCorrectionService
{
    public class ProductionCorrectionService : IProductionCorrectionService
    {
        private readonly CbeContext _cbeContext;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IProductionCaseTimeLineService _productionCaseTimeLineService ;
        public ProductionCorrectionService(CbeContext cbeContext, IMapper mapper, IHttpContextAccessor httpContextAccessor, IProductionCaseTimeLineService productionCaseTimeLineService)
        {
            _cbeContext = cbeContext;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
            _productionCaseTimeLineService = productionCaseTimeLineService;
            
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

            await _productionCaseTimeLineService.CreateProductionCaseTimeLine(new ProductionCaseTimeLinePostDto
            {
                ProductionCaseId = loanCase.ProductionCaseId,
                Activity = $"<strong>A case with ID {loanCase.ProductionCaseId} out of the collaterals list one collateral wiht ID {createCorrectionDto.ProductionCapacityId} has been Returned to Maker  For correction</strong>",
                CurrentStage = "Relation Manager"
            });

            return loanCase;
        }

        public async Task<ProductionCapcityCorrectionPostDto> GetProductionCorrection(Guid Id)
        {
            var loanCase = await _cbeContext.ProductionCapcityCorrections.Include(res => res.ProductionCapacityId).FirstOrDefaultAsync(c => c.Id == Id);
            return _mapper.Map<ProductionCapcityCorrectionPostDto>(loanCase);
        }
       

    }
}

