using Jobsity.CodeChallenge.Chat.UI.Infra.CrossCutting.MessageBroker.Interfaces.MbHostConfig;
using Jobsity.CodeChallenge.Chat.UI.Infra.CrossCutting.MessageBroker.MbHostConfig.Base;
using Jobsity.CodeChallenge.Chat.UI.Infra.CrossCutting.MessageBroker.Providers.MbHostConfig;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Jobsity.CodeChallenge.Chat.UI.Infra.CrossCutting.MessageBroker.MbHostConfig
{
    public class MbHostConfigProducer : BaseMbHostConfig, IMbHostConfigProducer
    {
        protected override string ClientProvidedNameType => $"Producer / Env: '{EnvironmentName}'";
        public MbHostConfigProducer(IOptions<MbHostSettingsProvider> settings,
            ILogger<MbHostConfigProducer> logger)
            : base(settings, logger)
        {

        }
    }
}