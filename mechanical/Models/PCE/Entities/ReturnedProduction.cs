using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using mechanical.Models.PCE.Entities;

namespace mechanical.Models.Entities
{
    [Index(nameof(PCEId))]
    public class ReturnedProduction
    {
        [Key]
        public Guid Id { get; set; }
        public required Guid PCEId { get; set; }
        public required Guid ReturnedById { get; set; }
        public required string Reason { get; set; }
        public DateTime ReturnedAt { get; set; }

        [ForeignKey("PCEId")]
        public virtual ProductionCapacity? ProductionCapacity { get; set; }

        [ForeignKey("ReturnedById")]
        public CreateUser? ReturnedBy { get; set; }
    }
}