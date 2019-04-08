using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace bugTracker.Models.ViewModels
{
    public class EditMemberProject
    {
        public int ProjectId { get; set; }
        public List<string> AssignedList { get; set; }
        public List<string> NotAssignedList { get; set; }


    }
}