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

        //public int TicketTypeId { get; set; }
        public virtual TicketType TicketType { get; set; }

        //public int TicketPriorityId { get; set; }
        public virtual TicketPriority TicketPriority { get; set; }

        //public int TicketStatusId { get; set; }
        public virtual TicketStatus TicketStatus { get; set; }

        public string AssignedToId { get; set; }
        public virtual ApplicationUser AssignedTo { get; set; }

        public string CreatedById { get; set; }
        public virtual ApplicationUser CreatedBy { get; set; }


        //public string ProjectName { get; set; }

        //public string TypeName { get; set; }

        //public string StatusName { get; set; }
    
        //public string PriorityName { get; set; }
    
        //public string Creator { get; set; }
     
        //public string Assignee { get; set; }
   
        //public int ProjectId { get; set; }
        //public string AssigneeId { get; set; }
        //public string CreatorId { get; set; }
    }
}