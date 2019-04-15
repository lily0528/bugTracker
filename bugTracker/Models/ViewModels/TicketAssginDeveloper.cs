using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace bugTracker.Models.Domain
{
    public class TicketAssginDeveloper
    {
        public string DeveloperId { get; set; }
        public List<ApplicationUser> Developers { get; set; }
        public SelectList DeveloperList { get; set; }
        public int TicketId { get; set; }
        public string TicketTitle { get; set; }
        public int ProjectId { get; set; }
        public TicketAssginDeveloper()
        {
            Developers = new List<ApplicationUser>();
        }
    }
}