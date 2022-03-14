using Jobsity.CodeChallenge.Chat.UI.Infra.CrossCutting.MessageBroker.Providers.MqClientConfig.Base;
using System.Threading.Tasks;

namespace Jobsity.CodeChallenge.Chat.UI.Infra.CrossCutting.MessageBroker.Interfaces.MqClientConfig
{
    public interface IRabbitMqClientConsumer<in TOptions>
        where TOptions : BaseMqSettingsProvider, new()
    {
        Task<string> ConsumeMessageAsync(string message, string routingKey = null);
        //IHandler<DomainNotification> Notifications { get; }
    }
}
