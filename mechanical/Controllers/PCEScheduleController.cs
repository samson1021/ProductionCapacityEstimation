using Humanizer;
using AutoMapper;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Mvc;

using mechanical.Data;
using mechanical.Models.Dto.MailDto;
using mechanical.Services.MailService;
using mechanical.Models.PCE.Entities;
using mechanical.Services.PCE.MOPCECaseService;
using mechanical.Services.PCE.PCECaseScheduleService;
using mechanical.Models.PCE.Dto.PCECaseScheduleDto;

namespace mechanical.Controllers
{
    public class PCEScheduleController : BaseController
    {
        private readonly IMapper _mapper;
        private readonly CbeContext _cbeContext;
        private readonly IMailService _mailService;
        private readonly IMOPCECaseService _MOPCECaseService;
        private readonly IPCECaseScheduleService _PCECaseScheduleService;

        public PCEScheduleController(CbeContext cbeContext, IMapper mapper, IMOPCECaseService IMOPCECaseService, IPCECaseScheduleService PCECaseScheduleService, IMailService mailService)
        {
            _mapper = mapper;
            _cbeContext = cbeContext;
            _mailService = mailService;
            _MOPCECaseService = IMOPCECaseService;
            _PCECaseScheduleService = PCECaseScheduleService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateSchedule(PCECaseSchedulePostDto CaseSchedulePostDto)
        {
     
            var PCECaseSchedule = await _PCECaseScheduleService.CreatePCECaseSchedule(base.GetCurrentUserId(), CaseSchedulePostDto);

            if (PCECaseSchedule == null)
            {
                return BadRequest("Unable to create PCECase Schedule");
            }

            await SendScheduleEmail(PCECaseSchedule, "Valuation Schedule for PCECase Number ");
            return Ok();
        }


     
        [HttpPost]
        public async Task<IActionResult> CreateProposeSchedule(PCECaseSchedulePostDto CaseSchedulePostDto)
        {
            var caseSche = await _cbeContext.PCECaseSchedules.FindAsync(CaseSchedulePostDto.Id);
            caseSche.Status = "Rejected";
            caseSche.Reason = CaseSchedulePostDto.Reason;
            _cbeContext.Update(caseSche);
            await _cbeContext.SaveChangesAsync();
            CaseSchedulePostDto.Reason = null;
            CaseSchedulePostDto.Id = Guid.Empty;
            var PCECaseSchedule = await _PCECaseScheduleService.CreatePCECaseSchedule(base.GetCurrentUserId(), CaseSchedulePostDto);
            if (PCECaseSchedule == null) { return BadRequest("Unable to Create case Schdule"); }
            return await SendScheduleEmail(PCECaseSchedule, "Valuation Schedule for PCECase Number ");
        }



        [HttpPost]
        public async Task<IActionResult> UpdateSchedule(Guid Id, PCECaseSchedulePostDto CaseSchedulePostDto)
        {
            var caseSchedule = await _cbeContext.PCECaseSchedules.FindAsync(Id);
            if (caseSchedule == null)  { throw new Exception("case Schedule not Found"); }
            if (caseSchedule.UserId != base.GetCurrentUserId()) { throw new Exception("unauthorized user"); }
            caseSchedule.CreatedAt = DateTime.Now;
            caseSchedule.ScheduleDate = CaseSchedulePostDto.ScheduleDate;
            _cbeContext.Update(caseSchedule);
            var caseSchedules = await _cbeContext.SaveChangesAsync();
            if (caseSchedules == null)  { return BadRequest("Unable to update case Schdule"); }
            await SendScheduleEmail(_mapper.Map<PCECaseScheduleReturnDto>(caseSchedule), "Valuation Schedule for PCECase Number");
            return Ok(caseSchedule);
        }

        [HttpPost]
        public async Task<IActionResult> ApproveSchedule(Guid Id)
        {
            var caseSchedule = await _cbeContext.PCECaseSchedules.FindAsync(Id);
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



        private async Task<IActionResult> SendScheduleEmail(PCECaseScheduleReturnDto PCECaseSchedule, string subjectPrefix)
        {
            var pceCaseInfo = await _MOPCECaseService.GetPCECase(base.GetCurrentUserId(), PCECaseSchedule.PCECaseId);
            await _mailService.SendEmail(new MailPostDto
            {
                SenderEmail = "sender@cbe.com.et",
                SenderPassword = "test@1234",
                RecipantEmail = "recipient@cbe.com.et",
                Subject = $"{subjectPrefix}{pceCaseInfo.CaseNo}",
                Body = $"Dear! Valuation Schedule For Applicant: {pceCaseInfo.ApplicantName} is {PCECaseSchedule.ScheduleDate}. For further details, please check the Production Valuation System."
            });
            string jsonData = JsonConvert.SerializeObject(PCECaseSchedule);
            return Ok(PCECaseSchedule);
        }



    }
}
