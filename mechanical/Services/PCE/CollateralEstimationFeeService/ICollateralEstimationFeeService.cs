using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using mechanical.Models.PCE.Dto.CollateralEstimationFeeDto;

namespace mechanical.Services.PCE.CollateralEstimationFeeService
{
    public interface ICollateralEstimationFeeService
    {
        Task<CollateralEstimationFeeDto> CreateCollateralEstimationFee(Guid userId, CollateralEstimationFeeDto dto);
        Task<CollateralEstimationFeeDto> EditCollateralEstimationFee(Guid userId, Guid id, CollateralEstimationFeeDto dto);
        Task<CollateralEstimationFeeDto> GetCollateralEstimationFee(Guid userId, Guid id);
        Task<IEnumerable<CollateralEstimationFeeDto>> GetAllCollateralEstimationFees(Guid userId, Guid caseId);
        Task<bool> DeleteCollateralEstimationFee(Guid userId, Guid id);
        Task<bool> ValidateFees(Guid userId, Guid caseId);
        Task<bool> CommitFees(Guid userId, Guid caseId);
        
        decimal CalculateTotalFee(decimal estimationFeePerUnit, int quantity);
    }
}
