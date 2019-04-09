using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace bugTracker.Models.Domain
{
    public class TicketPriority
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public virtual List<Ticket> Tickets { get; set; }
        public TicketPriority()
        {
            Tickets = new List<Ticket>();
        }
    }
}