using AutoMapper;
using mechanical.Data;
using mechanical.Models;
using mechanical.Models.Dto.CaseTimeLineDto;
using mechanical.Models.Dto.CollateralDto;
using mechanical.Models.Dto.MotorVehicleDto;
using mechanical.Models.Entities;
using mechanical.Services.AnnexService;
using mechanical.Services.CaseTimeLineService;
using Microsoft.EntityFrameworkCore;
using System.Runtime.InteropServices;

namespace mechanical.Services.MotorVehicleService
{
    public class MotorVehicleService : IMotorVehicleService
    {
        private readonly CbeContext _cbeContext;
        private readonly IMapper _mapper;
        private readonly IAnnexService _motorVehicleAnnexService;
        private readonly ICaseTimeLineService _caseTimeLineService;
        public MotorVehicleService(CbeContext cbeContext, IMapper mapper, ICaseTimeLineService caseTimeLineService, IAnnexService motorVehicleAnnexService)
        {
            _cbeContext = cbeContext;
            _mapper = mapper;
            _motorVehicleAnnexService = motorVehicleAnnexService;
            _caseTimeLineService = caseTimeLineService;
        }
        public async Task<MotorVehicle> CreateMotorVehicle( Guid userId, CreateMotorVehicleDto createMotorVehicleDto)
        {
            var motorVehicle = _mapper.Map<MotorVehicle>(createMotorVehicleDto);
            motorVehicle.InvoiceValue = motorVehicle.InvoiceValue * motorVehicle.ExchangeRate;

            var collateral = await _cbeContext.Collaterals.FindAsync(motorVehicle.CollateralId);

            motorVehicle.MarketShareFactor = await _motorVehicleAnnexService.GetMOVMarketShareFactor(motorVehicle.MotorVehicleMake, motorVehicle.BodyType);
            motorVehicle.DepreciationRate = await _motorVehicleAnnexService.GetMOVDepreciationRate(DateTime.Now.Year - motorVehicle.YearOfManufacture, motorVehicle.BodyType);
            motorVehicle.EqpmntConditionFactor = await _motorVehicleAnnexService.GetEquipmentConditionFactor(motorVehicle.CurrentEqpmntCondition, motorVehicle.AllocatedPointsRange);
            motorVehicle.ReplacementCost = motorVehicle.InvoiceValue;
            motorVehicle.NetEstimationValue = motorVehicle.MarketShareFactor * motorVehicle.DepreciationRate * motorVehicle.EqpmntConditionFactor * motorVehicle.ReplacementCost;
            motorVehicle.EvaluatorUserID = userId;
            _cbeContext.MotorVehicles.Add(motorVehicle);
            _cbeContext.Update(collateral);
            await _cbeContext.SaveChangesAsync();
            await _caseTimeLineService.CreateCaseTimeLine(new CaseTimeLinePostDto
            {
                CaseId = collateral.CaseId,
                Activity = $" <strong class=\"text-sucess\">collateral maker Evaluation has been Completed. </strong> <br> <i class='text-purple'>Property Owner:</i> {collateral.PropertyOwner}. &nbsp; <i class='text-purple'>Role:</i> {collateral.Role}.&nbsp; <i class='text-purple'>Collateral Catagory:</i> {EnumHelper.GetEnumDisplayName(collateral.Category)}. &nbsp; <i class='text-purple'>Collateral Type:</i> {EnumHelper.GetEnumDisplayName(collateral.Type)}.",
                CurrentStage = "Maker Manager"
            });

            return motorVehicle;
        }
        public async Task<ReturnMotorVehicleDto> CheckMotorVehicle(Guid userId, Guid Id,CreateMotorVehicleDto createMotorVehicleDto)
        {
            var motorVehicle = _mapper.Map<MotorVehicle>(createMotorVehicleDto);
            motorVehicle.InvoiceValue = motorVehicle.InvoiceValue * motorVehicle.ExchangeRate;

            var collateral = await _cbeContext.Collaterals.FindAsync(motorVehicle.CollateralId);

            motorVehicle.MarketShareFactor = await _motorVehicleAnnexService.GetMOVMarketShareFactor(motorVehicle.MotorVehicleMake, motorVehicle.BodyType);
            motorVehicle.DepreciationRate = await _motorVehicleAnnexService.GetMOVDepreciationRate(DateTime.Now.Year - motorVehicle.YearOfManufacture, motorVehicle.BodyType);
            motorVehicle.EqpmntConditionFactor = await _motorVehicleAnnexService.GetEquipmentConditionFactor(motorVehicle.CurrentEqpmntCondition, motorVehicle.AllocatedPointsRange);
            motorVehicle.ReplacementCost = motorVehicle.InvoiceValue;
            motorVehicle.NetEstimationValue = motorVehicle.MarketShareFactor * motorVehicle.DepreciationRate * motorVehicle.EqpmntConditionFactor * motorVehicle.ReplacementCost;
            var motervechleReturn = _mapper.Map<ReturnMotorVehicleDto>(motorVehicle);
            motervechleReturn.Id = Id;
            return motervechleReturn;
        }

        public async Task<ReturnMotorVehicleDto> GetMotorVehicle(Guid Id)
        {
            var motorVehicle = await _cbeContext.MotorVehicles.Include(res=>res.Collateral).FirstOrDefaultAsync(res=>res.Id==Id);
            return _mapper.Map<ReturnMotorVehicleDto>(motorVehicle);

        }
        public async Task<ReturnMotorVehicleDto> GetMotorVehicleByCollateralId(Guid collateralId)
        {
            var motorVehicle = await _cbeContext.MotorVehicles.Include(res => res.Collateral).FirstOrDefaultAsync(res => res.CollateralId == collateralId);
            return _mapper.Map<ReturnMotorVehicleDto>(motorVehicle);

        }
        public async Task<ReturnEvaluatedCaseDto> GetEvaluatedMotorVehicle(Guid Id)
        {
            CaseCommenAttributeDto caseCommenAttributeDto = new CaseCommenAttributeDto();
            ReturnEvaluatedCaseDto returnEvaluatedCaseDto = new ReturnEvaluatedCaseDto();
            var motorVehicle = await _cbeContext.MotorVehicles.Include(res => res.Collateral).FirstOrDefaultAsync(res => res.CollateralId == Id);
            if (motorVehicle != null)
            {
                var motorVehicles =  _cbeContext.Collaterals.Where(res => res.Id == motorVehicle.CollateralId).FirstOrDefault();
                caseCommenAttributeDto.PropertyOwner = motorVehicles.PropertyOwner;
                caseCommenAttributeDto.Role = motorVehicles.Role;
                caseCommenAttributeDto.Type = motorVehicles.Type.ToString();
                caseCommenAttributeDto.Category = motorVehicles.Category.ToString();
                caseCommenAttributeDto.CollateralId = motorVehicle.CollateralId;
            }

            returnEvaluatedCaseDto.ReturnMotorVehicleDto = _mapper.Map< ReturnMotorVehicleDto> (motorVehicle);
            returnEvaluatedCaseDto.CaseCommenAttributeDto = caseCommenAttributeDto;

            return returnEvaluatedCaseDto;
        }


        public async Task<MotorVehicle> EditMotorVehicle(Guid Id, CreateMotorVehicleDto createMotorVehicleDto)
        {
            var motorVehicle = await _cbeContext.MotorVehicles.FindAsync(Id);

            _mapper.Map(createMotorVehicleDto, motorVehicle);

            motorVehicle.InvoiceValue = motorVehicle.InvoiceValue * motorVehicle.ExchangeRate;
            var collateral = await _cbeContext.Collaterals.FindAsync(motorVehicle.CollateralId);

            motorVehicle.MarketShareFactor = await _motorVehicleAnnexService.GetMOVMarketShareFactor(motorVehicle.MotorVehicleMake, motorVehicle.BodyType);
            motorVehicle.DepreciationRate = await _motorVehicleAnnexService.GetMOVDepreciationRate(DateTime.Now.Year - motorVehicle.YearOfManufacture, motorVehicle.BodyType);
            motorVehicle.EqpmntConditionFactor = await _motorVehicleAnnexService.GetEquipmentConditionFactor(motorVehicle.CurrentEqpmntCondition, motorVehicle.AllocatedPointsRange);
            motorVehicle.ReplacementCost = motorVehicle.InvoiceValue;
            motorVehicle.NetEstimationValue = motorVehicle.MarketShareFactor * motorVehicle.DepreciationRate * motorVehicle.EqpmntConditionFactor * motorVehicle.ReplacementCost;
           
            _cbeContext.Update(motorVehicle);
            await _cbeContext.SaveChangesAsync();
            await _caseTimeLineService.CreateCaseTimeLine(new CaseTimeLinePostDto
            {
                CaseId = collateral.CaseId,
                Activity = $" <strong class=\"text-sucess\">collateral maker Revaluation has been Completed. </strong> <br> <i class='text-purple'>Property Owner:</i> {collateral.PropertyOwner}. &nbsp; <i class='text-purple'>Role:</i> {collateral.Role}.&nbsp; <i class='text-purple'>Collateral Catagory:</i> {EnumHelper.GetEnumDisplayName(collateral.Category)}. &nbsp; <i class='text-purple'>Collateral Type:</i> {EnumHelper.GetEnumDisplayName(collateral.Type)}.",
                CurrentStage = "Maker Manager"
            });

            return motorVehicle;
        }

    }
}
