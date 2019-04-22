using AutoMapper;
using bugTracker.Models;
using bugTracker.Models.Domain;
using bugTracker.Models.ViewModels;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace bugTracker.Controllers
{
    public class TicketCommentController : Controller
    {
        // GET: TicketComment
        private ApplicationDbContext DbContext;
        private bool editStatus;

        public TicketCommentController()
        {
            DbContext = new ApplicationDbContext();
        }

        public ActionResult Index(int? id)
        {
            if (!id.HasValue)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var ticketComments = DbContext.TicketComments.Where(p => p.TicketId == id).ToList();
            if (ticketComments == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var model = Mapper.Map<List<TicketCommentView>>(ticketComments);
            return View(model);
        }

        [HttpGet]
        [Authorize(Roles = "Admin,Project Manager,Submitter,Developer")]
        public ActionResult CreateComment(int? id)
        {
            if (!id.HasValue)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var ticket = DbContext.Tickets.Where(p => p.Id == id).FirstOrDefault();
            if (ticket == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var userId = User.Identity.GetUserId();
            if (User.IsInRole("Admin") || User.IsInRole("Project Manager") || (User.IsInRole("Developer") && ticket.AssignedToId == userId)
              || (User.IsInRole("Submitter") && ticket.CreatedById == userId))
            {
                editStatus = true;
            }
            else
            {
                editStatus = false;
            };

            var model = new CreateComment
            {
                TicketId = ticket.Id,
                TicketTitle = ticket.Title,
                IfEdit = editStatus
            };

            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Project Manager,Submitter,Developer")]
        public ActionResult CreateComment(int? id, CreateComment formData)
        {
            if (!id.HasValue)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var userId = User.Identity.GetUserId();
            var ticket = DbContext.Tickets.FirstOrDefault(p => p.Id == id);
            if (ticket == null)
            {
                return HttpNotFound();
            };
            if ((User.Identity.IsAuthenticated && User.IsInRole("Admin") && User.IsInRole("Project Manager"))
              || (User.Identity.IsAuthenticated && User.IsInRole("Submitter") && ticket.CreatedById == userId)
              || (User.Identity.IsAuthenticated && User.IsInRole("Developer") && ticket.AssignedToId == userId))
            {
                var ticketComment = new TicketComment
                {
                    TicketId = ticket.Id,
                    CreatorId = userId,
                    Comment = formData.Comment,
                    Created = DateTime.Now,
                };
                DbContext.TicketComments.Add(ticketComment);
                DbContext.SaveChanges();
                return RedirectToAction("CreateComment", "TicketComment", new { id = ticketComment.TicketId });
            }
            else
            {
                return RedirectToAction("CreateComment", "TicketComment");
            }
        }
    }
}