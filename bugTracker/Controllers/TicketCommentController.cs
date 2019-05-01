using AutoMapper;
using bugTracker.Models;
using bugTracker.Models.Domain;
using bugTracker.Models.ViewModels.CommentView;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;

namespace bugTracker.Controllers
{
    public class TicketCommentController : Controller
    {
        // GET: TicketComment
        private ApplicationDbContext DbContext;
        private TicketHelper TicketHelper { get; }
        private bool editStatus;

        public TicketCommentController()
        {
            DbContext = new ApplicationDbContext();
            TicketHelper = new TicketHelper(DbContext);
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
            var userId = User.Identity.GetUserId();
            var model = Mapper.Map<List<TicketCommentView>>(ticketComments);
            foreach (var ticketComment in model)
            {
                if (User.IsInRole("Admin") || User.IsInRole("Project Manager") || ((User.IsInRole("Developer") )
              || (User.IsInRole("Submitter"))&& ticketComment.CreatorId == userId))

                {
                    ticketComment.IfEdit = true;
                }
                else
                {
                    ticketComment.IfEdit = false;
                }
            }
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
        [ValidateAntiForgeryToken]
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
            if (!ModelState.IsValid)
            {
                var model = new CreateComment
                {
                    TicketId = ticket.Id,
                    TicketTitle = ticket.Title,
                    IfEdit = true
                };
                return View(model);
            }

            if ((User.Identity.IsAuthenticated && (User.IsInRole("Admin") || User.IsInRole("Project Manager")))
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

                //Send Email to developer
                if (ticket.AssignedTo != null)
                {
                    string subject = "New ticket comment was added.";
                    string body = "New ticket comment was added.";
                    TicketHelper.EmailServiceSend(id, subject, body);
                }

                return RedirectToAction("CreateComment", "TicketComment", new { id = ticketComment.TicketId });
            }
            else
            {
                return RedirectToAction("CreateComment", "TicketComment");
            }
        }

        [HttpGet]
        public ActionResult EditComment(int? id)
        {
            if (!id.HasValue)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var userId = User.Identity.GetUserId();
            var ticketComment = DbContext.TicketComments.Where(p => p.Id == id).FirstOrDefault();
            var ticket = DbContext.Tickets.FirstOrDefault(p => p.Id == ticketComment.TicketId);
            if (User.IsInRole("Admin") || User.IsInRole("Project Manager") || ((User.IsInRole("Developer"))
              || (User.IsInRole("Submitter")) && ticketComment.CreatorId == userId))
            {
                var model = new CreateComment
                {
                    TicketId = ticketComment.TicketId,
                    TicketTitle = ticket.Title,
                    Comment = ticketComment.Comment,
                    Id = ticketComment.Id,
                    IfEdit = true
                };
                return View(model);
            }
            else
            {
                return RedirectToAction("CreateComment", "TicketComment", new { id = ticketComment.TicketId });
            }
        }

        [HttpPost]
        public ActionResult EditComment(CreateComment formdata)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("CreateComment", "TicketComment", new { id = formdata.TicketId });
            }
            var ticketComment = DbContext.TicketComments.Where(p => p.Id == formdata.Id).FirstOrDefault();
            ticketComment.Comment = formdata.Comment;
            ticketComment.Updated = DateTime.Now;
            DbContext.SaveChanges();
            return RedirectToAction("CreateComment", "TicketComment", new { id = ticketComment.TicketId });
        }

        public ActionResult DeleteComment(int? id)
        {
            if (!id.HasValue)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var ticketComment = DbContext.TicketComments.Where(p => p.Id == id).FirstOrDefault();
            if (ticketComment == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var userId = User.Identity.GetUserId();
            if (User.IsInRole("Admin") || User.IsInRole("Project Manager") || ((User.IsInRole("Developer"))
              || (User.IsInRole("Submitter")) && ticketComment.CreatorId == userId))
            {
                DbContext.TicketComments.Remove(ticketComment);
                DbContext.SaveChanges();
            }
            return RedirectToAction("CreateComment", "TicketComment", new { id = ticketComment.TicketId });
        }
    }
}