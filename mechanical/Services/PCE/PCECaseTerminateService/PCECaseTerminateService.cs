using AutoMapper;
using mechanical.Data;
using mechanical.Models.Dto.CaseTerminateDto;
using mechanical.Models.Entities;
using mechanical.Models.PCE.Dto.PCECaseTerminateDto;
using mechanical.Models.PCE.Entities;
using Microsoft.EntityFrameworkCore;

namespace mechanical.Services.PCE.PCECaseTerminateService
{
    public class PCECaseTerminateService:IPCECaseTerminateService
    {
        private readonly CbeContext _cbeContext;
        private readonly IMapper _mapper;
        public PCECaseTerminateService(CbeContext cbeContext, IMapper mapper)
        {
            _cbeContext = cbeContext;
            _mapper = mapper;
        }

      
        public async Task<PCECaseTerminateReturnDto> ApproveCaseTerminate(Guid id)
        {
            var pcecaseTerminate = await _cbeContext.PCECaseTerminates.FindAsync(id);
            if (pcecaseTerminate == null)
            {
                throw new Exception("case Terminatenot Found");
            }
            pcecaseTerminate.CurrentStatus = "Approved";
            _cbeContext.Update(pcecaseTerminate);
            await _cbeContext.SaveChangesAsync();
            return _mapper.Map<PCECaseTerminateReturnDto>(pcecaseTerminate);
        }




       
        public async Task<PCECaseTerminateReturnDto> CreateCaseTerminate(Guid userId, PCECaseTerminatePostDto pcecaseTerminatePostDto)
        {
            var pcecaseTerminate = _mapper.Map<PCECaseTerminate>(pcecaseTerminatePostDto);
            pcecaseTerminate.RMUserId = userId;
            pcecaseTerminate.CreationDate = DateTime.Now;
            pcecaseTerminate.CurrentStatus = "proposed";
            await _cbeContext.PCECaseTerminates.AddAsync(pcecaseTerminate);
            await _cbeContext.SaveChangesAsync();
            return _mapper.Map<PCECaseTerminateReturnDto>(pcecaseTerminate);

        }

       

        public async Task<IEnumerable<PCECaseTerminateReturnDto>> GetCaseTerminates(Guid pcecaseId)
        {
            try
            {
                var caseTerminatess = await _cbeContext.PCECaseTerminates.Include(res => res.RMUser).Where(res => res.PCECaseId == pcecaseId).OrderBy(res => res.CreationDate).ToListAsync();
               
                return _mapper.Map<IEnumerable<PCECaseTerminateReturnDto>>(caseTerminatess);
            }
            catch (Exception ex)
            {

                var ErrorMessage = "An error occurred: " + ex.Message;
                return _mapper.Map<IEnumerable<PCECaseTerminateReturnDto>>(ErrorMessage);

            }
        }

       


        public async Task<PCECaseTerminateReturnDto> UpdateCaseTerminate(Guid userId, Guid id, PCECaseTerminatePostDto pcecaseTerminatePostDto)
        {
            var caseTerminate = await _cbeContext.PCECaseTerminates.FindAsync(id);
            if (caseTerminate == null)
            {
                throw new Exception("Case Terminate not Found");
            }
            if (caseTerminate.RMUserId != userId)
            {
                throw new Exception("unauthorized user");
            }
            pcecaseTerminatePostDto.PCECaseId = caseTerminate.PCECaseId;
            _mapper.Map(pcecaseTerminatePostDto, caseTerminate);
            caseTerminate.CreationDate = DateTime.Now;
            _cbeContext.Update(caseTerminate);
            await _cbeContext.SaveChangesAsync();
            return _mapper.Map<PCECaseTerminateReturnDto>(caseTerminate);
        }
    }
}
