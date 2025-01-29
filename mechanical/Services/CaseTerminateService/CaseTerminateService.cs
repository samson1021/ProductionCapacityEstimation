using AutoMapper;
using mechanical.Data;
using mechanical.Models.Dto.CaseCommentDto;
using mechanical.Models.Dto.CaseTerminateDto;
using mechanical.Models.Entities;
using Microsoft.AspNetCore.Server.IIS.Core;
using Microsoft.EntityFrameworkCore;

namespace mechanical.Services.CaseTerminateService
{
    public class CaseTerminateService : ICaseTerminateService
    {
        private readonly CbeContext _cbeContext;
        private readonly IMapper _mapper;
        public CaseTerminateService(CbeContext cbeContext, IMapper mapper)
        {
            _cbeContext = cbeContext;
            _mapper = mapper;
        }

        public async Task<CaseTerminateReturnDto> ApproveCaseTerminate(Guid id)
        {
            var caseTerminate = await _cbeContext.CaseTerminates.FindAsync(id);
            if (caseTerminate == null)
            {
                throw new Exception("case Terminatenot Found");
            }
            caseTerminate.Status = "Approved";
            _cbeContext.Update(caseTerminate);
            await _cbeContext.SaveChangesAsync();
            return _mapper.Map<CaseTerminateReturnDto>(caseTerminate);
        }

        public async  Task<CaseTerminateReturnDto> CreateCaseTerminate(Guid userId, CaseTerminatePostDto caseTerminatePostDto)
        {
            var caseTerminate = _mapper.Map<CaseTerminate>(caseTerminatePostDto);
            caseTerminate.UserId = userId;
            caseTerminate.CreatedAt = DateTime.Now;
            caseTerminate.Status = "proposed";
            await _cbeContext.CaseTerminates.AddAsync(caseTerminate);
            await _cbeContext.SaveChangesAsync();
            return _mapper.Map<CaseTerminateReturnDto>(caseTerminate);
          
        }

        public async Task<IEnumerable<CaseTerminateReturnDto>> GetCaseTerminates(Guid caseId)
        {
            try
            {
                var caseTerminatess = await _cbeContext.CaseTerminates.Include(res => res.User).Where(res => res.CaseId == caseId).OrderBy(res => res.CreatedAt).ToListAsync();
                //var test = "thse";
                return _mapper.Map<IEnumerable<CaseTerminateReturnDto>>(caseTerminatess);
            }
            catch (Exception ex)
            {

                var ErrorMessage = "An error occurred: " + ex.Message;
                return _mapper.Map<IEnumerable<CaseTerminateReturnDto>>(ErrorMessage);

            }
        }

        public async Task<CaseTerminateReturnDto> UpdateCaseTerminate(Guid userId, Guid id, CaseTerminatePostDto caseTerminatePostDto)
        {
            var caseTerminate = await _cbeContext.CaseTerminates.FindAsync(id);
            if(caseTerminate == null)
            {
                throw new Exception("Case Terminate not Found");
            }
            if(caseTerminate.UserId != userId)
            {
                throw new Exception("unauthorized user");
            }
            caseTerminatePostDto.CaseId = caseTerminate.CaseId;
            _mapper.Map(caseTerminatePostDto, caseTerminate);
            caseTerminate.CreatedAt = DateTime.Now;
            _cbeContext.Update(caseTerminate);
            await _cbeContext.SaveChangesAsync();
            return _mapper.Map<CaseTerminateReturnDto>(caseTerminate);
        }
    }
}
