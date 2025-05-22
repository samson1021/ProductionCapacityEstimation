using mechanical.Models.Entities;

namespace mechanical.Models.Dto.IndBldgFacilityEquipmentCostsDto
{
    public class EquipmentGrouViewModel
    {
        public IndBldgFacilityEquipmentCostsReturnDto Cost { get; set; } // Your cost type here
        public List<IndBldgFacilityEquipment>? EquipmentItems { get; set; }
    }
}
