using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace mechanical.Models.PCE.Entities
{
    [Index(nameof(ProductionCapacityId))]
    public class ProductionReestimation
    {
        [Key]
        public Guid Id { get; set; }
        public Guid ProductionCapacityId { get; set; }
        public string? Reason { get; set; }
        public DateTime CreatedAt { get; set; }

        [ForeignKey("ProductionCapacityId")]
        public virtual ProductionCapacity? ProductionCapacity { get; set; }
    }
}
