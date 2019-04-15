using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace bugTracker.Models.ViewModels
{
    public class CreateTicketAttachment
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public string FileUrl { get; set; }
        public int TicketId { get; set; }
        public HttpPostedFileBase Media { get; set; }
        public string CreatorId { get; set; }
        public string Creator { get; set; }
    }
}