using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace mechanical.Models.Entities
{
    [Index(nameof(UserId))]
    [Index(nameof(CollateralId))]
    public class CaseAssignment
    {
        [Key]
        public Guid Id { get; set; }
        public Guid? CollateralId { get; set; }
        public required Guid UserId { get; set; }
        public DateTime AssignmentDate { get; set; }
        public DateTime CompletionDate { get; set; }
        public string Status { get; set; } = string.Empty;


        [ForeignKey("CollateralId")]
        public virtual Collateral? Collateral { get; set; }
        [ForeignKey("UserId")]
        public virtual User? User { get; set; }
    }
}
