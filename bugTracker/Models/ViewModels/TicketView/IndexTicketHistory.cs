using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace bugTracker.Models.ViewModels.TicketView
{
    public class IndexTicketHistory
    {
        public int Id { get; set; }
        public string OldValue { get; set; }
        public string NewValue { get; set; }
        public string Property { get; set; }
        public DateTime Updated { get; set; }

        public string UserName { get; set; }
        public int TicketId { get; set; }
        public virtual Ticket Ticket { get; set; } 

    }
}