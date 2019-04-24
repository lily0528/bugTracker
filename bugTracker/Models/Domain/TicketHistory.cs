using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace bugTracker.Models.Domain
{
    public class TicketHistory
    {
        public int Id { get; set; }
        public string OldValue { get; set; }
        public string NewValue { get; set; }
        public string Property { get; set; }

        public virtual ApplicationUser User { get; set; }
        public virtual Ticket Ticket { get; set; }
    }
}