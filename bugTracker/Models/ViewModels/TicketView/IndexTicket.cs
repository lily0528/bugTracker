using bugTracker.Models.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace bugTracker.Models.ViewModels.TicketView
{
    public class IndexTicket
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }

        public DateTime Created { get; set; }
        public DateTime? Updated { get; set; }

        public int ProjectId { get; set; }
        //User automapper to do
        public string ProjectName { get; set; }
        public string TicketTypeName { get; set; }
        public string TicketPriorityName { get; set; }
        public string TicketStatusName { get; set; }
        public string AssignedName { get; set; }
        public string CreatedByName { get; set; }
        public bool IfEdit { get; set; }

        //Common implementation methods
        // public virtual Project Project { get; set; }
        //public virtual TicketType TicketType { get; set; }
        //public virtual TicketPriority TicketPriority { get; set; }
        //public virtual TicketStatus TicketStatus { get; set; }
        //public virtual ApplicationUser CreatedBy { get; set; }
        public string AssignedToId { get; set; }
        public string CreatedById { get; set; }
        //public virtual ApplicationUser AssignedTo { get; set; }
    }
}