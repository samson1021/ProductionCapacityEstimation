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
                var pceCase = _mapper.Map<ProductionCapacityCorrection>(createCorrectionDto);
                var getcaseId = _cbeContext.Collaterals.Where(c => c.Id == createCorrectionDto.ProductionCapacityId).Select(c => c.CaseId).FirstOrDefault();
                pceCase.ProductionCapacityId = getcaseId;
                pceCase.CommentedByUserId = Guid.Parse(httpContext.Session.GetString("userId"));
                pceCase.CreationDate = DateTime.Now;
                await _cbeContext.ProductionCapacityCorrections.AddAsync(pceCase);

                await _cbeContext.SaveChangesAsync();  
                await transaction.CommitAsync();

                return _mapper.Map<ProductionCapacityCorrectionReturnDto>(pceCase);

                //await _IPCECaseTimeLineService.GetPCECaseTimeLines(new PCECaseTimeLinePostDto
                //{
                //    CaseId = pceCase.PCECaseId,
                //    Activity = $"<strong>A PCE case with ID {pceCase.PCECaseId} out of the productions list one production wiht ID {createCorrectionDto.ProductionCapacityId} has been Returned to Maker  For correction</strong>",
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
            var pceCase = await _cbeContext.ProductionCapacityCorrections.Include(res => res.ProductionCapacityId).FirstOrDefaultAsync(c => c.Id == Id);
            return _mapper.Map<ProductionCapacityCorrectionPostDto>(pceCase);
        }


    }
}

