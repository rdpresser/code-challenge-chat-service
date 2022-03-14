using Jobsity.CodeChallenge.Chat.UI.Infra.CrossCutting.MessageBroker.Providers.MqClientConfig.Base;

namespace Jobsity.CodeChallenge.Chat.UI.Infra.CrossCutting.MessageBroker.Providers.MqClientConfig
{
    public class MqErrorSettingsProvider : BaseMqSettingsProvider
    {
        public override string ServiceQueueName => "Q.CODE.CHALLENGE.CHAT.ERROR";
        public override string RoutingKey => ServiceQueueName;
    }
}
