using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace mechanical.Models.PCE.Enum.Collateral
{
    public enum CollateralClass
    {
        [Display(Name = "Mechanical")]
        MECHANICAL,

        [Display(Name = "Civil")]
        CIVIL,

        [Display(Name = "Agriculture")]
        AGRICULTURE
    }

    public enum CollateralCategory
    {
        [Display(Name = "Motor Vehicle")]
        MOTOR_VEHICLE,

        [Display(Name = "Construction Machinery")]
        CONSTRUCTION_MACHINERY,

        [Display(Name = "IND (Mfg) & Building Facility Equipment")]
        IND_MFG_BUILDING_FACILITY_EQUIPMENT,

        [Display(Name = "Residential House")]
        RESIDENTIAL_HOUSE,

        [Display(Name = "Condominium House/Shop")]
        CONDOMINIUM_HOUSE_SHOP,

        [Display(Name = "Apartment House/Shop")]
        APARTMENT_HOUSE_SHOP,

        [Display(Name = "Real Estate Building")]
        REAL_ESTATE_BUILDING,

        [Display(Name = "Commercial Tower")]
        COMMERCIAL_TOWER,

        [Display(Name = "Factory Building")]
        FACTORY_BUILDING,

        [Display(Name = "Warehouse Building")]
        WAREHOUSE_BUILDING,

        [Display(Name = "Coffee Hulling or Washing Site")]
        COFFEE_HULLING_OR_WASHING_SITE,

        [Display(Name = "Farm Site")]
        FARM_SITE,

        [Display(Name = "Block of Building")]
        BLOCK_OF_BUILDING
    }

    public enum UnitOfMeasure
    {
        [Display(Name = "No. of Vehicle")]
        NO_OF_VEHICLE,

        [Display(Name = "No. of Machinery")]
        NO_OF_MACHINERY,

        [Display(Name = "No. of Production Line")]
        NO_OF_PRODUCTION_LINE,

        [Display(Name = "No. of Fuel Tanks")]
        NO_OF_FUEL_TANKS,

        [Display(Name = "No. of Hotel or Laboratory Mechanical Equipment")]
        NO_OF_HOTEL_OR_LABORATORY_MECHANICAL_EQUIPMENT,

        [Display(Name = "No. of House")]
        NO_OF_HOUSE,

        [Display(Name = "No. of Floor")]
        NO_OF_FLOOR,

        [Display(Name = "No. of Buildings")]
        NO_OF_BUILDINGS,

        [Display(Name = "No. of Site")]
        NO_OF_SITE,

        [Display(Name = "No. of Blocks")]
        NO_OF_BLOCKS
    }

    public static class CollateralMapping
    {
        public static readonly Dictionary<CollateralClass, List<CollateralCategory>> ClassToCategoryMap = new Dictionary<CollateralClass, List<CollateralCategory>>
        {
            { CollateralClass.MECHANICAL, new List<CollateralCategory> { CollateralCategory.MOTOR_VEHICLE, CollateralCategory.CONSTRUCTION_MACHINERY, CollateralCategory.IND_MFG_BUILDING_FACILITY_EQUIPMENT } },
            { CollateralClass.CIVIL, new List<CollateralCategory> { CollateralCategory.RESIDENTIAL_HOUSE, CollateralCategory.CONDOMINIUM_HOUSE_SHOP, CollateralCategory.APARTMENT_HOUSE_SHOP, CollateralCategory.REAL_ESTATE_BUILDING, CollateralCategory.COMMERCIAL_TOWER, CollateralCategory.FACTORY_BUILDING, CollateralCategory.WAREHOUSE_BUILDING } },
            { CollateralClass.AGRICULTURE, new List<CollateralCategory> { CollateralCategory.COFFEE_HULLING_OR_WASHING_SITE, CollateralCategory.FARM_SITE, CollateralCategory.BLOCK_OF_BUILDING } }
        };

        public static readonly Dictionary<CollateralCategory, List<UnitOfMeasure>> CategoryToUnitMap = new Dictionary<CollateralCategory, List<UnitOfMeasure>>
        {
            { CollateralCategory.MOTOR_VEHICLE, new List<UnitOfMeasure> { UnitOfMeasure.NO_OF_VEHICLE } },
            { CollateralCategory.CONSTRUCTION_MACHINERY, new List<UnitOfMeasure> { UnitOfMeasure.NO_OF_MACHINERY } },
            { CollateralCategory.IND_MFG_BUILDING_FACILITY_EQUIPMENT, new List<UnitOfMeasure> { UnitOfMeasure.NO_OF_PRODUCTION_LINE, UnitOfMeasure.NO_OF_FUEL_TANKS, UnitOfMeasure.NO_OF_HOTEL_OR_LABORATORY_MECHANICAL_EQUIPMENT } },
            { CollateralCategory.RESIDENTIAL_HOUSE, new List<UnitOfMeasure> { UnitOfMeasure.NO_OF_HOUSE, UnitOfMeasure.NO_OF_FLOOR } },
            { CollateralCategory.CONDOMINIUM_HOUSE_SHOP, new List<UnitOfMeasure> { UnitOfMeasure.NO_OF_HOUSE, UnitOfMeasure.NO_OF_FLOOR } },
            { CollateralCategory.APARTMENT_HOUSE_SHOP, new List<UnitOfMeasure> { UnitOfMeasure.NO_OF_HOUSE, UnitOfMeasure.NO_OF_FLOOR } },
            { CollateralCategory.REAL_ESTATE_BUILDING, new List<UnitOfMeasure> { UnitOfMeasure.NO_OF_HOUSE, UnitOfMeasure.NO_OF_FLOOR } },
            { CollateralCategory.COMMERCIAL_TOWER, new List<UnitOfMeasure> { UnitOfMeasure.NO_OF_BUILDINGS, UnitOfMeasure.NO_OF_FLOOR } },
            { CollateralCategory.FACTORY_BUILDING, new List<UnitOfMeasure> { UnitOfMeasure.NO_OF_BUILDINGS, UnitOfMeasure.NO_OF_FLOOR } },
            { CollateralCategory.WAREHOUSE_BUILDING, new List<UnitOfMeasure> { UnitOfMeasure.NO_OF_BUILDINGS, UnitOfMeasure.NO_OF_FLOOR } },
            { CollateralCategory.COFFEE_HULLING_OR_WASHING_SITE, new List<UnitOfMeasure> { UnitOfMeasure.NO_OF_SITE } },
            { CollateralCategory.FARM_SITE, new List<UnitOfMeasure> { UnitOfMeasure.NO_OF_SITE } },
            { CollateralCategory.BLOCK_OF_BUILDING, new List<UnitOfMeasure> { UnitOfMeasure.NO_OF_BLOCKS } }
        };
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
