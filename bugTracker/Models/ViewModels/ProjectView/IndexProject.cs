using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace bugTracker.Models.ViewModels.ProjectView
{
    public class IndexProject
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public DateTime Created { get; set; }
        public DateTime? Updated { get; set; }
        public int CountOfMembers { get; set; }
        public int CountOfTickets { get; set; }
    }
}