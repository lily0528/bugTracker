﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace bugTracker.Models.ViewModels.CommentView
{
    public class TicketCommentView
    {
        public int Id { get; set; }
        public string Comment { get; set; }

        public DateTime Created { get; set; }
        public DateTime? Updated { get; set; }
        public int TicketId { get; set; }
        public virtual Ticket Ticket { get; set; }
        public string CreatorId { get; set; }
        public virtual ApplicationUser Creator { get; set; }
        public bool IfEdit { get; set; }
    }
}