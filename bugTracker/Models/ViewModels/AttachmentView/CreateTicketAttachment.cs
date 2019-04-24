using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace bugTracker.Models.ViewModels.AttachmentView
{
    public class CreateTicketAttachment
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Description is required")]
        public string Description { get; set; }
        public string FileUrl { get; set; }
        public int TicketId { get; set; }
        [Required(ErrorMessage = "Media required")]
        public HttpPostedFileBase Media { get; set; }
        public string CreatorId { get; set; }
        public string Creator { get; set; }
        public string TicketTitle { get; set; }
        public bool IfEdit { get; set; }
    }
}