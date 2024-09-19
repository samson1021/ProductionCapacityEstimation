using AutoMapper;
using Humanizer;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Mvc;

using mechanical.Data;
using mechanical.Models.Dto.MailDto;
using mechanical.Models.PCE.Dto.ProductionCaseScheduleDto;
using mechanical.Models.PCE.Entities;
using mechanical.Services.MailService;
using mechanical.Services.PCE.MOPCECaseService;
using mechanical.Services.PCE.PCECaseScheduleService;
using mechanical.Services.PCE.ProductionCaseScheduleService;

namespace mechanical.Controllers
{
    public class PCEScheduleController : BaseController
    {
        private readonly CbeContext _cbeContext;
        private readonly IProductionCaseScheduleService _productionCaseScheduleService;
        private readonly IMailService _mailService;
        private readonly IMOPCECaseService _MOPCECaseService;
        private readonly IMapper _mapper;

        public PCEScheduleController(CbeContext cbeContext, IMapper mapper, IMOPCECaseService IMOPCECaseService, IProductionCaseScheduleService ProductionCaseScheduleService, IMailService mailService)
        {
            _cbeContext = cbeContext;
            _mapper = mapper;
            _productionCaseScheduleService = ProductionCaseScheduleService;
            _mailService = mailService;
            _MOPCECaseService = IMOPCECaseService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateSchedule(ProductionCaseSchedulePostDto CaseSchedulePostDto)
        {
     
            var productionCaseSchedule = await _productionCaseScheduleService.CreateProductionCaseSchedule(base.GetCurrentUserId(), CaseSchedulePostDto);

            if (productionCaseSchedule == null)
            {
                return BadRequest("Unable to create PCECase Schedule");
            }

            await SendScheduleEmail(productionCaseSchedule, "Valuation Schedule for PCECase Number ");
            return Ok();
        }


     
        [HttpPost]
        public async Task<IActionResult> CreateProposeSchedule(ProductionCaseSchedulePostDto CaseSchedulePostDto)
        {
            var caseSche = await _cbeContext.ProductionCaseSchedules.FindAsync(CaseSchedulePostDto.Id);
            caseSche.Status = "Rejected";
            caseSche.Reason = CaseSchedulePostDto.Reason;
            _cbeContext.Update(caseSche);
            await _cbeContext.SaveChangesAsync();
            CaseSchedulePostDto.Reason = null;
            CaseSchedulePostDto.Id = Guid.Empty;
            var productionCaseSchedule = await _productionCaseScheduleService.CreateProductionCaseSchedule(base.GetCurrentUserId(), CaseSchedulePostDto);
            if (productionCaseSchedule == null) { return BadRequest("Unable to Create case Schdule"); }
            return await SendScheduleEmail(productionCaseSchedule, "Valuation Schedule for PCECase Number ");
        }



        [HttpPost]
        public async Task<IActionResult> UpdateSchedule(Guid Id, ProductionCaseSchedulePostDto CaseSchedulePostDto)
        {
            var caseSchedule = await _cbeContext.ProductionCaseSchedules.FindAsync(Id);
            if (caseSchedule == null)  { throw new Exception("case Schedule not Found"); }
            if (caseSchedule.UserId != base.GetCurrentUserId()) { throw new Exception("unauthorized user"); }
            caseSchedule.CreatedAt = DateTime.Now;
            caseSchedule.ScheduleDate = CaseSchedulePostDto.ScheduleDate;
            _cbeContext.Update(caseSchedule);
            var caseSchedules = await _cbeContext.SaveChangesAsync();
            if (caseSchedules == null)  { return BadRequest("Unable to update case Schdule"); }
            await SendScheduleEmail(_mapper.Map<ProductionCaseScheduleReturnDto>(caseSchedule), "Valuation Schedule for PCECase Number");
            return Ok(caseSchedule);
        }

        [HttpPost]
        public async Task<IActionResult> ApproveSchedule(Guid Id)
        {
            var caseSchedule = await _cbeContext.ProductionCaseSchedules.FindAsync(Id);
            if (caseSchedule == null)
            {
                throw new Exception("case Schedule not Found");
            }
            caseSchedule.Status = "Approved";
            _cbeContext.Update(caseSchedule);
            await _cbeContext.SaveChangesAsync();
            string jsonData = JsonConvert.SerializeObject(caseSchedule);
            return Ok(jsonData);
        }



        private async Task<IActionResult> SendScheduleEmail(ProductionCaseScheduleReturnDto productionCaseSchedule, string subjectPrefix)
        {
            var pceCaseInfo = await _MOPCECaseService.GetPCECase(base.GetCurrentUserId(), productionCaseSchedule.PCECaseId);
            await _mailService.SendEmail(new MailPostDto
            {
                SenderEmail = "sender@cbe.com.et",
                SenderPassword = "test@1234",
                RecipantEmail = "recipient@cbe.com.et",
                Subject = $"{subjectPrefix}{pceCaseInfo.CaseNo}",
                Body = $"Dear! Valuation Schedule For Applicant: {pceCaseInfo.ApplicantName} is {productionCaseSchedule.ScheduleDate}. For further details, please check the Production Valuation System."
            });
            string jsonData = JsonConvert.SerializeObject(productionCaseSchedule);
            return Ok(productionCaseSchedule);
        }



    }
}
