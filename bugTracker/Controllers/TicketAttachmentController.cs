using AutoMapper;
using bugTracker.Models;
using bugTracker.Models.Domain;
using bugTracker.Models.Helpers;
using bugTracker.Models.ViewModels.AttachmentView;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using Constants = bugTracker.Models.Constants;

namespace bugTracker.Controllers
{
    public class TicketAttachmentController : Controller
    {
        // GET: TicketAttachment

        private ApplicationDbContext DbContext;
        private TicketHelper TicketHelper { get; }
        private bool editStatus;

        public TicketAttachmentController()
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
            var userId = User.Identity.GetUserId();
            var ticketAttachments = DbContext.TicketAttachments.Where(p => p.TicketId == id).ToList();
            var model = Mapper.Map<List<TicketAttachmentView>>(ticketAttachments);

            //Assign a status that each role can edit
            foreach (var ticketAttachment in model)
            {
                if (User.IsInRole("Admin") || User.IsInRole("Project Manager") ||
                    ((User.IsInRole("Developer") || User.IsInRole("Submitter")) && ticketAttachment.CreatorId == userId))

                {
                    ticketAttachment.IfEdit = true;
                }
                else
                {
                    ticketAttachment.IfEdit = false;
                }
            }
            return View(model);
        }

        [HttpGet]
        [Authorize(Roles = "Admin, Project Manager, Submitter, Developer")]
        public ActionResult CreateAttachment(int? id)
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
            var model = new CreateTicketAttachment
            {
                TicketId = ticket.Id,
                TicketTitle = ticket.Title,
                IfEdit = editStatus
            };
            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "Admin, Project Manager, Submitter, Developer")]
        [ValidateAntiForgeryToken]
        public ActionResult CreateAttachment(int? id, CreateTicketAttachment formData)
        {
            if (!id.HasValue)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var userId = User.Identity.GetUserId();
            var ticket = DbContext.Tickets.FirstOrDefault(p => p.Id == id);
            if (ticket == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (!ModelState.IsValid)
            {
                var model = new CreateTicketAttachment
                {
                    TicketId = ticket.Id,
                    TicketTitle = ticket.Title,
                    IfEdit = true
                };
                return View(model);
            }

            if (User.IsInRole("Admin") || User.IsInRole("Project Manager")
                 || ( User.IsInRole("Submitter") && ticket.CreatedById == userId)
                 || ( User.IsInRole("Developer") && ticket.AssignedToId == userId))
            {
                string fileExtension;
                if (formData.Media != null)
                {
                    fileExtension = Path.GetExtension(formData.Media.FileName);
                    if (!Constants.AllowedFileExtensions.Contains(fileExtension))
                    {
                        ModelState.AddModelError("", "File extension is not allowed.");
                        return View();
                    }
                }

                var ticketAttachment = new TicketAttachment
                {
                    TicketId = ticket.Id,
                    CreatorId = userId,
                    Description = formData.Description,
                    Created = DateTime.Now
                };
                if (formData.Media != null)
                {
                    if (!Directory.Exists(Constants.MappedUploadFolder))
                    {
                        Directory.CreateDirectory(Constants.MappedUploadFolder);
                    }
                    var fileName = formData.Media.FileName;
                    var fullPathWithName = Constants.MappedUploadFolder + fileName;
                    formData.Media.SaveAs(fullPathWithName);
                    ticketAttachment.FileUrl = Constants.UploadFolder + fileName;
                }

                DbContext.TicketAttachments.Add(ticketAttachment);
                DbContext.SaveChanges();

                // Send Email to developer/admin/Project manager who should receive notification.
                var roleHelper = new UserRoleHelper(DbContext);
                string subject = $"<p>New attachement [{formData.Description}] was added!</p>";
                string body = $"<p>New attachement [{formData.Description}] was added!</p>";
                var receivers = roleHelper.UsersInRole("Admin").Concat(roleHelper.UsersInRole("Project Manager"))
                               .Where(u => u.Id != ticket.AssignedToId && !ticket.BlockingUsers.Any(b => b.Id == u.Id));
                var emails = receivers.Select(u => u.Email).ToList();
                if (ticket.AssignedTo != null)
                {
                    emails.Add(ticket.AssignedTo.Email);
                }
                var allEmails = string.Join(",", emails);
                TicketHelper.SendNotification(allEmails, subject, body);
                return RedirectToAction("CreateAttachment", "TicketAttachment", new { id = ticketAttachment.TicketId });
            }
            else
            {
                return RedirectToAction("CreateAttachment", "TicketAttachment");
            }
        }

        [Authorize(Roles = "Admin, Project Manager, Submitter, Developer")]
        public ActionResult AttachmentDelete(int? id)
        {
            if (!id.HasValue)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var attachment = DbContext.TicketAttachments.Where(p => p.Id == id).FirstOrDefault();
            if (attachment == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var userId = User.Identity.GetUserId();
            if (User.IsInRole("Admin") || User.IsInRole("Project Manager") ||
                ((User.IsInRole("Developer") || User.IsInRole("Submitter")) && attachment.CreatorId == userId))
            {
                DbContext.TicketAttachments.Remove(attachment);
                DbContext.SaveChanges();
            }
            return RedirectToAction("CreateAttachment", "TicketAttachment", new { id = attachment.TicketId });
        }

    }
}