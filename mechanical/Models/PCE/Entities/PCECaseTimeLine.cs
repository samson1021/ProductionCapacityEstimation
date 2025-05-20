using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using mechanical.Models.Entities;

namespace mechanical.Models.PCE.Entities
{
    [Index(nameof(PCECaseId))]
    public class PCECaseTimeLine
    {
        [Key]
        public Guid Id { get; set; }
        public required Guid PCECaseId { get; set; }
        public required Guid UserId { get; set; }
        public DateTime CreatedAt { get; set; }
        public required string Activity { get; set; }
        public required string CurrentStage { get; set; }


        [ForeignKey("PCECaseId")]
        public virtual PCECase? PCECase { get; set; }

        [ForeignKey("UserId")]
        public virtual CreateUser? User { get; set; }
    }
}
