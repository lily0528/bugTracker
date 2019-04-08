using bugTracker.Models;
using bugTracker.Models.Helpers;
using bugTracker.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace bugTracker.Controllers
{
    public class UserRoleController : Controller
    {
        // GET: UserRole

        private ApplicationDbContext DbContext;

        private UserRoleHelper UserRoleHelper { get; }

        public UserRoleController()
        {
            DbContext = new ApplicationDbContext();
            UserRoleHelper = new UserRoleHelper(DbContext);
        }

        [Authorize(Roles = "Admin")]
        public ActionResult Index()
        {
            var model = new List<UserRole>();
            List<ApplicationUser> users = DbContext.Users.ToList();
            foreach (var item in users)
            {
                if (item.UserName != "admin@mybugtracker.com")
                {
                    var tempModel = new UserRole
                    {
                        Id = item.Id,
                        UserName = item.UserName,
                        DisplayName = item.DisplayName,
                        Roles = UserRoleHelper.GetRoles(item.Id)
                    };
                    model.Add(tempModel);
                }
            }

            return View(model);
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public ActionResult ManageUserRole(string Id)
        {
            if (Id == null)
            {
                return RedirectToAction(nameof(UserRoleController.Index));
            }

            var UserRole = UserRoleHelper.GetUserById(Id);
            if (UserRole == null)
            {
                return RedirectToAction(nameof(UserRoleController.Index));
            }
            var model = new EditUserRole
            {
                Id = UserRole.Id,
                UserName = UserRole.UserName,
                DisplayName = UserRole.DisplayName,
                Roles = UserRoleHelper.GetRoles(UserRole.Id)
            };
            var allRoles = UserRoleHelper.GetAllRoles();
            model.RolesToAdd = allRoles.Where(r => !model.Roles.Contains(r.Name))
                .Select(r => r.Name).ToList();
            return View(model);
        }

        [Authorize(Roles = "Admin")]
        public ActionResult AddRole(string userId, string roleName)
        {
            UserRoleHelper.AddToRole(userId, roleName);
            return RedirectToAction("ManageUserRole", new { id = userId });
        }

        [Authorize(Roles = "Admin")]
        public ActionResult DeleteRole(string userId, string roleName)
        {
            UserRoleHelper.RemoveFromRole(userId, roleName);
            return RedirectToAction("ManageUserRole", new { id = userId });
          }
    }
}