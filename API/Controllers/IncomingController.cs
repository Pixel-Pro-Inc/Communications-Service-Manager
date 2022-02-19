using API.Data;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Twilio.AspNet.Core;
using Twilio.TwiML;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class IncomingController : TwilioController
    {

        [HttpPost("message")]
        public ActionResult<TwiMLResult> Msg()
        {
            var requestBody = Request.Form["Body"];
            var response = new MessagingResponse();

            if (requestBody.ToString().ToLower() == "menu")
            {
                response.Message("Here are options for auto response.\r\n1. Option 1\r\n2. Option 2\r\n3. Option 3");
            }

            else if (requestBody.ToString().ToLower() == "1")
            {
                response.Message("Response for option 1");
            }
            else if (requestBody.ToString().ToLower() == "2")
            {
                response.Message("Response for option 2");
            }
            else if (requestBody.ToString().ToLower() == "3")
            {
                response.Message("Response for option 3");
            }

            return TwiML(response);
        }

        [HttpPost("voice")]
        public ActionResult<TwiMLResult> Voice()
        {
            var response = new VoiceResponse();
            response.Say("This is a call center example. Bye bye.", voice: "alice");

            return TwiML(response);
        }
    }
}
