using System.Collections.Generic;

namespace Jobsity.CodeChallenge.Chat.UI.Infra.CrossCutting.MessageBroker.Providers.MqClientConfig.Base
{
    public abstract class BaseMqSettingsProvider
    {
        public string AMQPExchangeName { get; set; }
        public abstract string ServiceQueueName { get; }
        public abstract string RoutingKey { get; }
        public List<string> RoutingKeys { get; set; }
        public string ExchangeType { get; set; }
    }
}