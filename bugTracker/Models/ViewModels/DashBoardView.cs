using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace bugTracker.Models.ViewModels
{
    public class DashboardView
    {
        public int NumberOfProjects { get; set; }
        public int NumberOfTickets { get; set; }
        public int OpenOfTickets { get; set; }
        public int ResolvedOfTickets { get; set; }
        public int RejectedOfTickets { get; set; }
        public int AssignedOfProjects { get; set; }
        public int AssignedOfTickets { get; set; }
        public int CreatedOfTickets { get; set; }
    }
}