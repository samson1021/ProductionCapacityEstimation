using mechanical.Models.Entities;
using mechanical.Models.Dto.CaseDto;
using mechanical.Models.PCE.Dto.PCECaseDto;
using mechanical.Models.Dto.NotificationDto;

namespace mechanical.Models.ViewModels
{
    public class DashboardViewModel
    {
        public IEnumerable<CaseDto> Cases { get; set; }
        public IEnumerable<PCECaseReturnDto> PCECases { get; set; }
        public IEnumerable<NotificationReturnDto> Notifications { get; set; }
    }
}