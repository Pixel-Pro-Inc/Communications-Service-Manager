using API.DTO;
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
using Twilio.TwiML;
using Twilio.TwiML.Messaging;
using Twilio.Types;

namespace API.Controllers
{
    public class CommunicationsController : BaseApiController
    {
        private readonly string accountSid = Configuration["twillosettings:accountSid"];
        private readonly string apiKeySid = Configuration["twillosettings:apiKeySid"];
        private readonly string apiKeySecret = Configuration["twillosettings:apiKeySecret"];

        public List<string> messageFeedback { get; set; }
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

        public List<string> Sendback(List<Response> responses)
        {
            List<string> mes = new List<string>();
            foreach (var resp in responses)
            {
                var message = "Your messages could not be processed at this time. Please try again later.";

                if (resp.StatusCode == HttpStatusCode.Accepted)
                {
                    message = "Thank you for your message, someone will be in touch soon!";
                }
                mes.Add(message);
            }
            return mes;
        }
        [HttpPost("sendsms")]
        public async void SendSMS(string msg, string phonenumber)
        {
            //Send SMS
           
             TwilioClient.Init(apiKeySid, apiKeySecret, accountSid);

            var message = await MessageResource.CreateAsync(
                body: msg,
                from: "BOTC",
                to: new Twilio.Types.PhoneNumber("+267" + phonenumber)
            );
            Console.WriteLine("Sms has been sent.");

        }
        [HttpPost("sendemail")]
        public async Task<List<Response>> SendEmail(CreateMessageDto createMessageDto)
        {
            //Because if it is true that means the message is whatsapp exclusive
            if (createMessageDto.Whatsappswitch) return null;
            var apiKey = Environment.GetEnvironmentVariable("SENDGRID_API_KEY");
            Console.WriteLine("Don't forget to set up the environment variables for the online hosted version");
            var client = new SendGridClient(apiKey);
            List<Response> response = new List<Response>();
            //Here put in logic for batch sending. As it stands the O notation is linear instead of log, so we will need to bring in a library,
            //I want to hack it with a hashmap but implementation seems bothersome, which is why I just made this not for whoever is reading, so 
            //please don't reel back in horror at the logic just yet.
            foreach (var reciepient in createMessageDto.recipients)
            {
                var msg = new SendGridMessage()
                {
                    From = new EmailAddress("pixelprocompanyco@gmail.com", "PX Team"),
                    Subject = createMessageDto.subject,
                    PlainTextContent = createMessageDto.content,
                    HtmlContent = "\n Powered by PixelPro",
                    ReplyTo = new EmailAddress("apexmachine2@gmail.com")
                };
                response.Add(await client.SendEmailAsync(msg).ConfigureAwait(false));
            }
            //This should give messages to 
            messageFeedback=Sendback(response);
            return response;

            /*
            var apiKey = Environment.GetEnvironmentVariable("TestingSendGridKey");
            var client = new SendGridClient(apiKey);
            var from = new EmailAddress("test@example.com", "Example User");
            var subject = "Sending with SendGrid is Fun";
            var to = new EmailAddress("test@example.com", "Example User");
            var plainTextContent = "and easy to do anywhere, even with C#";
            var htmlContent = "<strong>and easy to do anywhere, even with C#</strong>";
            var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
            return await client.SendEmailAsync(msg);
            */


        }


        //This is for sandBox testing. Please note to use the correct info when in production or when submitting, check code for more info
        //The sandbox can't be used for more than 24 hours
        [HttpPost("sendwhatsapp")]
        public void SendViaWhatsapp(CreateMessageDto createMessageDto)
        {
            var authToken = "5a1a5a6da35cd87d05b58e7bf776e8d2";
            TwilioClient.Init(accountSid, authToken);

            foreach (var reciepient in createMessageDto.recipients)
            {
                //if(reciepient.Contains(alphabet))continue;
                var messageOptions = new CreateMessageOptions(
                new PhoneNumber("whatsapp:+267"+reciepient));
                //The number below will be replaced with the approved one from whatsapp
                messageOptions.From = new PhoneNumber("whatsapp:+14155238886");
                messageOptions.Body = createMessageDto.content;

                var message = MessageResource.Create(messageOptions);
                Console.WriteLine(message.Body);
            }
            
        }
        /*
         
        [HttpPost("sendwhatsappSS")]
        public void MethodName()
        {
            var authToken = "5a1a5a6da35cd87d05b58e7bf776e8d2";
            TwilioClient.Init(accountSid, authToken);

            var messageOptions = new CreateMessageOptions(
                new PhoneNumber("whatsapp:+26776181742"));
            messageOptions.From = new PhoneNumber("whatsapp:+14155238886");
            messageOptions.Body = "The test works perfectly from the app";

            var message = MessageResource.Create(messageOptions);
            Console.WriteLine(message.Body);
        }

        [HttpPost("sendwhatsappSS")]
        public void MethodName()
        {
            var authToken = "5a1a5a6da35cd87d05b58e7bf776e8d2";
            TwilioClient.Init(accountSid, authToken);

            var messageOptions = new CreateMessageOptions(
                new PhoneNumber("whatsapp:+26776181742"));
            messageOptions.From = new PhoneNumber("whatsapp:+14155238886");
            messageOptions.Body = "The test works perfectly from the app";

            var message = MessageResource.Create(messageOptions);
            Console.WriteLine(message.Body);
        }

         */

        //This is a method that will catch twillo's webhooks for testing
        //It's does seem to be firing so I guess you'll have to figure that out
        [HttpPost("respondwhatsapp")]
        public void RespondWhatsapp()
        {
            var response = new MessagingResponse();
            var message = new Message();
            message.Body("Wait, you can see me?!");
            response.Append(message);
            response.Redirect(url: new Uri("https://demo.twilio.com/welcome/sms/"));

            Console.WriteLine(response.ToString());
        }

    }
}
