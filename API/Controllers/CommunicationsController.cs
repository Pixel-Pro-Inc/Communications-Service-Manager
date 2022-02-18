using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Twilio;
using Twilio.Rest.Api.V2010.Account;

namespace API.Controllers
{
    public class CommunicationsController : BaseApiController
    {
        private readonly string accountSid = Configuration["twillosettings:accountSid"];
        private readonly string apiKeySid = Configuration["twillosettings:apiKeySid"];
        private readonly string apiKeySecret = Configuration["twillosettings:apiKeySecret"];
        public CommunicationsController()
        {

        }

        public async void SendMessage(string phonenumber, string email, string msg, string subject)
        {
            //Send SMS
            /*TwilioClient.Init(apiKeySid, apiKeySecret, accountSid);

            var message = await MessageResource.CreateAsync(
                body: msg,
                from: "BOTC",
                to: new Twilio.Types.PhoneNumber("+267" + phonenumber)
            );*/

            if (email == "")
                return;

            //Send Email
            MailMessage mailMessage = new MailMessage();
            MailAddress fromMail = new MailAddress(Configuration["smtpSettings:Account"]);
            mailMessage.From = fromMail;

            mailMessage.To.Add(new MailAddress(email));

            mailMessage.Subject = subject;
            mailMessage.Body = msg;

            var smtpClient = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(Configuration["smtpSettings:Account"], Configuration["smtpSettings:Password"])
            };
            try
            {
                smtpClient.Send(mailMessage);
            }
            catch
            {
                return;
            }
        }
    }
}
