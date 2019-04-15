using AutoMapper;
using bugTracker.Models;
using bugTracker.Models.Domain;
using bugTracker.Models.ViewModels;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Constants = bugTracker.Models.Constants;

namespace bugTracker.Controllers
{
    public class TicketAttachmentController : Controller
    {
        // GET: TicketAttachment

        private ApplicationDbContext DbContext;
        public TicketAttachmentController()
        {
            DbContext = new ApplicationDbContext();
        }
        public ActionResult Index(int? id)
        {
            //Todo:judge id  check again
            if (!id.HasValue)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var ticketAttachments = DbContext.TicketAttachments.Where(p => p.TicketId ==id).ToList();
            var model = Mapper.Map<List<TicketAttachmentView>>(ticketAttachments);
           
            return View(model);
        }

        [HttpGet]
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
            var model = new CreateTicketAttachment
            {
                TicketId = ticket.Id
            };

            return View(model);
        }

        [HttpPost]
        public ActionResult CreateAttachment(int? id, CreateTicketAttachment formData)
        {
            if (!id.HasValue)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            if (!ModelState.IsValid)
            {
                return View();
            }
           
            var ticket = DbContext.Tickets.FirstOrDefault(
              p => p.Id == id);
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
            var userId = User.Identity.GetUserId();
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
            return RedirectToAction("CreateAttachment", "TicketAttachment", new { id = ticketAttachment.TicketId });
            //return View();
            //return RedirectToAction("Index", "TicketAttachment", new { id = ticketAttachment.TicketId });
        }
    }
}