using RabbitMQ.Client;

namespace Jobsity.CodeChallenge.Chat.UI.Infra.CrossCutting.MessageBroker.Interfaces.MbHostConfig
{
    public interface IMbHostConfig
    {
        IModel Channel { get; }
        void Dispose();
    }
}
