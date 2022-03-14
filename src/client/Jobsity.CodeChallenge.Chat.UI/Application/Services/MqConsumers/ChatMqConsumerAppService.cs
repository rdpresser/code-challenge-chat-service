using Jobsity.CodeChallenge.Chat.UI.Application.Dtos;
using Jobsity.CodeChallenge.Chat.UI.Infra.Commons.Extensions;
using Jobsity.CodeChallenge.Chat.UI.Infra.CrossCutting.Hubs;
using Jobsity.CodeChallenge.Chat.UI.Infra.CrossCutting.MessageBroker.Interfaces.MqClientConfig;
using Jobsity.CodeChallenge.Chat.UI.Infra.CrossCutting.MessageBroker.Providers.MqClientConfig;
using Microsoft.AspNetCore.SignalR;
using StooqApi;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Jobsity.CodeChallenge.Chat.UI.Application.Services.MqConsumers
{
    public class ChatMqConsumerAppService : IRabbitMqClientConsumer<MqChatClientSettingsProvider>
    {
        private readonly IHubContext<ChatHub> _hub;
        private const string stockCode = "/stock=";
        public ChatMqConsumerAppService(IHubContext<ChatHub> hub)
        {
            _hub = hub;
        }
        public async Task<string> ConsumeMessageAsync(string message, string routingKey = null)
        {
            //TODO: Consumes Stoq api to return query info per request by user
            //TODO: Parse the query/message sent by user, and identify if is a regular conversation message or a query filter request

            var messageObj = message.ToObject<ChatMessageDto>();
            var msgResponse = string.Empty;

            if (messageObj.Message.IndexOf(stockCode, System.StringComparison.CurrentCultureIgnoreCase) != -1)
            {
                msgResponse = messageObj.Message.ToLower().Replace(stockCode, "");
            }

            //TODO: perform clean up from the original message
            IList<Candle> stooqResponse = null;
            if (!string.IsNullOrWhiteSpace(msgResponse))
            {
                stooqResponse = await Stooq.GetHistoricalAsync(msgResponse);
            }

            if (stooqResponse != null && stooqResponse.Any())
            {
                msgResponse = $"{msgResponse} quote is ${stooqResponse.FirstOrDefault().Close:0.00} per share";
            }
            else
            {
                msgResponse = messageObj.Message;
            }

            await _hub.Clients.Groups(messageObj.ChatRoom).SendAsync("ReceiveMessage", messageObj.User, msgResponse);
            return string.Empty;
        }
    }
}
