namespace mechanical.Models.Dto.CaseScheduleDto
{
    public class CaseSchedulePostDto
    {
        public Guid Id { get; set; }
        public DateTime ScheduleDate { get; set; }
        public string? Reason { get; set; }
        public Guid CaseId { get; set; }
    }
}
