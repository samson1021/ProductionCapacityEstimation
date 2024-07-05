
// using AutoMapper;
// using Microsoft.EntityFrameworkCore;
// using Microsoft.Extensions.Logging;
// using Microsoft.AspNetCore.Mvc;

// using System;
// using System.IO;
// using System.Linq;
// using System.Collections.Generic;
// using System.Threading.Tasks;

// using mechanical.Data;
// using mechanical.Models.PCE.Entities;
// using mechanical.Models.PCE.Dto.PCECaseScheduleDto;
// using mechanical.Models.PCE.Dto.FileUploadDto;
// using mechanical.Services.PCE.PCECaseScheduleService;

// namespace mechanical.Services.PCE.PCECaseScheduleService
// {
//     public class PCECaseScheduleService : IPCECaseScheduleService
//     {
//         private readonly CbeContext _cbeContext;
//         private readonly IMapper _mapper;
//         private readonly ILogger<PCECaseScheduleService> _logger;

//         public PCECaseScheduleService(IWebHostEnvironment webHostEnvironment, CbeContext context, IMapper mapper, ILogger<PCECaseScheduleService> logger)
//         {
//             _cbeContext = context;
//             _mapper = mapper;
//             _logger = logger;
//         }                 
             
          
//         public async Task<PCECaseSchedulePostDto> CreatePCECaseSchedule(Guid UserId, PCECaseSchedulePostDto dto)
//         {
//             try
//             {
//                 var schedule = _mapper.Map<PCECaseSchedule>(dto);
//                 schedule.Id = Guid.NewGuid();
//                 schedule.CreatedBy = UserId;             
//                 // schedule.PCECaseId = dto.PCECaseId;
         
//                 await _cbeContext.PCECaseSchedules.AddAsync(schedule);
//                 await _cbeContext.SaveChangesAsync();

//                 return  _mapper.Map<PCECaseSchedulePostDto>(schedule);
            
//             }
//             catch (Exception ex)
//             {
//                 _logger.LogError(ex, "Error creating PCECaseSchedule");
//                 throw new ApplicationException("An error occurred while creating the PCECaseSchedule.");
//             }
//         }
        
//         public async Task<PCECaseSchedulePostDto> EditPCECaseSchedule(Guid UserId, Guid id, PCECaseSchedulePostDto dto)
//         {
//             try
//             {
//                 var schedule = await _cbeContext.PCECaseSchedules.FindAsync(base.GetCurrentUserId(), id);
//                 if (schedule == null)
//                 {
//                     _logger.LogWarning("PCECaseSchedule with id {Id} not found", id);
//                     throw new KeyNotFoundException("PCECaseSchedule not found");
//                 }

//                 _mapper.Map(dto, schedule);
//                 _cbeContext.PCECaseSchedules.Update(schedule);
//                 await _cbeContext.SaveChangesAsync();
//                 var resultDto = _mapper.Map<PCECaseSchedulePostDto>(schedule);

//                 return resultDto;
          
//                 return _mapper.Map<PCECaseSchedulePostDto>(schedule);
//             }
//             catch (Exception ex)
//             {
//                 _logger.LogError(ex, "Error editing PCECaseSchedule");
//                 throw new ApplicationException("An error occurred while editing the PCECaseSchedule.");
//             }
//         }

//         public async Task<PCECaseScheduleReturnDto> GetPCECaseSchedule(Guid UserId, Guid id)
//         {
//             try
//             {
//                 var schedule = await _cbeContext.PCECaseSchedules.FirstOrDefaultAsync(e => e.Id == id);
//                 if (schedule == null)
//                 {
//                     _logger.LogWarning("PCECaseSchedule with id {Id} not found", id);
//                     throw new KeyNotFoundException("PCECaseSchedule not found");
//                 }
//                 return _mapper.Map<PCECaseScheduleReturnDto>(schedule);
//             }
//             catch (Exception ex)
//             {
//                 _logger.LogError(ex, "Error fetching PCECaseSchedule");
//                 throw new ApplicationException("An error occurred while fetching the PCECaseSchedule.");
//             }
//         }

//         public async Task<bool> DeletePCECaseSchedule(Guid UserId, Guid id)
//         {
//             try
//             {
//                 var schedule = await _cbeContext.PCECaseSchedules.FindAsync(base.GetCurrentUserId(), id);
//                 if (schedule == null)
//                 {
//                     return false;
//                 }

//                 _cbeContext.PCECaseSchedules.Remove(schedule);
//                 await _cbeContext.SaveChangesAsync();
//                 return true;
//             }
//             catch (Exception ex)
//             {
//                 _logger.LogError(ex, "Error deleting PCECaseSchedule");
//                 throw new ApplicationException("An error occurred while deleting the PCECaseSchedule.");
//             }
//         }
//     }
// }
