using DocumentFormat.OpenXml.Vml;
using mechanical.Models.Entities;
using Microsoft.AspNetCore.Http.HttpResults;

namespace mechanical.Models.Dto.InternalReport
{
    public class InternalCaseReportDto
    {
        public Guid Id { get; set; }
        public DateTime CreationAt { get; set; }
        public required string CaseNo { get; set; }
        public required string RequestingUnit { get; set; }
        public required string Segment { get; set; }
        public required string ApplicantName { get; set; }
        public required string CustomerId { get; set; }
        public string? CustomerEmail { get; set; } = string.Empty;
        public string District {  get; set; } = string.Empty;
        public required string Status { get; set; }
        public int NoOfCollateral { get; set; } = 0;
        public int TotalNoOfCollateral { get; set; } = 0;


        public string? TeamLeader { get; set; } = string.Empty;

    }
}
