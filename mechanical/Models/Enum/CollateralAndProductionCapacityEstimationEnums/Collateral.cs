using System.ComponentModel.DataAnnotations;

namespace mechanical.Models.Enum.CollateralAndProductionCapacityEstimationEnums.Collateral
{
    public enum CollateralClass
    {
        [Display(Name = "Mechanical")]
        Mechanical,

        [Display(Name = "Civil")]
        Civil,

        [Display(Name = "Agriculture")]
        Agriculture
    }

    public enum CollateralCategory
    {
        // Mechanical Categories
        [Display(Name = "Motor Vehicle")]
        MotorVehicle,

        [Display(Name = "Construction Machinery")]
        ConstructionMachinery,

        [Display(Name = "Manufacturing Building Facility Equipment")]
        ManufacturingBuildingFacilityEquipment,

        [Display(Name = "Fuel Tanks")]
        FuelTanks,

        [Display(Name = "Hotel or Laboratory Mechanical Equipment")]
        HotelOrLaboratoryMechanicalEquipment,

        // Civil Categories
        [Display(Name = "Residential House")]
        ResidentialHouse,

        [Display(Name = "Condominium House/Shop")]
        CondominiumHouseShop,

        [Display(Name = "Apartment House/Shop")]
        ApartmentHouseShop,

        [Display(Name = "Real Estate Building")]
        RealEstateBuilding,

        [Display(Name = "Commercial Tower")]
        CommercialTower,

        [Display(Name = "Factory Building")]
        FactoryBuilding,

        [Display(Name = "Warehouse Building")]
        WarehouseBuilding,

        // Agriculture Categories
        [Display(Name = "Coffee Hulling or Washing Site")]
        CoffeeHullingOrWashingSite,

        [Display(Name = "Farm Site")]
        FarmSite,

        [Display(Name = "Block of Building")]
        BlockOfBuilding
    }

    public enum UnitOfMeasure
    {
        [Display(Name = "Number of Vehicles")]
        NumberOfVehicle,

        [Display(Name = "Number of Machinery")]
        NumberOfMachinery,

        [Display(Name = "Number of Production Lines")]
        NumberOfProductionLine,

        [Display(Name = "Number of Fuel Tanks")]
        NumberOfFuelTanks,

        [Display(Name = "Number of Hotel or Laboratory Mechanical Equipment")]
        NumberOfHotelOrLaboratoryMechanicalEquipment,

        [Display(Name = "Number of Houses")]
        NumberOfHouse,

        [Display(Name = "Number of Floors")]
        NumberOfFloor,

        [Display(Name = "Number of Buildings")]
        NumberOfBuildings,

        [Display(Name = "Number of Blocks")]
        NumberOfBlocks,

        [Display(Name = "Number of Sites")]
        NumberOfSite
    }  
    
    public enum FeeStatus
    {
        [Display(Name = "New")]
        New,

        [Display(Name = "Pending")]
        Pending,

        [Display(Name = "Rejected")]
        Rejected,

        [Display(Name = "Validated")]
        Validated,

        [Display(Name = "Committed")]
        Committed
    }
}
