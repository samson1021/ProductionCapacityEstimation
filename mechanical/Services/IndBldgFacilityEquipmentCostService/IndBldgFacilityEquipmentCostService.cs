using AutoMapper;
using mechanical.Data;
using mechanical.Models.Dto.IndBldgFacilityEquipmentCostsDto;
using mechanical.Models.Dto.IndBldgFacilityEquipmentDto;
using mechanical.Models.Entities;
using Microsoft.CodeAnalysis.Operations;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace mechanical.Services.IndBldgFacilityEquipmentCostService
{
    public class IndBldgFacilityEquipmentCostService : IIndBldgFacilityEquipmentCostService
    {
        private readonly CbeContext _cbeContext;
        private readonly IMapper _mapper;
        public IndBldgFacilityEquipmentCostService(CbeContext cbeContext,IMapper mapper)
        {
            _cbeContext = cbeContext;
            _mapper = mapper;
        }
        public async Task<bool> Create(Guid caseId, IndBldgFacilityEquipmentCostsPostDto indBldgFacilityEquipmentCostsPostDto)
        {
            var Case = await _cbeContext.Cases.FindAsync(caseId);
            if (Case == null)
            {
                throw new ValidationException("Case not found");
            }
            try
            {
                var indBldgFacilityEquipmentCosts = _mapper.Map<IndBldgFacilityEquipmentCosts>(indBldgFacilityEquipmentCostsPostDto);
                indBldgFacilityEquipmentCosts.CaseId = caseId;
                await _cbeContext.IndBldgFacilityEquipmentCosts.AddAsync(indBldgFacilityEquipmentCosts);
                await _cbeContext.SaveChangesAsync();
                return true;
            }
            catch(Exception ex)
            {
                throw new ValidationException("Error while creating IndBldgFacilityEquipmentCosts: " + ex.Message);
            }
        }
        public async Task<IndBldgFacilityEquipmentCostsDto> Get(Guid id)
        {
            var indBldgFacilityEquipmentCosts = await _cbeContext.IndBldgFacilityEquipmentCosts.FindAsync(id);
            if (indBldgFacilityEquipmentCosts == null)
            {
                throw new ValidationException("Industrial and Building Facility Equipment Costs not found");
            }
            return _mapper.Map<IndBldgFacilityEquipmentCostsDto>(indBldgFacilityEquipmentCosts);
        }
        public async Task<IndBldgFacilityEquipmentCostsReturnDto> GetByCostId(Guid? id)
        {
            if (id != null)
            {
                var indBldgFacilityEquipment = await _cbeContext.IndBldgFacilityEquipment.Where(res => res.IndBldgFacilityEquipmentCostsId == id).ToListAsync();
                var indbldgFacilityEquipmentCost = await _cbeContext.IndBldgFacilityEquipmentCosts.Where(res => res.Id == id).FirstOrDefaultAsync();
                
                var item = _mapper.Map<IndBldgFacilityEquipmentCostsReturnDto>(indbldgFacilityEquipmentCost);


                double invoiceValue = 0;
                double marketShare = 0;
                double Depreciated = 0;
                double equipmentcondition = 0;
                double totalReplacmentCost = 0;
                double totalNetEstimationValue = 0;
                foreach (var indBldg in indBldgFacilityEquipment)
                {
                    invoiceValue += indBldg.InvoiceValue * indBldg.ExchangeRate;
                    marketShare += indBldg.MarketShareFactor;
                    Depreciated += indBldg.DepreciationRate;
                    equipmentcondition += indBldg.EqpmntConditionFactor;
                    totalReplacmentCost += indBldg.ReplacementCost;
                    totalNetEstimationValue += indBldg.NetEstimationValue;
                }
                if (indBldgFacilityEquipment.Count() > 0)
                {
                    marketShare = marketShare / indBldgFacilityEquipment.Count();
                    Depreciated = Depreciated / indBldgFacilityEquipment.Count();
                    equipmentcondition = equipmentcondition / indBldgFacilityEquipment.Count();
                }
                item.LandTransportLoadingUnloadingInstallationCommissioningCost = invoiceValue * 0.075;
                item.DepreciatedInsuranceFreightOthersCost = item.InsuranceFreightOthersCost * marketShare * Depreciated * equipmentcondition;
                item.DepreciatedLandTransportLoadingUnloadingInstallationCommissioningCost = item.LandTransportLoadingUnloadingInstallationCommissioningCost * marketShare * Depreciated * equipmentcondition;
                item.TotalReplacementCost = totalReplacmentCost + item.LandTransportLoadingUnloadingInstallationCommissioningCost + item.InsuranceFreightOthersCost;
                item.TotalNetEstimationValue = totalNetEstimationValue + item.DepreciatedInsuranceFreightOthersCost + item.DepreciatedLandTransportLoadingUnloadingInstallationCommissioningCost;
                item.RemainingCollateral = item.CollateralCount - indBldgFacilityEquipment.Count();
                return item;
            }
            return null;
        }
        public async Task<IEnumerable<IndBldgFacilityEquipmentCostsReturnDto>> GetByCaseId(Guid caseId)
        {
            var Case = await _cbeContext.Cases.FindAsync(caseId);
            if (Case == null)
            {
                throw new ValidationException("Case not found");
            }
            var indBldgFacilityEquipmentCosts = await _cbeContext.IndBldgFacilityEquipmentCosts.Where(res => res.CaseId == caseId).ToListAsync();
            if(indBldgFacilityEquipmentCosts!= null)
            {
                var indBldgFacilityEquipmentCostsReturnDto = _mapper.Map<IEnumerable<IndBldgFacilityEquipmentCostsReturnDto>>(indBldgFacilityEquipmentCosts);
                foreach (var item in indBldgFacilityEquipmentCostsReturnDto)
                {
                    var indBldgFacilityEquipment = await _cbeContext.IndBldgFacilityEquipment.Where(res => res.IndBldgFacilityEquipmentCostsId == item.Id).ToListAsync();
                    double invoiceValue = 0;
                    double marketShare = 0;
                    double Depreciated = 0;
                    double equipmentcondition = 0;
                    double totalReplacmentCost = 0;
                    double totalNetEstimationValue = 0;
                    foreach (var indBldg in indBldgFacilityEquipment)
                    {
                        invoiceValue += indBldg.InvoiceValue * indBldg.ExchangeRate;
                        marketShare += indBldg.MarketShareFactor;
                        Depreciated += indBldg.DepreciationRate;
                        equipmentcondition += indBldg.EqpmntConditionFactor;
                        totalReplacmentCost += indBldg.ReplacementCost;
                        totalNetEstimationValue += indBldg.NetEstimationValue;
                    }
                    if (indBldgFacilityEquipment.Count() > 0)
                    {
                        marketShare = marketShare / indBldgFacilityEquipment.Count();
                        Depreciated = Depreciated / indBldgFacilityEquipment.Count();
                        equipmentcondition = equipmentcondition / indBldgFacilityEquipment.Count();
                    }
                    item.LandTransportLoadingUnloadingInstallationCommissioningCost = invoiceValue * 0.075;
                    item.DepreciatedInsuranceFreightOthersCost = item.InsuranceFreightOthersCost * marketShare * Depreciated * equipmentcondition;
                    item.DepreciatedLandTransportLoadingUnloadingInstallationCommissioningCost = item.LandTransportLoadingUnloadingInstallationCommissioningCost * marketShare * Depreciated * equipmentcondition;
                    item.TotalReplacementCost = totalReplacmentCost + item.LandTransportLoadingUnloadingInstallationCommissioningCost + item.InsuranceFreightOthersCost;
                    item.TotalNetEstimationValue = totalNetEstimationValue + item.DepreciatedInsuranceFreightOthersCost + item.DepreciatedLandTransportLoadingUnloadingInstallationCommissioningCost;
                    item.RemainingCollateral = item.CollateralCount - indBldgFacilityEquipment.Count();
                }


                return indBldgFacilityEquipmentCostsReturnDto;
            }
            return _mapper.Map<IEnumerable<IndBldgFacilityEquipmentCostsReturnDto>>(indBldgFacilityEquipmentCosts);
        }

        public async Task<bool> Update(Guid id, IndBldgFacilityEquipmentCostsPostDto indBldgFacilityEquipmentCostsPostDto)
        {
            var indBldgFacilityEquipmentCosts = await _cbeContext.IndBldgFacilityEquipmentCosts.FindAsync(id);
            if (indBldgFacilityEquipmentCosts == null)
            {
                throw new ValidationException("Industrial and Building Facility Equipment Costs not found");
            }
            try
            {
                _mapper.Map(indBldgFacilityEquipmentCostsPostDto, indBldgFacilityEquipmentCosts);
                _cbeContext.IndBldgFacilityEquipmentCosts.Update(indBldgFacilityEquipmentCosts);
                await _cbeContext.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                throw new ValidationException("Error while updating IndBldgFacilityEquipmentCosts: " + ex.Message);
            }
        }

        public async Task<bool> Delete(Guid id)
        {
            var indBldgFacilityEquipmentCosts = await _cbeContext.IndBldgFacilityEquipmentCosts.FindAsync(id);
            if (indBldgFacilityEquipmentCosts == null)
            {
                throw new ValidationException("Industrial and Building Facility Equipment Costs not found");
            }
            try
            {
                _cbeContext.IndBldgFacilityEquipmentCosts.Remove(indBldgFacilityEquipmentCosts);
                await _cbeContext.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                throw new ValidationException("Error while Deleting IndBldgFacilityEquipmentCosts: " + ex.Message);
            }
        }

    }
}
