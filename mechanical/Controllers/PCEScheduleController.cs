using Humanizer;
using AutoMapper;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Mvc;

using mechanical.Data;
using mechanical.Models.Dto.MailDto;
using mechanical.Services.MailService;
using mechanical.Models.PCE.Entities;
using mechanical.Services.PCE.PCECaseService;
using mechanical.Services.PCE.PCECaseScheduleService;
using mechanical.Models.PCE.Dto.PCECaseScheduleDto;

namespace mechanical.Controllers
{
    public class PCEScheduleController : BaseController
    {
        private readonly IMapper _mapper;
        private readonly CbeContext _cbeContext;
        private readonly IMailService _mailService;
        private readonly IPCECaseService _PCECaseService;
        private readonly IPCECaseScheduleService _PCECaseScheduleService;

        public PCEScheduleController(CbeContext cbeContext, IMapper mapper, IPCECaseService IPCECaseService, IPCECaseScheduleService PCECaseScheduleService, IMailService mailService)
        {
            _mapper = mapper;
            _cbeContext = cbeContext;
            _mailService = mailService;
            _PCECaseService = IPCECaseService;
            _PCECaseScheduleService = PCECaseScheduleService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateSchedule(PCECaseSchedulePostDto PCECaseScheduleDto)
        {
     
            var pceCaseSchedule = await _PCECaseScheduleService.CreatePCECaseSchedule(base.GetCurrentUserId(), PCECaseScheduleDto);

            if (pceCaseSchedule == null)
            {
                return BadRequest("Unable to create PCECase Schedule");
            }

            await SendScheduleEmail(pceCaseSchedule, "Valuation Schedule for PCECase Number ");
            return Ok();
        }


     
        [HttpPost]
        public async Task<IActionResult> CreateProposeSchedule(PCECaseSchedulePostDto PCECaseScheduleDto)
        {
            var pceCaseSchedule = await _cbeContext.PCECaseSchedules.FindAsync(PCECaseScheduleDto.Id);
            pceCaseSchedule.Status = "Rejected";
            pceCaseSchedule.Reason = PCECaseScheduleDto.Reason;

            _cbeContext.Update(pceCaseSchedule);            
            await _cbeContext.SaveChangesAsync();
            
            PCECaseScheduleDto.Reason = null;
            PCECaseScheduleDto.Id = Guid.Empty;
            
            var PCECaseSchedule = await _PCECaseScheduleService.CreatePCECaseSchedule(base.GetCurrentUserId(), PCECaseScheduleDto);
            
            if (PCECaseSchedule == null) 
            {
                return BadRequest("Unable to Create case Schdule"); 
            }

            return await SendScheduleEmail(PCECaseSchedule, "Valuation Schedule for PCECase Number ");
        }

        [HttpPost]
        public async Task<IActionResult> UpdateSchedule(Guid Id, PCECaseSchedulePostDto PCECaseScheduleDto)
        {
            var pceCaseSchedule = await _cbeContext.PCECaseSchedules.FindAsync(Id);

            if (pceCaseSchedule == null)  
            { 
                throw new Exception("case Schedule not Found"); 
            }

            if (pceCaseSchedule.UserId != base.GetCurrentUserId()) 
            { 
                throw new Exception("unauthorized user"); 
            }

            pceCaseSchedule.CreatedAt = DateTime.Now;
            pceCaseSchedule.ScheduleDate = PCECaseScheduleDto.ScheduleDate;
            _cbeContext.Update(pceCaseSchedule);

            var pceCaseSchedules = await _cbeContext.SaveChangesAsync();

            if (pceCaseSchedules == null)  
            { 
                return BadRequest("Unable to update case Schdule"); 
            }

            await SendScheduleEmail(_mapper.Map<PCECaseScheduleReturnDto>(pceCaseSchedule), "Valuation Schedule for PCECase Number");
            return Ok(pceCaseSchedule);
        }

        [HttpPost]
        public async Task<IActionResult> ApproveSchedule(Guid Id)
        {
            var pceCaseSchedule = await _cbeContext.PCECaseSchedules.FindAsync(Id);
            
            if (pceCaseSchedule == null)
            {
                throw new Exception("case Schedule not Found");
            }

            pceCaseSchedule.Status = "Approved";
            _cbeContext.Update(pceCaseSchedule);
            await _cbeContext.SaveChangesAsync();
            
            string jsonData = JsonConvert.SerializeObject(pceCaseSchedule);
            return Ok(jsonData);
        }

        private async Task<IActionResult> SendScheduleEmail(PCECaseScheduleReturnDto PCECaseSchedule, string subjectPrefix)
        {
            var pceCaseInfo = await _PCECaseService.GetPCECase(PCECaseSchedule.PCECaseId);
            
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
