using API.Entities;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace API.Controllers
{
    public class EmailController : BaseApiController
    {
        /*[HttpPost("incoming")]
        public async Task<HttpResponseMessage> Post()
        {
            var root = "wwwroot";
            var provider = new MultipartFormDataStreamProvider(root);
            await Request.ContentType.ReadAsMultipartAsync(provider);

            var email = new MessageLog()
            {
                Id = await GetId("MessageLog"),
                Author = provider.FormData.GetValues("from").FirstOrDefault(),
                Recipient = provider.FormData.GetValues("to").FirstOrDefault(),
                Body = provider.FormData.GetValues("text").FirstOrDefault(),
                DateSent = (DateTime.UtcNow.AddHours(2)).ToString(),
                Channel = "Email"
            };

            _firebaseDataContext.StoreData("MessageLog/" + email.Id, email);

            return new HttpResponseMessage(HttpStatusCode.OK);
        }*/
    }
}
