using bugTracker.Models;
using bugTracker.Models.Helpers;
using bugTracker.Models.ViewModels;
using bugTracker.Models.ViewModels.ProjectView;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace bugTracker.Controllers
{
    public class ProjectController : Controller
    {
        // GET: Project
        private ApplicationDbContext DbContext;

        private ProjectsHelper ProjectsHelper { get; }

        public ProjectController()
        {
            DbContext = new ApplicationDbContext();
            ProjectsHelper = new ProjectsHelper(DbContext);
        }

        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        [Authorize(Roles = "Admin,Project Manager")]
        public ActionResult ProjectList()
        {
            //var userId = User.Identity.GetUserId();
            var model = DbContext.Projects.Where(p => p.IfArchive != true)
               .Select(p => new IndexProject
               {
                   Id = p.Id,
                   Name = p.Name,
                   Created = p.Created,
                   Updated = p.Updated,
                   CountOfMembers = p.Users.Count(),
                   CountOfTickets = p.Tickets.Count()
               }).ToList();
            return View("ProjectList", model);
        }

        [HttpGet]
        [Authorize]
        public ActionResult MyProject()
        {
            var userId = User.Identity.GetUserId();
            var model = DbContext.Projects
               .Where(p => p.Users.Any(u => u.Id == userId) && p.IfArchive != true)
               .Select(p => new IndexProject
               {
                   Id = p.Id,
                   Name = p.Name,
                   Created = p.Created,
                   Updated = p.Updated,
                   CountOfMembers = p.Users.Count(),
                   CountOfTickets = p.Tickets.Count()
               }).ToList();
            return View("myProject", model);
        }

        [HttpGet]
        public ActionResult CreateProject()
        {
            return View();
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Project Manager")]
        public ActionResult CreateProject(CreateProject formData)
        {
            return SaveProject(null, formData);
        }

        private ActionResult SaveProject(int? id, CreateProject formData)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
            var userId = User.Identity.GetUserId();
            Project project;
            if (!id.HasValue)
            {
                project = new Project();
                project.Created = DateTime.Now;
                DbContext.Projects.Add(project);
            }
            else
            {
                project = ProjectsHelper.GetProjectById(id.Value);
                if (project == null)
                {
                    return HttpNotFound();
                }
                project.Updated = DateTime.Now;
            }
            project.UserId = userId;
            project.Name = formData.Name;
            project.Description = formData.Description;
            DbContext.SaveChanges();
            return RedirectToAction(nameof(ProjectController.ProjectList));
        }

        [HttpGet]
        [Authorize(Roles = "Admin,Project Manager")]
        public ActionResult EditProject(int? id)
        {
            if (!id.HasValue)
            {
                return RedirectToAction(nameof(ProjectController.ProjectList));
            }

            var project = ProjectsHelper.GetProjectById(id.Value);
            if (project == null)
            {
                return RedirectToAction(nameof(ProjectController.ProjectList));
            }
            var model = new EditProject
            {
                Name = project.Name,
                Description = project.Description,
                Updated = DateTime.Now
            };
            return View(model);
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Project Manager")]
        public ActionResult EditProject(int? id, CreateProject formData)
        {
            if (!id.HasValue)
            {
                return RedirectToAction(nameof(ProjectController.ProjectList));
            }
            if (!ModelState.IsValid)
            {
                return View();
            }
            return SaveProject(id, formData);
        }

        [HttpGet]
        [Authorize(Roles = "Admin,Project Manager")]
        public ActionResult EditMemberProject(int? id)
        {
            if (!id.HasValue)
            {
                return RedirectToAction(nameof(ProjectController.ProjectList));
            }
          
            var project = DbContext.Projects.FirstOrDefault(p => p.Id == id);

            if (project == null)
            {
                return RedirectToAction(nameof(UserRoleController.Index));
            }

            var model = new EditMemberProject
            {
                ProjectId = id.Value,
                AssignedList = project.Users.Select(p => p.UserName).ToList(),
            };
            var users = DbContext.Users.ToList();
            model.NotAssignedList = users.Where(r => !model.AssignedList.Contains(r.UserName))
                .Select(r => r.UserName).ToList();
                return View(model);
        }


        [Authorize(Roles = "Admin,Project Manager")]
        public ActionResult AddMemberToProject(string userName, int? id)
        {
            if (!id.HasValue || userName == null)
            {
                return RedirectToAction(nameof(ProjectController.EditMemberProject));
            }
            var project = DbContext.Projects.FirstOrDefault(p => p.Id == id);
            var user = DbContext.Users.FirstOrDefault(p => p.UserName == userName);
            if (project == null || user == null)
            {
                return RedirectToAction("EditMemberProject", new { id = project.Id });
            }
            project.Users.Add(user);
            DbContext.SaveChanges();
            return RedirectToAction("EditMemberProject", new {id = project.Id});
        }

        [Authorize(Roles = "Admin,Project Manager")]
        public ActionResult RemoveMemberToProject(string userName, int? id)
        {
            if (!id.HasValue || userName == null)
            {
                return RedirectToAction(nameof(ProjectController.EditMemberProject));
            }
            var project = DbContext.Projects.FirstOrDefault(p => p.Id == id);
            var user = DbContext.Users.FirstOrDefault(p => p.UserName == userName);
            if (project == null || user == null)
            {
                return RedirectToAction("EditMemberProject", new { id = project.Id });
            }
            project.Users.Remove(user);
            DbContext.SaveChanges();
            return RedirectToAction("EditMemberProject", new { id = project.Id });
        }


        [Authorize(Roles = "Admin,Project Manager")]
        public ActionResult Archive(int? id)
        {
            if (!id.HasValue)
            {
                return RedirectToAction(nameof(ProjectController.ProjectList));
            }
            var userId = User.Identity.GetUserId();
            var project = DbContext.Projects.FirstOrDefault(p => p.Id == id);
            if (User.Identity.IsAuthenticated && (User.IsInRole("Admin") || User.IsInRole("Project Manager")))
            {
                if (project.IfArchive == false || project.IfArchive == null)
                {
                    project.IfArchive = true;
                    DbContext.SaveChanges();
                }
            }
            return RedirectToAction("ProjectList", new { id = project.Id });
        }
    }
}