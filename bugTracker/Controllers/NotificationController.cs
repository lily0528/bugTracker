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
            var managerUserId = User.Identity.GetUserId();
            var ticket = DbContext.Tickets.FirstOrDefault(p => p.Id == id);
            if(ticket == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var userNotice = DbContext.TicketNotifications.FirstOrDefault(p => p.TicketId == id && p.UserId == managerUserId);
            bool noticeValue;
            if (userNotice == null)
            {
                noticeValue = false;
            }
            else
            {
                noticeValue = true;
            }
            var model = new IndexTicketNotification
            {
                UserId = managerUserId,
                TicketId = ticket.Id,
                IfNotice = noticeValue
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
            if (!ticketId.HasValue)
            {
                return HttpNotFound();
            }

            var ticketNotification = DbContext.TicketNotifications.FirstOrDefault(p => p.TicketId == ticketId && p.UserId == managerUserId);
            if (ticketNotification == null && formdata.IfNotice == true)
            {
                var notification = new TicketNotification
                {
                    UserId = managerUserId,
                    TicketId = formdata.TicketId,
                };
                DbContext.TicketNotifications.Add(notification);
            }
            else if (ticketNotification != null && formdata.IfNotice == true)
            {
                ticketNotification.TicketId = formdata.TicketId;
                ticketNotification.UserId = managerUserId;
            }
            else if (ticketNotification != null && formdata.IfNotice == false)
            {
                DbContext.TicketNotifications.Remove(ticketNotification);
            }
            else
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DbContext.SaveChanges();

            return RedirectToAction("Index", new { id = ticketId });
            //return RedirectToAction(nameof(TicketController.Index));
        }
    }
}