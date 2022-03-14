using Jobsity.CodeChallenge.Chat.UI.Application.Dtos;
using Jobsity.CodeChallenge.Chat.UI.Infra.Commons.Extensions;
using Jobsity.CodeChallenge.Chat.UI.Infra.CrossCutting.Hubs;
using Jobsity.CodeChallenge.Chat.UI.Infra.CrossCutting.MessageBroker.Interfaces.MqClientConfig;
using Jobsity.CodeChallenge.Chat.UI.Infra.CrossCutting.MessageBroker.Providers.MqClientConfig;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace Jobsity.CodeChallenge.Chat.UI.Application.Services.MqConsumers
{
    public class ChatMqConsumerAppService : IRabbitMqClientConsumer<MqChatClientSettingsProvider>
    {
        private readonly IHubContext<ChatHub> _hub;

        public ChatMqConsumerAppService(IHubContext<ChatHub> hub)
        {
            _hub = hub;
        }
        public async Task<string> ConsumeMessageAsync(string message, string routingKey = null)
        {
            //TODO: Receive parameter or message object with groupRoomNameId
            //TODO: Consumes Stoq api to return query info per request by user
            //TODO: Parse the query/message sent by user, and identify if is a regular conversation message or a query filter request

            var messageObj = message.ToObject<ChatMessageDto>();

            await _hub.Groups.AddToGroupAsync(messageObj.ConnectionId, messageObj.ChatRoom);
            await _hub.Clients.Groups(messageObj.ChatRoom).SendAsync("ReceiveMessage", messageObj.User, messageObj.Message);
            return string.Empty;
        }
    }
}
