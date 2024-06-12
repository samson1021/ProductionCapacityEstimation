using AutoMapper;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging; // Add this using directive
using mechanical.Data;
// using mechanical.Services.UploadFileService;
using mechanical.Models.Entities.ProductionCapacity;
using mechanical.Models.Dto.ProductionCapacityDto;
using mechanical.Models.Enum.CollateralAndProductionCapacityEstimationEnums;

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

        public async Task<CollateralEstimationFeeDto> CreateCollateralEstimationFee(Guid userId, CollateralEstimationFeeDto dto)
        {
            try
            {
                var entity = _mapper.Map<CollateralEstimationFee>(dto);
                entity.Id = Guid.NewGuid();
                entity.CreatedBy = userId;
                entity.CreatedAt = DateTime.Now;
                entity.TotalFee = CalculateTotalFee(entity.EstimationFeePerUnit, entity.Quantity);

                await _cbeContext.CollateralEstimationFees.AddAsync(entity);
                await _cbeContext.SaveChangesAsync();
                return _mapper.Map<CollateralEstimationFeeDto>(entity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating collateral estimation fee");
                throw;
            }
        }

        public async Task<CollateralEstimationFeeDto> EditCollateralEstimationFee(Guid userId, Guid id, CollateralEstimationFeeDto dto)
        {
            try
            {
                var entity = await _cbeContext.CollateralEstimationFees.FindAsync(id);
                if (entity == null)
                {
                    _logger.LogWarning($"Collateral estimation fee with ID {id} not found");
                    return null;
                }

                _mapper.Map(dto, entity);
                entity.TotalFee = CalculateTotalFee(entity.EstimationFeePerUnit, entity.Quantity);
                _cbeContext.CollateralEstimationFees.Update(entity);
                await _cbeContext.SaveChangesAsync();
                return _mapper.Map<CollateralEstimationFeeDto>(entity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error editing collateral estimation fee with ID {id}");
                throw;
            }
        }

        public async Task<CollateralEstimationFeeDto> GetCollateralEstimationFee(Guid userId, Guid id)
        {
            try
            {
                var entity = await _cbeContext.CollateralEstimationFees.FindAsync(id);
                return _mapper.Map<CollateralEstimationFeeDto>(entity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error fetching collateral estimation fee with ID {id}");
                throw;
            }
        }

        public async Task<IEnumerable<CollateralEstimationFeeDto>> GetAllCollateralEstimationFees(Guid caseId)
        {
            try
            {
                var entities = await _cbeContext.CollateralEstimationFees.Where(f => f.CaseId == caseId).ToListAsync();
                return _mapper.Map<IEnumerable<CollateralEstimationFeeDto>>(entities);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error fetching all collateral estimation fees for case ID {caseId}");
                throw;
            }
        }

        public async Task<bool> DeleteCollateralEstimationFee(Guid id)
        {
            try
            {
                var entity = await _cbeContext.CollateralEstimationFees.FindAsync(id);
                if (entity == null)
                {
                    _logger.LogWarning($"Collateral estimation fee with ID {id} not found");
                    return false;
                }

                _cbeContext.CollateralEstimationFees.Remove(entity);
                await _cbeContext.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting collateral estimation fee with ID {id}");
                throw;
            }
        }

        public async Task<bool> ValidateFees(Guid caseId)
        {
            
            try
            {
                // var result = await _temenosClient.ValidateEstimationFee(caseId);
                // return result.IsSuccess;

                var fees = await _cbeContext.CollateralEstimationFees
                                            .Where(f => f.CaseId == caseId && f.Status == FeeStatus.Pending)
                                            .ToListAsync();

                if (!fees.Any())
                {
                    _logger.LogWarning($"No pending fees found for case ID {caseId}");
                    return false;
                }

                foreach (var fee in fees)
                {
                    if (!ValidateFee(fee))
                    {
                        _logger.LogWarning($"Validation failed for fee ID {fee.Id}");
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
                _logger.LogError(ex, $"Error validating fees for case ID {caseId}");
                throw;
            }
        }

        private bool ValidateFee(CollateralEstimationFee fee)
        {
            if (fee.Quantity <= 0 || fee.EstimationFeePerUnit <= 0)
            {
                _logger.LogWarning($"Invalid fee data for fee ID {fee.Id}");
                return false;
            }

            decimal expectedTotalFee = CalculateTotalFee(fee.EstimationFeePerUnit, fee.Quantity);
            return fee.TotalFee == expectedTotalFee;
        }

        public async Task<bool> CommitFees(Guid caseId)
        {
            
            try
            {
                // var result = await _temenosClient.CommitEstimationFee(caseId);
                // return result.IsSuccess;
                
                var fees = await _cbeContext.CollateralEstimationFees
                                            .Where(f => f.CaseId == caseId && f.Status == FeeStatus.Validated)
                                            .ToListAsync();

                if (!fees.Any())
                {
                    _logger.LogWarning($"No validated fees found for case ID {caseId}");
                    return false;
                }

                foreach (var fee in fees)
                {
                    bool success = await DeductFeeFromCoreBankingSystem(fee);

                    if (!success)
                    {
                        _logger.LogWarning($"Failed to deduct fee from core banking system for fee ID {fee.Id}");
                        return false;
                    }

                    fee.Status = FeeStatus.Committed;
                    _cbeContext.Update(fee);
                }

                // _cbeContext.CollateralEstimationFees.UpdateRange(fees);
                await _cbeContext.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error committing fees for case ID {caseId}");
                throw;
            }
        }

        private async Task<bool> DeductFeeFromCoreBankingSystem(CollateralEstimationFee fee)
        {
            try
            {
                // External API call to the core banking system
                _logger.LogInformation($"Deducting fee from core banking system for fee ID {fee.Id}");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deducting fee from core banking system for fee ID {fee.Id}");
                return false;
            }
        }

        public decimal CalculateTotalFee(decimal estimationFeePerUnit, int quantity)
        {
            return estimationFeePerUnit * quantity;
        }
    }
}
