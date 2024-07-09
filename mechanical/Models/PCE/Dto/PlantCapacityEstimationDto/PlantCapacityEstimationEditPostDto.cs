namespace mechanical.Models.PCE.Dto.PlantCapacityEstimationDto
{
    public class PlantCapacityEstimationEditPostDto
    {
        public required Guid CaseId { get; set; }
        public string? CollateralType { get; set; }
        public required string OwnerOfPlant { get; set; }
        public required string NamePlant { get; set; }
        public required string TradeLicenseNo { get; set; }
        public required string LHCNo { get; set; }
        public required string OwnerNameLHC { get; set; }
        public int YearOfManifacturing { get; set; }
        public required string PurposeOfPCE { get; set; }
        public required string ObsolescenceStatus { get; set; }
        public required string PlantDepreciationRate { get; set; }
        public required string PlantRegion { get; set; }
        public string? PlantCity { get; set; }
        public string? PlantZone { get; set; }
        public string? PlantSubCity { get; set; }
        public string? PlantWereda { get; set; }
        public string? PlantKebele { get; set; }


    }
}
