using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;

using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

using mechanical.Data;
using mechanical.Services.ProductionCapacityService;
using mechanical.Models.Dto.ProductionCapacityDto;
using mechanical.Models.Dto.ProductionCapacityDto.FileUploadDto;
using mechanical.Models.Entities.ProductionCapacity;
using mechanical.Models.Enum.CollateralAndProductionCapacityEstimationEnums.File;
using mechanical.Models.Enum.CollateralAndProductionCapacityEstimationEnums.ProductionCapacityEstimation;

namespace mechanical.Services.ProductionCapacityService
{
    public class ProductionCapacityEstimationService : IProductionCapacityEstimationService
    {
        private readonly CbeContext _cbeContext;
        private readonly IMapper _mapper;
        private readonly ILogger<ProductionCapacityEstimationService> _logger;
        private readonly IFileUploadService _fileUploadService;

        public ProductionCapacityEstimationService(IWebHostEnvironment webHostEnvironment, CbeContext context, IMapper mapper, ILogger<ProductionCapacityEstimationService> logger, IFileUploadService fileUploadService)
        {
            _cbeContext = context;
            _mapper = mapper;
            _logger = logger;
            _fileUploadService = fileUploadService;
        }
        
        public async Task<ProductionCapacityEstimationDto> CreateProductionCapacityEstimation(Guid UserId, Guid PCECaseId, ProductionCapacityEstimationDto dto)
        {
            try
            {
                Console.WriteLine("PCE service create file...............................................................");
                var entity = _mapper.Map<ProductionCapacityEstimation>(dto);
                entity.Id = Guid.NewGuid();
                entity.CreatedBy = UserId;
                PCECaseId = Guid.Parse("E1BBBE4A-F804-439A-A8E6-539232CCC6F0");
                entity.PCECaseId = PCECaseId;
                entity.CreatedAt = DateTime.Now;
                entity.Status = Status.New;
                entity.RejectionReason = null;
        
                await _cbeContext.ProductionCapacityEstimations.AddAsync(entity);
                await _cbeContext.SaveChangesAsync();

                var supportingEvidences = await _fileUploadService.CreateFiles(UserId, entity.Id, dto.SupportingEvidences, DocumentType.SupportingEvidence, "supportingEvidences");
                var productionDiagrams = await _fileUploadService.CreateFiles(UserId, entity.Id, dto.ProductionProcessFlowDiagrams, DocumentType.ProductionProcessFlowDiagram, "productionDiagrams");
                // _cbeContext.FileUploads.AddRange(supportingEvidences);
                // _cbeContext.FileUploads.AddRange(productionDiagrams);
                
                await _cbeContext.SaveChangesAsync();
                Console.WriteLine("after create file...............................................................");
                ////
                var resultDto = _mapper.Map<ProductionCapacityEstimationDto>(entity);
                // resultDto.PerShiftProduction = ProductionCapacityCalculationUtility.CalculatePerShiftProduction(entity.EffectiveProductionHourPerShift, entity.ProductionPerHour);
                // resultDto.PerDayProduction = ProductionCapacityCalculationUtility.CalculatePerDayProduction(entity.ShiftsPerDay, resultDto.PerShiftProduction);
                // resultDto.PerMonthProduction = ProductionCapacityCalculationUtility.CalculatePerMonthProduction(entity.WorkingDaysPerMonth, resultDto.PerDayProduction);
                // resultDto.PerYearProduction = ProductionCapacityCalculationUtility.CalculatePerYearProduction(resultDto.PerMonthProduction);
                return resultDto;
                ////
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating production capacity estimation");
                throw new ApplicationException("An error occurred while creating the production capacity estimation.");
            }
        }
        
        public async Task<ProductionCapacityEstimationDto> EditProductionCapacityEstimation(Guid UserId, Guid id, ProductionCapacityEstimationDto dto)
        {
            try
            {
                var entity = await _cbeContext.ProductionCapacityEstimations.FindAsync(id);
                if (entity == null)
                {
                    _logger.LogWarning("Production capacity estimation with id {Id} not found", id);
                    throw new KeyNotFoundException("Production capacity estimation not found");
                }

                _mapper.Map(dto, entity);
                _cbeContext.ProductionCapacityEstimations.Update(entity);
                await _cbeContext.SaveChangesAsync();
                /////
                var resultDto = _mapper.Map<ProductionCapacityEstimationDto>(entity);
                // resultDto.PerShiftProduction = ProductionCapacityCalculationUtility.CalculatePerShiftProduction(entity.EffectiveProductionHourPerShift, entity.ProductionPerHour);
                // resultDto.PerDayProduction = ProductionCapacityCalculationUtility.CalculatePerDayProduction(entity.ShiftsPerDay, resultDto.PerShiftProduction);
                // resultDto.PerMonthProduction = ProductionCapacityCalculationUtility.CalculatePerMonthProduction(entity.WorkingDaysPerMonth, resultDto.PerDayProduction);
                // resultDto.PerYearProduction = ProductionCapacityCalculationUtility.CalculatePerYearProduction(resultDto.PerMonthProduction);
                return resultDto;
                /////
                return _mapper.Map<ProductionCapacityEstimationDto>(entity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error editing production capacity estimation");
                throw new ApplicationException("An error occurred while editing the production capacity estimation.");
            }
        }

        public async Task<ProductionCapacityEstimationDto> GetProductionCapacityEstimation(Guid UserId, Guid id)
        {
            try
            {
                var entity = await _cbeContext.ProductionCapacityEstimations
                    .Include(e => e.TimeConsumedToCheck)
                    .Include(e => e.SupportingDocuments)
                    // .Include(e => e.SupportingEvidences)
                    // .Include(e => e.ProductionProcessFlowDiagrams)
                    .FirstOrDefaultAsync(e => e.Id == id);
                if (entity == null)
                {
                    _logger.LogWarning("Production capacity estimation with id {Id} not found", id);
                    throw new KeyNotFoundException("Production capacity estimation not found");
                }
                return _mapper.Map<ProductionCapacityEstimationDto>(entity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching production capacity estimation");
                throw new ApplicationException("An error occurred while fetching the production capacity estimation.");
            }
        }

        public async Task<IEnumerable<ProductionCapacityEstimationDto>> GetNewEstimations(Guid UserId)
        {
            try
            {
                var entities = await _cbeContext.ProductionCapacityEstimations.Where(e => e.Status == Status.New).ToListAsync();
                return _mapper.Map<IEnumerable<ProductionCapacityEstimationDto>>(entities);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching new estimations");
                throw new ApplicationException("An error occurred while fetching new estimations.");
            }
        }

        public async Task<IEnumerable<ProductionCapacityEstimationDto>> GetRejectedEstimations(Guid UserId)
        {
            try
            {
                var entities = await _cbeContext.ProductionCapacityEstimations.Where(e => e.Status == Status.Rejected).ToListAsync();
                return _mapper.Map<IEnumerable<ProductionCapacityEstimationDto>>(entities);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching rejected estimations");
                throw new ApplicationException("An error occurred while fetching rejected estimations.");
            }
        }

        public async Task<IEnumerable<ProductionCapacityEstimationDto>> GetTerminatedEstimations(Guid UserId)
        {
            try
            {
                var entities = await _cbeContext.ProductionCapacityEstimations.Where(e => e.Status == Status.Terminated).ToListAsync();
                return _mapper.Map<IEnumerable<ProductionCapacityEstimationDto>>(entities);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching terminated estimations");
                throw new ApplicationException("An error occurred while fetching terminated estimations.");
            }
        }

        public async Task<IEnumerable<ProductionCapacityEstimationDto>> GetPendingEstimations(Guid UserId)
        {
            try
            {
                var entities = await _cbeContext.ProductionCapacityEstimations.Where(e => e.Status == Status.Pending).ToListAsync();
                return _mapper.Map<IEnumerable<ProductionCapacityEstimationDto>>(entities);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching pending estimations");
                throw new ApplicationException("An error occurred while fetching pending estimations.");
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
                    estimation.Status = Status.Approved;
                }
                await _cbeContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending estimations for approval");
                throw new ApplicationException("An error occurred while sending estimations for approval.");
            }
        }

        public async Task RejectEstimation(Guid id, string rejectionReason)
        {
            try
            {
                var estimation = await _cbeContext.ProductionCapacityEstimations.FindAsync(id);
                if (estimation == null)
                {
                    _logger.LogWarning("Production capacity estimation with id {Id} not found", id);
                    throw new KeyNotFoundException("Production capacity estimation not found");
                }

                estimation.Status = Status.Rejected;
                estimation.RejectionReason = rejectionReason;
                _cbeContext.ProductionCapacityEstimations.Update(estimation);
                await _cbeContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error rejecting estimation");
                throw new ApplicationException("An error occurred while rejecting the estimation.");
            }
        }

        public async Task<ProductionCapacityScheduleDto> CreateSchedule(Guid UserId, ProductionCapacityScheduleDto scheduleDto)
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
                _logger.LogError(ex, "Error creating schedule");
                throw new ApplicationException("An error occurred while creating the schedule.");
            }
        }

        public async Task<int> GetDashboardEstimationCount(Guid UserId)
        {
            try
            {
                return await _cbeContext.ProductionCapacityEstimations.CountAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching dashboard estimation count");
                throw new ApplicationException("An error occurred while fetching the dashboard estimation count.");
            }
        }

        public async Task<int> GetMyDashboardEstimationCount(Guid UserId)
        {
            try
            {
                return await _cbeContext.ProductionCapacityEstimations.CountAsync(e => e.CreatedBy == UserId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching my dashboard estimation count");
                throw new ApplicationException("An error occurred while fetching your dashboard estimation count.");
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
                _logger.LogError(ex, "Error deleting supporting evidence");
                throw new ApplicationException("An error occurred while deleting the supporting evidence.");
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
                _logger.LogError(ex, "Error deleting process flow diagram");
                throw new ApplicationException("An error occurred while deleting the process flow diagram.");
            }
        }

        public async Task<IEnumerable<ProductionCapacityEstimationDto>> GetAllProductionCapacityEstimations(Guid UserId)
        {
            try
            {
                var entities = await _cbeContext.ProductionCapacityEstimations
                    .Include(p => p.ShiftHours)
                    .Include(p => p.TimeConsumedToCheck)
                    .Include(p => p.SupportingDocuments)
                    // .Include(e => e.SupportingEvidences)
                    // .Include(e => e.ProductionProcessFlowDiagrams)
                    .ToListAsync();
                return _mapper.Map<IEnumerable<ProductionCapacityEstimationDto>>(entities);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching all production capacity estimations");
                throw new ApplicationException("An error occurred while fetching all production capacity estimations.");
            }
        }

        public async Task<bool> DeleteProductionCapacityEstimation(Guid UserId, Guid id)
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
                _logger.LogError(ex, "Error deleting production capacity estimation");
                throw new ApplicationException("An error occurred while deleting the production capacity estimation.");
            }
        }

        // private async Task UploadFile(Guid UserId, ProductionCapacityEstimation entity, IFormFile? file, String Type)
        // {
        //     if (file != null)
        //     {
        //         await _fileUploadService.CreateFile(UserId, new FileCreateDto()
        //         {
        //             File = file,
        //             Id = Guid.NewGuid(),
        //             PCEId = entity.Id
        //             Type = Type
        //         });
        //     }
        // }

        
        // public async Task<bool> UploadSupportingEvidence(Guid UserId, IFormFile file, Guid PCEId)
        // {
        //     try
        //     {
        //         if (file == null || PCEId == Guid.Empty)
        //         {
        //             return false;
        //         }

        //         var createFileDto = new CreateFileDto
        //         {
        //             File = file,
        //             Catagory = "Supporting Evidence",
        //             PCEId = PCEId
        //         };

        //         var result = await _fileUploadService.CreateUploadFile(UserId, createFileDto);
        //         return result != Guid.Empty;
        //     }
        //     catch (Exception ex)
        //     {
        //         _logger.LogError(ex, "Error uploading supporting evidence");
        //         throw new ApplicationException("An error occurred while uploading the supporting evidence.");
        //     }
        // }

        // public async Task<bool> UploadProcessFlowDiagram(Guid UserId, IFormFile file, Guid PCEId)
        // {
        //     try
        //     {
        //         if (file == null || PCEId == Guid.Empty)
        //         {
        //             return false;
        //         }

        //         var createFileDto = new CreateFileDto
        //         {
        //             File = file,
        //             Catagory = "Process Flow Diagram",
        //             PCEId = PCEId
        //         };

        //         var result = await _fileUploadService.CreateUploadFile(UserId, createFileDto);
        //         return result != Guid.Empty;
        //     }
        //     catch (Exception ex)
        //     {
        //         _logger.LogError(ex, "Error uploading process flow diagram");
        //         throw new ApplicationException("An error occurred while uploading the process flow diagram.");
        //     }
        // }
    }
}
