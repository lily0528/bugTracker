using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace bugTracker.Models.Domain
{
    public class TicketAttachment
    {
        public int Id { get; set; }
        public DateTime Created { get; set; }
        public string FileName { get; set; }
        public string FileUrl { get; set; }
        public string ContentType { get; set; }
        public int TicketId { get; set; }
        public virtual Ticket Ticket { get; set; }
        public string CreatorId { get; set; }
        public virtual ApplicationUser Creator { get; set; }

        public TicketAttachment()
        {
            Created = DateTime.Now;
        }
    }
}