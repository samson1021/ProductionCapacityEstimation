using System.ComponentModel.DataAnnotations;

namespace mechanical.Models.Enum
{
    public enum MechanicalCollateralType
    {
        [Display(Name = "Industrial (Mfg) Machinery")]
        IndustrialMachinery,

        [Display(Name = "Industrial (Mfg.) Process Line Equipment")]
        IndustrialMfgProcessLineEquipment,

        [Display(Name = "HVAC system, Fuel Station and Security Apparatus etc")]
        HVACSystemFuelStationSecurityApparatus,

        [Display(Name = "Hotel, Office, Laboratory Equipment etc")]
        HotelOfficeLaboratoryEquipment,
       
        [Display(Name = "Building Facility Equipment")]
        BuildingFacilityEquipment ,

        [Display(Name = "Automobile")]
        Automobile,

        [Display(Name = "Bus")]
        Bus,

        [Display(Name = "Minibus")]
        Minibus,

        [Display(Name = "Truck")]
        Truck,

        [Display(Name = "Truck with trail")]
        TruckWithTrail,

        [Display(Name = "Hatchback")]
        Hatchback,

        [Display(Name = "SUV")]
        SUV,

        [Display(Name = "Minivan")]
        Minivan,

        [Display(Name = "Station Wagon")]
        StationWagon,

        [Display(Name = "Sedan")]
        Sedan,

        [Display(Name = "Coupe")]
        Coupe,

        [Display(Name = "Crossover")]
        Crossover,

        [Display(Name = "Convertible")]
        Convertible,

        [Display(Name = "Sports car")]
        SportsCar,

        [Display(Name = "Pickup")]
        Pickup,

        [Display(Name = "MUV")]
        MUV,

        [Display(Name = "Motorcycles")]
        Motorcycles,

        [Display(Name = "Compact car")]
        CompactCar,

        [Display(Name = "Limousine")]
        Limousine,

        [Display(Name = "Taxi")]
        Taxi,

        [Display(Name = "Family car")]
        FamilyCar,

        [Display(Name = "Recreational vehicle")]
        RecreationalVehicle,

        [Display(Name = "Full-size car")]
        FullSizeCar,

        [Display(Name = "Roadster")]
        Roadster,

        [Display(Name = "CUV")]
        CUV,

        [Display(Name = "Others, please specify")]
        Others, [Display(Name = "Chain Excavator")]
        ChainExcavator,

        [Display(Name = "Bull Dozer")]
        BullDozer,

        [Display(Name = "Wheel Loader")]
        WheelLoader,

        [Display(Name = "Motor Grader")]
        MotorGrader,

        [Display(Name = "Dry Cargo Truck")]
        DryCargoTruck,

        [Display(Name = "Concrete Mixer")]
        ConcreteMixer,

        [Display(Name = "Power Truck")]
        PowerTruck,

        [Display(Name = "Bulk Cement Cargo")]
        BulkCementCargo,

        [Display(Name = "Chain Loader")]
        ChainLoader,

        [Display(Name = "Sewage Cleaner Truck")]
        SewageCleanerTruck,

        [Display(Name = "Closed Cargo Truck")]
        ClosedCargoTruck,

        [Display(Name = "Draw Bar Trailer")]
        DrawBarTrailer,

        [Display(Name = "Rigid Dump Truck")]
        RigidDumpTruck,

        [Display(Name = "Concrete Pump")]
        ConcretePump,

        [Display(Name = "Fuel Cargo")]
        FuelCargo,

        [Display(Name = "Water Sprinkler")]
        WaterSprinkler,

        [Display(Name = "Tipper Dump Truck")]
        TipperDumpTruck,

        [Display(Name = "Cargo Half Crane Truck")]
        CargoHalfCraneTruck,

        [Display(Name = "Mobile Crane")]
        MobileCrane,

        [Display(Name = "Low Bed 2Axle Semi Trailer")]
        LowBed2AxleSemiTrailer,

        [Display(Name = "Low Bed 3Axle Semi Trailer")]
        LowBed3AxleSemiTrailer,

        [Display(Name = "Concert Batching Plant")]
        ConcertBatchingPlant,

        [Display(Name = "Asphalt Batching Plant")]
        AsphaltBatchingPlant,

        [Display(Name = "Drilling Rig")]
        DrillingRig,

        [Display(Name = "Compactor")]
        Compactor,

        [Display(Name = "Backhoe")]
        Backhoe,

        [Display(Name = "Wheel Tractor-Scraper")]
        WheelTractorScraper,

        [Display(Name = "Backhoe Loader")]
        BackhoeLoader,

        [Display(Name = "Skid-Steer Loader")]
        SkidSteerLoader,

        [Display(Name = "Telescopic Handler")]
        TelescopicHandler,

        [Display(Name = "Drag-line Excavator")]
        DragLineExcavator,

        [Display(Name = "Forklift")]
        Forklift,

        [Display(Name = "Factory Machinery")]
        FactoryMachinery,

        [Display(Name = "Tractor")]
        Tractor,

        [Display(Name = "Combine Harvester")]
        CombineHarvester,

        [Display(Name = "ATV or UTV")]
        ATVOrUTV,

        [Display(Name = "Tractor with Disc plow")]
        TractorWithDiscPlow,

        [Display(Name = "Tractor with Disc harrow")]
        TractorWithDiscHarrow,

        [Display(Name = "Tractor with Disc plough & Disc harrow")]
        TractorWithDiscPloughAndDiscHarrow,

        [Display(Name = "Tractor with backhoe")]
        TractorWithBackhoe,

        [Display(Name = "Tractor with loader")]
        TractorWithLoader,

        [Display(Name = "Tractor with bucket")]
        TractorWithBucket,

        [Display(Name = "Tractor with trolley/trailer")]
        TractorWithTrolleyOrTrailer,

        [Display(Name = "Tractor with seeder")]
        TractorWithSeeder,

        [Display(Name = "Tractor with Chemical spreader")]
        TractorWithChemicalSpreader,

        [Display(Name = "Tractor with cultivator")]
        TractorWithCultivator,

        [Display(Name = "Tractor with baler")]
        TractorWithBaler,

        [Display(Name = "Tractor with Rotavator")]
        TractorWithRotavator,

        [Display(Name = "Disc plow")]
        DiscPlow,

        [Display(Name = "Disc harrow")]
        DiscHarrow,

        [Display(Name = "Harvester")]
        Harvester,

        [Display(Name = "Others, please specify")]
        Other
    }
}
