namespace bugTracker.Migrations
{
    using bugTracker.Models;
    using bugTracker.Models.Domain;
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<bugTracker.Models.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
        }

        protected override void Seed(bugTracker.Models.ApplicationDbContext context)
        {

            var roleManager =
                new RoleManager<IdentityRole>(
                    new RoleStore<IdentityRole>(context));

            //UserManager, used to manage users
            var userManager =
                new UserManager<ApplicationUser>(
                        new UserStore<ApplicationUser>(context));

            //Adding admin role if it doesn't exist.
            if (!context.Roles.Any(p => p.Name == "Admin"))
            {
                var adminRole = new IdentityRole("Admin");
                roleManager.Create(adminRole);
            }

            //Creating the adminuser
            ApplicationUser adminUser;
          

            if (!context.Users.Any(
                p => p.UserName == "admin@mybugtracker.com"))
            {
                adminUser = new ApplicationUser();
                adminUser.UserName = "admin@mybugtracker.com";
                adminUser.DisplayName = "admin";
                adminUser.Email = "admin@mybugtracker.com";

                userManager.Create(adminUser, "Password-1");
            }
            else
            {
                adminUser = context
                    .Users
                    .First(p => p.UserName == "admin@mybugtracker.com");
            }

            //Make sure the user is on the admin role
            if (!userManager.IsInRole(adminUser.Id, "Admin"))
            {
                userManager.AddToRole(adminUser.Id, "Admin");
            }

            //project Manager seed
            ApplicationUser projectMangerUser;
            if (!context.Roles.Any(p => p.Name == "Project Manager"))
            {
                var ModeratorRole = new IdentityRole("Project Manager");
                roleManager.Create(ModeratorRole);
            }

            if (!context.Users.Any(p => p.UserName == "ProjectManager@gmail.com"))
            {
                projectMangerUser = new ApplicationUser();
                projectMangerUser.UserName = "ProjectManager@gmail.com";
                projectMangerUser.Email = "ProjectManager@gmail.com";
                projectMangerUser.DisplayName = "ProjectManager";

                userManager.Create(projectMangerUser, "Password-1");
            }
            else
            {
                projectMangerUser = context
                    .Users
                    .First(p => p.UserName == "ProjectManager@gmail.com");
            }

            if (!userManager.IsInRole(projectMangerUser.Id, "Project Manager"))
            {
                userManager.AddToRole(projectMangerUser.Id, "Project Manager");
            }

            //Submitter seed
            ApplicationUser SubmitterUser;
            if (!context.Roles.Any(p => p.Name == "Submitter"))
            {
                var ModeratorRole = new IdentityRole("Submitter");
                roleManager.Create(ModeratorRole);
            }
            if (!context.Users.Any(p => p.UserName == "Submitter@gmail.com"))
            {
                SubmitterUser = new ApplicationUser();
                SubmitterUser.UserName = "Submitter@gmail.com";
                SubmitterUser.Email = "Submitter@gmail.com";
                SubmitterUser.DisplayName = "Submitter";

                userManager.Create(SubmitterUser, "Password-1");
            }
            else
            {
                SubmitterUser = context
                    .Users
                    .First(p => p.UserName == "Submitter@gmail.com");
            }

            if (!userManager.IsInRole(SubmitterUser.Id, "Submitter"))
            {
                userManager.AddToRole(SubmitterUser.Id, "Submitter");
            }

            //Developer seed
            ApplicationUser DeveloperUser;
            if (!context.Roles.Any(p => p.Name == "Developer"))
            {
                var ModeratorRole = new IdentityRole("Developer");
                roleManager.Create(ModeratorRole);
            }
            if (!context.Users.Any(p => p.UserName == "ProjectManager@gmail.com"))
            {
                DeveloperUser = new ApplicationUser();
                DeveloperUser.UserName = "Developer@gmail.com";
                DeveloperUser.Email = "Developer@gmail.com";
                DeveloperUser.DisplayName = "Developer";

                userManager.Create(DeveloperUser, "Password-1");
            }
            else
            {
                DeveloperUser = context
                    .Users
                    .First(p => p.UserName == "Developer@gmail.com");
            }

            if (!userManager.IsInRole(DeveloperUser.Id, "Developer"))
            {
                userManager.AddToRole(DeveloperUser.Id, "Developer");
            }


            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data.

            context.TicketTypes.AddOrUpdate(t => t.Name,
                new TicketType { Name = "Bug" },
                new TicketType { Name = "Feature" },
                new TicketType { Name = "Database" },
                new TicketType { Name = "Support" });

            context.TicketPriorities.AddOrUpdate(t => t.Name,
               new TicketPriority { Name = "Low" },
               new TicketPriority { Name = "Medium" },
               new TicketPriority { Name = "High" }
              );

            context.TicketStatuses.AddOrUpdate(t => t.Name,
              new TicketStatus { Name = "Open" },
              new TicketStatus { Name = "Resolved" },
              new TicketStatus { Name = "Rejected" }
              );
        }
    }
}
