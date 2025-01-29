using mechanical.Services.UploadFileService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Management;

namespace mechanical.Controllers
{
    public class UploadFileController : Controller
    {
        private readonly IUploadFileService _uploadFileService;
        public UploadFileController (IUploadFileService uploadFileService)
        {
            _uploadFileService = uploadFileService;
        }
        [HttpGet]
        public async Task<ActionResult> GetUploadFileByCollateralId(Guid? CollateralId)
        {
            var collateralFiles = await _uploadFileService.GetUploadFileByCollateralId(CollateralId);
            return Json(collateralFiles);
        }
        public async Task<IActionResult> DownloadFile(Guid id)
        {
            return await _uploadFileService.DownloadFile(id);
        }
        public async Task<IActionResult> ViewFile(Guid id)
        {
            var uploadService = await _uploadFileService.ViewFile(id);
            HttpContext.Response.Headers.Add("Content-Disposition", "inline; filename=" + uploadService.Item2);
            return new FileContentResult(uploadService.Item1, uploadService.Item3);
        }
        public async Task<IActionResult> DeleteFile(Guid id)
        {   
            var result = await _uploadFileService.DeleteFile(id);
            return Json(new { success = result });
        }
      
    }
}
