﻿using AutoMapper;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;

using mechanical.Data;
using mechanical.Models.Entities;
using mechanical.Models.PCE.Entities;
using mechanical.Models.PCE.Dto.PCECaseTerminateDto;

namespace mechanical.Services.PCE.PCECaseTerminateService
{
    public class PCECaseTerminateService:IPCECaseTerminateService
    {
        private readonly CbeContext _cbeContext;
        private readonly IMapper _mapper;
        private readonly ILogger<PCECaseTerminateService> _logger;
        
        public PCECaseTerminateService(CbeContext cbeContext, IMapper mapper, ILogger<PCECaseTerminateService> logger)
        {
            _cbeContext = cbeContext;
            _mapper = mapper;
            _logger = logger;
        }      
       
        public async Task<PCECaseTerminateReturnDto> CreateCaseTerminate(Guid UserId, PCECaseTerminatePostDto pceCaseTerminatePostDto)
        {
            using var transaction = await _cbeContext.Database.BeginTransactionAsync();
            try
            {  
                var pceCaseTerminate = _mapper.Map<PCECaseTerminate>(pceCaseTerminatePostDto);
                pceCaseTerminate.RMUserId = UserId;
                pceCaseTerminate.CreationDate = DateTime.Now;
                pceCaseTerminate.Status = "Proposed";
                // pceCaseTerminate.CurrentStatus = "Proposed";

                await _cbeContext.PCECaseTerminates.AddAsync(pceCaseTerminate);
                await _cbeContext.SaveChangesAsync();
                await transaction.CommitAsync();
                
                return _mapper.Map<PCECaseTerminateReturnDto>(pceCaseTerminate);  
            }

            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating case termination");
                await transaction.RollbackAsync();
                throw new ApplicationException("An error occurred while creating case termination.");
            }

        } 
       
        public async Task<PCECaseTerminateReturnDto> UpdateCaseTerminate(Guid UserId, Guid Id, PCECaseTerminatePostDto pceCaseTerminatePostDto)
        {
            using var transaction = await _cbeContext.Database.BeginTransactionAsync();
            try
            { 

                var pceCaseTerminate = await _cbeContext.PCECaseTerminates.FindAsync(Id);
                if (pceCaseTerminate == null)
                {
                    return _mapper.Map<PCECaseTerminateReturnDto>(pceCaseTerminate);
                }
                if (pceCaseTerminate.RMUserId != UserId)
                {
                    // throw new Exception("unauthorized user");
                    return _mapper.Map<PCECaseTerminateReturnDto>(pceCaseTerminate);
                }
                pceCaseTerminatePostDto.PCECaseId = pceCaseTerminate.PCECaseId;
                _mapper.Map(pceCaseTerminatePostDto, pceCaseTerminate);
                pceCaseTerminate.CreationDate = DateTime.Now;
                _cbeContext.Update(pceCaseTerminate);

                await _cbeContext.SaveChangesAsync();
                await transaction.CommitAsync();

                return _mapper.Map<PCECaseTerminateReturnDto>(pceCaseTerminate); 
            }

            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating the case termination");
                await transaction.RollbackAsync();
                throw new ApplicationException("An error occurred while updating the case termination.");
            }
        }

        public async Task<PCECaseTerminateReturnDto> ApproveCaseTerminate(Guid Id)
        {
            using var transaction = await _cbeContext.Database.BeginTransactionAsync();
            try
            { 
                var pceCaseTerminate = await _cbeContext.PCECaseTerminates.FindAsync(Id);
                if (pceCaseTerminate == null)
                {
                    return _mapper.Map<PCECaseTerminateReturnDto>(pceCaseTerminate);
                }
                
                pceCaseTerminate.Status = "Approved";
                // pceCaseTerminate.CurrentStatus = "Approved";
                _cbeContext.Update(pceCaseTerminate);

                await _cbeContext.SaveChangesAsync();
                await transaction.CommitAsync();
                
                return _mapper.Map<PCECaseTerminateReturnDto>(pceCaseTerminate);   
            }

            catch (Exception ex)
            {
                _logger.LogError(ex, "Error approving the case termination");
                await transaction.RollbackAsync();
                throw new ApplicationException("An error occurred while approving the case termination.");
            }
        }       

        public async Task<PCECaseTerminateReturnDto> GetCaseTerminate(Guid Id)
        {
            var pceCaseTerminate = await _cbeContext.PCECaseTerminates.Include(res => res.RMUser).FirstOrDefaultAsync(res => res.Id == Id);
               
            return _mapper.Map<PCECaseTerminateReturnDto>(pceCaseTerminate);            
        }        

        public async Task<IEnumerable<PCECaseTerminateReturnDto>> GetCaseTerminates(Guid PCECaseId)
        {
            var pceCaseTerminates = await _cbeContext.PCECaseTerminates.Include(res => res.RMUser).Where(res => res.PCECaseId == PCECaseId).OrderBy(res => res.CreationDate).ToListAsync();
            
            return _mapper.Map<IEnumerable<PCECaseTerminateReturnDto>>(pceCaseTerminates);            
        }
    }
}
