using AutoMapper;
using mechanical.Data;
using mechanical.Models.Dto.CaseDto;
using mechanical.Models.Dto.CaseScheduleDto;
using mechanical.Models.Dto.MailDto;
using mechanical.Models.Dto.ProductionCaseDto;
using mechanical.Models.Dto.ProductionCaseScheduleDto;
using mechanical.Services.CaseAssignmentService;
using mechanical.Services.CaseScheduleService;
using mechanical.Services.CaseServices;
using mechanical.Services.CaseTerminateService;
using mechanical.Services.MailService;
using mechanical.Services.ProductionCaseAssignmentServices;
using mechanical.Services.ProductionCaseScheduleService;
using mechanical.Services.ProductionCaseService;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace mechanical.Controllers
{
    public class ProductionCaseController :BaseController
    {
        private readonly IProductionCaseService _productionCaseService;
        private readonly IProductionCaseAssignmentServices _productionCaseAssignmentServices;
        private readonly CbeContext _cbeContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ICaseScheduleService _caseScheduleService;
        private readonly ICaseTerminateService _caseTermnateService;
        private readonly IMailService _mailService;
        private readonly IMapper _mapper;
        private readonly ILogger<ProductionCaseController> _logger;
        private readonly IProductionCaseScheduleService _productionCaseScheduleService;
       
        public ProductionCaseController(IMapper mapper, ICaseTerminateService caseTerminateService, IProductionCaseService productionCaseService, ICaseScheduleService caseScheduleService, CbeContext cbeContext, IHttpContextAccessor httpContextAccessor, IProductionCaseAssignmentServices productionCaseAssignmentServices, IMailService mailService, IProductionCaseScheduleService productionCaseScheduleService)
        {
            _productionCaseService = productionCaseService;
            _cbeContext = cbeContext;
            _httpContextAccessor = httpContextAccessor;
          _productionCaseAssignmentServices = productionCaseAssignmentServices;
            _caseScheduleService = caseScheduleService;
            _caseTermnateService = caseTerminateService;
            _mailService = mailService;
            _mapper = mapper;
            _productionCaseScheduleService = productionCaseScheduleService;
        }

        //public IActionResult RemarkCases()
        //{
        //    return View();
        //}
        public IActionResult MyProductionAssignments()
        {
            return View();
        }
        public async Task<IActionResult> MyProductionAssignment(Guid Id)
        {
            var loanCase = await _productionCaseService.GetProductionCaseDetail(Id);
            if (loanCase == null) { return RedirectToAction("PNewCases"); }
            ViewData["case"] = loanCase;
            return View();
        }
        //[HttpGet]
        //public async Task<IActionResult> GetRemarkedCases()
        //{
        //    var myCase = await _productionCaseService.GetRmRemarkedCases(GetCurrentUserId());
        //    if (myCase == null) { return BadRequest("Unable to load case"); }
        //    string jsonData = JsonConvert.SerializeObject(myCase);
        //    return Content(jsonData, "application/json");
        //}



        [HttpGet]
        public IActionResult PNewCases()
        {
            return View();
        }
        [HttpGet]
   
        public async Task<IActionResult> GetNewCases()
        {
            var newCases = await _productionCaseService.GetNewProductionCases(base.GetCurrentUserId());
            return Content(JsonConvert.SerializeObject(newCases), "application/json");
        }
        public async Task<IActionResult> Detail(Guid id)
        {
            var loanCase = await _productionCaseService.GetProductionCase(base.GetCurrentUserId(), id);
            var caseSchedule = await _productionCaseScheduleService.GetProductionCaseSchedules(id);
            if (loanCase == null) { return RedirectToAction("PNewCases"); }
            ViewData["case"] = loanCase;
            ViewData["CaseSchedule"] = caseSchedule;
            ViewData["Id"] = base.GetCurrentUserId();
            return View();

        }
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            ViewData["EmployeeId"] = HttpContext.Session.GetString("EmployeeId") ?? null;
            return View();
        }
        [HttpPost]
      //  [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductionCasePostDto caseDto)
        {
            if (ModelState.IsValid)
            {
                var cases = await _productionCaseService.CreateProductionCase(base.GetCurrentUserId(), caseDto);
                return RedirectToAction("Detail", new { id = cases.Id });
            }
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> CreateProductionSchedule(ProductionCaseSchedulePostDto CaseSchedulePostDto)
        {
            var caseSche = await _cbeContext.ProductionCaseSchedules.FindAsync(CaseSchedulePostDto.Id);
            caseSche.Status = "Rejected";
            caseSche.Reason = CaseSchedulePostDto.Reason;
            _cbeContext.Update(caseSche);
            await _cbeContext.SaveChangesAsync();
            CaseSchedulePostDto.Reason = null;
            CaseSchedulePostDto.Id = Guid.Empty;
            var caseSchedule = await _productionCaseScheduleService.CreateProductionCaseSchedule(base.GetCurrentUserId(), CaseSchedulePostDto);
            if (caseSchedule == null) { return BadRequest("Unable to Create case Schdule"); }
            var CaseInfo = await _productionCaseService.GetProductionCaseDetail(caseSchedule.ProductionCaseId);
            await _mailService.SendEmail(new MailPostDto
            {
                SenderEmail = " getnetadane1@cbe.com.et",
                SenderPassword = "Gechlove@1234",
                RecipantEmail = "yohannessintayhu@cbe.com.et",
                Subject = "RM Proposed New Valuation Schedule for Case Number " + CaseInfo.CaseNo,
                Body = "Dear! </br> Valuation Schedule Update  For Applicant:-" + CaseInfo.ApplicantName + " Is " + caseSchedule.ScheduleDate + "</br></br> For further Detail please check Collateral Valuation System",
            });
            string jsonData = JsonConvert.SerializeObject(caseSchedule);
            return Ok(caseSchedule);
        }
        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            var loanCase = await _productionCaseService.GetProductionCase(base.GetCurrentUserId(), id);
            if (loanCase == null) { return RedirectToAction("PNewCases"); }
            return View(loanCase);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, ProductionCasePostDto caseDto)
        {
            if (ModelState.IsValid)
            {
                var cases = await _productionCaseService.EditProductionCase(base.GetCurrentUserId(), id, caseDto);
                return RedirectToAction("PNewCases");
            }
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> DeleteProductionBussinessLicence(Guid Id)
        {
            if (await _productionCaseService.DeleteProuctionBussinessLicence(Id))
            {
                return Ok();
            }
            return BadRequest();
        }
        [HttpPost]
        public async Task<ActionResult> UploadProductionBussinessLicence(IFormFile BussinessLicence, Guid caseId)
        {
            if (await _productionCaseService.UploadProductionBussinessLicence(base.GetCurrentUserId(), BussinessLicence, caseId))
            {
                return Ok();
            }
            return BadRequest();
        }
        [HttpPost]
        public async Task<IActionResult> SendForValuation(string selectedCollateralIds, string CenterId)
        {
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
    }
}
