using Jobsity.CodeChallenge.Chat.UI.Infra.CrossCutting.MessageBroker.Interfaces.MbHostConfig;
using Jobsity.CodeChallenge.Chat.UI.Infra.CrossCutting.MessageBroker.Interfaces.MqClientConfig;
using Jobsity.CodeChallenge.Chat.UI.Infra.CrossCutting.MessageBroker.MqClientConfig.Base;
using Jobsity.CodeChallenge.Chat.UI.Infra.CrossCutting.MessageBroker.Providers.MqClientConfig.Base;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using System;
using System.Text;

namespace Jobsity.CodeChallenge.Chat.UI.Infra.CrossCutting.MessageBroker.MqClientConfig
{
    public class RabbitMqMessageProducer<TOptions> : BaseRabbitMqClient<TOptions>, IRabbitMqMessageProducer<TOptions>
        where TOptions : BaseMqSettingsProvider, new()
    {
        public string AppId => $"{nameof(IRabbitMqMessageProducer<TOptions>)}<{typeof(TOptions).Name}>";

        private const string _contentType = "application/json";
        private readonly IMbHostConfigProducer _mbHostProducer;

        public RabbitMqMessageProducer(IMbHostConfigProducer mbHostProducer,
            IOptions<TOptions> mqSettingsProvider,
            ILogger<RabbitMqMessageProducer<TOptions>> logger)
            : base(mbHostProducer, mqSettingsProvider, logger)
        {
            _mbHostProducer = mbHostProducer;
        }

        public virtual void ProduceMessage(string message)
        {
            var body = Encoding.UTF8.GetBytes(message);

            var properties = _mbHostProducer.Channel.CreateBasicProperties();
            properties.AppId = AppId;
            properties.ContentType = _contentType;
            properties.Persistent = true;
            properties.DeliveryMode = 2;
            properties.Timestamp = new AmqpTimestamp(DateTimeOffset.UtcNow.ToUnixTimeSeconds());

            _mbHostProducer.Channel.BasicPublish(exchange: ExchangeName,
                                 routingKey: RoutingKeyName,
                                 basicProperties: properties,
                                 body: body);
        }
    }
}