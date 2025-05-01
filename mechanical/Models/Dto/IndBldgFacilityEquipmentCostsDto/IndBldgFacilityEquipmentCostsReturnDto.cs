namespace mechanical.Models.Dto.IndBldgFacilityEquipmentCostsDto
{
    public class IndBldgFacilityEquipmentCostsReturnDto
    {
        public Guid Id { get; set; }
        public Guid CaseId { get; set; }
        public double InsuranceFreightOthersCost { get; set; }
        public double DepreciatedInsuranceFreightOthersCost { get; set; }
        public double LandTransportLoadingUnloadingInstallationCommissioningCost { get; set; }
        public double DepreciatedLandTransportLoadingUnloadingInstallationCommissioningCost { get; set; }
        public int CollateralCount { get; set; }
        public int RemainingCollateral { get; set; }
    }
}
