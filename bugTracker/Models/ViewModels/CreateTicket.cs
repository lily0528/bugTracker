using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace bugTracker.Models.ViewModels
{
	public class CreateTicket
	{
        public string Title { get; set; }
        public string Description { get; set; }
        public int ProjectId { get; set; }
        public int TypeId { get; set; }
        public int PriorityId { get; set; }
        public int StatusId { get; set; }
    }
}