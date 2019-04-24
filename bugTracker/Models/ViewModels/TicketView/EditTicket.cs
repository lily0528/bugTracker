using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace bugTracker.Models.ViewModels.TicketView
{
    public class EditTicket
    {
        public int Id { get; set; }
        [Required]
        [StringLength(100, MinimumLength = 4, ErrorMessage = "The {0} must be between {2} and {1} characters.")]
        public string Title { get; set; }
        [Required]
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