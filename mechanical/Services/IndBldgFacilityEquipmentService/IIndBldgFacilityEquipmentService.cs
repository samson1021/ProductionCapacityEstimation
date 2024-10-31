using AutoMapper;
using mechanical.Data;
using mechanical.Models.Dto.CollateralDto;
using mechanical.Models.Dto.IndBldgFacilityEquipmentDto;
using mechanical.Models.Dto.MotorVehicleDto;
using mechanical.Models.Entities;
using mechanical.Services.UploadFileService;

namespace mechanical.Services.IndBldgF
{
    public interface IIndBldgFacilityEquipmentService
    {
       
        Task<IndBldgFacilityEquipment> CreateIndBldgFacilityEquipment(Guid userId, IndBldgFacilityEquipmentPostDto indBldgFacilityEquipmentPostDto );
        Task<IndBldgFacilityEquipmentReturnDto> CheckIndBldgFacilityEquipment(Guid userId,Guid Id, IndBldgFacilityEquipmentPostDto indBldgFacilityEquipmentPostDto);
        Task<IndBldgFacilityEquipmentReturnDto> GetIndBldgFacilityEquipment(Guid Id);
        Task<Dictionary<string, string>> GetCollateralComment(Guid Id);
        Task<IndBldgFacilityEquipmentReturnDto> GetIndBldgFacilityEquipmentByCollateralId(Guid collateralId);
        Task<IndBldgFacilityEquipmentReturnDto> GetEvaluatedIndBldgFacilityEquipment(Guid Id);
        Task<IndBldgFacilityEquipmentPostDto> GetReturnedEvaluatedIndBldgFacilityEquipment(Guid Id);
        Task<IndBldgFacilityEquipment> EditIndBldgFacilityEquipment(Guid userId, IndBldgFacilityEquipmentPostDto indBldgFacilityEquipmentPostDto);


    }
}
