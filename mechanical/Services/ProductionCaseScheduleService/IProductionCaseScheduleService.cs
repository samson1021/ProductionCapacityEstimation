using mechanical.Models.Dto.ProductionCaseScheduleDto;

namespace mechanical.Services.ProductionCaseScheduleService
{
    public interface IProductionCaseScheduleService
    {
        Task<ProductionCaseScheduleReturnDto> CreateProductionCaseSchedule(Guid userId, ProductionCaseSchedulePostDto caseCommentPostDto);
        Task<ProductionCaseScheduleReturnDto> UpdateProductionCaseSchedule(Guid userId, Guid id, ProductionCaseSchedulePostDto caseCommentPostDto);
        Task<ProductionCaseScheduleReturnDto> ApproveProductionCaseSchedule(Guid id);
        Task<IEnumerable<ProductionCaseScheduleReturnDto>> GetProductionCaseSchedules(Guid caseId);

    }
}
