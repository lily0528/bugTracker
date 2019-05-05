using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace bugTracker.Models.Helpers
{
    public class UserRoleHelper
    {
        private ApplicationDbContext DbContext;
        private UserManager<ApplicationUser> UserManager { get; set; }
        private RoleManager<IdentityRole> RoleManager { get; set; }
        private List<string> Project { get; set; }

        public UserRoleHelper(ApplicationDbContext dbContext)
        {
            DbContext = dbContext;
            UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(dbContext));
            RoleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(dbContext));
           
        }

        public List<string> GetRoles(string UserId)
        {
            return UserManager.GetRoles(UserId).ToList();
        }

        public List<ApplicationUser> UsersInRole(string roleName)
        {
            // get all users in the role
            var roleId = RoleManager.FindByName(roleName).Id;
            return DbContext.Users.Where(u => u.Roles.Select(r => r.RoleId).Contains(roleId)).ToList();
        }

        public List<IdentityRole> GetAllRoles()
        {
            return RoleManager.Roles.ToList();
        }

        public ApplicationUser GetUserById(string UserId)
        {
            return UserManager.FindById(UserId);
        }

        public void AddToRole(string userId, string role)
        {
            UserManager.AddToRole(userId, role);
        }

        public void RemoveFromRole(string userId, string role)
        {
            UserManager.RemoveFromRole(userId, role);
        }

        //public void AddToProject(string userId, string id)
        //{
           
        //}

        //public void RemoveFromProject(string userId, string ID)
        //{
            
        //}
    }
}