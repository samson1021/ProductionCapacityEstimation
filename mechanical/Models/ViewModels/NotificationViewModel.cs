using System.Collections.Generic;
using mechanical.Models.Entities;

namespace mechanical.Models.ViewModels
{
    public class NotificationViewModel
    {
        public IEnumerable<Notification> Notifications { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
    }
}