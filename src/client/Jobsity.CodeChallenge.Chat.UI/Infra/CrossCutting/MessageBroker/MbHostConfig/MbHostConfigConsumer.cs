using Jobsity.CodeChallenge.Chat.UI.Infra.CrossCutting.MessageBroker.Interfaces.MbHostConfig;
using Jobsity.CodeChallenge.Chat.UI.Infra.CrossCutting.MessageBroker.MbHostConfig.Base;
using Jobsity.CodeChallenge.Chat.UI.Infra.CrossCutting.MessageBroker.Providers.MbHostConfig;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Jobsity.CodeChallenge.Chat.UI.Infra.CrossCutting.MessageBroker.MbHostConfig
{
    public class MbHostConfigConsumer : BaseMbHostConfig, IMbHostConfigConsumer
    {
        protected override string ClientProvidedNameType => $"Consumer / Env: '{EnvironmentName}'";
        public MbHostConfigConsumer(IOptions<MbHostSettingsProvider> settings,
            ILogger<MbHostConfigConsumer> logger)
            : base(settings, logger)
        {

        }
    }
}
