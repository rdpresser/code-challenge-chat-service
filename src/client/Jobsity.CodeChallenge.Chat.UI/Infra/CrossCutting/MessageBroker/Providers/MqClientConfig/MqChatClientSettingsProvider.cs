using Jobsity.CodeChallenge.Chat.UI.Infra.CrossCutting.MessageBroker.Providers.MqClientConfig.Base;

namespace Jobsity.CodeChallenge.Chat.UI.Infra.CrossCutting.MessageBroker.Providers.MqClientConfig
{
    public class MqChatClientSettingsProvider : BaseMqSettingsProvider
    {
        public override string ServiceQueueName => "Q.CODE.CHALLENGE.CLIENT.CHAT";
        public override string RoutingKey => ServiceQueueName;
    }
}
