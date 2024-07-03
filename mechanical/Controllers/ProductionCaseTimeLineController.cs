using mechanical.Services.ProductionCaseTimeLineService;
using Microsoft.AspNetCore.Mvc;

namespace mechanical.Controllers
{
    public class ProductionCaseTimeLineController : Controller
    {
        private readonly IProductionCaseTimeLineService _productionCaseTimeLineService;
        public ProductionCaseTimeLineController(IProductionCaseTimeLineService productionCapacityServices)
        {
            _productionCaseTimeLineService = productionCapacityServices;
            
        }
        
        public async Task<IActionResult> Index(Guid ProductionCaseId)
        {
            var productioncaseTimeline = await _productionCaseTimeLineService.GetProductionCaseTimeLines(ProductionCaseId);
            return View(productioncaseTimeline);
        }
    }
}
