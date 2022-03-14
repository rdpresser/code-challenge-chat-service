using Jobsity.CodeChallenge.Chat.UI.Infra.CrossCutting.MessageBroker.Providers.MqClientConfig.Base;

namespace Jobsity.CodeChallenge.Chat.UI.Infra.CrossCutting.MessageBroker.Interfaces.MqClientConfig
{
    public interface IRabbitMqMessageProducer<in TOptions>
        where TOptions : BaseMqSettingsProvider, new()
    {
        void ProduceMessage(string message);
    }
}
