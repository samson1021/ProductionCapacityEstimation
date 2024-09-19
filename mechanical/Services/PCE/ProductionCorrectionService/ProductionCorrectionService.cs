using AutoMapper;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;

using mechanical.Data;
using mechanical.Models.Entities;

using mechanical.Models.PCE.Entities;
using mechanical.Models.PCE.Dto.PCECaseTimeLineDto;
using mechanical.Models.PCE.Dto.ProductionCapacityCorrectionDto;
using mechanical.Services.PCE.PCECaseTimeLineService;

namespace mechanical.Services.PCE.ProductionCorrectionService
{
    public class ProductionCorrectionService : IProductionCorrectionService
    {
        private readonly CbeContext _cbeContext;
        private readonly IMapper _mapper;
        private readonly ILogger<ProductionCorrectionService> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IPCECaseTimeLineService _IPCECaseTimeLineService;
        public ProductionCorrectionService(CbeContext cbeContext, IMapper mapper, ILogger<ProductionCorrectionService> logger, IHttpContextAccessor httpContextAccessor, IPCECaseTimeLineService IPCECaseTimeLineService)
        {
            _cbeContext = cbeContext;
            _mapper = mapper;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
            _IPCECaseTimeLineService = _IPCECaseTimeLineService;

        }
        public async Task<ProductionCapacityCorrectionReturnDto> CreateProductionCorrection(ProductionCapacityCorrectionPostDto createCorrectionDto)
        {            
            using var transaction = await _cbeContext.Database.BeginTransactionAsync();
            try
            {     
                var httpContext = _httpContextAccessor.HttpContext;
                var loanCase = _mapper.Map<ProductionCapacityCorrection>(createCorrectionDto);
                var getcaseId = _cbeContext.Collaterals.Where(c => c.Id == createCorrectionDto.ProductionCapacityId).Select(c => c.CaseId).FirstOrDefault();
                loanCase.ProductionCapacityId = getcaseId;
                loanCase.CommentedByUserId = Guid.Parse(httpContext.Session.GetString("userId"));
                loanCase.CreationDate = DateTime.Now;
                await _cbeContext.ProductionCapacityCorrections.AddAsync(loanCase);
                await _cbeContext.SaveChangesAsync();  
                await transaction.CommitAsync();
                return _mapper.Map<ProductionCapacityCorrectionReturnDto>(loanCase);

                //await _IPCECaseTimeLineService.GetPCECaseTimeLines(new PCECaseTimeLinePostDto
                //{
                //    CaseId = loanCase.PCECaseId,
                //    Activity = $"<strong>A case with ID {loanCase.PCECaseId} out of the collaterals list one collateral wiht ID {createCorrectionDto.ProductionCapacityId} has been Returned to Maker  For correction</strong>",
                //    CurrentStage = "Relation Manager"
                //});
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating correction");
                await transaction.RollbackAsync();
                throw new ApplicationException("An error occurred while creating correction.");
            }
        }

        public async Task<ProductionCapacityCorrectionPostDto> GetProductionCorrection(Guid Id)
        {
            var loanCase = await _cbeContext.ProductionCapacityCorrections.Include(res => res.ProductionCapacityId).FirstOrDefaultAsync(c => c.Id == Id);
            return _mapper.Map<ProductionCapacityCorrectionPostDto>(loanCase);
        }


    }
}

