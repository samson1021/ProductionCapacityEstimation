using AutoMapper;
using mechanical.Data;
using mechanical.Models;
using mechanical.Models.Dto.CaseTimeLineDto;
using mechanical.Models.Dto.CollateralDto;
using mechanical.Models.Dto.ConstMngAgrMachineryDto;
using mechanical.Models.Dto.IndBldgFacilityEquipmentDto;
using mechanical.Models.Dto.MotorVehicleDto;
using mechanical.Models.Entities;
using mechanical.Services.AnnexService;
using mechanical.Services.CaseTimeLineService;
using mechanical.Services.IndBldgF;
using Microsoft.EntityFrameworkCore;
using System.Runtime.InteropServices;

namespace mechanical.Services.IndBldgFacilityEquipmentService
{
    public class IndBldgFacilityEquipmentService : IIndBldgFacilityEquipmentService
    {
        private readonly CbeContext _cbeContext;
        private readonly IMapper _mapper;
        private readonly IAnnexService _motorVehicleAnnexService;
        private readonly ICaseTimeLineService _caseTimeLineService;
        public IndBldgFacilityEquipmentService(CbeContext cbeContext, IMapper mapper, ICaseTimeLineService caseTimeLineService, IAnnexService motorVehicleAnnexService)
        {
            _cbeContext = cbeContext;
            _mapper = mapper;
            _motorVehicleAnnexService = motorVehicleAnnexService;
            _caseTimeLineService = caseTimeLineService;
        }
        public async Task<IndBldgFacilityEquipment> CreateIndBldgFacilityEquipment(Guid userId, IndBldgFacilityEquipmentPostDto indBldgFacilityEquipmentPostDto)
        {
            var indBldgFacilityEquipment = _mapper.Map<IndBldgFacilityEquipment>(indBldgFacilityEquipmentPostDto);

            var collateral = await _cbeContext.Collaterals.FindAsync(indBldgFacilityEquipment.CollateralId);

            indBldgFacilityEquipment.MarketShareFactor = await _motorVehicleAnnexService.GetCAMIBFMarketShareFactor(indBldgFacilityEquipment.TechnologyStandard);
            indBldgFacilityEquipment.DepreciationRate = await _motorVehicleAnnexService.GetIBMDepreciationRate(DateTime.Now.Year - indBldgFacilityEquipment.YearOfManufacture, indBldgFacilityEquipment.IndustrialBuildingMachineryType);
            indBldgFacilityEquipment.EqpmntConditionFactor = await _motorVehicleAnnexService.GetEquipmentConditionFactor(indBldgFacilityEquipment.CurrentEqpmntCondition, indBldgFacilityEquipment.AllocatedPointsRange);
            indBldgFacilityEquipment.ReplacementCost = (indBldgFacilityEquipment.InvoiceValue * indBldgFacilityEquipment.ExchangeRate);
            indBldgFacilityEquipment.NetEstimationValue = indBldgFacilityEquipment.MarketShareFactor * indBldgFacilityEquipment.DepreciationRate * indBldgFacilityEquipment.EqpmntConditionFactor * indBldgFacilityEquipment.ReplacementCost;
            indBldgFacilityEquipment.EvaluatorUserID = userId;
            _cbeContext.IndBldgFacilityEquipment.Add(indBldgFacilityEquipment);
            await _cbeContext.SaveChangesAsync();
            await _caseTimeLineService.CreateCaseTimeLine(new CaseTimeLinePostDto
            {
                CaseId = collateral.CaseId,
                Activity = $" <strong class=\"text-sucess\">collateral maker Evaluation has been Completed. </strong> <br> <i class='text-purple'>Property Owner:</i> {collateral.PropertyOwner}. &nbsp; <i class='text-purple'>Role:</i> {collateral.Role}.&nbsp; <i class='text-purple'>Collateral Category:</i> {EnumHelper.GetEnumDisplayName(collateral.Category)}. &nbsp; <i class='text-purple'>Collateral Type:</i> {EnumHelper.GetEnumDisplayName(collateral.Type)}.",
                CurrentStage = "Maker Manager"
            });

            return indBldgFacilityEquipment;
        }
        public async Task<IndBldgFacilityEquipmentReturnDto> CheckIndBldgFacilityEquipment(Guid userId,Guid Id, IndBldgFacilityEquipmentPostDto indBldgFacilityEquipmentPostDto)
        {
            var indBldgFacilityEquipment = _mapper.Map<IndBldgFacilityEquipment>(indBldgFacilityEquipmentPostDto);

            var collateral = await _cbeContext.Collaterals.FindAsync(indBldgFacilityEquipment.CollateralId);

            indBldgFacilityEquipment.MarketShareFactor = await _motorVehicleAnnexService.GetCAMIBFMarketShareFactor(indBldgFacilityEquipment.TechnologyStandard);
            indBldgFacilityEquipment.DepreciationRate = await _motorVehicleAnnexService.GetIBMDepreciationRate(DateTime.Now.Year - indBldgFacilityEquipment.YearOfManufacture, indBldgFacilityEquipment.IndustrialBuildingMachineryType);
            indBldgFacilityEquipment.EqpmntConditionFactor = await _motorVehicleAnnexService.GetEquipmentConditionFactor(indBldgFacilityEquipment.CurrentEqpmntCondition, indBldgFacilityEquipment.AllocatedPointsRange);
            indBldgFacilityEquipment.ReplacementCost = (indBldgFacilityEquipment.InvoiceValue * indBldgFacilityEquipment.ExchangeRate);
            indBldgFacilityEquipment.NetEstimationValue = indBldgFacilityEquipment.MarketShareFactor * indBldgFacilityEquipment.DepreciationRate * indBldgFacilityEquipment.EqpmntConditionFactor * indBldgFacilityEquipment.ReplacementCost;
            var indBldgFacilityReturn = _mapper.Map<IndBldgFacilityEquipmentReturnDto>(indBldgFacilityEquipment);
            indBldgFacilityReturn.Id = Id;

            return indBldgFacilityReturn;
        }
        public async Task<IndBldgFacilityEquipmentReturnDto> GetIndBldgFacilityEquipment(Guid Id)
        {
            var indBldgFacilityEquipment = await _cbeContext.IndBldgFacilityEquipment.Include(res=>res.Collateral).FirstOrDefaultAsync(res=>res.Id==Id);
            return _mapper.Map<IndBldgFacilityEquipmentReturnDto>(indBldgFacilityEquipment);

        }
        public async Task<IndBldgFacilityEquipmentReturnDto> GetIndBldgFacilityEquipmentByCollateralId(Guid collateralId)
        {
            var indBldgFacilityEquipment = await _cbeContext.IndBldgFacilityEquipment.Include(res => res.Collateral).FirstOrDefaultAsync(res => res.CollateralId == collateralId);
            return _mapper.Map<IndBldgFacilityEquipmentReturnDto>(indBldgFacilityEquipment);

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
        public async Task<IndBldgFacilityEquipmentReturnDto> GetEvaluatedIndBldgFacilityEquipment(Guid Id)
        {
            CaseCommenAttributeDto caseCommenAttributeDto = new CaseCommenAttributeDto();
            ReturnEvaluatedCaseDto returnEvaluatedCaseDto = new ReturnEvaluatedCaseDto();
            var indBldgFacilityEquipment = await _cbeContext.IndBldgFacilityEquipment.Include(res => res.Collateral).FirstOrDefaultAsync(res => res.CollateralId == Id);
            return _mapper.Map<IndBldgFacilityEquipmentReturnDto>(indBldgFacilityEquipment);
        }
        public async Task<IndBldgFacilityEquipmentPostDto> GetReturnedEvaluatedIndBldgFacilityEquipment(Guid Id)
        {
            var indBldgFacilityEquipment = await _cbeContext.IndBldgFacilityEquipment.Include(res => res.Collateral).FirstOrDefaultAsync(res => res.CollateralId == Id);
            return _mapper.Map<IndBldgFacilityEquipmentPostDto>(indBldgFacilityEquipment);
        }
        //this service is to edit the collateral 
        public async Task<IndBldgFacilityEquipment> EditIndBldgFacilityEquipment(Guid Id, IndBldgFacilityEquipmentPostDto indBldgFacilityEquipmentPostDto)
        {
            var indBldgFacilityEquipment =await _cbeContext.IndBldgFacilityEquipment.FindAsync(Id);
            _mapper.Map(indBldgFacilityEquipmentPostDto, indBldgFacilityEquipment);
            

            var collateral = await _cbeContext.Collaterals.FindAsync(indBldgFacilityEquipment.CollateralId);

            indBldgFacilityEquipment.MarketShareFactor = await _motorVehicleAnnexService.GetCAMIBFMarketShareFactor(indBldgFacilityEquipment.TechnologyStandard);
            indBldgFacilityEquipment.DepreciationRate = await _motorVehicleAnnexService.GetIBMDepreciationRate(DateTime.Now.Year - indBldgFacilityEquipment.YearOfManufacture, indBldgFacilityEquipment.IndustrialBuildingMachineryType);
            indBldgFacilityEquipment.EqpmntConditionFactor = await _motorVehicleAnnexService.GetEquipmentConditionFactor(indBldgFacilityEquipment.CurrentEqpmntCondition, indBldgFacilityEquipment.AllocatedPointsRange);
            indBldgFacilityEquipment.ReplacementCost = (indBldgFacilityEquipment.InvoiceValue * indBldgFacilityEquipment.ExchangeRate);
            indBldgFacilityEquipment.NetEstimationValue = indBldgFacilityEquipment.MarketShareFactor * indBldgFacilityEquipment.DepreciationRate * indBldgFacilityEquipment.EqpmntConditionFactor * indBldgFacilityEquipment.ReplacementCost;
            _cbeContext.Update(indBldgFacilityEquipment);
            await _cbeContext.SaveChangesAsync();
            await _caseTimeLineService.CreateCaseTimeLine(new CaseTimeLinePostDto
            {
                CaseId = collateral.CaseId,
                Activity = $" <strong class=\"text-sucess\">collateral maker valuation Correction has been Completed. </strong> <br> <i class='text-purple'>Property Owner:</i> {collateral.PropertyOwner}. &nbsp; <i class='text-purple'>Role:</i> {collateral.Role}.&nbsp; <i class='text-purple'>Collateral Category:</i> {EnumHelper.GetEnumDisplayName(collateral.Category)}. &nbsp; <i class='text-purple'>Collateral Type:</i> {EnumHelper.GetEnumDisplayName(collateral.Type)}.",
                CurrentStage = "Maker Manager"
            });

            return indBldgFacilityEquipment;


            

        }
    }
}
