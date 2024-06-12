using System.ComponentModel.DataAnnotations;

namespace mechanical.Models.Enum.CollateralAndProductionCapacityEstimationEnums
{
    public enum CollateralClass
    {
        Mechanical,
        Civil,
        Agriculture
    }

    public enum CollateralCategory
    {
        // Mechanical Categories
        MotorVehicle,
        ConstructionMachinery,
        ManufacturingBuildingFacilityEquipment,
        FuelTanks,
        HotelOrLaboratoryMechanicalEquipment,

        // Civil Categories
        ResidentialHouse,
        CondominiumHouseShop,
        ApartmentHouseShop,
        RealEstateBuilding,
        CommercialTower,
        FactoryBuilding,
        WarehouseBuilding,

        // Agriculture Categories
        CoffeeHullingOrWashingSite,
        FarmSite,
        BlockOfBuilding
    }

    public enum UnitOfMeasure
    {
        NumberOfVehicle,
        NumberOfMachinery,
        NumberOfProductionLine,
        NumberOfFuelTanks,
        NumberOfHotelOrLaboratoryMechanicalEquipment,
        NumberOfHouse,
        NumberOfFloor,
        NumberOfBuildings,
        NumberOfBlocks,
        NumberOfSite
    }  
    
    public enum FeeStatus
    {
        New,
        Pending,
        Rejected,
        Validated,
        Committed
    }
}
