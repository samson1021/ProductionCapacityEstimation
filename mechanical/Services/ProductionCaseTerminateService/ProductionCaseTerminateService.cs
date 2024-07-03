using AutoMapper;
using mechanical.Data;
using mechanical.Models.Dto.CaseTerminateDto;
using mechanical.Models.Dto.ProductionCaseTerminateDto;
using mechanical.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace mechanical.Services.ProductionCaseTerminateService
{
    public class ProductionCaseTerminateService : IProductionCaseTerminateService
    {
        private readonly CbeContext _cbeContext;
        private readonly IMapper _mapper;
        public ProductionCaseTerminateService(CbeContext cbeContext, IMapper mapper)
        {
            _cbeContext = cbeContext;
            _mapper = mapper;
        }
        public async Task<ProductionCaseTerminateReturnDto> ApproveProductionCaseTerminate(Guid id)
        {
            var caseTerminate = await _cbeContext.ProductionCaseTimeLines.FindAsync(id);
            if (caseTerminate == null)
            {
                throw new Exception("Production case Terminatenot Found");
            }
            caseTerminate.Status = "Approved";
            _cbeContext.Update(caseTerminate);
            await _cbeContext.SaveChangesAsync();
            return _mapper.Map<ProductionCaseTerminateReturnDto>(caseTerminate);
        }

        public async Task<ProductionCaseTerminateReturnDto> CreateProductionCaseTerminate(Guid userId, ProductionCaseTerminateReturnDto caseTerminatePostDto)
        {
            var caseTerminate = _mapper.Map<ProductionCaseTerminate>(caseTerminatePostDto);
            caseTerminate.UserId = userId;
            caseTerminate.CreatedAt = DateTime.Now;
            caseTerminate.Status = "proposed";
            await _cbeContext.ProductionCaseTerminates.AddAsync(caseTerminate);
            await _cbeContext.SaveChangesAsync();
            return _mapper.Map<ProductionCaseTerminateReturnDto>(caseTerminate);
        }

        public async Task<IEnumerable<ProductionCaseTerminateReturnDto>> GetProductionCaseTerminates(Guid caseId)
        {
            try
            {
                var caseTerminatess = await _cbeContext.ProductionCaseTerminates.Include(res => res.User).Where(res => res.ProductionCaseId == caseId).OrderBy(res => res.CreatedAt).ToListAsync();
                //var test = "thse";
                return _mapper.Map<IEnumerable<ProductionCaseTerminateReturnDto>>(caseTerminatess);
            }
            catch (Exception ex)
            {

                var ErrorMessage = "An error occurred: " + ex.Message;
                return _mapper.Map<IEnumerable<ProductionCaseTerminateReturnDto>>(ErrorMessage);

            }
        }

        public async Task<ProductionCaseTerminateReturnDto> UpdateProductionCaseTerminate(Guid userId, Guid id, ProductionCaseTerminateReturnDto caseTerminatePostDto)
        {
            var caseTerminate = await _cbeContext.ProductionCaseTerminates.FindAsync(id);
            if (caseTerminate == null)
            {
                throw new Exception("Case Terminate not Found");
            }
            if (caseTerminate.UserId != userId)
            {
                throw new Exception("unauthorized user");
            }
            caseTerminatePostDto.ProductionCaseId = caseTerminate.ProductionCaseId;
            _mapper.Map(caseTerminatePostDto, caseTerminate);
            caseTerminate.CreatedAt = DateTime.Now;
            _cbeContext.Update(caseTerminate);
            await _cbeContext.SaveChangesAsync();
            return _mapper.Map<ProductionCaseTerminateReturnDto>(caseTerminate);
        }
    }
    
}
