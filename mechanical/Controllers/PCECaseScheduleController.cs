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
using mechanical.Models.Entities;
using DocumentFormat.OpenXml.Spreadsheet;

namespace mechanical.Controllers
{
    public class PCECaseScheduleController : BaseController
    {
        private readonly IMapper _mapper;
        private readonly IMailService _mailService;
        private readonly IPCECaseService _PCECaseService;
        private readonly IPCECaseScheduleService _PCECaseScheduleService;

        public PCECaseScheduleController(IMapper mapper, IPCECaseService IPCECaseService, IPCECaseScheduleService PCECaseScheduleService, IMailService mailService)
        {
            _mapper = mapper;
            _mailService = mailService;
            _PCECaseService = IPCECaseService;
            _PCECaseScheduleService = PCECaseScheduleService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateSchedule(PCECaseSchedulePostDto pceCaseScheduleDto)
        {     
            var pceCaseSchedule = await _PCECaseScheduleService.CreateSchedule(base.GetCurrentUserId(), pceCaseScheduleDto);

            if (pceCaseSchedule == null)
            {
                return BadRequest("Unable to create PCE case Schedule");
            }

            await SendScheduleEmail(pceCaseSchedule, "Valuation Schedule for PCE case Number ");
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> ProposeSchedule(PCECaseSchedulePostDto pceCaseScheduleDto)
        {
            var pceCaseSchedule = await _PCECaseScheduleService.ProposeSchedule(base.GetCurrentUserId(), pceCaseScheduleDto);

            if (pceCaseSchedule == null)
            {
                return BadRequest("Unable to propose new PCE case Schedule");
            }

            await SendScheduleEmail(pceCaseSchedule, "Valuation Schedule for PCE case Number ");
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> CreateReschedule(PCECaseSchedulePostDto pceCaseScheduleDto)
        {              
            var pceCaseSchedule = await _PCECaseScheduleService.CreateReschedule(base.GetCurrentUserId(), pceCaseScheduleDto);

            if (pceCaseSchedule == null)
            {
                return new BadRequestObjectResult("Unable to create PCE case schedule");
            }

            await SendScheduleEmail(pceCaseSchedule, "Valuation Re-Schedule for PCE case Number ");
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> UpdateSchedule(Guid Id, PCECaseSchedulePostDto pceCaseScheduleDto)
        {          
            var pceCaseSchedule = await _PCECaseScheduleService.UpdateSchedule(base.GetCurrentUserId(), pceCaseScheduleDto);

            if (pceCaseSchedule == null)
            {
                return new BadRequestObjectResult("Unable to update PCE case schedule");
            }            

            await SendScheduleEmail(pceCaseSchedule, "Valuation Schedule for PCE case Number");
            return Ok(pceCaseSchedule);
        }

        [HttpPost]
        public async Task<IActionResult> ApproveSchedule(Guid Id)
        {  
            var pceCaseSchedule = await _PCECaseScheduleService.ApproveSchedule(base.GetCurrentUserId(), Id);

            if (pceCaseSchedule == null)
            {
                return new BadRequestObjectResult("Unable to approve PCE case schedule");
            }          

            string jsonData = JsonConvert.SerializeObject(pceCaseSchedule);
            return Ok(jsonData);
        }

        private async Task<IActionResult> SendScheduleEmail(PCECaseScheduleReturnDto PCECaseSchedule, string subjectPrefix)
        {            
            var userId = base.GetCurrentUserId();
            var pceCaseInfo = await _PCECaseService.GetPCECase(userId, PCECaseSchedule.PCECaseId);
            
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
