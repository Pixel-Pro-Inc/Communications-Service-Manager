using API.DTO;
using API.Entities;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Twilio;
using Twilio.Jwt.AccessToken;
using Twilio.Rest.Api.V2010.Account.Conference;
using Twilio.Rest.Conversations.V1;
using Twilio.Rest.Conversations.V1.Conversation;
using ParticipantResource = Twilio.Rest.Conversations.V1.Conversation.ParticipantResource;

namespace API.Controllers
{
    public class LiveChatController : BaseApiController
    {
        private readonly string accountSid = Configuration["twillosettings:accountSid"];
        private readonly string apiKeySid = Configuration["twillosettings:apiKeySid"];
        private readonly string apiKeySecret = Configuration["twillosettings:apiKeySecret"];

        public LiveChatController()
        {
            TwilioClient.Init(apiKeySid, apiKeySecret, accountSid);
        }

        [HttpPost("start")]
        public async Task<ActionResult<ConversationDto>> CreateConversation(LiveChatDto liveChatDto)
        {
            await CloseOldChats();

            string conversationName = liveChatDto.conversationName;
            List<string> participants = await ConvertToList(liveChatDto.participants);
            string APIKey = liveChatDto.apiKey;

            var conversation = await ConversationResource.CreateAsync(
                friendlyName: conversationName
            );

            string conversationSid = conversation.Sid;

            #region StoreConversationId
            int id = await GetId("Conversations");

            Chat chat = new Chat()
            {
                Id = id,
                conversationSid = conversationSid
            };

            _firebaseDataContext.StoreData("Conversations/" + chat.Id, chat);
            #endregion

            string chatServiceSid = await ChatServiceSid(conversation.Sid);

            var user = await GetUser(APIKey);

            foreach (var item in participants)
            {
                var participantSMS = await ParticipantResource.CreateAsync(
                    messagingBindingAddress: "+267" + item,
                    attributes: GetJSONString("+267" + item),
                    messagingBindingProxyAddress: "+18453823904",
                    pathConversationSid: conversationSid
                    );
            }

            var participantChat = await ParticipantResource.CreateAsync(
                identity: user.OrganizationSenderCode,
                pathConversationSid: conversationSid
            );

            // These are specific to Chat
            string serviceSid = chatServiceSid;
            string identity = user.OrganizationSenderCode;

            // Create an Chat grant for this token

            var grant = new ChatGrant
            {
                ServiceSid = serviceSid
            };

            var grants = new HashSet<IGrant>
            {
                { grant }
            };

            // Create an Access Token generator
            var token = new Token(
                accountSid,
                apiKeySid,
                apiKeySecret,
                identity,
                grants: grants);

            return new ConversationDto() { 
                ConversationSid = conversationSid,
                Token = token.ToJwt()
            };
        } 

        async Task CloseOldChats() {

            List<Chat> conversations = await _firebaseDataContext.GetData<Chat>("Conversations");

            foreach (var item in conversations)
            {
                await StoreMessages(item.conversationSid);

                var conversation = await ConversationResource.UpdateAsync(
                    state: ConversationResource.StateEnum.Closed,
                    pathSid: item.conversationSid
                );

                _firebaseDataContext.DeleteData("Conversations/" + item.Id);
            }
        }

        async Task StoreMessages(string convoSid)
        {
            var messages = await MessageResource.ReadAsync(
                pathConversationSid: convoSid,
                limit: 10000
            );

            var participants = await ParticipantResource.ReadAsync(
                pathConversationSid: convoSid,
                limit: 1000
            );

            foreach (var record in messages)
            {
                MessageLog messageLog = new MessageLog()
                {
                    Author = record.Author,
                    Body = record.Body,
                    Channel = "Mobile",
                    DateSent = record.DateCreated.ToString(),
                    Id = await GetId("MessageLog")
                };

                foreach (var item in participants)
                {
                    if(item.Sid != record.ParticipantSid)
                    {
                        string phonenumber = (JsonConvert.DeserializeObject<List<string>>(((JValue)item.Attributes).ToString()))[0];

                        string p = phonenumber + ", ";
                        messageLog.Recipient += p;
                    }
                }

                messageLog.Recipient = messageLog.Recipient.Trim();
                messageLog.Recipient = messageLog.Recipient.Remove(messageLog.Recipient.Length - 1, 1);

                _firebaseDataContext.StoreData("MessageLog/"+ messageLog.Id, messageLog);
            }
        }
        
        async Task<string> ChatServiceSid(string Sid)
        {
            var conversation = await ConversationResource.FetchAsync(
            pathSid: Sid
            );

            return conversation.ChatServiceSid;
        }
    }
}
