using AutoMapper;
using mechanical.Data;
using mechanical.Models.Dto.CollateralDto;
using mechanical.Models.Dto.MotorVehicleDto;
using mechanical.Models.Entities;
using mechanical.Services.UploadFileService;

namespace mechanical.Services.MotorVehicleService
{
    public interface IMotorVehicleService
    {
        Task<double> Currency(string currency, DateTime currencyDate);
        Task<MotorVehicle> CreateMotorVehicle(Guid userId, CreateMotorVehicleDto createMotorVehicleDto );
        Task<ReturnMotorVehicleDto> CheckMotorVehicle(Guid userId, Guid Id, CreateMotorVehicleDto createMotorVehicleDto);
        Task<ReturnMotorVehicleDto> GetMotorVehicle(Guid Id);
        Task<ReturnMotorVehicleDto> GetMotorVehicleByCollateralId(Guid collateralId);
        Task<ReturnEvaluatedCaseDto> GetEvaluatedMotorVehicle(Guid Id);
        Task<MotorVehicle> EditMotorVehicle(Guid id, CreateMotorVehicleDto createMotorVehicleDto);


    }
}
