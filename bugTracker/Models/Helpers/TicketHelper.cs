using bugTracker.Models;
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
        public void EmailServiceSend(int? id, string subject, string body)
        {

            var emailService = new EmailService();
            //var userList = DbContext.TicketNotifications.Where(p => p.Users.Any(u => u.Id == userId)).
            var addresses = DbContext.TicketNotifications.Where(p => p.TicketId == id).Select(m => m.User.Email).ToList();
            var message = new MailMessage(ConfigurationManager.AppSettings["SmtpFrom"], string.Join(",", addresses.ToArray()));

            message.Subject = subject;
            message.Body = body;
            message.IsBodyHtml = true;
            emailService.Send(string.Join(",", addresses.ToArray()), message.Body, message.Subject);
        }
    }
}