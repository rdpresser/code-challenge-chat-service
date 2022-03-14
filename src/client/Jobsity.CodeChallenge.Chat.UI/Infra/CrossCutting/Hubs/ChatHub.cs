using Jobsity.CodeChallenge.Chat.UI.Application.Dtos;
using Jobsity.CodeChallenge.Chat.UI.Infra.Commons.Extensions;
using Jobsity.CodeChallenge.Chat.UI.Infra.CrossCutting.MessageBroker.Interfaces.MqClientConfig;
using Jobsity.CodeChallenge.Chat.UI.Infra.CrossCutting.MessageBroker.Providers.MqClientConfig;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace Jobsity.CodeChallenge.Chat.UI.Infra.CrossCutting.Hubs
{
    //TODO: Create a base ChatHub<TOptions> to provide generic interface for child Hub's in the future
    public class ChatHub : Hub
    {
        private readonly IRabbitMqMessageProducer<MqChatClientSettingsProvider> _messageProducer;
        private readonly ILogger<ChatHub> _logger;

        public ChatHub(IRabbitMqMessageProducer<MqChatClientSettingsProvider> messageProducer,
            ILogger<ChatHub> logger)
        {
            _messageProducer = messageProducer;
            _logger = logger;
        }

        public string GetConnectionId
        {
            get
            {
                return Context.ConnectionId;
            }
        }

        public async Task SendMessage(ChatMessageDto chatMessage)
        {
            chatMessage.ConnectionId = GetConnectionId;
            var message = chatMessage.ToJson();

            _logger.LogInformation(message);

            //TODO: Publish to RabbitMq
            _messageProducer.ProduceMessage(message);

            //--------------------------------------------------
            //await Clients.All.SendAsync("ReceiveMessage", user, message);
        }

        public async Task LeaveGroup(ChatMessageDto chatMessage)
        {
            await Groups.RemoveFromGroupAsync(GetConnectionId, chatMessage.ChatRoom);
            await Clients.Groups(chatMessage.ChatRoom).SendAsync("ReceiveUserLeaveMessage", chatMessage.User, chatMessage.ChatRoom);
        }

        public async Task JoinGroup(ChatMessageDto chatMessage)
        {
            await Clients.Groups(chatMessage.ChatRoom).SendAsync("ReceiveUserJoinMessage", chatMessage.User, chatMessage.ChatRoom);
            await Groups.AddToGroupAsync(GetConnectionId, chatMessage.ChatRoom);
        }
    }
}
