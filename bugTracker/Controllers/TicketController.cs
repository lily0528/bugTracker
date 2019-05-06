using AutoMapper;
using bugTracker.Models;
using bugTracker.Models.Domain;
using bugTracker.Models.Filters;
using bugTracker.Models.Helpers;
using bugTracker.Models.ViewModels.TicketView;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
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
        private UserManager<ApplicationUser> UserManager;

        public TicketController()
        {
            DbContext = new ApplicationDbContext();
            TicketHelper = new TicketHelper(DbContext);
            UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(DbContext));
        }

        [Authorize(Roles = "Admin, Project Manager")]
        public ActionResult Index()
        {
            var userId = User.Identity.GetUserId();

            //need set relationship in AutoMapperConfig
            var tickets = DbContext.Tickets.Where(p => p.Project.IfArchive != true).ToList();
            var model = Mapper.Map<List<IndexTicket>>(tickets);

            return View(model);
        }

        [Authorize(Roles = "Developer,Submitter")]
        public ActionResult FromProjects()
        {
            var userId = User.Identity.GetUserId();
            var tickets = DbContext.Tickets.Where(p => (p.Project.Users.Any(u => u.Id == userId) || p.CreatedById == userId || p.AssignedToId == userId) && p.Project.IfArchive != true).ToList();
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
            var tickets = DbContext.Tickets.Where(p => (p.CreatedById == userId || p.AssignedToId == userId) && p.Project.IfArchive != true).ToList();
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
            var assignedProjetcs = DbContext.Projects.Where(p => p.Users.Any(u => u.Id == userId) && p.IfArchive != true).ToList();
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
            var user = DbContext.Users.FirstOrDefault(u => u.Id == userId);
            Ticket ticket;
            if (!id.HasValue)
            {
                var ticketStatus = DbContext.TicketStatuses.Where(p => p.Name == "Open").FirstOrDefault();
                ticket = new Ticket
                {
                    TicketStatusId = ticketStatus.Id,
                    CreatedById = User.Identity.GetUserId(),
                    Created = DateTime.Now,
                    Title = formData.Title,
                    Description = formData.Description,
                    ProjectId = formData.ProjectId,
                    TicketTypeId = formData.TicketTypeId,
                    TicketPriorityId = formData.TicketPriorityId,
                };

                // var notifyMe = (User.IsInRole("Admin") || User.IsInRole("Project Manager")) ? formData.NotifyMe : true;
                if ((User.IsInRole("Admin") || User.IsInRole("Project Manager")) && !formData.NotifyMe)
                {
                    ticket.BlockingUsers.Add(user);
                }

                DbContext.Tickets.Add(ticket);
                DbContext.SaveChanges();
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
                    if (originalValue != currentValue)
                    {
                        if (propertyName == "Title")
                        {
                            fieldName = "Title";
                        }
                        if (propertyName == "Description")
                        {
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
                            UserId = userId
                        };
                        changes.Add(ticketHistory);
                    }
                }

                if (User.IsInRole("Admin") || User.IsInRole("Project Manager"))
                {
                    if (formData.NotifyMe)
                    {
                        ticket.BlockingUsers.Remove(user);
                    }
                    else
                    {
                        ticket.BlockingUsers.Add(user);
                    }
                }

                ticket.Updated = DateTime.Now;
                DbContext.TicketHistories.AddRange(changes);
                DbContext.SaveChanges();
                string subject = "New ticket was modified to you.";
                string body = "New ticket was modified to you.";

                // subject = "..."
                // body = "..."
                //if (ticket.AssignedToId != userId)
                //{
                //    TicketHelper.SendNotification(ticket.AssignedTo.Email, subject, body);
                //}

                var roleHelper = new UserRoleHelper(DbContext);
                //var roleIds = roleHelper.GetAllRoles().Where(r => r.Name == "Admin" || r.Name == "Project Manager").Select(r => r.Id).ToList();
                //var receivers = UserManager.Users
                //    .Where(u => u.Roles.Any(r => roleIds.Contains(r.RoleId)) &&
                //                !ticket.BlockingUsers.Any(b => b.Id == userId) &&
                //                u.Id != ticket.AssignedToId).Select(x => x.Email).ToList();
                //var roleHelper = new UserRoleHelper(DbContext);
                var receivers = roleHelper.UsersInRole("Admin").Concat(roleHelper.UsersInRole("Project Manager"))
                               .Where(u => u.Id != ticket.AssignedToId && !ticket.BlockingUsers.Any(b => b.Id == u.Id));
                var emails = receivers.Select(u => u.Email).ToList();
                             emails.Add(ticket.AssignedTo.Email);
                var allEmails = string.Join(",", emails);
                TicketHelper.SendNotification(allEmails, subject, body);

                //TicketHelper.EmailServiceSend(id, subject, body);
            }

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
                // If the user is in the blocking list, NotifyMe is false
                // NotifyMe will be true if the user is not in the blocking list
                NotifyMe = !ticket.BlockingUsers.Any(u => u.Id == userId)
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
            var userId = User.Identity.GetUserId();
            //var user = DbContext.Users.FirstOrDefault(u => u.Id == userId);
            var ticket = DbContext.Tickets.FirstOrDefault(p => p.Id == id);
            //Del developer who existed in notification table
            //var NotificationDeveloper = DbContext.TicketNotifications.FirstOrDefault(p => p.UserId == ticket.AssignedToId);
            //if (NotificationDeveloper != null)
            //{
            //    DbContext.TicketNotifications.Remove(NotificationDeveloper);
            //}

            ////Add developer to notification table
            //var ticketNotification = DbContext.TicketNotifications.FirstOrDefault(p => p.UserId == formData.DeveloperId);
            //if (ticketNotification == null)
            //{
            //    var notification = new TicketNotification
            //    {
            //        TicketId = ticket.Id,
            //        UserId = formData.DeveloperId
            //    };
            //    DbContext.TicketNotifications.Add(notification);
            //}

            //Update ticket user information
            ticket.AssignedToId = formData.DeveloperId;

            var changes = new List<TicketHistory>();
            var entry = DbContext.Entry(ticket);

            foreach (var propertyName in entry.OriginalValues.PropertyNames)
            {
                var originalValue = entry.OriginalValues[propertyName]?.ToString();
                var currentValue = entry.CurrentValues[propertyName]?.ToString();
                string fieldName = null;
                if (originalValue != currentValue)
                {
                    if (propertyName == "AssignedToId")
                    {
                        
                        originalValue = DbContext.Users.FirstOrDefault(p => p.Id == ticket.AssignedToId).UserName;
                        currentValue = DbContext.Users.FirstOrDefault(p => p.Id == formData.DeveloperId).UserName;
                        fieldName = "Assigned to";
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
            DbContext.TicketHistories.AddRange(changes);
            ticket.Updated = DateTime.Now;
            DbContext.SaveChanges();

            //Send Email
            string subject = $"New ticket was assigned to {ticket.AssignedTo.UserName}.";
            string body = $"New ticket was assigned to {ticket.AssignedTo.UserName}.";

            var roleHelper = new UserRoleHelper(DbContext);
            var receivers = roleHelper.UsersInRole("Admin").Concat(roleHelper.UsersInRole("Project Manager"))
                           .Where(u => u.Id != ticket.AssignedToId && !ticket.BlockingUsers.Any(b => b.Id == u.Id));
            var emails = receivers.Select(u => u.Email).ToList();
            emails.Add(ticket.AssignedTo.Email);
            var allEmails = string.Join(",", emails);
            TicketHelper.SendNotification(allEmails, subject, body);
            return RedirectToAction(nameof(TicketController.Index));
        }
    }
}