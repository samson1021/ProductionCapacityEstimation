using mechanical.Models.Dto.CaseDto;
using mechanical.Models.Entities;
using mechanical.Models.Enum;

namespace mechanical.Services.AnnexService
{
    public interface IAnnexService
    {
        Task<double> GetMOVDepreciationRate(int serviceYear, BodyType bodyType);
        Task<double> GetMOVMarketShareFactor(MotorVehicleMake motorVehicleMake, BodyType bodyType);


        Task<double> GetCAMDepreciationRate(int serviceYear, ConstructionMiningAgriculturalMachineryType bodyType); 
        Task<double> GetIBMDepreciationRate(int serviceYear, IndustrialBuildingMachineryType bodyType);

       

        Task<double> GetCAMIBFMarketShareFactor(MachineryTechnologyStandard machineryTechnologyStandard);



        Task<double> GetEquipmentConditionFactor(EquipmentCondition equipmentCondition, AllocatedPointRange allocatedPointRange);
    }
}
