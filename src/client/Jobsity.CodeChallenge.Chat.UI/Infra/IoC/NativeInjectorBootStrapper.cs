using Jobsity.CodeChallenge.Chat.UI.Application.Services.MqConsumers;
using Jobsity.CodeChallenge.Chat.UI.Infra.CrossCutting.MessageBroker.Interfaces.MbHostConfig;
using Jobsity.CodeChallenge.Chat.UI.Infra.CrossCutting.MessageBroker.Interfaces.MqClientConfig;
using Jobsity.CodeChallenge.Chat.UI.Infra.CrossCutting.MessageBroker.MbHostConfig;
using Jobsity.CodeChallenge.Chat.UI.Infra.CrossCutting.MessageBroker.MqClientConfig;
using Jobsity.CodeChallenge.Chat.UI.Infra.CrossCutting.MessageBroker.Providers.MqClientConfig;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Jobsity.CodeChallenge.Chat.UI.Infra.IoC
{
    public static class NativeInjectorBootStrapper
    {
        public static IServiceCollection RegisterServices(this IServiceCollection services)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));

            //services.AddSingleton<IRabbitMqService, RabbitMqService>();
            services.AddSingleton(typeof(IRabbitMqMessageProducer<>), typeof(RabbitMqMessageProducer<>));
            services.AddSingleton<IMbHostConfigProducer, MbHostConfigProducer>();
            services.AddSingleton<IMbHostConfigConsumer, MbHostConfigConsumer>();

            //services.AddScoped<IRabbitMqClientConsumer<MqErrorSettingsProvider>, ApiErrorLogMQConsumerAppService>();
            services.AddScoped<IRabbitMqClientConsumer<MqChatClientSettingsProvider>, ChatMqConsumerAppService>();

            return services;
        }
    }
}
