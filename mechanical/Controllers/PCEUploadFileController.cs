using mechanical.Services.PCE.PCECollateralService;
using mechanical.Services.PCE.UploadFileService;
using Microsoft.AspNetCore.Mvc;

namespace mechanical.Controllers
{
    public class PCEUploadFileController : Controller
    {
        private readonly IPCEUploadFileService _pceuploadFileService;

        public PCEUploadFileController(IPCEUploadFileService pCEUploadFileService)
        {
            _pceuploadFileService = pCEUploadFileService;
        }




        public async Task<IActionResult> DownloadFile(Guid id)
        {
            return await _pceuploadFileService.DownloadFile(id);
        }

        public async Task<IActionResult> ViewFile(Guid id)
        {
            var uploadService = await _pceuploadFileService.ViewFile(id);
            HttpContext.Response.Headers.Add("Content-Disposition", "inline; filename=" + uploadService.Item2);
            return new FileContentResult(uploadService.Item1, uploadService.Item3);
        }
    }
}
