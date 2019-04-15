using bugTracker.Models.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace bugTracker.Models.ViewModels
{
    public class IndexTicket
    {
        public int Id { get; set; }
        public string Title { get; set; }

        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }

        public int ProjectId { get; set; }
        public virtual Project Project { get; set; }

        //Common implementation methods
        //public virtual TicketType TicketType { get; set; }
        //public virtual TicketPriority TicketPriority { get; set; }
        //public virtual TicketStatus TicketStatus { get; set; }

        //User automapper to do
        public string TicketTypeName { get; set; }
        public string TicketPriorityName { get; set; }
        public string TicketStatusName { get; set; }

        public string AssignedToId { get; set; }
        public virtual ApplicationUser AssignedTo { get; set; }

        public string CreatedById { get; set; }
        public virtual ApplicationUser CreatedBy { get; set; }

    }
}