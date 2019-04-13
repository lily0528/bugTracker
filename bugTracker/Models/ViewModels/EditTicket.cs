using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace bugTracker.Models.ViewModels
{
    public class EditTicket
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int ProjectId { get; set; }
        public int TicketTypeId { get; set; }
        public int TicketPriorityId { get; set; }
        public int TicketStatusId { get; set; }
        
        public string CreatedById { get; set; }
        public SelectList Project { get; set; }
        public SelectList TicketType { get; set; }
        public SelectList TicketPriority { get; set; }
        public SelectList TicketStatus { get; set; }

        public DateTime Updated { get; set; }
    }
}