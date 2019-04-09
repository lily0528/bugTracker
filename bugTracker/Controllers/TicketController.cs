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

        public ActionResult Index()
        {
            var userId = User.Identity.GetUserId();
            //var model = DbContext.Tickets.Select(p = new )
            return View();
        }

        [HttpGet]
        public ActionResult CreateTicket()
        {
            //ViewBag.ProjectId = new SelectList(DbContext.Projects, "Id", "Name");
            //ViewBag.PriorityId = new SelectList(DbContext.TicketPriorities, "Id", "Name");
            //ViewBag.StatusId = new SelectList(DbContext.TicketStatuses, "Id", "Name");
            //ViewBag.TypeId = new SelectList(DbContext.TicketTypes, "Id", "Name");
            return View();
        }

        [HttpPost]
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
                ticket = new Ticket();
                ticket.Created = DateTime.Now;
                DbContext.Tickets.Add(ticket);
            }
            else
            {
                ticket = DbContext.Tickets.FirstOrDefault(
                p => p.Id == id); ;
                if (ticket == null)
                {
                    return HttpNotFound();
                }
                ticket.Updated = DateTime.Now;
            }
            ticket.Title = formData.Title;
            ticket.Description = formData.Description;
            ticket.ProjectId = formData.ProjectId;
            ticket.TicketStatusId = formData.StatusId;
            ticket.TicketTypeId = formData.TypeId;
            ticket.TicketPriorityId = formData.PriorityId;
            DbContext.SaveChanges();
            return RedirectToAction(nameof(TicketController.Index));
        }
    }
}