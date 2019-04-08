using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace bugTracker.Models.ViewModels
{
    public class UserRole
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string DisplayName { get; set; }
        public List<string> Roles { get; set; }
        //public string[] SelectedRoles { get; set; }
    }

    //public class UserRoleEditViewModel : UserRole
    //{
    //    public List<string> RolesToAdd { get; set; }
    //}
}