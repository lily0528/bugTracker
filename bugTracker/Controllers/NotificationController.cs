using bugTracker.Models;
using bugTracker.Models.Domain;
using bugTracker.Models.ViewModels.Notification;
using bugTracker.Models.ViewModels.NotificationView;
using bugTracker.Models.ViewModels.TicketView;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace bugTracker.Controllers
{
    public class NotificationController : Controller
    {
        // GET: Ticket
        private ApplicationDbContext DbContext;
        public NotificationController()
        {
            DbContext = new ApplicationDbContext();
        }
        // GET: Notification
        public ActionResult Index(int? id)
        {
            if (!id.HasValue)
            {
                return HttpNotFound();
            }
            var userId = User.Identity.GetUserId();
            var managerUserId = User.Identity.GetUserId();
            var ticket = DbContext.Tickets.FirstOrDefault(p => p.Id == id);
            if (ticket == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var model = new IndexTicketNotification
            {
                UserId = managerUserId,
                TicketId = ticket.Id,
                // If the user is in the blocking list, NotifyMe is false
                // NotifyMe will be true if the user is not in the blocking list
                NotifyMe = !ticket.BlockingUsers.Any(u => u.Id == userId)
            };
            return View(model);
        }

  
        [Authorize(Roles = "Admin,Project Manager")]
        public ActionResult SaveNotification(int? ticketId, IndexTicketNotification formdata)
        {
            var managerUserId = User.Identity.GetUserId();
            var user = DbContext.Users.FirstOrDefault(u => u.Id == managerUserId);

            if (!ticketId.HasValue)
            {
                return HttpNotFound();
            }
            var ticket = DbContext.Tickets.FirstOrDefault(p => p.Id == ticketId);

            if (User.IsInRole("Admin") || User.IsInRole("Project Manager"))
            {
                if (formdata.NotifyMe)
                {
                    ticket.BlockingUsers.Remove(user);
                }
                else
                {
                    ticket.BlockingUsers.Add(user);
                }
            }
            DbContext.SaveChanges();

            return RedirectToAction("Index", new { id = ticketId });
        }
    }
}