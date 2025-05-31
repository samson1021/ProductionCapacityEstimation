using DocumentFormat.OpenXml.Vml;
using Humanizer;
using mechanical.Data;
using mechanical.Models.Entities;
using Microsoft.AspNetCore.Http.HttpResults;
using PdfSharp.Diagnostics;

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
    public class ValuationReportDto
    {
        public Guid Id { get; set; }
        public DateTime? CreatedAt { get; set; } // New property
        public string DistrictName { get; set; } // Replaces IsHeadOfficeCase
        public string CurrentStatus { get; set; }
        public string CaseNo { get; set; }
        public string ApplicantName { get; set; }
        public string CasePriority { get; set; }
        public string CaseFRQ { get; set; }
        public string RequestedOrgan { get; set; }
        public string CustomerApplicantRelationship { get; set; }
        public DateTime? DateCaseDeliveredToValuationOffice { get; set; }
        public DateTime? DateCaseAssignedToTeamLeader { get; set; }
        public DateTime? DateCaseAssignedToValuators { get; set; }
        public string PurposeOfValuationRequest { get; set; }
        public DateTime? LastRecentValuationDate { get; set; }
        public int? DurationReceiptGrossDays { get; set; }
        public int? DurationAssignedToTMGrossDays { get; set; }
        public int? DurationAssignedGrossDays { get; set; }
        public string RequestedEngineer { get; set; }
        public int? ProcessingWCDays { get; set; }
        public int? QuantityForSimilarMechanicalItem { get; set; }
        public string NameOfValuator { get; set; }
        public int? AssignedNo { get; set; }
        public int? DeliveredNo { get; set; }
        public int? ReturnedWithAdvice { get; set; }
        public int? OnHandNo { get; set; }
        public double? DeliveredPercentage { get; set; }
        public int? NetDaysConsumed { get; set; }
        public string SDTAccomplishment { get; set; }
        public string FulfillmentOfDocumentation { get; set; }
        public DateTime? ScheduledVisitDate { get; set; }
        public string Location { get; set; }
        public string QuantityComplexityOfProperty { get; set; }
        public string LHCTitleDeedSerialNo { get; set; }
        public string TypeOfProperty { get; set; }
        public string PropertyCategory { get; set; }
        public DateTime? SiteInspectionDate { get; set; }
        public DateTime? DateSentForChecking { get; set; }
        public int? TotalNumberOfComments { get; set; }
        public DateTime? DateCommentReceivedFromChecking { get; set; }
        public DateTime? DateReportSentToRequestingOrgan { get; set; }
        public DateTime? DateReturnedWithAdviceToRO { get; set; }
        public int? GrossDaysConsumedChecker { get; set; }
        public int? GrossDaysConsumed { get; set; }
        public string ReasonOfReturn { get; set; }
    }
}


