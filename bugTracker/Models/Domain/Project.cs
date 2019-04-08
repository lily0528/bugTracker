using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace bugTracker.Models
{
    public class Project
    {
        public Project()
        {
            Users = new List<ApplicationUser>();
            Created = DateTime.Now;
        }

        public int Id { get; set; }
        [Required]
        [StringLength(100, MinimumLength = 4, ErrorMessage = "The {0} must be between {2} and {1} characters.")]
        public string Name { get; set; }
        public string Description { get; set; }


        public DateTime Created { get; set; }
        public DateTime? Updated { get; set; }
        public string UserId { get; set; }
        public virtual List<ApplicationUser> Users { get; set; }
        public virtual List<Ticket> Tickets { get; set; }
    }
}