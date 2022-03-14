using Jobsity.CodeChallenge.Chat.UI.Infra.CrossCutting.MessageBroker.Interfaces.MbHostConfig;
using Jobsity.CodeChallenge.Chat.UI.Infra.CrossCutting.MessageBroker.Interfaces.MqClientConfig;
using Jobsity.CodeChallenge.Chat.UI.Infra.CrossCutting.MessageBroker.MqClientConfig.Base;
using Jobsity.CodeChallenge.Chat.UI.Infra.CrossCutting.MessageBroker.Providers.MqClientConfig;
using Jobsity.CodeChallenge.Chat.UI.Infra.CrossCutting.MessageBroker.Providers.MqClientConfig.Base;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Jobsity.CodeChallenge.Chat.UI.Infra.CrossCutting.MessageBroker.MqClientConfig
{
    public class RabbitMqEventConsumer<TOptions> : BaseRabbitMqClient<TOptions>, IHostedService
        where TOptions : BaseMqSettingsProvider, new()
    {
        private readonly TOptions _mqSettingsProvider;
        private readonly IMbHostConfigConsumer _mbHostConsumer;
        private readonly ILogger<RabbitMqEventConsumer<TOptions>> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly IRabbitMqMessageProducer<MqErrorSettingsProvider> _mqErrorProducer;
        private static string AppId => $"{nameof(RabbitMqEventConsumer<TOptions>)}<{typeof(TOptions).Name}>";

        public RabbitMqEventConsumer(IMbHostConfigConsumer mbHostConsumer,
            ILogger<RabbitMqEventConsumer<TOptions>> logger,
            IOptions<TOptions> mqSettingsProvider,
            IServiceProvider serviceProvider,
            IRabbitMqMessageProducer<MqErrorSettingsProvider> mqErrorProducer)
            : base(mbHostConsumer, mqSettingsProvider, logger)
        {
            _mqSettingsProvider = mqSettingsProvider?.Value;
            _mbHostConsumer = mbHostConsumer;
            _logger = logger;
            _serviceProvider = serviceProvider;
            _mqErrorProducer = mqErrorProducer;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            var consumer = new AsyncEventingBasicConsumer(_mbHostConsumer.Channel);
            consumer.Received += ConsumerReceived;

            _mbHostConsumer.Channel.BasicConsume(
                queue: _mqSettingsProvider.ServiceQueueName,
                autoAck: false,
                consumer: consumer);

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            Dispose();
            return Task.CompletedTask;
        }

        private async Task ConsumerReceived(object sender, BasicDeliverEventArgs @event)
        {
            _logger.LogInformation($"Message received: {Encoding.UTF8.GetString(@event.Body.ToArray())}");

            await DefaultBasicConsumer(@event);
        }

        private async Task DefaultBasicConsumer(BasicDeliverEventArgs @event)
        {
            try
            {
                var message = Encoding.UTF8.GetString(@event.Body.ToArray());
                await EventConsumerHandler(@event, message);
            }
            catch (Exception ex)
            {
                var errorMsg = ex.Message;
                _logger.LogError(errorMsg);
                //TODO: Error Queue
                //TryProduceToErrorQueue(@event, executionId, ex);
            }
            finally
            {
                _mbHostConsumer.Channel.BasicAck(deliveryTag: @event.DeliveryTag, multiple: false);
            }
        }

        private async Task<string> EventConsumerHandler(BasicDeliverEventArgs eventArgs, string message)
        {
            using var scope = _serviceProvider.CreateScope();
            var _mqProcessConsumer = scope.ServiceProvider.GetRequiredService<IRabbitMqClientConsumer<TOptions>>();

            var processedMessage = await _mqProcessConsumer.ConsumeMessageAsync(message, eventArgs.RoutingKey);

            //if (_mqProcessConsumer.Notifications.HasNotifications())
            //{
            //    TryProduceToErrorQueue(eventArgs, executionId, _mqProcessConsumer.Notifications.GetNotifications());
            //}

            return processedMessage;
        }

        //private void TryProduceToErrorQueue(BasicDeliverEventArgs eventArgs, Guid executionId, DomainNotification notification)
        //{
        //    try
        //    {
        //        var errorLog = new
        //        {
        //            RootCause = $"[{AppId}][{eventArgs.RoutingKey}]",
        //            ExecutionId = executionId,
        //            Message = $"[{notification.Key}]: {notification.Value}",
        //            Timestamp = DateTime.UtcNow,
        //            Type = "Error",
        //        };

        //        _logger.LogError(errorLog.Message);
        //        _mqErrorProducer.ProduceMessage(errorLog.ToJson());
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogFatal(ex);
        //    }
        //}

        //private void TryProduceToErrorQueue(BasicDeliverEventArgs eventArgs, Guid executionId, List<DomainNotification> notifications)
        //{
        //    notifications.ForEach(x => TryProduceToErrorQueue(eventArgs, executionId, x));
        //}

        //private void TryProduceToErrorQueue(BasicDeliverEventArgs eventArgs, Guid executionId, Exception exception)
        //{
        //    try
        //    {
        //        var errorLog = new
        //        {
        //            RootCause = $"[MqEventConsumer][ConsumerReceived][{eventArgs.RoutingKey}]",
        //            ExecutionId = executionId,
        //            Message = exception.GetErrorMsg(),
        //            Timestamp = DateTime.UtcNow,
        //            Type = "Exception",
        //            ExceptionStackTrace = exception.StackTrace,
        //        };

        //        _logger.LogError(exception);
        //        _mqErrorProducer.ProduceMessage(errorLog.ToJson());
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogFatal(ex);
        //    }
        //}
    }
}
