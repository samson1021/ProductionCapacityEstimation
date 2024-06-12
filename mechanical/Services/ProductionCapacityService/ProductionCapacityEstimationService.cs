using AutoMapper;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using mechanical.Data;
using mechanical.Services.UploadFileService;
using mechanical.Models.Entities.ProductionCapacity;
using mechanical.Models.Dto.ProductionCapacityDto;
using mechanical.Models.Dto.UploadFileDto;

namespace mechanical.Services.ProductionCapacityService
{
    public class ProductionCapacityEstimationService : IProductionCapacityEstimationService
    {
        private readonly CbeContext _cbeContext;
        private readonly IMapper _mapper;
        private readonly IUploadFileService _uploadFileService;
        private readonly ILogger<ProductionCapacityEstimationService> _logger;

        public ProductionCapacityEstimationService(CbeContext context, IMapper mapper, IUploadFileService uploadFileService, ILogger<ProductionCapacityEstimationService> logger)
        {
            _cbeContext = context;
            _mapper = mapper;
            _uploadFileService = uploadFileService;
            _logger = logger;
        }

        public async Task<ProductionCapacityEstimationDto> CreateProductionCapacityEstimation(Guid userId, ProductionCapacityEstimationDto dto)
        {
            try
            {
                var entity = _mapper.Map<ProductionCapacityEstimation>(dto);
                entity.Id = Guid.NewGuid();
                entity.CreatedBy = userId;
                await _cbeContext.ProductionCapacityEstimations.AddAsync(entity);
                await _cbeContext.SaveChangesAsync();
                return _mapper.Map<ProductionCapacityEstimationDto>(entity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating production capacity estimation for user {UserId}", userId);
                throw;
            }
        }

        public async Task<ProductionCapacityEstimationDto> EditProductionCapacityEstimation(Guid userId, Guid id, ProductionCapacityEstimationDto dto)
        {
            try
            {
                var entity = await _cbeContext.ProductionCapacityEstimations.FindAsync(id);
                if (entity == null) return null;

                _mapper.Map(dto, entity);
                _cbeContext.ProductionCapacityEstimations.Update(entity);
                await _cbeContext.SaveChangesAsync();
                return _mapper.Map<ProductionCapacityEstimationDto>(entity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error editing production capacity estimation with ID {Id} for user {UserId}", id, userId);
                throw;
            }
        }

        public async Task<ProductionCapacityEstimationDto> GetProductionCapacityEstimation(Guid userId, Guid id)
        {
            try
            {
                var entity = await _cbeContext.ProductionCapacityEstimations.Include(e => e.SupportingEvidences)
                                                                         .Include(e => e.ProductionProcessFlowDiagrams)
                                                                         .FirstOrDefaultAsync(e => e.Id == id);
                return _mapper.Map<ProductionCapacityEstimationDto>(entity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching production capacity estimation with ID {Id} for user {UserId}", id, userId);
                throw;
            }
        }

        public async Task<IEnumerable<ProductionCapacityEstimationDto>> GetNewEstimations(Guid userId)
        {
            try
            {
                var entities = await _cbeContext.ProductionCapacityEstimations.Where(e => e.Status == "New").ToListAsync();
                return _mapper.Map<IEnumerable<ProductionCapacityEstimationDto>>(entities);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching new production capacity estimations for user {UserId}", userId);
                throw;
            }
        }

        public async Task<IEnumerable<ProductionCapacityEstimationDto>> GetRejectedEstimations(Guid userId)
        {
            try
            {
                var entities = await _cbeContext.ProductionCapacityEstimations.Where(e => e.Status == "Rejected").ToListAsync();
                return _mapper.Map<IEnumerable<ProductionCapacityEstimationDto>>(entities);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching rejected production capacity estimations for user {UserId}", userId);
                throw;
            }
        }

        public async Task<IEnumerable<ProductionCapacityEstimationDto>> GetTerminatedEstimations(Guid userId)
        {
            try
            {
                var entities = await _cbeContext.ProductionCapacityEstimations.Where(e => e.Status == "Terminated").ToListAsync();
                return _mapper.Map<IEnumerable<ProductionCapacityEstimationDto>>(entities);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching terminated production capacity estimations for user {UserId}", userId);
                throw;
            }
        }

        public async Task<IEnumerable<ProductionCapacityEstimationDto>> GetPendingEstimations(Guid userId)
        {
            try
            {
                var entities = await _cbeContext.ProductionCapacityEstimations.Where(e => e.Status == "Pending").ToListAsync();
                return _mapper.Map<IEnumerable<ProductionCapacityEstimationDto>>(entities);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching pending production capacity estimations for user {UserId}", userId);
                throw;
            }
        }

        public async Task SendForApproval(string selectedEstimationIds, string CenterId)
        {
            try
            {
                var ids = selectedEstimationIds.Split(',').Select(id => Guid.Parse(id)).ToList();
                var estimations = await _cbeContext.ProductionCapacityEstimations.Where(e => ids.Contains(e.Id)).ToListAsync();
                foreach (var estimation in estimations)
                {
                    estimation.Status = "PendingApproval";
                }
                await _cbeContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending production capacity estimations for approval");
                throw;
            }
        }

        public async Task RejectEstimation(Guid id, string rejectionReason)
        {
            try
            {
                var estimation = await _cbeContext.ProductionCapacityEstimations.FindAsync(id);
                if (estimation != null)
                {
                    estimation.Status = "Rejected";
                    estimation.RejectionReason = rejectionReason;
                    _cbeContext.ProductionCapacityEstimations.Update(estimation);
                    await _cbeContext.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error rejecting production capacity estimation with ID {Id}", id);
                throw;
            }
        }

        public async Task<ProductionCapacityScheduleDto> CreateSchedule(Guid userId, ProductionCapacityScheduleDto scheduleDto)
        {
            try
            {
                var schedule = _mapper.Map<ProductionCapacitySchedule>(scheduleDto);
                schedule.Id = Guid.NewGuid();
                await _cbeContext.ProductionCapacitySchedules.AddAsync(schedule);
                await _cbeContext.SaveChangesAsync();
                return _mapper.Map<ProductionCapacityScheduleDto>(schedule);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating schedule for production capacity estimation");
                throw;
            }
        }

          

        public async Task<int> GetDashboardEstimationCount(Guid userId)
        {
            try
            {
                return await _cbeContext.ProductionCapacityEstimations.CountAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching dashboard estimation count for user {UserId}", userId);
                throw;
            }
        }

        public async Task<int> GetMyDashboardEstimationCount(Guid userId)
        {
            try
            {
                return await _cbeContext.ProductionCapacityEstimations.CountAsync(e => e.CreatedBy == userId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching my dashboard estimation count for user {UserId}", userId);
                throw;
            }
        }

        public async Task<bool> DeleteSupportingEvidence(Guid Id)
        {
            try
            {
                var evidence = await _cbeContext.UploadFiles.FindAsync(Id);
                if (evidence == null) return false;
                _cbeContext.UploadFiles.Remove(evidence);
                await _cbeContext.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting supporting evidence with ID {Id}", Id);
                throw;
            }
        }

        public async Task<bool> DeleteProcessFlowDiagram(Guid Id)
        {
            try
            {
                var diagram = await _cbeContext.UploadFiles.FindAsync(Id);
                if (diagram == null) return false;
                _cbeContext.UploadFiles.Remove(diagram);
                await _cbeContext.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting process flow diagram with ID {Id}", Id);
                throw;
            }
        }

        public async Task<bool> UploadSupportingEvidence(Guid userId, IFormFile file, Guid caseId)
        {
            try
            {
                if (file == null || caseId == Guid.Empty)
                {
                    return false;
                }

                var createFileDto = new CreateFileDto
                {
                    File = file,
                    Catagory = "Supporting Evidence",
                    CaseId = caseId
                };

                var result = await _uploadFileService.CreateUploadFile(userId, createFileDto);
                return result != Guid.Empty;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading supporting evidence for case ID {CaseId}", caseId);
                throw;
            }
        }

        public async Task<bool> UploadProcessFlowDiagram(Guid userId, IFormFile file, Guid caseId)
        {
            try
            {
                if (file == null || caseId == Guid.Empty)
                {
                    return false;
                }

                var createFileDto = new CreateFileDto
                {
                    File = file,
                    Catagory = "Process Flow Diagram",
                    CaseId = caseId
                };

                var result = await _uploadFileService.CreateUploadFile(userId, createFileDto);
                return result != Guid.Empty;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading process flow diagram for case ID {CaseId}", caseId);
                throw;
            }
        }

        public async Task<IEnumerable<ProductionCapacityEstimationDto>> GetAllProductionCapacityEstimations()
        {
            try
            {
                var entities = await _cbeContext.ProductionCapacityEstimations
                    .Include(p => p.ShiftHours)
                    .Include(p => p.SupportingEvidences)
                    .Include(p => p.ProductionProcessFlowDiagrams)
                    .ToListAsync();

                return _mapper.Map<IEnumerable<ProductionCapacityEstimationDto>>(entities);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching all production capacity estimations");
                throw;
            }
        }

        public async Task<bool> DeleteProductionCapacityEstimation(Guid userId, Guid id)
        {
            try
            {
                var entity = await _cbeContext.ProductionCapacityEstimations.FindAsync(id);
                if (entity == null)
                {
                    return false;
                }

                _cbeContext.ProductionCapacityEstimations.Remove(entity);
                await _cbeContext.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting production capacity estimation with ID {Id}", id);
                throw;
            }
        }
    }
}
