using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace bugTracker.Models.ViewModels
{
    public class CreateComment
    {
        public int Id { get; set; }
        [Required]
        public string Comment { get; set; }

        public DateTime Created { get; set; }
        public DateTime? Updated { get; set; }
        public int TicketId { get; set; }
        //public virtual Ticket Ticket { get; set; }
        public string CreatorId { get; set; }
        //public virtual ApplicationUser Creator { get; set; }
    }
}