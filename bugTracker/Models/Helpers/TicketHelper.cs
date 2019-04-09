using bugTracker.Models;

namespace bugTracker.Controllers
{
    internal class TicketHelper
    {
        private ApplicationDbContext dbContext;

        public TicketHelper(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }
    }
}