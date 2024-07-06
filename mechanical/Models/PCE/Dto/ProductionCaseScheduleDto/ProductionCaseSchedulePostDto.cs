﻿namespace mechanical.Models.PCE.Dto.ProductionCaseScheduleDto;

public class ProductionCaseSchedulePostDto
{
    public Guid Id { get; set; }
    public DateTime ScheduleDate { get; set; }
    public string? Reason { get; set; }
    public Guid PCECaseId { get; set; }
}
