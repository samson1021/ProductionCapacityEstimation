﻿using mechanical.Models.Entities;

namespace mechanical.Models.PCE.Dto.PlantCapacityEstimationDto
{
    public class PCEReturnCollateralDto
    {
        public Guid Id { get; set; }
        public required Guid CaseId { get; set; }
        public required string CollateralType { get; set; }
        public string? Region { get; set; }
        public string? City { get; set; }
        public string? Zone { get; set; }
        public string? SubCity { get; set; }
        public string? Wereda { get; set; }
        public string? Kebele { get; set; }
        public string? HouseNo { get; set; }
        public string? OwnerOfPlant { get; set; }


        public required string TradeLicenseNo { get; set; }
        public required string LHCNo { get; set; }
        public required string OwnerNameLHC { get; set; }
        public required string NamePlant { get; set; }
        // [Range(1900,DateTime())]
        public int? YearOfManifacturing { get; set; }
        public required string PurposeOfPCE { get; set; }
        public  string? ObsolescenceStatus { get; set; }
        public  string? PlantDepreciationRate { get; set; }

        //address of the plant
        public required string? PlantRegion { get; set; }
        public string? PlantCity { get; set; }
        public string? PlantZone { get; set; }
        public string? PlantSubCity { get; set; }
        public string? PlantWereda { get; set; }
        public string? PlantKebele { get; set; }

        public DateTime? DateOfInspection { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime? EndDate { get; set; }

        public string? CurrentStage { get; set; }
        public string? CurrentStatus { get; set; }

    }
}
