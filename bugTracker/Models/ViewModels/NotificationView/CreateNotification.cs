using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace bugTracker.Models.ViewModels.Notification
{
    public class CreateNotification
    {
        public int Id { get; set; }
        public int TicketId { get; set; }
        public string UserId { get; set; }
    }
}