using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace mechanical.Models.Entities
{
    [Index(nameof(CaseId))]
    public class IndBldgFacilityEquipmentCosts
    {
        [Key]
        public Guid Id { get; set; }
        public Guid CaseId { get; set; }
        public double InsuranceFreightOthersCost { get; set; }
        public int CollateralCount { get; set; }
        
        [ForeignKey("CaseId")]
        public virtual Case? Case { get; set; }
        public virtual ICollection<IndBldgFacilityEquipment>? IndBldgFacilityEquipments { get; set; } = new List<IndBldgFacilityEquipment>();
    }
}
