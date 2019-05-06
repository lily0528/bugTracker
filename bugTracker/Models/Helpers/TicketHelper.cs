using bugTracker.Models;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Mail;

namespace bugTracker.Controllers
{
    public class TicketHelper
    {
        private ApplicationDbContext DbContext;

        public TicketHelper(ApplicationDbContext dbContext)
        {
            DbContext = dbContext;
        }


        public void SendNotification(string allEmails, string subject, string body)
        {
            //foreach (var email in emails)
            //{
                var message = new MailMessage(ConfigurationManager.AppSettings["SmtpFrom"], allEmails)
                {
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true
                };
                var emailService = new EmailService();
               
                emailService.Send(allEmails, message.Body, message.Subject);
            //}
        }

        // overload for single user
        //public void SendNotification(string email, string subject, string body)
        //{
        //    SendNotification(new List<string> { email }, subject, body); 
        //}

        //public void EmailServiceSend(int? id, string subject, string body)
        //{

        //    var emailService = new EmailService();
        //    //var userList = DbContext.TicketNotifications.Where(p => p.Users.Any(u => u.Id == userId)).
        //    var addresses = DbContext.TicketNotifications.Where(p => p.TicketId == id).Select(m => m.User.Email).ToList();
        //    if (addresses.Any())
        //    {
        //        var addressesList = string.Join(",", addresses.ToArray());
        //        var message = new MailMessage(ConfigurationManager.AppSettings["SmtpFrom"], addressesList)
        //        {
        //            Subject = subject,
        //            Body = body,
        //            IsBodyHtml = true
        //        };
        //        emailService.Send(addressesList, message.Body, message.Subject);
        //    }
        //}
    }
}