using AutoMapper;
using bugTracker.Models;
using bugTracker.Models.Domain;
using bugTracker.Models.Filters;
using bugTracker.Models.Helpers;
using bugTracker.Models.ViewModels.TicketView;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;

namespace bugTracker.Controllers
{
    public class TicketController : Controller
    {
        // GET: Ticket
        private ApplicationDbContext DbContext;
        private TicketHelper TicketHelper { get; }

        public TicketController()
        {
            DbContext = new ApplicationDbContext();
            TicketHelper = new TicketHelper(DbContext);
        }

        [Authorize(Roles = "Admin, Project Manager")]
        public ActionResult Index()
        {
            var userId = User.Identity.GetUserId();

            //need set relationship in AutoMapperConfig
            var tickets = DbContext.Tickets.ToList();
            var model = Mapper.Map<List<IndexTicket>>(tickets);

            return View(model);
        }

        [Authorize(Roles = "Developer,Submitter")]
        public ActionResult FromProjects()
        {
            var userId = User.Identity.GetUserId();
            var tickets = DbContext.Tickets.Where(p => p.Project.Users.Any(u => u.Id == userId) || p.CreatedById == userId || p.AssignedToId == userId).ToList();
            var model = Mapper.Map<List<IndexTicket>>(tickets);

            //Assign a status that each role can edit
            foreach (var ticket in model)
            {
                if ((ticket.AssignedToId == userId && User.IsInRole("Developer"))
                    || (ticket.CreatedById == userId && User.IsInRole("Submitter")))
                {
                    ticket.IfEdit = true;
                }
                else
                {
                    ticket.IfEdit = false;
                }
            }
            return View(model);
        }

        [Authorize(Roles = "Developer,Submitter")]
        public ActionResult MyTickets()
        {
            var userId = User.Identity.GetUserId();
            var tickets = DbContext.Tickets.Where(p => p.CreatedById == userId || p.AssignedToId == userId).ToList();
            var model = Mapper.Map<List<IndexTicket>>(tickets);
            return View(model);
        }

        public ActionResult AssignedTickets()
        {
            return View();
        }

        [Authorize(Roles = "Admin, Project Manager, Submitter, Developer")]
        public ActionResult Details(int? id)
        {
            if (!id.HasValue)
            {
                return RedirectToAction(nameof(HomeController.Index));
            }
            var ticket = DbContext.Tickets.Where(p => p.Id == id).FirstOrDefault();
            if (ticket == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var model = Mapper.Map<IndexTicket>(ticket);
            return View(model);
        }

        [HttpGet]
        [Authorize(Roles = "Submitter")]
        public ActionResult CreateTicket()
        {
            var userId = User.Identity.GetUserId();
            var assignedProjetcs = DbContext.Projects.Where(p => p.Users.Any(u => u.Id == userId)).ToList();
            var model = new CreateTicket();
            model.Project = new SelectList(assignedProjetcs, "Id", "Name");
            model.TicketType = new SelectList(DbContext.TicketTypes.ToList(), "Id", "Name");
            model.TicketPriority = new SelectList(DbContext.TicketPriorities.ToList(), "Id", "Name");
            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "Submitter")]
        public ActionResult CreateTicket(CreateTicket formData)
        {

            return SaveTicket(null, formData);
        }

        private ActionResult SaveTicket(int? id, CreateTicket formData)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
            var userId = User.Identity.GetUserId();
            Ticket ticket;
            if (!id.HasValue)
            {
                var ticketStatus = DbContext.TicketStatuses.Where(p => p.Name == "Open").FirstOrDefault();
                ticket = new Ticket();
                ticket.TicketStatusId = ticketStatus.Id;
                ticket.CreatedById = User.Identity.GetUserId();
                ticket.Created = DateTime.Now;
                DbContext.Tickets.Add(ticket);
                ticket.Title = formData.Title;
                ticket.Description = formData.Description;
                ticket.ProjectId = formData.ProjectId;
                ticket.TicketTypeId = formData.TicketTypeId;
                ticket.TicketPriorityId = formData.TicketPriorityId;
                DbContext.SaveChanges();
                string subject = "New ticket was added to you.";
                string body = "New ticket was added  to you.";
                TicketHelper.EmailServiceSend(id, subject, body);
            }
            else
            {
                ticket = DbContext.Tickets.FirstOrDefault(
                p => p.Id == id);
                if (ticket == null)
                {
                    return HttpNotFound();
                }
                ticket.Title = formData.Title;
                ticket.Description = formData.Description;
                ticket.TicketTypeId = formData.TicketTypeId;
                ticket.TicketPriorityId = formData.TicketPriorityId;
                ticket.TicketStatusId = formData.TicketStatusId;
                ticket.ProjectId = formData.ProjectId;
                // Add Ticket History
                var changes = new List<TicketHistory>();
                var entry = DbContext.Entry(ticket);
                foreach (var propertyName in entry.OriginalValues.PropertyNames)
                {
                    var originalValue = entry.OriginalValues[propertyName]?.ToString();
                    var currentValue = entry.CurrentValues[propertyName]?.ToString();
                    string fieldName = null;
                    if (originalValue != currentValue /*&& propertyName !="Updated"*/)
                    {
                        if (propertyName == "Title")
                        {
                            //originalValue = ticket.Title;
                            //currentValue = formData.Title;
                            fieldName = "Title";
                        }
                        if (propertyName == "Description") {
                            //originalValue = ticket.Description;
                            //currentValue = formData.Description;
                            fieldName = "Description";
                       }
                        if (propertyName == "ProjectId")
                        {
                            var projectId = Convert.ToInt32(originalValue);
                            originalValue = DbContext.Projects.FirstOrDefault(p => p.Id == projectId).Name;
                            currentValue = DbContext.Projects.FirstOrDefault(p => p.Id == formData.ProjectId).Name;
                            fieldName = "Project";
                        }
                        if (propertyName == "TicketTypeId")
                        {
                            var ticketTypeId = Convert.ToInt32(originalValue);
                            originalValue = DbContext.TicketTypes.FirstOrDefault(p => p.Id == ticketTypeId).Name;
                            currentValue = DbContext.TicketTypes.FirstOrDefault(p => p.Id == formData.TicketTypeId).Name;
                            fieldName = "TicketType";
                        }
                        if (propertyName == "TicketPriorityId")
                        {
                            var ticketPriorityId = Convert.ToInt32(originalValue);
                            originalValue = DbContext.TicketPriorities.FirstOrDefault(p => p.Id == ticketPriorityId).Name;
                            currentValue = DbContext.TicketPriorities.FirstOrDefault(p => p.Id == formData.TicketPriorityId).Name;
                            fieldName = "TicketPriority";
                        }
                        if (propertyName == "TicketStatusId")
                        {
                            var ticketStatusId = Convert.ToInt32(originalValue);
                            originalValue = DbContext.TicketStatuses.FirstOrDefault(p => p.Id == ticketStatusId).Name;
                            currentValue = DbContext.TicketStatuses.FirstOrDefault(p => p.Id == formData.TicketStatusId).Name;
                            fieldName = "TicketStatus";
                        }
                        var ticketHistory = new TicketHistory
                        {
                            Updated = DateTime.Now,
                            OldValue = originalValue,
                            NewValue = currentValue,
                            Property = fieldName,
                            TicketId = ticket.Id,
                            UserId = User.Identity.GetUserId()
                        };
                        changes.Add(ticketHistory);
                    }
                }
                
                ticket.Updated = DateTime.Now;
                DbContext.TicketHistories.AddRange(changes);
                DbContext.SaveChanges();
                string subject = "New ticket was modified to you.";
                string body = "New ticket was modified to you.";
                TicketHelper.EmailServiceSend(id, subject, body);
            }
            //Email Send

            //var emailService = new EmailService();
            ////var userList = DbContext.TicketNotifications.Where(p => p.Users.Any(u => u.Id == userId)).
            //var addresses = DbContext.TicketNotifications.Where(p => p.TicketId == id).Select(m => m.User.Email).ToList();
            //var message = new MailMessage(ConfigurationManager.AppSettings["SmtpFrom"], string.Join(",", addresses.ToArray()));

            //message.Subject = "New ticket was modified to you.";
            //message.Body = "New ticket was modified to you.";
            //message.IsBodyHtml = true;
            //emailService.Send(string.Join(",", addresses.ToArray()), message.Body, message.Subject);



            if (User.IsInRole("Submitter") || User.IsInRole("Developer"))
            {
                return RedirectToAction(nameof(TicketController.FromProjects));
            }
            else
            {
                return RedirectToAction(nameof(TicketController.Index));
            }
        }

        [HttpGet]
        [Authorize(Roles = "Admin, Project Manager, Submitter, Developer")]
        public ActionResult EditTicket(int? id)
        {
            if (!id.HasValue)
            {
                return RedirectToAction(nameof(TicketController.Index));
            }
            var userId = User.Identity.GetUserId();
            var ticket = DbContext.Tickets.Where(p => p.Id == id).FirstOrDefault();
            List<Project> selectProjects;
            if (User.IsInRole("Admin") || User.IsInRole("Project Manager"))
            {
                selectProjects = DbContext.Projects.ToList();
            }
            else
            {
                selectProjects = DbContext.Projects.Where(p => p.Users.Any(u => u.Id == userId)).ToList();
            }

            var model = new EditTicket
            {
                Id = id.Value,
                Title = ticket.Title,
                Description = ticket.Description,
                Project = new SelectList(selectProjects, "Id", "Name"),
                TicketType = new SelectList(DbContext.TicketTypes, "Id", "Name"),
                TicketPriority = new SelectList(DbContext.TicketPriorities, "Id", "Name"),
                TicketStatus = new SelectList(DbContext.TicketStatuses, "Id", "Name"),
                ProjectId = ticket.ProjectId,
                TicketTypeId = ticket.TicketTypeId,
                TicketPriorityId = ticket.TicketPriorityId,
                TicketStatusId = ticket.TicketStatusId,
            };
            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "Admin, Project Manager, Submitter, Developer")]
        public ActionResult EditTicket(int? id, CreateTicket formData)
        {
            if (!id.HasValue)
            {
                return RedirectToAction(nameof(TicketController.Index));
            }
            if (!ModelState.IsValid)
            {
                return View();
            }
            return SaveTicket(id, formData);
        }

        [HttpGet]
        //[MVCFiltersAuthorization(Roles = "Admin,Project Manager")]
        [Authorize(Roles = "Admin,Project Manager")]
        public ActionResult AssignDeveloper(int id)
        {
            var ticket = DbContext.Tickets.FirstOrDefault(p => p.Id == id);
            if (ticket == null)
            {
                return RedirectToAction(nameof(TicketController.Index));
            }
            var project = DbContext.Projects.Where(p => p.Id == ticket.ProjectId).FirstOrDefault();
            var helper = new UserRoleHelper(DbContext);
            // Search developer from project when role is developer
            var projectDeveloper = project.Users.Where(u => helper.GetRoles(u.Id).Contains("Developer")).ToList();
            var model = new TicketAssginDeveloper
            {
                TicketTitle = ticket.Title,
                TicketId = ticket.Id,
                ProjectId = ticket.ProjectId,
                DeveloperList = new SelectList(projectDeveloper, "Id", "UserName")
            };
            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Project Manager")]
        public ActionResult AssignDeveloper(int? id, TicketAssginDeveloper formData)
        {
            if (!id.HasValue)
            {
                return RedirectToAction(nameof(TicketController.Index));
            }
            var ticket = DbContext.Tickets.FirstOrDefault(p => p.Id == id);
            //Update ticket user information
            ticket.AssignedToId = formData.DeveloperId;
            ticket.Updated = DateTime.Now;

            //Add developerf to notification table
            var ticketNotification = DbContext.TicketNotifications.FirstOrDefault(p => p.UserId == formData.DeveloperId);
            if (ticketNotification == null)
            {
                var notification = new TicketNotification
                {
                    TicketId = ticket.Id,
                    UserId = formData.DeveloperId
                };
                DbContext.TicketNotifications.Add(notification);
            }

            //var changes = new TicketHistory();
            var changes = new List<TicketHistory>();
            var entry = DbContext.Entry(ticket);

            foreach (var propertyName in entry.OriginalValues.PropertyNames)
            {
                var originalValue = entry.OriginalValues[propertyName]?.ToString();
                var currentValue = entry.CurrentValues[propertyName]?.ToString();

                if (originalValue != currentValue)
                {
                    var ticketHistory = new TicketHistory
                    {
                        Updated = DateTime.Now,
                        OldValue = originalValue,
                        NewValue = currentValue,
                        Property = propertyName,
                        TicketId = ticket.Id,
                        UserId = User.Identity.GetUserId()
                    };
                    changes.Add(ticketHistory);
                }
            }
         
            DbContext.TicketHistories.AddRange(changes);
            DbContext.SaveChanges();

            //Send Email
            var developer = DbContext.Users.FirstOrDefault(u => u.Id == formData.DeveloperId);
            string subject = $"New ticket was assigned to {developer.Email}.";
            string body = $"New ticket was assigned to {developer.Email}.";
            TicketHelper.EmailServiceSend(id, subject, body);
            //var emailService = new EmailService();
            //var message = new MailMessage(ConfigurationManager.AppSettings["SmtpFrom"], developer.Email);
            //message.Subject = $"New ticket was assigned to {developer.Email}.";
            //message.Body = $"New ticket was assigned to {developer.Email}.";
            //message.IsBodyHtml = true;
            ////Send the message
            //emailService.Send(developer.Email, message.Body, message.Subject);


            //var originalValues = DbContext.Entry(ticket).OriginalValues;
            //var currentValues = DbContext.Entry(ticket).CurrentValues;

            //var entry = DbContext.Entry(ticket);
            //var originalValue = entry.OriginalValues.ToString();
            //var currentValue = entry.CurrentValues.ToString();

            //if (originalValue != currentValue)
            //{
            //    var ticketHistory = new TicketHistory
            //    {
            //        Updated = DateTime.Now,
            //        OldValue = originalValue,
            //        NewValue = currentValue,
            //        Property = "AssignedToId",
            //        TicketId = ticket.Id,
            //        UserId = User.Identity.GetUserId()
            //    };
            //    DbContext.TicketHistories.Add(ticketHistory);
            //}

            //DbContext.SaveChanges();
            return RedirectToAction(nameof(TicketController.Index));
        }

        public ActionResult TicketHistory()
        {
            var userId = User.Identity.GetUserId();
            //var model = DbContext.TicketHistories.Include(t => t.Ticket).Include(t => t.User).ToList();
            //.Select(p => new IndexTicketHisttory
            //{
            //    Id = p.Id,


            //}).ToList();
            //var TicketHisttories = DbContext.TicketHistories.ToList();
            //var model = Mapper.Map<List<IndexTicketHisttory>>(TicketHisttories);
            return View();

        }
    }
}