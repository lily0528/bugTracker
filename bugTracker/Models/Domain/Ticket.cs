using System;

namespace bugTracker.Models
{
    public class Ticket
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime Created { get; set; }
        public DateTime? Updated { get; set; }
        public int ProjectId { get; set; }
        public virtual Project Project { get; set; }

        public int OwnerUseId { get; set; }
        public int AssignedToUserId { get; set; }
    }
}