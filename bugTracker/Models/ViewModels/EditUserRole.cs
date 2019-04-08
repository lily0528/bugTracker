using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace bugTracker.Models.ViewModels
{
    public class EditUserRole
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string DisplayName { get; set; }
        public List<string> Roles { get; set; }
        public List<string> RolesToAdd { get; set; }
    }
}