using Jobsity.CodeChallenge.Chat.UI.Infra.Commons.Extensions;
using Jobsity.CodeChallenge.Chat.UI.Infra.CrossCutting.MessageBroker.Providers.MqClientConfig.Base;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Linq;

namespace Jobsity.CodeChallenge.Chat.UI.Configurations.Extensions
{
    internal static class SettingsProviderExtension
    {
        internal static void GetSettingsProvider<TSettingsProvider>(this TSettingsProvider settingsProvider, IConfiguration configuration, string name)
            where TSettingsProvider : BaseMqSettingsProvider, new()
        {
            var defaultExchangeType = configuration.GetSection($"{name}:DefaultExchangeType").Get<string>();

            var serviceQueue = configuration.GetSection($"{name}:Queues")
                .GetChildren()
                .Where(s => s["ServiceQueueName"] == settingsProvider.ServiceQueueName)
                .Select(s => new TSettingsProvider
                {
                    AMQPExchangeName = s["AMQPExchangeName"],
                    RoutingKeys = s.GetSection("RoutingKeys").Get<List<string>>().IsNullOrEmpty() ? new List<string> { s["ServiceQueueName"] } : s.GetSection("RoutingKeys").Get<List<string>>(),
                    ExchangeType = s["ExchangeType"] ?? defaultExchangeType,
                })
                .FirstOrDefault();

            settingsProvider.AMQPExchangeName = serviceQueue.AMQPExchangeName;
            settingsProvider.ExchangeType = serviceQueue.ExchangeType;
            settingsProvider.RoutingKeys = serviceQueue.RoutingKeys;
        }
    }
}