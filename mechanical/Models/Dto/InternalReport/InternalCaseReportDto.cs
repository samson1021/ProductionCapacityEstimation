using DocumentFormat.OpenXml.Vml;
using Humanizer;
using mechanical.Data;
using mechanical.Models.Entities;
using Microsoft.AspNetCore.Http.HttpResults;
using PdfSharp.Diagnostics;

namespace mechanical.Models.Dto.InternalReport
{
   
    public class ValuationReportDto
    {
        public Guid Id { get; set; }
        public DateTime? CreatedAt { get; set; } // New property
        public string? DistrictName { get; set; } // Replaces IsHeadOfficeCase
        public string? CurrentStatus { get; set; }
        public string CaseNo { get; set; }
        public string? ApplicantName { get; set; }
        public string? CasePriority { get; set; }
        public string? CaseFRQ { get; set; }
        public string? RequestedOrgan { get; set; }
        public string? CustomerApplicantRelationship { get; set; }
        public string? PurposeOfValuationRequest { get; set; }
        public string? RequestedEngineer { get; set; }
        public string? ProcessingWCDays { get; set; }
        public String? QuantityForSimilarMechanicalItem { get; set; }
        //maker manager stage
        public string? NameOfValuator { get; set; }
        public string? NameOfMakerManager { get; set; }
        public string? NameOfMakerTeamLeader { get; set; }
        public DateTime? DateCaseDeliveredToValuationOffice { get; set; }
        public DateTime? DateCaseAssignedToTeamLeader { get; set; }
        public DateTime? DateCaseAssignedToValuators { get; set; }
        public DateTime? LastRecentValuationDate { get; set; }
        public string? DurationReceiptGrossDays { get; set; }
        public string? DurationAssignedToTMGrossDays { get; set; }
        public string? DurationAssignedGrossDays { get; set; }
        public string? GrossDaysConsumed { get; set; }
        public string? NetDaysConsumed { get; set; }

        //checker manager stage
        public string? NameOfCheckerManager { get; set; }
        public string? NameOfCheckerTeamLeader { get; set; }
        public string? NameOfChecker { get; set; }
        public string? DurationCheckerReceiptGrossDays { get; set; }
        public string? DurationCheckerAssignedToTMGrossDays { get; set; }
        public string? DurationCheckerAssignedGrossDays { get; set; }
        public DateTime? DateSentForChecking { get; set; }
        public DateTime? DateCaseAssignedToCheckerTeamLeader { get; set; }
        public DateTime? DateCaseAssignedToCheckerValuators { get; set; }
        public string? GrossDaysConsumedChecker { get; set; }

        //between
        public DateTime? DateCommentReceivedFromChecking { get; set; }

        //return to RM when complete and modification
        public DateTime? DateReportSentToRequestingOrgan { get; set; }
        public DateTime? DateReturnedWithAdviceToRO { get; set; }
        public string? ReasonOfReturn { get; set; }
        public int? TotalNumberOfComments { get; set; }

        public int? AssignedNo { get; set; }
        public int? DeliveredNo { get; set; }
        public int? ReturnedWithAdvice { get; set; }
        public int? OnHandNo { get; set; }
        public double? DeliveredPercentage { get; set; }
        public string? SDTAccomplishment { get; set; }
        public string? FulfillmentOfDocumentation { get; set; }
        public DateTime? ScheduledVisitDate { get; set; }
        public string? Location { get; set; }
        public string? QuantityComplexityOfProperty { get; set; }
        public string? LHCTitleDeedSerialNo { get; set; }
        public string? TypeOfProperty { get; set; }
        public string? PropertyCategory { get; set; }
        public DateTime? SiteInspectionDate { get; set; }

    }


}


