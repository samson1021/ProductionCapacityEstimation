using mechanical.Data;
using mechanical.Models.Dto.CaseDto;
using mechanical.Models.Dto.SignatureDto;
using mechanical.Models.Entities;
using mechanical.Services.CaseServices;
using mechanical.Services.SignatureService;
using mechanical.Services.UploadFileService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Newtonsoft.Json;
using NuGet.Packaging.Signing;

namespace mechanical.Controllers
{
    public class SignatureController : BaseController
    {
        private readonly ISignatureService _signature;
        private readonly IUploadFileService _uploadFileService;

        private readonly CbeContext _cbeContext;
        public SignatureController(ISignatureService signatureservice, CbeContext cbeContext, IUploadFileService uploadFileService)
        {
            _signature = signatureservice;
            _cbeContext = cbeContext;
            _uploadFileService= uploadFileService;
        }

        // GET: SignatureController
        public ActionResult Index()
        {
            return View();
        }

        // GET: SignatureController/Details/5
        public ActionResult MySignature()
        {
            return View();
        }
        public async Task<ActionResult> Details()
        {  // this is to get the signature of the logdin user by employee Id 
            var mysignature = await _signature.Getsignature(base.GetCurrentUserId().ToString());
            string jsonData = JsonConvert.SerializeObject(mysignature);
            return Content(jsonData, "application/json");

        }
        public async Task<IActionResult> DownloadFile(Guid id)
        {
            return await _uploadFileService.DownloadFile(id);
        }
        // GET: SignatureController/Create
        public ActionResult Create()
        {
            var userID = base.GetCurrentUserId();
            var UserIds=_cbeContext.Users.Where(i=>i.Id == userID).Select(i=>i.emp_ID).FirstOrDefault();
            ViewData["UserId"] = UserIds;
            return View();
        }
        public ActionResult Profile()
        {
            return View();
        }
        // POST: SignatureController/Create
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SignatureDto signatureDto)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var res = await _signature.CreateSignature(base.GetCurrentUserId(), signatureDto);
                    if (res == null)
                    {
                        return Json(new { response = false});
                    }
                    else
                    {
                        return Json(new { response = true, data = res });
                    }

                    // Handle other responses if needed
                    // Example: return Json(new { response = "success", data = res });
                }
                catch
                {
                    // Handle exception if needed
                    return Json(new { response = false });
                }
            }
            return View();
        }
        // GET: SignatureController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: SignatureController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: SignatureController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: SignatureController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        public async Task< ActionResult> getEmployeeSignature(Guid emp_ID)
        {
           var mysignature = await _signature.Getsignature(base.GetCurrentUserId().ToString());
            string jsonData = JsonConvert.SerializeObject(mysignature);
            return Content(jsonData, "application/json");
        }
        public async Task<ActionResult> getEmployeeName(string emp_ID)
        {
            var mysignature = await _signature.GetEmployeeName(emp_ID);
            string jsonData = JsonConvert.SerializeObject(mysignature);
            return Content(jsonData, "application/json");
        }
    }
}
