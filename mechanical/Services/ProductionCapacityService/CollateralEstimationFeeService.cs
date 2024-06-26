using AutoMapper;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using mechanical.Data;
using mechanical.Models.Dto.ProductionCapacityDto;
using mechanical.Models.Entities.ProductionCapacity;
using mechanical.Models.Enum.CollateralAndProductionCapacityEstimationEnums.Collateral;

namespace mechanical.Services.ProductionCapacityService
{
    public class CollateralEstimationFeeService : ICollateralEstimationFeeService
    {
        private readonly CbeContext _cbeContext;
        private readonly IMapper _mapper;
        private readonly ILogger<CollateralEstimationFeeService> _logger;

        public CollateralEstimationFeeService(CbeContext context, IMapper mapper, ILogger<CollateralEstimationFeeService> logger)
        {
            _cbeContext = context;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<CollateralEstimationFeeDto> CreateCollateralEstimationFee(Guid UserId, CollateralEstimationFeeDto dto)
        {
            try
            {
                var entity = _mapper.Map<CollateralEstimationFee>(dto);
                entity.Id = Guid.NewGuid();
                entity.CreatedBy = UserId;
                entity.CreatedAt = DateTime.Now;
                entity.TotalFee = CalculateTotalFee(entity.EstimationFeePerUnit, entity.Quantity);
                entity.Status = FeeStatus.New;

                ////// 
                entity.PCECaseId = Guid.Parse("E1BBBE4A-F804-439A-A8E6-539232CCC6F0");
                entity.RejectionReason = null;
                /////////

                await _cbeContext.CollateralEstimationFees.AddAsync(entity);
                await _cbeContext.SaveChangesAsync();
                return _mapper.Map<CollateralEstimationFeeDto>(entity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating collateral estimation fee");
                throw new ApplicationException("An error occurred while creating the collateral estimation fee.");
            }
        }

        public async Task<CollateralEstimationFeeDto> EditCollateralEstimationFee(Guid UserId, Guid id, CollateralEstimationFeeDto dto)
        {
            try
            {
                var entity = await _cbeContext.CollateralEstimationFees.FindAsync(id);
                if (entity == null)
                {
                    _logger.LogWarning("Collateral estimation fee with id {Id} not found", id);
                    throw new KeyNotFoundException("Collateral estimation fee not found");
                }

                _mapper.Map(dto, entity);
                entity.TotalFee = CalculateTotalFee(entity.EstimationFeePerUnit, entity.Quantity);
                _cbeContext.CollateralEstimationFees.Update(entity);
                await _cbeContext.SaveChangesAsync();
                return _mapper.Map<CollateralEstimationFeeDto>(entity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error editing collateral estimation fee");
                throw new ApplicationException("An error occurred while editing the collateral estimation fee.");
            }
        }

        public async Task<CollateralEstimationFeeDto> GetCollateralEstimationFee(Guid UserId, Guid id)
        {
            try
            {
                var entity = await _cbeContext.CollateralEstimationFees.FindAsync(id);
                if (entity == null)
                {
                    _logger.LogWarning("Collateral estimation fee with id {Id} not found", id);
                    throw new KeyNotFoundException("Collateral estimation fee not found");
                }
                return _mapper.Map<CollateralEstimationFeeDto>(entity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching collateral estimation fee");
                throw new ApplicationException("An error occurred while fetching the collateral estimation fee.");
            }
        }

        public async Task<IEnumerable<CollateralEstimationFeeDto>> GetAllCollateralEstimationFees(Guid UserId, Guid PCECaseId)
        {
            try
            {
                var entities = await _cbeContext.CollateralEstimationFees.Where(f => f.PCECaseId == PCECaseId).ToListAsync();
                return _mapper.Map<IEnumerable<CollateralEstimationFeeDto>>(entities);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching all collateral estimation fees");
                throw new ApplicationException("An error occurred while fetching all collateral estimation fees.");
            }
        }

        public async Task<bool> DeleteCollateralEstimationFee(Guid UserId, Guid id)
        {
            try
            {
                var entity = await _cbeContext.CollateralEstimationFees.FindAsync(id);
                if (entity == null)
                {
                    _logger.LogWarning("Collateral estimation fee with id {Id} not found", id);
                    return false;
                }

                _cbeContext.CollateralEstimationFees.Remove(entity);
                await _cbeContext.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting collateral estimation fee");
                throw new ApplicationException("An error occurred while deleting the collateral estimation fee.");
            }
        }

        public async Task<bool> ValidateFees(Guid UserId, Guid PCECaseId)
        {
            try
            {
                var fees = await _cbeContext.CollateralEstimationFees
                    .Where(f => f.PCECaseId == PCECaseId && f.Status == FeeStatus.Pending)
                    .ToListAsync();

                if (!fees.Any())
                {
                    _logger.LogWarning("No pending fees found for case id {PCECaseId}", PCECaseId);
                    return false;
                }

                foreach (var fee in fees)
                {
                    if (!ValidateFee(fee))
                    {
                        _logger.LogWarning("Fee validation failed for fee id {Id}", fee.Id);
                        return false;
                    }

                    fee.Status = FeeStatus.Validated;
                    _cbeContext.Update(fee);
                }

                await _cbeContext.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validating fees");
                throw new ApplicationException("An error occurred while validating the fees.");
            }
        }

        private bool ValidateFee(CollateralEstimationFee fee)
        {
            if (fee.Quantity <= 0 || fee.EstimationFeePerUnit <= 0)
            {
                return false;
            }
            decimal expectedTotalFee = CalculateTotalFee(fee.EstimationFeePerUnit, fee.Quantity);
            return fee.TotalFee == expectedTotalFee;
        }

        public async Task<bool> CommitFees(Guid UserId, Guid PCECaseId)
        {
            try
            {
                var fees = await _cbeContext.CollateralEstimationFees
                    .Where(f => f.PCECaseId == PCECaseId && f.Status == FeeStatus.Validated)
                    .ToListAsync();

                if (!fees.Any())
                {
                    _logger.LogWarning("No validated fees found for case id {PCECaseId}", PCECaseId);
                    return false;
                }

                foreach (var fee in fees)
                {
                    bool success = await DeductFeeFromCoreBankingSystem(fee);

                    if (!success)
                    {
                        _logger.LogWarning("Fee deduction failed for fee id {Id}", fee.Id);
                        return false;
                    }

                    fee.Status = FeeStatus.Committed;
                    _cbeContext.Update(fee);
                }

                await _cbeContext.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error committing fees");
                throw new ApplicationException("An error occurred while committing the fees.");
            }
        }

        private async Task<bool> DeductFeeFromCoreBankingSystem(CollateralEstimationFee fee)
        {
            // External API call to core banking system
            return true; 
        }

        public decimal CalculateTotalFee(decimal estimationFeePerUnit, int quantity)
        {
            return estimationFeePerUnit * quantity;
        }
    }
}
