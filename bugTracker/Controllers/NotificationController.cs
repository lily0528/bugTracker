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
            //if(ticket == null)
            //{
            //    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            //}
            //var userNotice = DbContext.TicketNotifications.FirstOrDefault(p => p.TicketId == id && p.UserId == managerUserId);
            //bool noticeValue;
            //if (userNotice == null)
            //{
            //    noticeValue = false;
            //}
            //else
            //{
            //    noticeValue = true;
            //}
            var model = new IndexTicketNotification
            {
                UserId = managerUserId,
                TicketId = ticket.Id,
                //IfNotice = noticeValue
                NotifyMe = !ticket.BlockingUsers.Any(u => u.Id == userId)
            };
            return View(model);
        }

        //public ActionResult Index(string responsables, bool checkResp = false)
        //{

        //}

        [Authorize(Roles = "Admin,Project Manager")]
        public ActionResult SaveNotification(int? ticketId, IndexTicketNotification formdata)
        {
            var managerUserId = User.Identity.GetUserId();
            var user = DbContext.Users.FirstOrDefault(u => u.Id == managerUserId);
            //var userId = User.Identity.GetUserId();
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
            //var ticketNotification = DbContext.TicketNotifications.FirstOrDefault(p => p.TicketId == ticketId && p.UserId == managerUserId);
            //if (ticketNotification == null && formdata.IfNotice == true)
            //{
            //var notification = new IndexTicketNotification
            //{
            //        UserId = managerUserId,
            //        TicketId = formdata.TicketId,
            //        NotifyMe = !ticket.BlockingUsers.Any(u => u.Id == managerUserId)
            //    };
            //    DbContext.TicketNotifications.Add(notification);
            //}
            //else if (ticketNotification != null && formdata.IfNotice == true)
            //{
            //    ticketNotification.TicketId = formdata.TicketId;
            //    ticketNotification.UserId = managerUserId;
            //}
            //else if (ticketNotification != null && formdata.IfNotice == false)
            //{
            //    DbContext.TicketNotifications.Remove(ticketNotification);
            //}
            //else
            //{
            //    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            //}
            DbContext.SaveChanges();

            return RedirectToAction("Index", new { id = ticketId });
            //return RedirectToAction(nameof(TicketController.Index));
        }
    }
}