using mechanical.Models.Entities;

namespace mechanical.Models.PCE.Dto.ProductionCaseScheduleDto;

public class ProductionCaseScheduleReturnDto
{
    public Guid Id { get; set; }
    public DateTime ScheduleDate { get; set; }
    public string? Reason { get; set; }
    public string? Status { get; set; }
    public DateTime CreatedAt { get; set; }

    public Guid ProductionCaseId { get; set; }

    public virtual CreateUser? User { get; set; }
}
