using AutoMapper;
using mechanical.Data;
using mechanical.Models.Dto.CaseCommentDto;
using mechanical.Models.Dto.CaseScheduleDto;
using mechanical.Models.Entities;
using Microsoft.AspNetCore.Server.IIS.Core;
using Microsoft.EntityFrameworkCore;

namespace mechanical.Services.CaseScheduleService
{
    public class CaseScheduleService : ICaseScheduleService
    {
        private readonly CbeContext _cbeContext;
        private readonly IMapper _mapper;
        public CaseScheduleService(CbeContext cbeContext, IMapper mapper)
        {
            _cbeContext = cbeContext;
            _mapper = mapper;
        }

        public async Task<CaseScheduleReturnDto> ApproveCaseSchedule(Guid id)
        {
            var caseSchedule = await _cbeContext.CaseSchedules.FindAsync(id);
            if (caseSchedule == null)
            {
                throw new Exception("case Schedule not Found");
            }
            caseSchedule.Status = "Approved";
            _cbeContext.Update(caseSchedule);
            await _cbeContext.SaveChangesAsync();
            return _mapper.Map<CaseScheduleReturnDto>(caseSchedule);
        }

        public async Task<CaseScheduleReturnDto> CreateCaseSchedule(Guid userId, CaseSchedulePostDto caseCommentPostDto)
        {
            var caseSchedule = _mapper.Map<CaseSchedule>(caseCommentPostDto);
            caseSchedule.UserId = userId;
            caseSchedule.CreatedAt = DateTime.Now;
            caseSchedule.Status = "proposed";

            await _cbeContext.CaseSchedules.AddAsync(caseSchedule);
            await _cbeContext.SaveChangesAsync();
            return _mapper.Map<CaseScheduleReturnDto>(caseSchedule);
          
        }

        public async Task<IEnumerable<CaseScheduleReturnDto>> GetCaseSchedules(Guid caseId)
        {
            var caseSchedules = await _cbeContext.CaseSchedules.Include(res=>res.User).Where(res => res.CaseId == caseId).OrderBy(res=>res.CreatedAt).ToListAsync();
            return _mapper.Map<IEnumerable<CaseScheduleReturnDto>>(caseSchedules);
        }

        public async Task<CaseScheduleReturnDto> UpdateCaseSchedule(Guid userId, Guid Id, CaseSchedulePostDto caseCommentPostDto)
        {
            var caseSchedule = await _cbeContext.CaseSchedules.FindAsync(Id);
            if(caseSchedule == null)
            {
                throw new Exception("case Schedule not Found");
            }
            if(caseSchedule.UserId != userId)
            {
                throw new Exception("unauthorized user");
            }
            _mapper.Map(caseCommentPostDto, caseSchedule);
            caseSchedule.CreatedAt = DateTime.Now;
            _cbeContext.Update(caseSchedule);
            await _cbeContext.SaveChangesAsync();
            return _mapper.Map<CaseScheduleReturnDto>(caseSchedule);
        }
    }
}
