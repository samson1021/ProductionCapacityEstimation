using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using mechanical.Models.Entities;

namespace mechanical.Models.Dto.NotificationDto
{
    public class NotificationResultDto
    {
        public IEnumerable<Notification> Notifications { get; set; }
        public int TotalCount { get; set; }
        public int UnreadCount { get; set; }
        public int UnseenCount { get; set; }
    }
}
