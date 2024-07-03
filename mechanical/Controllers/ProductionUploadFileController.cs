using mechanical.Services.ProductionUploadFileService;
using mechanical.Services.UploadFileService;
using Microsoft.AspNetCore.Mvc;

namespace mechanical.Controllers
{
    public class ProductionUploadFileController : Controller
    {
        private readonly IProductionUploadFileService _productionUploadFileService;
        public ProductionUploadFileController(IProductionUploadFileService productionUploadFileService)
        {
             _productionUploadFileService = productionUploadFileService;
        }
        [HttpGet]
        public async Task<ActionResult> GetUploadFileByProductionById(Guid? ProductionCapcityId)
        {
            var collateralFiles = await _productionUploadFileService.GetProductionUploadFile(ProductionCapcityId);
            return Json(collateralFiles);
        }
        public async Task<IActionResult> DownloadFile(Guid id)
        {
            return await _productionUploadFileService.DownloadProductionFile(id);
        }
        public async Task<IActionResult> ViewFile(Guid id)
        {
            var uploadService = await _productionUploadFileService.ViewProductionFile(id);
            HttpContext.Response.Headers.Add("Content-Disposition", "inline; filename=" + uploadService.Item2);
            return new FileContentResult(uploadService.Item1, uploadService.Item3);
        }
    }
}
