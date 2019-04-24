using AutoMapper;
using bugTracker.Models;
using bugTracker.Models.Domain;
using bugTracker.Models.Filters;
using bugTracker.Models.Helpers;
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
    public class TicketController : Controller
    {
        // GET: Ticket
        private ApplicationDbContext DbContext;
        //private TicketHelper TicketHelper { get; }

        public TicketController()
        {
            DbContext = new ApplicationDbContext();
            //TicketHelper = new TicketHelper(DbContext);
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
            }
            else
            {
                ticket = DbContext.Tickets.FirstOrDefault(
                p => p.Id == id);
                if (ticket == null)
                {
                    return HttpNotFound();
                }
                ticket.TicketStatusId = formData.TicketStatusId;
                ticket.Updated = DateTime.Now;
            }
            ticket.Title = formData.Title;
            ticket.Description = formData.Description;
            ticket.ProjectId = formData.ProjectId;
            ticket.TicketTypeId = formData.TicketTypeId;
            ticket.TicketPriorityId = formData.TicketPriorityId;

            DbContext.SaveChanges();
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

            ticket.AssignedToId = formData.DeveloperId;
            DbContext.SaveChanges();
            return RedirectToAction(nameof(TicketController.Index));
        }
    }
}