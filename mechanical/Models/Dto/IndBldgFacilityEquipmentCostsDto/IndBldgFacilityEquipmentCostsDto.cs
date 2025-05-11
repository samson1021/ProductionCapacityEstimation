namespace mechanical.Models.Dto.IndBldgFacilityEquipmentCostsDto
{
    public class IndBldgFacilityEquipmentCostsDto
    {
        public Guid Id { get; set; }
        public Guid CaseId { get; set; }
        public double InsuranceFreightOthersCost { get; set; }
        public int CollateralCount { get; set; }
    }
}
