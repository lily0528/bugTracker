using bugTracker.Models;
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
    public class TicketHistoryController : Controller
    {
        private ApplicationDbContext DbContext;
        //private TicketHelper TicketHelper { get; }

        public TicketHistoryController()
        {
            DbContext = new ApplicationDbContext();
            //TicketHelper = new TicketHelper(DbContext);
        }
        // GET: TicketHistory
        public ActionResult Index(int? id)
        {

            if (!id.HasValue)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var userId = User.Identity.GetUserId();
            var model = DbContext.TicketHistories.Where(p => p.TicketId == id)
            .Select(p => new IndexTicketHistory
            {
                Id = p.Id,
                OldValue = p.OldValue,
                NewValue = p.NewValue,
                Property = p.Property,
                UserName = p.User.UserName,
                Updated = p.Updated
            }).ToList();
            //var TicketHisttories = DbContext.TicketHistories.ToList();
            //var model = Mapper.Map<List<IndexTicketHisttory>>(TicketHisttories);
            return View(model);
        }
    }
}