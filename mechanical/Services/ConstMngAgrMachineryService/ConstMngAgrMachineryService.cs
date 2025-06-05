using AutoMapper;
using mechanical.Data;
using mechanical.Models;
using mechanical.Models.Dto.CaseTimeLineDto;
using mechanical.Models.Dto.ConstMngAgrMachineryDto;
using mechanical.Models.Dto.IndBldgFacilityEquipmentDto;
using mechanical.Models.Dto.MotorVehicleDto;
using mechanical.Models.Entities;
using mechanical.Services.AnnexService;
using mechanical.Services.CaseTimeLineService;
using Microsoft.EntityFrameworkCore;

namespace mechanical.Services.ConstMngAgrMachineryService
{
    public class ConstMngAgrMachineryService : IConstMngAgrMachineryService
    {
        private readonly CbeContext _cbeContext;
        private readonly IMapper _mapper;
        private readonly IAnnexService _constMngAgrAnnexService;
        private readonly ICaseTimeLineService _caseTimeLineService;
        public ConstMngAgrMachineryService(CbeContext cbeContext, IMapper mapper, ICaseTimeLineService caseTimeLineService, IAnnexService constMngAgAnnexService)
        {
            _cbeContext = cbeContext;
            _mapper = mapper;
            _constMngAgrAnnexService = constMngAgAnnexService;
            _caseTimeLineService = caseTimeLineService;
        }
        public async Task<ConstMngAgrMachinery> CreateConstMngAgrMachinery(Guid userId, ConstMngAgrMachineryPostDto constMngAgrMachineryPostDto)
        {
            var constMngAgrMachinery = _mapper.Map<ConstMngAgrMachinery>(constMngAgrMachineryPostDto);

            var collateral = await _cbeContext.Collaterals.FindAsync(constMngAgrMachinery.CollateralId);

            constMngAgrMachinery.MarketShareFactor = await _constMngAgrAnnexService.GetCAMIBFMarketShareFactor(constMngAgrMachineryPostDto.TechnologyStandard);
            constMngAgrMachinery.DepreciationRate = await _constMngAgrAnnexService.GetCAMDepreciationRate(DateTime.UtcNow.Year - constMngAgrMachineryPostDto.YearOfManufacture, constMngAgrMachineryPostDto.constructionMiningAgriculturalMachineryType);
            constMngAgrMachinery.EqpmntConditionFactor = await _constMngAgrAnnexService.GetEquipmentConditionFactor(constMngAgrMachineryPostDto.CurrentEqpmntCondition, constMngAgrMachineryPostDto.AllocatedPointsRange);
            constMngAgrMachinery.ReplacementCost = (constMngAgrMachinery.InvoiceValue * constMngAgrMachinery.ExchangeRate);
            constMngAgrMachinery.NetEstimationValue = constMngAgrMachinery.MarketShareFactor * constMngAgrMachinery.DepreciationRate * constMngAgrMachinery.EqpmntConditionFactor * constMngAgrMachinery.ReplacementCost;
            constMngAgrMachinery.EvaluatorUserID = userId;
            await _cbeContext.ConstMngAgrMachineries.AddAsync(constMngAgrMachinery);
            await _cbeContext.SaveChangesAsync();
            await _caseTimeLineService.CreateCaseTimeLine(new CaseTimeLinePostDto
            {
                CaseId = collateral.CaseId,
                Activity = $" <strong class='text-sucess'>collateral maker Evaluation has been Completed. </strong> <br> <i class='text-purple'>Property Owner:</i> {collateral.PropertyOwner}. &nbsp; <i class='text-purple'>Role:</i> {collateral.Role}.&nbsp; <i class='text-purple'>Collateral Category:</i> {EnumHelper.GetEnumDisplayName(collateral.Category)}. &nbsp; <i class='text-purple'>Collateral Type:</i> {EnumHelper.GetEnumDisplayName(collateral.Type)}.",
                CurrentStage = "Maker Manager"
            });

            return constMngAgrMachinery;
        }
        public async Task<ConstMngAgMachineryReturnDto> CheckConstMngAgrMachinery(Guid userId, Guid Id, ConstMngAgrMachineryPostDto constMngAgrMachineryPostDto)
        {
            var constMngAgrMachinery = _mapper.Map<ConstMngAgrMachinery>(constMngAgrMachineryPostDto);

            var collateral = await _cbeContext.Collaterals.FindAsync(constMngAgrMachinery.CollateralId);

            constMngAgrMachinery.MarketShareFactor = await _constMngAgrAnnexService.GetCAMIBFMarketShareFactor(constMngAgrMachineryPostDto.TechnologyStandard);
            constMngAgrMachinery.DepreciationRate = await _constMngAgrAnnexService.GetCAMDepreciationRate(DateTime.UtcNow.Year - constMngAgrMachineryPostDto.YearOfManufacture, constMngAgrMachineryPostDto.constructionMiningAgriculturalMachineryType);
            constMngAgrMachinery.EqpmntConditionFactor = await _constMngAgrAnnexService.GetEquipmentConditionFactor(constMngAgrMachineryPostDto.CurrentEqpmntCondition, constMngAgrMachineryPostDto.AllocatedPointsRange);
            constMngAgrMachinery.ReplacementCost = (constMngAgrMachinery.InvoiceValue * constMngAgrMachinery.ExchangeRate);
            constMngAgrMachinery.NetEstimationValue = constMngAgrMachinery.MarketShareFactor * constMngAgrMachinery.DepreciationRate * constMngAgrMachinery.EqpmntConditionFactor * constMngAgrMachinery.ReplacementCost;
            var constMngAgrMachineryCheck = _mapper.Map<ConstMngAgMachineryReturnDto>(constMngAgrMachinery);
            constMngAgrMachineryCheck.Id = Id;
            return constMngAgrMachineryCheck;
        }

        public async Task<ConstMngAgMachineryReturnDto> GetConstMngAgrMachinery(Guid Id)
        {
            var constMngAgrMachinery = await _cbeContext.ConstMngAgrMachineries.Include(res => res.Collateral).FirstOrDefaultAsync(res => res.Id == Id);
            return _mapper.Map<ConstMngAgMachineryReturnDto>(constMngAgrMachinery);

        }
        public async Task<ConstMngAgMachineryReturnDto> GetConstMngAgrMachineryByCollateralId(Guid collateralId)
        {
            var motorVehicle = await _cbeContext.ConstMngAgrMachineries.Include(res => res.Collateral).FirstOrDefaultAsync(res => res.CollateralId == collateralId);
            return _mapper.Map<ConstMngAgMachineryReturnDto>(motorVehicle);

        }
        public async Task<ConstMngAgMachineryReturnDto> GetEvaluatedConstMngAgrMachinery(Guid Id)
        {
            CaseCommenAttributeDto caseCommenAttributeDto = new CaseCommenAttributeDto();
            ReturnEvaluatedCaseDto returnEvaluatedCaseDto = new ReturnEvaluatedCaseDto();
            var constMngAgrMachinery = await _cbeContext.ConstMngAgrMachineries.Include(res => res.Collateral).FirstOrDefaultAsync(res => res.CollateralId == Id);
            return _mapper.Map<ConstMngAgMachineryReturnDto>(constMngAgrMachinery);
        }
        public async Task<Dictionary<string, string>> GetCollateralComment(Guid Id)
        {
            var comments = await _cbeContext.Corrections.Where(c => c.CollateralID == Id).ToListAsync();

            Dictionary<string, string> chekerComment = new Dictionary<string, string>();

            foreach (var comment in comments)
            {
                chekerComment[comment.CommentedAttribute] = comment.Comment;
            }

            return chekerComment;
        }

        public async Task<ConstMngAgrMachineryPostDto> GetReturnedEvaluatedConstMngAgrMachinery(Guid Id)
        {
            var constMngAgrMachinery = await _cbeContext.ConstMngAgrMachineries.Include(res => res.Collateral).FirstOrDefaultAsync(res => res.CollateralId == Id);
            return _mapper.Map<ConstMngAgrMachineryPostDto>(constMngAgrMachinery);
        }
        //this service is to edit the collateral 
        public async Task<ConstMngAgrMachinery> EditConstMngAgrMachinery(Guid Id, ConstMngAgrMachineryPostDto constMngAgrMachineryPostDto)
        {
            var constMngAgrMachinery = await _cbeContext.ConstMngAgrMachineries.FindAsync(Id);

            _mapper.Map(constMngAgrMachineryPostDto, constMngAgrMachinery);


            var collateral = await _cbeContext.Collaterals.FindAsync(constMngAgrMachinery.CollateralId);

            constMngAgrMachinery.MarketShareFactor = await _constMngAgrAnnexService.GetCAMIBFMarketShareFactor(constMngAgrMachineryPostDto.TechnologyStandard);
            constMngAgrMachinery.DepreciationRate = await _constMngAgrAnnexService.GetCAMDepreciationRate(DateTime.UtcNow.Year - constMngAgrMachineryPostDto.YearOfManufacture, constMngAgrMachineryPostDto.constructionMiningAgriculturalMachineryType);
            constMngAgrMachinery.EqpmntConditionFactor = await _constMngAgrAnnexService.GetEquipmentConditionFactor(constMngAgrMachineryPostDto.CurrentEqpmntCondition, constMngAgrMachineryPostDto.AllocatedPointsRange);
            constMngAgrMachinery.ReplacementCost = (constMngAgrMachinery.InvoiceValue * constMngAgrMachinery.ExchangeRate);
            constMngAgrMachinery.NetEstimationValue = constMngAgrMachinery.MarketShareFactor * constMngAgrMachinery.DepreciationRate * constMngAgrMachinery.EqpmntConditionFactor * constMngAgrMachinery.ReplacementCost;
            constMngAgrMachinery.LastUpdatedAt = DateTime.UtcNow;
            _cbeContext.Update(constMngAgrMachinery);
            await _cbeContext.SaveChangesAsync();
            await _caseTimeLineService.CreateCaseTimeLine(new CaseTimeLinePostDto
            {
                CaseId = collateral.CaseId,
                Activity = $" <strong class=\"text-sucess\">collateral maker valuation Correction has been Completed. </strong> <br> <i class='text-purple'>Property Owner:</i> {collateral.PropertyOwner}. &nbsp; <i class='text-purple'>Role:</i> {collateral.Role}.&nbsp; <i class='text-purple'>Collateral Category:</i> {EnumHelper.GetEnumDisplayName(collateral.Category)}. &nbsp; <i class='text-purple'>Collateral Type:</i> {EnumHelper.GetEnumDisplayName(collateral.Type)}.",
                CurrentStage = "Maker Manager"
            });

            return constMngAgrMachinery;
        }



    }
}
