﻿using bugTracker.Models.Domain;
using System;
using System.Collections.Generic;

namespace bugTracker.Models
{
    public class Ticket
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime Created { get; set; }
        public DateTime? Updated { get; set; }

        public int ProjectId { get; set; }
        public virtual Project Project { get; set; }

        public int TicketTypeId { get; set; }
        public virtual TicketType TicketType { get; set; }

        public int TicketPriorityId { get; set; }
        public virtual TicketPriority TicketPriority { get; set; }

        public int TicketStatusId { get; set; }
        public virtual TicketStatus TicketStatus { get; set; }

        public string AssignedToId { get; set; }
        public virtual ApplicationUser AssignedTo { get; set; }

        public string CreatedById { get; set; }
        public virtual ApplicationUser CreatedBy { get; set; }

        public virtual List<TicketComment> Comments { get; set; }
        public virtual List<TicketAttachment> Attachments { get; set; }

        public virtual List<ApplicationUser> BlockingUsers { get; set; }

        public Ticket()
        {
            Comments = new List<TicketComment>();
            Attachments = new List<TicketAttachment>();
            Created = DateTime.Now;
        }
    }
}