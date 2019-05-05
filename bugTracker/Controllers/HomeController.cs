using bugTracker.Models;
using bugTracker.Models.ViewModels;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace bugTracker.Controllers
{
    public class HomeController : Controller
    {
        private ApplicationDbContext DbContext;

        public HomeController()
        {
            DbContext = new ApplicationDbContext();
        }
        [Authorize]
        public ActionResult Index()
        {
            var userId = User.Identity.GetUserId();
            var user = DbContext.Users.Where(p => p.Id == userId).FirstOrDefault();
            var model = new DashboardView
            {
                NumberOfProjects = DbContext.Projects.Where(p =>p.IfArchive != true).Count(),
                NumberOfTickets = DbContext.Tickets.Where(p => p.Project.IfArchive != true).Count(),
                OpenOfTickets = DbContext.Tickets.Where(p => p.TicketStatus.Name == "Open" && p.Project.IfArchive != true).Count(),
                ResolvedOfTickets = DbContext.Tickets.Where(p => p.TicketStatus.Name == "Resolved" && p.Project.IfArchive != true).Count(),
                RejectedOfTickets = DbContext.Tickets.Where(p => p.TicketStatus.Name == "Rejected" && p.Project.IfArchive != true).Count(),
                AssignedOfProjects = DbContext.Projects.Where(p => p.Users.Any(m => m.Id == userId && p.IfArchive != true)).Count(),
                AssignedOfTickets = user.AssignedTickets.Where(p =>p.Project.IfArchive != true).Count(),
                CreatedOfTickets = user.CreatedTickets.Where(p => p.Project.IfArchive != true).Count()
            };
            return View(model);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}