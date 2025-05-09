using System.ComponentModel.DataAnnotations;

namespace mechanical.Models.Dto.IndBldgFacilityEquipmentCostsDto
{
    public class IndBldgFacilityEquipmentCostsReturnDto
    {
        public Guid Id { get; set; }
        public Guid CaseId { get; set; }
        [DisplayFormat(DataFormatString = "{0:N2}")]
        public double InsuranceFreightOthersCost { get; set; }
        [DisplayFormat(DataFormatString = "{0:N2}")]
        public double? DepreciatedInsuranceFreightOthersCost { get; set; }
        [DisplayFormat(DataFormatString = "{0:N2}")]
        public double? LandTransportLoadingUnloadingInstallationCommissioningCost { get; set; }
        [DisplayFormat(DataFormatString = "{0:N2}")]
        public double? DepreciatedLandTransportLoadingUnloadingInstallationCommissioningCost { get; set; }
        [DisplayFormat(DataFormatString = "{0:N2}")]
        public double? TotalReplacementCost { get; set; }
        [DisplayFormat(DataFormatString = "{0:N2}")]
        public double? TotalNetEstimationValue { get; set; }
        public int? CollateralCount { get; set; }
        public int? RemainingCollateral { get; set; }
    }
}
