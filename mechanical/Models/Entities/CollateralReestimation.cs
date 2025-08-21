using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace mechanical.Models.Entities
{
    [Index(nameof(CollateralId))]
    public class CollateralReestimation
    {
        [Key]
        public Guid Id { get; set; }
        public Guid CollateralId { get; set; }
        public string? Reason { get; set; }
        public DateTime CreatedAt { get; set; }
        
        [ForeignKey("CollateralId")]
        public virtual Collateral? Collateral { get; set; }
    }
}
