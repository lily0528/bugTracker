using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web;

namespace bugTracker.Models
{
    /// <summary>
    /// Represents a e-mail sender
    /// </summary>
    public class EmailService : IIdentityMessageService
    {
        //Getting the settings from our private.config setup in web.config
        private string SmtpHost = ConfigurationManager.
            AppSettings["SmtpHost"];
        private int SmtpPort = Convert.ToInt32(ConfigurationManager.AppSettings["SmtpPort"]);
        private string SmtpUsername = ConfigurationManager.AppSettings["SmtpUsername"];
        private string SmtpPassword = ConfigurationManager.AppSettings["SmtpPassword"];
        private string SmtpFrom = ConfigurationManager.AppSettings["SmtpFrom"];


        /// <summary>
        /// Sends an e-mail
        /// </summary>
        /// <param name="to">The destination of the e-mail</param>
        /// <param name="body">The body of the e-mail</param>
        /// <param name="subject">The subject of the e-mail</param>
        public void Send(string to,
            string body,
            string subject)
        {
            //Creates a MailMessage required to send messages
            var message = new MailMessage(SmtpFrom, to);
            message.Body = body;
            message.Subject = subject;
            message.IsBodyHtml = true;

            //Creates a SmtpClient required to handle the communication
            //between our application and the SMTP Server
            var smtpClient = new SmtpClient(SmtpHost, SmtpPort);
            smtpClient.Credentials =
                new NetworkCredential(SmtpUsername, SmtpPassword);
            smtpClient.EnableSsl = true;

            //Send the message
            smtpClient.Send(message);
        }

        /// <summary>
        /// Required by Microsoft Interface IIdentityMessageService in order to work with Identity Framework
        /// </summary>
        /// <param name="message">Object containing the necessary information to send e-mails</param>
        /// <returns></returns>
        public Task SendAsync(IdentityMessage message)
        {
            //Caling our sync method in a async way.
            return Task.Run(() =>
            Send(message.Destination, message.Body, message.Subject));
        }
    }
}