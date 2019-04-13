using System.Collections.Generic;

namespace bugTracker.Models
{
    public class TicketStatus
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public virtual List<Ticket> Tickets { get; set; }

        //public TicketStatus()
        //{
        //    Tickets = new List<Ticket>();
        //}
    }
}