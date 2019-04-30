using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace bugTracker.Models.Domain
{
    public class TicketNotification
    {
        public int Id { get; set; }
        public int TicketId { get; set; }
        public string UserId { get; set; }
        public virtual ApplicationUser User { get; set; }

    }
}