using Microsoft.AspNetCore.Mvc;
using SendGrid;
using SendGrid.Helpers.Mail;
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

            /*
                if (email == "")
                return;

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
             
             */

        }
        public async Task<string> Send()
        {

            var response = await SendEmail();

            var message = "Your message could not be processed at this time. Please try again later.";

            if (response.StatusCode == HttpStatusCode.Accepted)
            {
                message = "Thank you for your message, someone will be in touch soon!";
            }
            return message;
        }

        private async void SendSMS()
        {
            //Send SMS
            /*
             TwilioClient.Init(apiKeySid, apiKeySecret, accountSid);

            var message = await MessageResource.CreateAsync(
                body: msg,
                from: "BOTC",
                to: new Twilio.Types.PhoneNumber("+267" + phonenumber)
            );
             */

        }
        [HttpPost("sendEmail")]
        private async Task<Response> SendEmail()
        {

            var apiKey = Configuration["SendGridsettings:apiKey"];
            var client = new SendGridClient(apiKey);
            var msg = new SendGridMessage()
            {
                From = new EmailAddress("pixelprocompanyco@gmail.com", "PX Team"),
                Subject = "Sending with Twilio SendGrid is Fun",
                PlainTextContent = "and easy to do anywhere, even with C#",
                HtmlContent = "<strong>and easy to do anywhere, even with C#</strong>"
            };
            msg.AddTo(new EmailAddress("pixelprocompanyco@gmail.com", "Test User"));
            var response = await client.SendEmailAsync(msg).ConfigureAwait(false);
            return response;
        }
    }
}
