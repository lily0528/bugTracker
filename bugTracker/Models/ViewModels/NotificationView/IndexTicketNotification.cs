using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace bugTracker.Models.ViewModels.NotificationView
{
    public class IndexTicketNotification
    {
        public int Id { get; set; }
        public int TicketId { get; set; }
        public string UserId { get; set; }
        public bool IfNotice { get; set; }
    }
}