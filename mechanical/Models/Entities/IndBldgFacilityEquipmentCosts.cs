namespace mechanical.Models.Entities
{
    public class IndBldgFacilityEquipmentCosts
    {
        public Guid Id { get; set; }
        public Guid CaseId { get; set; }
        public double InsuranceFreightOthersCost { get; set; }
        public int CollateralCount { get; set; }
        
        public virtual Case? Case { get; set; }
        public virtual ICollection<IndBldgFacilityEquipment>? IndBldgFacilityEquipments { get; set; } = new List<IndBldgFacilityEquipment>();
    }
}
