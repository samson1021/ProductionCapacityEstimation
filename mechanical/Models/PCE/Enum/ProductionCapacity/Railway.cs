
using System.ComponentModel.DataAnnotations;

namespace mechanical.Models.PCE.Enum.ProductionCapacity
{
    public enum Railway
    {
        [Display(Name = "Locomotive manufacturing")]
        LocomotiveManufacturing,

        [Display(Name = "Railway rolling stock manufacturing")]
        RailwayRollingStockManufacturing,

        [Display(Name = "Railway signaling and control systems manufacturing")]
        RailwaySignalingAndControlSystemsManufacturing,

        [Display(Name = "Railway track and infrastructure manufacturing")]
        RailwayTrackAndInfrastructureManufacturing,

        [Display(Name = "Railway maintenance equipment manufacturing")]
        RailwayMaintenanceEquipmentManufacturing,

        [Display(Name = "Railway electrical systems manufacturing")]
        RailwayElectricalSystemsManufacturing,

        [Display(Name = "Railway passenger and freight car manufacturing")]
        RailwayPassengerAndFreightCarManufacturing,

        [Display(Name = "Railway transportation services")]
        RailwayTransportationServices,

        [Display(Name = "Other")]
        Other
    }
}
