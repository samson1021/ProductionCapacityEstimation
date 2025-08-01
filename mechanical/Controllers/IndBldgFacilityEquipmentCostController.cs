﻿using mechanical.Models.Dto.IndBldgFacilityEquipmentCostsDto;
using mechanical.Models.Dto.IndBldgFacilityEquipmentDto;
using mechanical.Services.IndBldgFacilityEquipmentCostService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace mechanical.Controllers
{
    [Authorize(Roles = "Maker Manager,District Valuation Manager ,Maker Officer, Maker TeamLeader, Relation Manager,Checker Manager, Checker TeamLeader, Checker Officer")]
    [ApiController]
    [Route("api/[controller]")]
    public class IndustrialCollateralCostsController : ControllerBase
    {
        private readonly IIndBldgFacilityEquipmentCostService _indBldgFacilityEquipmentCostService;

        public IndustrialCollateralCostsController(IIndBldgFacilityEquipmentCostService indBldgFacilityEquipmentCostService)
        {
            _indBldgFacilityEquipmentCostService = indBldgFacilityEquipmentCostService;
        }

        [HttpPost("{caseId}")]
        public async Task<IActionResult> Create(Guid caseId, [FromBody] IndBldgFacilityEquipmentCostsPostDto indBldgFacilityEquipmentCostsPostDto)
        {
            try
            {
                var result = await _indBldgFacilityEquipmentCostService.Create(caseId, indBldgFacilityEquipmentCostsPostDto);
                return result ? Ok(new { success = true, message = "Data saved successfully" })
                             : BadRequest(new { success = false, message = "Failed to create industrial collateral cost" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(Guid id)
        {
            try
            {
                var result = await _indBldgFacilityEquipmentCostService.Get(id);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("by-case/{caseId}")]
        public async Task<IActionResult> GetByCaseId(Guid caseId)
        {
            try
            {
                var result = await _indBldgFacilityEquipmentCostService.GetByCaseId(caseId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("Edit/{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] IndBldgFacilityEquipmentCostsPostDto dto)
        {
            try
            {
                var result = await _indBldgFacilityEquipmentCostService.Update(id, dto);
                return result ? Ok(new { success = true, message = "Data updated successfully" })
                              : BadRequest(new { success = false, message = "Failed to create industrial collateral cost" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("Delete/{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                var result = await _indBldgFacilityEquipmentCostService.Delete(id);
                return result ? Ok() : BadRequest("Failed to delete industrial collateral cost");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
