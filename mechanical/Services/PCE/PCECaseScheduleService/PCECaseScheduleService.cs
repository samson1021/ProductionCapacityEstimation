using AutoMapper;
using mechanical.Data;
using mechanical.Models.Dto.CaseScheduleDto;
using mechanical.Models.Entities;
using mechanical.Models.PCE.Dto.ProductionCaseScheduleDto;
using mechanical.Models.PCE.Entities;
using Microsoft.EntityFrameworkCore;

namespace mechanical.Services.PCE.PCECaseScheduleService
{
    public class PCECaseScheduleService : IPCECaseScheduleService
    {
        private readonly CbeContext _cbeContext;
        private readonly IMapper _mapper;
        public PCECaseScheduleService(CbeContext cbeContext, IMapper mapper)
        {
            _cbeContext = cbeContext;
            _mapper = mapper;
        }

        public async Task<ProductionCaseScheduleReturnDto> ApproveCaseSchedule(Guid id)
        {
            var caseSchedule = await _cbeContext.ProductionCaseSchedules.FindAsync(id);
            if (caseSchedule == null)
            {
                throw new Exception("PCE Case Schedule not Found");
            }
            caseSchedule.Status = "Approved";
            _cbeContext.Update(caseSchedule);
            await _cbeContext.SaveChangesAsync();
            return _mapper.Map<ProductionCaseScheduleReturnDto>(caseSchedule);
        }
        public async Task<ProductionCaseScheduleReturnDto> CreateCaseSchedule(Guid userId, ProductionCaseSchedulePostDto caseCommentPostDto)
        {
            var caseSchedule = _mapper.Map<ProductionCaseSchedule>(caseCommentPostDto);
            caseSchedule.UserId = userId;
            caseSchedule.CreatedAt = DateTime.Now;
            caseSchedule.Status = "proposed";

            await _cbeContext.ProductionCaseSchedules.AddAsync(caseSchedule);
            await _cbeContext.SaveChangesAsync();
            return _mapper.Map<ProductionCaseScheduleReturnDto>(caseSchedule);

        }

        public async Task<IEnumerable<ProductionCaseScheduleReturnDto>> GetCaseSchedules(Guid caseId)
        {
            var caseSchedules = await _cbeContext.ProductionCaseSchedules.Include(res => res.User).Where(res => res.PCECaseId == caseId).OrderBy(res => res.CreatedAt).ToListAsync();
            return _mapper.Map<IEnumerable<ProductionCaseScheduleReturnDto>>(caseSchedules);
        }


        public async Task<ProductionCaseScheduleReturnDto> UpdateCaseSchedule(Guid userId, Guid Id, ProductionCaseSchedulePostDto caseCommentPostDto)
        {
            var caseSchedule = await _cbeContext.ProductionCaseSchedules.FindAsync(Id);
            if (caseSchedule == null)
            {
                throw new Exception(" PCE Case Schedule not Found");
            }
            if (caseSchedule.UserId != userId)
            {
                throw new Exception("unauthorized user");
            }
            _mapper.Map(caseCommentPostDto, caseSchedule);
            caseSchedule.CreatedAt = DateTime.Now;
            _cbeContext.Update(caseSchedule);
            await _cbeContext.SaveChangesAsync();
            return _mapper.Map<ProductionCaseScheduleReturnDto>(caseSchedule);
        }

       
    }
}
