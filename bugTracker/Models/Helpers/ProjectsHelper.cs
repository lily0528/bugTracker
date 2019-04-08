using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace bugTracker.Models.Helpers
{
    public class ProjectsHelper
    {
        private ApplicationDbContext DbContext;

        public ProjectsHelper(ApplicationDbContext dbContext)
        {
            DbContext = dbContext;
        }

        public Project GetProjectById(int id)
        {
            return DbContext.Projects.FirstOrDefault(
                p => p.Id == id );
        }


    }
}