using Jobsity.CodeChallenge.Chat.UI.Infra.CrossCutting.MessageBroker.Interfaces.MbHostConfig;
using Jobsity.CodeChallenge.Chat.UI.Infra.CrossCutting.MessageBroker.Providers.MqClientConfig.Base;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Reflection;

namespace Jobsity.CodeChallenge.Chat.UI.Infra.CrossCutting.MessageBroker.MqClientConfig.Base
{
    public abstract class BaseRabbitMqClient<TOptions> : IDisposable
        where TOptions : BaseMqSettingsProvider, new()
    {
        private readonly TOptions _mqSettingsProvider;
        private readonly IMbHostConfig _mbHostSettings;
        private readonly ILogger<BaseRabbitMqClient<TOptions>> _logger;
        private bool disposedValue;

        protected static string AssemblyName => Assembly.GetExecutingAssembly().GetName().Name.Replace(".Infra.CrossCutting.MessageBroker", "");
        protected string ExchangeName => _mqSettingsProvider.AMQPExchangeName;
        protected string RoutingKeyName => _mqSettingsProvider.RoutingKey;
        protected string QueueName => _mqSettingsProvider.ServiceQueueName;

        public BaseRabbitMqClient(IMbHostConfig mbHostSettings,
            IOptions<TOptions> mqSettingsProvider,
            ILogger<BaseRabbitMqClient<TOptions>> logger)
        {
            _mbHostSettings = mbHostSettings;
            _mqSettingsProvider = mqSettingsProvider?.Value;
            _logger = logger;

            QueueSetupBind();
        }

        private void QueueSetupBind()
        {
            _mbHostSettings.Channel.BasicQos(0, 1, false);

            _mbHostSettings.Channel.ExchangeDeclare(
                exchange: ExchangeName,
                type: _mqSettingsProvider.ExchangeType,
                durable: true,
                autoDelete: false,
                arguments: null);

            _mbHostSettings.Channel.QueueDeclare(
                queue: QueueName,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null);

            // Binds each routing key to the queue
            _mqSettingsProvider.RoutingKeys?.ForEach(routingKey =>
            {
                _mbHostSettings.Channel.QueueBind(
                    queue: QueueName,
                    exchange: ExchangeName,
                    routingKey: routingKey,
                    arguments: null);

                _logger.LogInformation($"ExchangeName: {ExchangeName} of QueueName: {QueueName} with RoutingKeyName: {routingKey}");
            });
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    _mbHostSettings?.Dispose();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~MBHostSettings()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
