using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace mechanical.Models.Entities
{
    [Index(nameof(CaseId))]
    public class CaseTimeLine
    {
        [Key]
        public Guid Id { get; set; }
        public required Guid CaseId { get; set; }
        public required Guid UserId { get; set; }
        public DateTime CreatedAt { get; set; }
        public required string Activity { get; set; }
        public required string CurrentStage { get; set; }

        [ForeignKey("CaseId")]
        public virtual Case? Case { get; set; }
        [ForeignKey("UserId")]
        public virtual User? User { get; set; }
    }
}
