using AutoMapper;
using DocumentFormat.OpenXml.Spreadsheet;
using mechanical.Data;
using mechanical.Models.Dto.CaseCommentDto;
using mechanical.Models.Dto.CaseScheduleDto;
using mechanical.Models.Dto.NotificationDto;
using mechanical.Models.Entities;
using mechanical.Models.PCE.Entities;
using mechanical.Services.NotificationService;
using Microsoft.AspNetCore.Server.IIS.Core;
using Microsoft.EntityFrameworkCore;

namespace mechanical.Services.CaseScheduleService
{
    public class CaseScheduleService : ICaseScheduleService
    {
        private readonly CbeContext _cbeContext;
        private readonly IMapper _mapper;
        private readonly INotificationService _notificationService;

        public CaseScheduleService(CbeContext cbeContext, IMapper mapper, INotificationService notificationService)
        {
            _cbeContext = cbeContext;
            _mapper = mapper;
            _notificationService = notificationService;
        }

        public async Task<CaseScheduleReturnDto> ApproveCaseSchedule(Guid id)
        {
            var caseSchedule = await _cbeContext.CaseSchedules.FindAsync(id);
            if (caseSchedule == null)
            {
                throw new Exception("case Schedule not Found");
            }
            caseSchedule.Status = "Approved";
            _cbeContext.Update(caseSchedule);
            await _cbeContext.SaveChangesAsync();
            //notification
            var notificationContent = "Case Schedule was Approved";
            var notificationType = "Case Schedule";
            var link = $"/MOCase/MyCase?Id={caseSchedule.CaseId}";
            NotificationReturnDto notification = null;
            // Add Notification
            var Case = await _cbeContext.Cases.AsNoTracking().FirstOrDefaultAsync(p => p.Id == caseSchedule.CaseId);

            var user = await _cbeContext.Users.AsNoTracking().Include(u => u.Role).FirstOrDefaultAsync(u => u.Id == Case.CaseOriginatorId);
            if (user.Role.Name == "Maker Officer")
            {
                link = $"/Case/PendDetail?Id={caseSchedule.CaseId}";
                notification = await _notificationService.AddNotification(Case.CaseOriginatorId, notificationContent, notificationType, link);
            }            
            else { 
                // Find a userId with role "Maker Officer"
                var moUserId = await _cbeContext.Users
                    .AsNoTracking()
                    .Where(u => u.Role.Name == "Maker Officer")
                    .Select(u => u.Id)
                    .FirstOrDefaultAsync();
                notification = await _notificationService.AddNotification(moUserId, notificationContent, notificationType, link);

            }             // Realtime Nofication
            if (notification != null) await _notificationService.SendNotification(notification);

            return _mapper.Map<CaseScheduleReturnDto>(caseSchedule);
        }

        public async Task<CaseScheduleReturnDto> CreateCaseSchedule(Guid userId, CaseSchedulePostDto caseCommentPostDto)
        {
            var caseSchedule = _mapper.Map<CaseSchedule>(caseCommentPostDto);
            caseSchedule.UserId = userId;
            caseSchedule.CreatedAt = DateTime.UtcNow;
            caseSchedule.Status = "proposed";

            await _cbeContext.CaseSchedules.AddAsync(caseSchedule);
            await _cbeContext.SaveChangesAsync();
            //notification
            var notificationContent = "New Schedule is Proposed";
            var notificationType = "Case Schedule";
            var link = $"MOCase/MyCase?Id={caseSchedule.CaseId}";
            NotificationReturnDto notification = null;
            // Add Notification
            var Case = await _cbeContext.Cases.AsNoTracking().FirstOrDefaultAsync(p => p.Id == caseSchedule.CaseId);
            var user = await _cbeContext.Users.AsNoTracking().Include(u => u.Role).FirstOrDefaultAsync(u => u.Id == userId);
            if (user.Role.Name == "Maker Officer")
            {
                link = $"/Case/PendDetail?Id={caseSchedule.CaseId}";
                if (Case != null)
                {
                    notification = await _notificationService.AddNotification(Case.CaseOriginatorId, notificationContent, notificationType, link);
                }
            }
            else
            {
                // Find a userId with role "Maker Officer"
                var moUserId = await _cbeContext.Users
                    .AsNoTracking()
                    .Where(u => u.Role.Name == "Maker Officer" && u.Id == user.Id)
                    .Select(u => u.Id)
                    .FirstOrDefaultAsync();
                notification = await _notificationService.AddNotification(user.Id, notificationContent, notificationType, link);

            }
            // Realtime Nofication
            if (notification != null) await _notificationService.SendNotification(notification);

            return _mapper.Map<CaseScheduleReturnDto>(caseSchedule);

        }

        public async Task<IEnumerable<CaseScheduleReturnDto>> GetCaseSchedules(Guid caseId)
        {
            var caseSchedules = await _cbeContext.CaseSchedules.Include(res => res.User).Where(res => res.CaseId == caseId).OrderBy(res => res.CreatedAt).ToListAsync();
            return _mapper.Map<IEnumerable<CaseScheduleReturnDto>>(caseSchedules);
        }
        public async Task<CaseScheduleReturnDto> GetApprovedCaseSchedule(Guid caseId)
        {
            var caseSchedule = await _cbeContext.CaseSchedules.Where(res => res.CaseId == caseId && res.Status == "Approved").FirstOrDefaultAsync();
            return _mapper.Map<CaseScheduleReturnDto>(caseSchedule);
        }
        public async Task<CaseScheduleReturnDto> UpdateCaseSchedule(Guid userId, Guid Id, CaseSchedulePostDto caseCommentPostDto)
        {
            var caseSchedule = await _cbeContext.CaseSchedules.FindAsync(Id);
            if (caseSchedule == null)
            {
                throw new Exception("case Schedule not Found");
            }
            if (caseSchedule.UserId != userId)
            {
                throw new Exception("unauthorized user");
            }
            _mapper.Map(caseCommentPostDto, caseSchedule);
            caseSchedule.CreatedAt = DateTime.UtcNow;
            _cbeContext.Update(caseSchedule);
            await _cbeContext.SaveChangesAsync();
            //notification

            var notificationContent = "Case Schedule was Updated";
            var notificationType = "Case Schedule";
            var link = $"MOCase/MyCase?Id={caseSchedule.CaseId}";
            NotificationReturnDto notification = null;
            // Add Notification
            var Case = await _cbeContext.Cases.AsNoTracking().FirstOrDefaultAsync(p => p.Id == caseSchedule.CaseId);

            var user = await _cbeContext.Users.AsNoTracking().Include(u => u.Role).FirstOrDefaultAsync(u => u.Id == userId);
            if (user.Role.Name == "Maker Officer")
            {
                link = $"/Case/PendDetail?Id={caseSchedule.CaseId}";
                notification = await _notificationService.AddNotification(Case.CaseOriginatorId, notificationContent, notificationType, link);
            }
            else
            {
                // Find a userId with role "Maker Officer"
                var moUserId = await _cbeContext.Users
                    .AsNoTracking()
                    .Where(u => u.Role.Name == "Maker Officer" && u.Id == user.Id)
                    .Select(u => u.Id)
                    .FirstOrDefaultAsync();
                notification = await _notificationService.AddNotification(moUserId, notificationContent, notificationType, link);

            }
            // Realtime Nofication
            if (notification != null) await _notificationService.SendNotification(notification);

            return _mapper.Map<CaseScheduleReturnDto>(caseSchedule);
        }
    }
}
