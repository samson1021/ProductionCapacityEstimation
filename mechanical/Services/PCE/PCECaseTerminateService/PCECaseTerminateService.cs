using AutoMapper;
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

        public async Task<PCECaseTerminateReturnDto> ApproveCaseTermination(Guid Id)
        {
            using var transaction = await _cbeContext.Database.BeginTransactionAsync();
            try
            {  
                var pceCaseTerminate = await _cbeContext.PCECaseTerminates.FindAsync(Id);
                if (pceCaseTerminate == null)
                {
                    throw new Exception("case Schedule not Found");
                }
                pceCaseTerminate.Status = "Approved";
                _cbeContext.Update(pceCaseTerminate);

                var cases = await _cbeContext.PCECases.FindAsync(pceCaseTerminate.PCECaseId);
                cases.Status = "Terminated";
                var productions = await _cbeContext.ProductionCapacities.Where(res => res.PCECaseId == pceCaseTerminate.PCECaseId).ToListAsync();

                foreach (var production in productions)
                {
                    production.CurrentStage = "Relation Manager";
                    production.CurrentStatus = "Terminated";
                    var caseAssignments = await _cbeContext.PCECaseAssignments.Where(res => res.ProductionCapacityId == production.Id).ToListAsync();
                    foreach (var caseAssignment in caseAssignments)
                    {
                        caseAssignment.Status = "Terminated";

                    }
                    _cbeContext.PCECaseAssignments.UpdateRange(caseAssignments);
                }

                _cbeContext.ProductionCapacities.UpdateRange(productions);
          
                await _cbeContext.SaveChangesAsync();
                await transaction.CommitAsync();
                
                return _mapper.Map<PCECaseTerminateReturnDto>(pceCaseTerminate);    
            }

            catch (Exception ex)
            {
                _logger.LogError(ex, "Error approving case termination");
                await transaction.RollbackAsync();
                throw new ApplicationException($"An error occurred while approving case termination. { ex.Message }");
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

        public async Task<IEnumerable<PCECaseTerminateDto>> GetPCECaseTerminates(Guid UserId)
        {
            var pceCases = await _cbeContext.PCECases.Include(x => x.ProductionCapacities).Where(res => res.RMUserId == UserId && res.Status == "Terminated").ToListAsync();
            var pceCaseDtos = _mapper.Map<IEnumerable<PCECaseTerminateDto>>(pceCases);
            foreach (var pceCaseDto in pceCaseDtos)
            {
                var pceCaseTerminate = await _cbeContext.PCECaseTerminates.Where(res => res.PCECaseId == pceCaseDto.Id).FirstOrDefaultAsync();
                pceCaseDto.TerminationReason = pceCaseTerminate?.Reason;
            }
            return pceCaseDtos;
        }
    }
}
