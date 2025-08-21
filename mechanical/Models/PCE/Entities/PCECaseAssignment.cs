using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using mechanical.Models.Entities;

namespace mechanical.Models.PCE.Entities
{
    [Index(nameof(ProductionCapacityId))]
    public class PCECaseAssignment
    {
        [Key]
        public Guid Id { get; set; }
        public required Guid ProductionCapacityId { get; set; }
        public required Guid UserId { get; set; }
        public DateTime AssignmentDate { get; set; }
        public DateTime? CompletedAt { get; set; }
        public string Status { get; set; } = string.Empty;

        [ForeignKey("ProductionCapacityId")]
        public virtual ProductionCapacity? ProductionCapacity { get; set; }

        [ForeignKey("UserId")]
        public virtual User? User { get; set; }
    }
}
