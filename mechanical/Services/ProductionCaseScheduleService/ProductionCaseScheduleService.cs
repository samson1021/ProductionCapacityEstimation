using AutoMapper;
using mechanical.Data;
using mechanical.Models.Dto.CaseScheduleDto;
using mechanical.Models.Dto.ProductionCaseScheduleDto;
using mechanical.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace mechanical.Services.ProductionCaseScheduleService
{
    public class ProductionCaseScheduleService : IProductionCaseScheduleService
    {
        private readonly CbeContext _cbeContext;
        private readonly IMapper _mapper;
        public ProductionCaseScheduleService(CbeContext cbeContext, IMapper mapper)
        {
            _cbeContext = cbeContext;
            _mapper = mapper;
        }
        //public async Task<CaseScheduleReturnDto> ApproveCaseSchedule(Guid id)
        //{
        //    var caseSchedule = await _cbeContext.CaseSchedules.FindAsync(id);
        //    if (caseSchedule == null)
        //    {
        //        throw new Exception("case Schedule not Found");
        //    }
        //    caseSchedule.Status = "Approved";
        //    _cbeContext.Update(caseSchedule);
        //    await _cbeContext.SaveChangesAsync();
        //    return _mapper.Map<CaseScheduleReturnDto>(caseSchedule);
        //}
        public async Task<ProductionCaseScheduleReturnDto> ApproveProductionCaseSchedule(Guid id)
        {
            var caseSchedule = await _cbeContext.ProductionCaseSchedules.FindAsync(id);
            if (caseSchedule == null)
            {
                throw new Exception("Production Case Schedule not Found");
            }
            caseSchedule.Status = "Approved";
            _cbeContext.Update(caseSchedule);
            await _cbeContext.SaveChangesAsync();
            return _mapper.Map<ProductionCaseScheduleReturnDto>(caseSchedule);
        }

        public async Task<ProductionCaseScheduleReturnDto> CreateProductionCaseSchedule(Guid userId, ProductionCaseSchedulePostDto caseCommentPostDto)
        {
            var caseSchedule = _mapper.Map<ProductionCaseSchedule>(caseCommentPostDto);
            caseSchedule.UserId = userId;
            caseSchedule.CreatedAt = DateTime.Now;
            caseSchedule.Status = "proposed";

            await _cbeContext.ProductionCaseSchedules.AddAsync(caseSchedule);
            await _cbeContext.SaveChangesAsync();
            return _mapper.Map<ProductionCaseScheduleReturnDto>(caseSchedule);
        }

        public async Task<IEnumerable<ProductionCaseScheduleReturnDto>> GetProductionCaseSchedules(Guid caseId)
        {
            var caseSchedules = await _cbeContext.ProductionCaseSchedules.Include(res => res.User).Where(res => res.ProductionCaseId == caseId).OrderBy(res => res.CreatedAt).ToListAsync();
            return _mapper.Map<IEnumerable<ProductionCaseScheduleReturnDto>>(caseSchedules);
        }

        public async Task<ProductionCaseScheduleReturnDto> UpdateProductionCaseSchedule(Guid userId, Guid id, ProductionCaseSchedulePostDto caseCommentPostDto)
        {
            var caseSchedule = await _cbeContext.ProductionCaseSchedules.FindAsync(id);
            if (caseSchedule == null)
            {
                throw new Exception("Production case Schedule not Found");
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
