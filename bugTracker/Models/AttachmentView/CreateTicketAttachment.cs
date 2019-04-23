using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace bugTracker.Models.AttachmentView
{
    public class CreateTicketAttachment
    {
        public int Id { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public string FileUrl { get; set; }
        public int TicketId { get; set; }
        public HttpPostedFileBase Media { get; set; }
        public string CreatorId { get; set; }
        public string Creator { get; set; }
        public string TicketTitle { get; set; }
        public bool IfEdit { get; set; }
    }
}