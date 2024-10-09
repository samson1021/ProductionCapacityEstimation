using AutoMapper;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;

using mechanical.Data;
using mechanical.Models.Entities;
using mechanical.Models.PCE.Entities;
using mechanical.Models.PCE.Dto.PCECaseScheduleDto;

namespace mechanical.Services.PCE.PCECaseScheduleService
{
    public class PCECaseScheduleService : IPCECaseScheduleService
    {
        private readonly IMapper _mapper;
        private readonly CbeContext _cbeContext;
        private readonly ILogger<PCECaseScheduleService> _logger;
        
        public PCECaseScheduleService(CbeContext cbeContext, IMapper mapper, ILogger<PCECaseScheduleService> logger)
        {
            _cbeContext = cbeContext;
            _mapper = mapper;
            _logger = logger;
        }
        
        public async Task<PCECaseScheduleReturnDto> ApprovePCECaseSchedule(Guid id)
        {
            using var transaction = await _cbeContext.Database.BeginTransactionAsync();
            try
            {  
                var pceCaseSchedule = await _cbeContext.PCECaseSchedules.FindAsync(id);
                if (pceCaseSchedule == null)
                {
                    throw new Exception("PCE Case Schedule not Found");
                }
                pceCaseSchedule.Status = "Approved";
                _cbeContext.Update(pceCaseSchedule);

                await _cbeContext.SaveChangesAsync();
                await transaction.CommitAsync();

                return _mapper.Map<PCECaseScheduleReturnDto>(pceCaseSchedule);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error approving PCE case schedule");
                await transaction.RollbackAsync();
                throw new ApplicationException("An error occurred while approving PCE case schedule.");
            }
        }

        public async Task<PCECaseScheduleReturnDto> CreatePCECaseReSchedule(Guid UserId, PCECaseSchedulePostDto pceCaseScheduleDto)
        {
            using var transaction = await _cbeContext.Database.BeginTransactionAsync();
            try
            {
                var existingSchedule = await _cbeContext.PCECaseSchedules.FindAsync(pceCaseScheduleDto.Id);

                if (existingSchedule == null)
                {
                    return _mapper.Map<PCECaseScheduleReturnDto>(existingSchedule);
                }

                existingSchedule.Status = "Reschedule";
                existingSchedule.Reason = pceCaseScheduleDto.Reason;
                _cbeContext.Update(existingSchedule);
                pceCaseScheduleDto.Reason = null;

                var pceCaseSchedule = _mapper.Map<Models.PCE.Entities.PCECaseSchedule>(pceCaseScheduleDto);
                pceCaseSchedule.Id = new Guid();
                pceCaseSchedule.UserId = UserId;
                pceCaseSchedule.Status = "Proposed";
                pceCaseSchedule.CreatedAt = DateTime.Now;

                await _cbeContext.PCECaseSchedules.AddAsync(pceCaseSchedule);
                await _cbeContext.SaveChangesAsync();
                await transaction.CommitAsync();

                return _mapper.Map<PCECaseScheduleReturnDto>(pceCaseSchedule);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating PCE case schedule");
                await transaction.RollbackAsync();
                throw new ApplicationException("An error occurred while creating PCE case schedule.");
            }
        }


        public async Task<PCECaseScheduleReturnDto> CreatePCECaseSchedule(Guid UserId, PCECaseSchedulePostDto pceCaseScheduleDto)
        {
            using var transaction = await _cbeContext.Database.BeginTransactionAsync();
            try
            {

                var pceCaseSchedule = _mapper.Map<Models.PCE.Entities.PCECaseSchedule>(pceCaseScheduleDto);
                pceCaseSchedule.Id = new Guid();
                pceCaseSchedule.UserId = UserId;
                pceCaseSchedule.Status = "Proposed";
                pceCaseSchedule.CreatedAt = DateTime.Now;

                await _cbeContext.PCECaseSchedules.AddAsync(pceCaseSchedule);
                await _cbeContext.SaveChangesAsync();
                await transaction.CommitAsync();

                return _mapper.Map<PCECaseScheduleReturnDto>(pceCaseSchedule);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating PCE case schedule");
                await transaction.RollbackAsync();
                throw new ApplicationException("An error occurred while creating PCE case schedule.");
            }
        }

        public async Task<IEnumerable<PCECaseScheduleReturnDto>> GetPCECaseSchedules(Guid PCECaseId)
        {
            var pceCaseSchedules = await _cbeContext.PCECaseSchedules.Include(res => res.User).Where(res => res.PCECaseId == PCECaseId).OrderBy(res => res.CreatedAt).ToListAsync();
            
            return _mapper.Map<IEnumerable<PCECaseScheduleReturnDto>>(pceCaseSchedules);
        }

        public async Task<PCECaseScheduleReturnDto> GetLatestPCECaseSchedule(Guid PCECaseId)
        {
            var pceCaseSchedule = await _cbeContext.PCECaseSchedules.AsNoTracking().Include(pcs => pcs.User).Where(pcs => pcs.PCECaseId == PCECaseId).OrderByDescending(pcs => pcs.CreatedAt).FirstOrDefaultAsync();
            return _mapper.Map<PCECaseScheduleReturnDto>(pceCaseSchedule);
        }

        public async Task<PCECaseScheduleReturnDto> UpdatePCECaseSchedule(Guid UserId, Guid id, PCECaseSchedulePostDto pceCaseScheduleDto)
        {
            using var transaction = await _cbeContext.Database.BeginTransactionAsync();
            try
            {  
                var pceCaseSchedule = await _cbeContext.PCECaseSchedules.FindAsync(id);
                if (pceCaseSchedule == null)
                {
                    throw new Exception("PCE case Schedule not Found");
                }
                if (pceCaseSchedule.UserId != UserId)
                {
                    throw new Exception("unauthorized user");
                }
                _mapper.Map(pceCaseScheduleDto, pceCaseSchedule);
                pceCaseSchedule.CreatedAt = DateTime.Now;
                _cbeContext.Update(pceCaseSchedule);

                await _cbeContext.SaveChangesAsync();
                await transaction.CommitAsync();

                return _mapper.Map<PCECaseScheduleReturnDto>(pceCaseSchedule);
            }

            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updatimg PCE case schedule");
                await transaction.RollbackAsync();
                throw new ApplicationException("An error occurred while updatimg PCE case schedule.");
            }
        }
    }
}