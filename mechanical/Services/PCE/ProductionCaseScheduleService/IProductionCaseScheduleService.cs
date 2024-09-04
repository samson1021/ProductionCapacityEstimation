using mechanical.Models.PCE.Dto.ProductionCaseScheduleDto;

namespace mechanical.Services.PCE.ProductionCaseScheduleService
{
    public interface IProductionCaseScheduleService
    {
        Task<ProductionCaseScheduleReturnDto> CreateProductionCaseSchedule(Guid UserId, ProductionCaseSchedulePostDto caseCommentPostDto);
        Task<ProductionCaseScheduleReturnDto> UpdateProductionCaseSchedule(Guid UserId, Guid id, ProductionCaseSchedulePostDto caseCommentPostDto);
        Task<ProductionCaseScheduleReturnDto> ApproveProductionCaseSchedule(Guid id);
        Task<IEnumerable<ProductionCaseScheduleReturnDto>> GetProductionCaseSchedules(Guid PCECaseId);

    }
}
