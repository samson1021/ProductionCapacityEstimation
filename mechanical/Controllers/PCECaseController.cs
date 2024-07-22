using mechanical.Data;
using mechanical.Models.PCE.Dto.PCECaseDto;
using mechanical.Models.PCE.Entities;
using mechanical.Services.PCE.PCECaseService;
using mechanical.Services.PCE.ProductionCaseAssignmentServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace mechanical.Controllers.PCE
{
    public class PCECaseController : BaseController
    {


        private readonly CbeContext _cbeContext;
        private readonly IPCECaseService _PCECaseService;
        private readonly ILogger<PCECaseController> _logger;
        private readonly IPCECaseService _iPCECaseService;
        private readonly IProductionCaseAssignmentServices _productionCaseAssignmentServices;


        public PCECaseController(CbeContext cbeContext, IPCECaseService ipCECaseService, IProductionCaseAssignmentServices productionCaseAssignmentServices)
        {
            _cbeContext = cbeContext;
            _PCECaseService = ipCECaseService;
            _productionCaseAssignmentServices = productionCaseAssignmentServices;
        }



        [HttpGet]
        public IActionResult PCECreate()
        {
            ViewData["EmployeeId"] = HttpContext.Session.GetString("EmployeeId") ?? null;
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> PCECreate(PCECaseDto caseDto)
        {
            if (ModelState.IsValid)
            {
                var cases = await _PCECaseService.PCECase(base.GetCurrentUserId(), caseDto);
                return RedirectToAction("PCEDetail", new { id = cases.Id });
            }
            return View();
        }

        [HttpGet]
        public IActionResult PCENewCases()
        {
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> GetPCENewCases()
        {
            var newCases = await _PCECaseService.GetPCENewCases(base.GetCurrentUserId());
            return Content(JsonConvert.SerializeObject(newCases), "application/json");
        }
        

        [HttpGet]
        public IActionResult PCEPendingCases()
        {
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> GetPCEPendingCases()
        {
            var newCases = await _PCECaseService.GetPCEPendingCases(base.GetCurrentUserId());
            return Content(JsonConvert.SerializeObject(newCases), "application/json");
        }


        [HttpGet]
        public IActionResult PCECompleteCases()
        {
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> GetPCECompleteCases()
        {
            var newCases = await _PCECaseService.GetPCECompleteCases(base.GetCurrentUserId());
            return Content(JsonConvert.SerializeObject(newCases), "application/json");
        }

        ////////
        [HttpGet]
        public IActionResult PCERejectedCases()
        {
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> GetPCERejectedCases()
        {
            var newCases = await _PCECaseService.GetPCERejectedCases(base.GetCurrentUserId());
            return Content(JsonConvert.SerializeObject(newCases), "application/json");
        }

        [HttpGet]
        public async Task<IActionResult> PCERejectedDetail(Guid id)
        {
            var pcecaseDto = _PCECaseService.GetPCECase(base.GetCurrentUserId(), id);      
            ViewData["pcecaseDtos"] = pcecaseDto;
            return View();
        }
        /////



        [HttpGet]
        public IActionResult PCETotalCases()
        {
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> GetPCETotalCases()
        {
            var newCases = await _PCECaseService.GetPCETotalCases(base.GetCurrentUserId());
            return Content(JsonConvert.SerializeObject(newCases), "application/json");
        }



        [HttpGet]
        public async Task<IActionResult> GetDashboardPCECaseCount()
        {
            var myCase = await _PCECaseService.GetDashboardPCECaseCount(base.GetCurrentUserId());
            string jsonData = JsonConvert.SerializeObject(myCase);
            return Content(jsonData, "application/json");
        }

        [HttpGet]
        public async Task<IActionResult> PCEDetail(Guid id)
        {
            var pcecaseDto =  _PCECaseService.GetPCECase(base.GetCurrentUserId(), id);
            ViewData["pcecaseDtos"] = pcecaseDto;
            return View();
        } 

      
        public async Task<IActionResult> PCEEdit(Guid Id)
        {
            var editCase =  _PCECaseService.GetPCECase(base.GetCurrentUserId(), Id);
            if (editCase == null) { return RedirectToAction("PCENewCases"); }
            return View(editCase);
        }

        [HttpPost]
        public async Task<IActionResult> PCEEdit(PCECaseReturntDto caseDto)
        {


            if (ModelState.IsValid)
            {
                var cases = await _PCECaseService.PCEEdit(caseDto.Id, caseDto);
                return RedirectToAction("PCENewCases");
            }
            return View();
        }





        [HttpGet]
        public IActionResult GetByApplicantName(string applicantName)
        {
            if (applicantName!=null)
            {
                var pceCase = _cbeContext.PCECases
                .Where(c => c.ApplicantName.ToLower().Contains(applicantName.ToLower()))
                .Select(c => new
                {
                    c.Id,
                    c.CaseNo,
                    c.ApplicantName,
                    c.CustomerUserId,
                    c.CustomerEmail
                })
            .ToList();
                return Json(pceCase);

            }
            else
            {
                    var pceCases = _cbeContext.PCECases
                    .Select(c => new
                    {
                        c.Id,
                        c.CaseNo,
                        c.ApplicantName,
                        c.CustomerUserId,
                        c.CustomerEmail
                    })
                .ToList();

                 return Json(pceCases);
            }
                
        }

        // Newly Added
        [HttpPost]
        public async Task<IActionResult> SendForValuation(string selectedCollateralIds, string CenterId)
        {

            var userId = base.GetCurrentUserId();
            try
            {
                await _productionCaseAssignmentServices.SendProductionForValuation(selectedCollateralIds, CenterId);
                var response = new { message = "PCE assigned successfully" };
                return Ok(response);
            }
            catch (Exception ex)
            {
                var error = new { message = ex.Message };
                return BadRequest(error);
            }
        }



        [HttpGet]
        public async Task<IActionResult> PCEPendingDetail(Guid id)
        {
            //var loanCase = await _PCECaseService.GetCase(base.GetCurrentUserId(), id);
            //var caseSchedule = await _caseScheduleService.GetCaseSchedules(id);
            //var caseTerminate = await _caseTermnateService.GetCaseTerminates(id);
            //if (loanCase == null) { return RedirectToAction("NewCases"); }
            //ViewData["case"] = loanCase;
            //ViewData["CaseSchedule"] = caseSchedule;
            //ViewData["caseTerminate"] = caseTerminate;
            //ViewData["Id"] = base.GetCurrentUserId();
            //return View();

            var pcecaseDto = _PCECaseService.GetPCECase(base.GetCurrentUserId(), id);
            ViewData["pcecaseDtos"] = pcecaseDto;
            return View();
        }


    }
}
