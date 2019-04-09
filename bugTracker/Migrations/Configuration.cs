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
                adminUser.DisplayName = "admin@mybugtracker.com";
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

            //Adding Moderator role if it doesn't exist.
            if (!context.Roles.Any(p => p.Name == "Project Manager"))
            {
                var ModeratorRole = new IdentityRole("Project Manager");
                roleManager.Create(ModeratorRole);
            }

            if (!context.Roles.Any(p => p.Name == "Developer"))
            {
                var ModeratorRole = new IdentityRole("Developer");
                roleManager.Create(ModeratorRole);
            }

            if (!context.Roles.Any(p => p.Name == "Submitter"))
            {
                var ModeratorRole = new IdentityRole("Submitter");
                roleManager.Create(ModeratorRole);
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
