using System.Management;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using mechanical.Services.PCE.FileUploadService;

namespace mechanical.Controllers
{
    public class FileUploadController : Controller
    {
        private readonly IFileUploadService _fileUploadService;

        public FileUploadController (IFileUploadService fileUploadService)
        {
            _fileUploadService = fileUploadService;
        }

        [HttpGet]
        public async Task<ActionResult> GetFileByPCEEId(Guid? PCEEId)
        {
            var files = await _fileUploadService.GetFileByPCEEId(PCEEId);
            return Json(files);
        }

        public async Task<IActionResult> DownloadFile(Guid id)
        {
            return await _fileUploadService.DownloadFile(id);
        }

        public async Task<IActionResult> ViewFile(Guid id)
        {
            var Servuploadice = await _fileUploadService.ViewFile(id);
            HttpContext.Response.Headers.Add("Content-Disposition", "inline; filename=" + Servuploadice.Item2);
            return new FileContentResult(Servuploadice.Item1, Servuploadice.Item3);
        }
        
        public async Task<IActionResult> DeleteFile(Guid id)
        {   
            var result = await _fileUploadService.DeleteFile(id);
            return Json(new { success = result });
        }
    }
}
