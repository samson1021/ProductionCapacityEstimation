using DocumentFormat.OpenXml.Office.PowerPoint.Y2022.M08.Main;
using mechanical.Models.Dto.IndBldgFacilityEquipmentCostsDto;
using mechanical.Models.Dto.IndBldgFacilityEquipmentDto;

namespace mechanical.Services.IndBldgFacilityEquipmentCostService
{
    public interface IIndBldgFacilityEquipmentCostService
    {
        Task<bool> Create(Guid caseId, IndBldgFacilityEquipmentCostsPostDto indBldgFacilityEquipmentCostsPostDto);
        Task<IndBldgFacilityEquipmentCostsDto> Get(Guid id);
        Task<IEnumerable<IndBldgFacilityEquipmentCostsReturnDto>> GetByCaseId(Guid caseId);
        Task <bool> Update(Guid id, IndBldgFacilityEquipmentCostsPostDto indBldgFacilityEquipmentCostsPostDto);
        Task<bool> Delete(Guid id);
    }
}
