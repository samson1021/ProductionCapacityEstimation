using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using mechanical.Models.Entities;

namespace mechanical.Models.PCE.Entities
{
    [Index(nameof(PCECaseId))]
    public class PCECaseTerminate
    {
        [Key]
        public Guid Id { get; set; }
        public string? Reason { get; set; }
        public string? Status { get; set; }
        public DateTime TerminatedAt { get; set; }
        public Guid PCECaseId { get; set; }
        public Guid PCECaseOriginatorId { get; set; }

        [ForeignKey("PCECaseId")]
        public virtual PCECase? PCECase { get; set; }

        [ForeignKey("PCECaseOriginatorId")]
        public virtual User? PCECaseOriginator { get; set; }
    }
}
