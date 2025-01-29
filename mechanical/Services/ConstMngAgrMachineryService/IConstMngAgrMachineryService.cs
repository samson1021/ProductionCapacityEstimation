using mechanical.Models.Dto.ConstMngAgrMachineryDto;
using mechanical.Models.Dto.IndBldgFacilityEquipmentDto;
using mechanical.Models.Dto.MotorVehicleDto;
using mechanical.Models.Entities;

namespace mechanical.Services.ConstMngAgrMachineryService
{
    public interface IConstMngAgrMachineryService
    {
        Task<ConstMngAgrMachinery> CreateConstMngAgrMachinery(Guid userId,ConstMngAgrMachineryPostDto constMngAgrMachineryPostDto);
        Task<ConstMngAgMachineryReturnDto> CheckConstMngAgrMachinery(Guid userId,Guid Id, ConstMngAgrMachineryPostDto constMngAgrMachineryPostDto);
        Task<ConstMngAgMachineryReturnDto> GetConstMngAgrMachinery(Guid Id);
        Task<ConstMngAgMachineryReturnDto> GetConstMngAgrMachineryByCollateralId(Guid collateralId);
        Task<ConstMngAgMachineryReturnDto> GetEvaluatedConstMngAgrMachinery(Guid Id);
        Task<Dictionary<string, string>> GetCollateralComment(Guid Id);
        Task<ConstMngAgrMachineryPostDto> GetReturnedEvaluatedConstMngAgrMachinery(Guid Id);
        Task<ConstMngAgrMachinery> EditConstMngAgrMachinery(Guid userId, ConstMngAgrMachineryPostDto constMngAgrMachineryPostDto);
    }
}
