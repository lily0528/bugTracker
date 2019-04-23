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
                NumberOfProjects = DbContext.Projects.Count(),
                NumberOfTickets = DbContext.Tickets.Count(),
                OpenOfTickets = DbContext.Tickets.Where(p => p.TicketStatus.Name == "Open").Count(),
                ResolvedOfTickets = DbContext.Tickets.Where(p => p.TicketStatus.Name == "Resolved").Count(),
                ClosedOfTickets = DbContext.Tickets.Where(p => p.TicketStatus.Name == "Closed").Count(),
                AssignedOfProjects = DbContext.Projects.Where(p => p.Users.Any(m => m.Id == userId )).Count(),
                AssignedOfTickets = user.AssignedTickets.Count(),
                CreatedOfTickets = user.CreatedTickets.Count()
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