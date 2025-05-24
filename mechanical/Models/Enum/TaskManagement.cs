using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace mechanical.Models.Enum
{
    public enum TaskList
    {
        [Display(Name = "All")]
        All,

        [Display(Name = "Collateral Addition")]
        CollateralAddition,

        [Display(Name = "Case Follow")]
        CaseFollow,

        [Display(Name = "Report Generation")]
        ReportGeneration,

        [Display(Name = "Revoke")]
        Revoke
    }
    public enum TaskStatus
    {
        [Display(Name = "All")]
        All,

        [Display(Name = "New")]
        New,

        [Display(Name = "Pending")]
        Pending,

        [Display(Name = "In Progress")]
        InProgress,

        [Display(Name = "Completed")]
        Completed,

        [Display(Name = "Returned")]
        Returned,
        
        [Display(Name = "Revoked")]
        Revoked,

        [Display(Name = "Overdue")]
        Overdue
    }
    public enum PriorityType
    {
        [Display(Name = "Urgent")]
        Urgent,

        [Display(Name = "High")]
        High,

        [Display(Name = "Medium")]
        Medium,

        [Display(Name = "Low")]
        Low
    }
}