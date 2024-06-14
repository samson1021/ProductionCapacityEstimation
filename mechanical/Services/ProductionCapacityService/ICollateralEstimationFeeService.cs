using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using mechanical.Models.Dto.ProductionCapacityDto;

namespace mechanical.Services.ProductionCapacityService
{
    public interface ICollateralEstimationFeeService
    {
        Task<CollateralEstimationFeeDto> CreateCollateralEstimationFee(Guid userId, CollateralEstimationFeeDto dto);
        Task<CollateralEstimationFeeDto> EditCollateralEstimationFee(Guid userId, Guid id, CollateralEstimationFeeDto dto);
        Task<CollateralEstimationFeeDto> GetCollateralEstimationFee(Guid userId, Guid id);
        Task<IEnumerable<CollateralEstimationFeeDto>> GetAllCollateralEstimationFees(Guid caseId);
        Task<bool> DeleteCollateralEstimationFee(Guid id);
        Task<bool> ValidateFees(Guid caseId);
        Task<bool> CommitFees(Guid caseId);
        
        decimal CalculateTotalFee(decimal estimationFeePerUnit, int quantity);
    }
}
