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
using mechanical.Services.PCE.PCEService;
using mechanical.Models.PCE.Entities;
using mechanical.Models.PCE.Dto.PCEDto;
using mechanical.Models.PCE.Dto.FileUploadDto;
using mechanical.Models.PCE.Enum.PCEEnums.PCE;
using mechanical.Models.PCE.Enum.PCEEnums.File;

namespace mechanical.Services.PCE.PCEService
{
    public class PCEService : IPCEService
    {
        private readonly CbeContext _cbeContext;
        private readonly IMapper _mapper;
        private readonly ILogger<PCEService> _logger;
        private readonly IFileUploadService _fileUploadService;

        public PCEService(IWebHostEnvironment webHostEnvironment, CbeContext context, IMapper mapper, ILogger<PCEService> logger, IFileUploadService fileUploadService)
        {
            _cbeContext = context;
            _mapper = mapper;
            _logger = logger;
            _fileUploadService = fileUploadService;
        }
        
        public async Task<PCEPostDto> CreatePCE(Guid UserId, Guid PCECaseId, PCEPostDto dto)
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
        

                if (dto.SupportingEvidences != null)
                {
                    foreach (var supportingEvidenceDto in dto.SupportingEvidences)
                    {
                        var supportingEvidenceEntity = new FileUpload
                        {
                            Name = supportingEvidenceDto.Name,
                            Type = supportingEvidenceDto.Type,
                            PCEId = entity.Id,
                            // File = supportingEvidenceDto.File,
                        };
                        entity.SupportingDocuments.Add(supportingEvidenceEntity);
                    }
                }

                if (dto.ProductionProcessFlowDiagrams != null)
                {
                    foreach (var flowDiagramDto in dto.ProductionProcessFlowDiagrams)
                    {
                        var flowDiagramEntity = new FileUpload
                        {
                            Name = flowDiagramDto.Name,
                            Type = flowDiagramDto.Type,
                            PCEId = entity.Id, 
                            // File = flowDiagramDto.File, 
                        };
                        entity.SupportingDocuments.Add(flowDiagramEntity);
                    }
                }


                await _cbeContext.ProductionCapacityEstimations.AddAsync(entity);
                await _cbeContext.SaveChangesAsync();

                var supportingEvidences = await _fileUploadService.CreateFiles(UserId, entity.Id, dto.SupportingEvidences, DocumentType.SupportingEvidence, "SupportingEvidences");
                var productionDiagrams = await _fileUploadService.CreateFiles(UserId, entity.Id, dto.ProductionProcessFlowDiagrams, DocumentType.ProductionProcessFlowDiagram, "ProductionDiagrams");
                // _cbeContext.FileUploads.AddRange(supportingEvidences);
                // _cbeContext.FileUploads.AddRange(productionDiagrams);
                
                await _cbeContext.SaveChangesAsync();
                Console.WriteLine("after create file...............................................................");
                ////
                var resultDto = _mapper.Map<PCEPostDto>(entity);
                // resultDto.PerShiftProduction = PCECalculationUtility.CalculatePerShiftProduction(entity.EffectiveProductionHourPerShift, entity.ProductionPerHour);
                // resultDto.PerDayProduction = PCECalculationUtility.CalculatePerDayProduction(entity.ShiftsPerDay, resultDto.PerShiftProduction);
                // resultDto.PerMonthProduction = PCECalculationUtility.CalculatePerMonthProduction(entity.WorkingDaysPerMonth, resultDto.PerDayProduction);
                // resultDto.PerYearProduction = PCECalculationUtility.CalculatePerYearProduction(resultDto.PerMonthProduction);
                return resultDto;
                ////
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating PCE");
                throw new ApplicationException("An error occurred while creating the PCE.");
            }
        }
        
        public async Task<PCEPostDto> EditPCE(Guid UserId, Guid id, PCEPostDto dto)
        {
            try
            {
                var entity = await _cbeContext.ProductionCapacityEstimations.FindAsync(id);
                if (entity == null)
                {
                    _logger.LogWarning("PCE with id {Id} not found", id);
                    throw new KeyNotFoundException("PCE not found");
                }

                _mapper.Map(dto, entity);
                _cbeContext.ProductionCapacityEstimations.Update(entity);
                await _cbeContext.SaveChangesAsync();
                /////
                var resultDto = _mapper.Map<PCEPostDto>(entity);
                // resultDto.PerShiftProduction = PCECalculationUtility.CalculatePerShiftProduction(entity.EffectiveProductionHourPerShift, entity.ProductionPerHour);
                // resultDto.PerDayProduction = PCECalculationUtility.CalculatePerDayProduction(entity.ShiftsPerDay, resultDto.PerShiftProduction);
                // resultDto.PerMonthProduction = PCECalculationUtility.CalculatePerMonthProduction(entity.WorkingDaysPerMonth, resultDto.PerDayProduction);
                // resultDto.PerYearProduction = PCECalculationUtility.CalculatePerYearProduction(resultDto.PerMonthProduction);
                return resultDto;
                /////
                return _mapper.Map<PCEPostDto>(entity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error editing PCE");
                throw new ApplicationException("An error occurred while editing the PCE.");
            }
        }

        public async Task<PCEReturnDto> GetPCE(Guid UserId, Guid id)
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
                    _logger.LogWarning("PCE with id {Id} not found", id);
                    throw new KeyNotFoundException("PCE not found");
                }
                return _mapper.Map<PCEReturnDto>(entity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching PCE");
                throw new ApplicationException("An error occurred while fetching the PCE.");
            }
        }

        public async Task<IEnumerable<PCEReturnDto>> GetNewPCEs(Guid UserId)
        {
            try
            {
                var entities = await _cbeContext.ProductionCapacityEstimations.Where(e => e.Status == Status.New).ToListAsync();
                return _mapper.Map<IEnumerable<PCEReturnDto>>(entities);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching new PCEs");
                throw new ApplicationException("An error occurred while fetching new PCEs.");
            }
        }

        public async Task<IEnumerable<PCEReturnDto>> GetRejectedPCEs(Guid UserId)
        {
            try
            {
                var entities = await _cbeContext.ProductionCapacityEstimations.Where(e => e.Status == Status.Rejected).ToListAsync();
                return _mapper.Map<IEnumerable<PCEReturnDto>>(entities);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching rejected PCEs");
                throw new ApplicationException("An error occurred while fetching rejected PCEs.");
            }
        }

        public async Task<IEnumerable<PCEReturnDto>> GetTerminatedPCEs(Guid UserId)
        {
            try
            {
                var entities = await _cbeContext.ProductionCapacityEstimations.Where(e => e.Status == Status.Terminated).ToListAsync();
                return _mapper.Map<IEnumerable<PCEReturnDto>>(entities);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching terminated PCEs");
                throw new ApplicationException("An error occurred while fetching terminated PCEs.");
            }
        }

        public async Task<IEnumerable<PCEReturnDto>> GetPendingPCEs(Guid UserId)
        {
            try
            {
                var entities = await _cbeContext.ProductionCapacityEstimations.Where(e => e.Status == Status.Pending).ToListAsync();
                return _mapper.Map<IEnumerable<PCEReturnDto>>(entities);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching pending PCEs");
                throw new ApplicationException("An error occurred while fetching pending PCEs.");
            }
        }

        public async Task SendForApproval(string selectedPCEIds, string CenterId)
        {
            try
            {
                var ids = selectedPCEIds.Split(',').Select(id => Guid.Parse(id)).ToList();
                var PCEs = await _cbeContext.ProductionCapacityEstimations.Where(e => ids.Contains(e.Id)).ToListAsync();
                foreach (var PCE in PCEs)
                {
                    PCE.Status = Status.Approved;
                }
                await _cbeContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending PCEs for approval");
                throw new ApplicationException("An error occurred while sending PCEs for approval.");
            }
        }

        public async Task RejectPCE(Guid id, string rejectionReason)
        {
            try
            {
                var PCE = await _cbeContext.ProductionCapacityEstimations.FindAsync(id);
                if (PCE == null)
                {
                    _logger.LogWarning("PCE with id {Id} not found", id);
                    throw new KeyNotFoundException("PCE not found");
                }

                PCE.Status = Status.Rejected;
                PCE.RejectionReason = rejectionReason;
                _cbeContext.ProductionCapacityEstimations.Update(PCE);
                await _cbeContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error rejecting PCE");
                throw new ApplicationException("An error occurred while rejecting the PCE.");
            }
        }

        public async Task<PCEScheduleDto> CreateSchedule(Guid UserId, PCEScheduleDto scheduleDto)
        {
            try
            {
                var schedule = _mapper.Map<PCESchedule>(scheduleDto);
                schedule.Id = Guid.NewGuid();
                await _cbeContext.PCESchedules.AddAsync(schedule);
                await _cbeContext.SaveChangesAsync();
                return _mapper.Map<PCEScheduleDto>(schedule);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating schedule");
                throw new ApplicationException("An error occurred while creating the schedule.");
            }
        }

        public async Task<int> GetDashboardPCECount(Guid UserId)
        {
            try
            {
                return await _cbeContext.ProductionCapacityEstimations.CountAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching dashboard PCE count");
                throw new ApplicationException("An error occurred while fetching the dashboard PCE count.");
            }
        }

        public async Task<int> GetMyDashboardPCECount(Guid UserId)
        {
            try
            {
                return await _cbeContext.ProductionCapacityEstimations.CountAsync(e => e.CreatedBy == UserId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching my dashboard PCE count");
                throw new ApplicationException("An error occurred while fetching your dashboard PCE count.");
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

        public async Task<IEnumerable<PCEReturnDto>> GetAllPCEs(Guid UserId)
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
                return _mapper.Map<IEnumerable<PCEReturnDto>>(entities);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching all PCEs");
                throw new ApplicationException("An error occurred while fetching all PCEs.");
            }
        }

        public async Task<bool> DeletePCE(Guid UserId, Guid id)
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
                _logger.LogError(ex, "Error deleting PCE");
                throw new ApplicationException("An error occurred while deleting the PCE.");
            }
        }

        // private async Task UploadFile(Guid UserId, PCE entity, IFormFile? file, String Type)
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
