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
        private readonly CbeContext _cbeContext;
        private readonly IMapper _mapper;
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
                var caseSchedule = await _cbeContext.PCECaseSchedules.FindAsync(id);
                if (caseSchedule == null)
                {
                    throw new Exception("PCE Case Schedule not Found");
                }
                caseSchedule.Status = "Approved";
                _cbeContext.Update(caseSchedule);

                await _cbeContext.SaveChangesAsync();
                await transaction.CommitAsync();

                return _mapper.Map<PCECaseScheduleReturnDto>(caseSchedule);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error approving case schedule");
                await transaction.RollbackAsync();
                throw new ApplicationException("An error occurred while approving case schedule.");
            }
        }


        public async Task<PCECaseScheduleReturnDto> CreatePCECaseSchedule(Guid UserId, PCECaseSchedulePostDto caseCommentPostDto)
        {
            using var transaction = await _cbeContext.Database.BeginTransactionAsync();
            try
            {
                var existingSchedule = await _cbeContext.PCECaseSchedules.FindAsync(caseCommentPostDto.Id);
                if (existingSchedule == null)
                {
                    return _mapper.Map<PCECaseScheduleReturnDto>(existingSchedule);
                }

                existingSchedule.Status = "Reschedule";
                existingSchedule.Reason= caseCommentPostDto.Reason;
                _cbeContext.Update(existingSchedule); 
                caseCommentPostDto.Reason = null;

                var caseSchedule = _mapper.Map<Models.PCE.Entities.PCECaseSchedule>(caseCommentPostDto);
                caseSchedule.UserId = UserId;
                caseSchedule.CreatedAt = DateTime.Now;
                caseSchedule.Status = "Proposed";
                caseSchedule.Id = new Guid();

                await _cbeContext.PCECaseSchedules.AddAsync(caseSchedule);
                await _cbeContext.SaveChangesAsync();
                await transaction.CommitAsync();

                return _mapper.Map<PCECaseScheduleReturnDto>(caseSchedule);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating case schedule");
                await transaction.RollbackAsync();
                throw new ApplicationException("An error occurred while creating case schedule.");
            }
        }

      

        public async Task<IEnumerable<PCECaseScheduleReturnDto>> GetPCECaseSchedules(Guid PCECaseId)
        {
            var caseSchedules = await _cbeContext.PCECaseSchedules.Include(res => res.User).Where(res => res.PCECaseId == PCECaseId).OrderBy(res => res.CreatedAt).ToListAsync();
            
            return _mapper.Map<IEnumerable<PCECaseScheduleReturnDto>>(caseSchedules);
        }

      

        public async Task<PCECaseScheduleReturnDto> UpdatePCECaseSchedule(Guid UserId, Guid id, PCECaseSchedulePostDto caseCommentPostDto)
        {
            using var transaction = await _cbeContext.Database.BeginTransactionAsync();
            try
            {  
                var caseSchedule = await _cbeContext.PCECaseSchedules.FindAsync(id);
                if (caseSchedule == null)
                {
                    throw new Exception("PCE case Schedule not Found");
                }
                if (caseSchedule.UserId != UserId)
                {
                    throw new Exception("unauthorized user");
                }
                _mapper.Map(caseCommentPostDto, caseSchedule);
                caseSchedule.CreatedAt = DateTime.Now;
                _cbeContext.Update(caseSchedule);

                await _cbeContext.SaveChangesAsync();
                await transaction.CommitAsync();

                return _mapper.Map<PCECaseScheduleReturnDto>(caseSchedule);
            }

            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updatimg case schedule");
                await transaction.RollbackAsync();
                throw new ApplicationException("An error occurred while updatimg case schedule.");
            }
        }
    }
    
}
