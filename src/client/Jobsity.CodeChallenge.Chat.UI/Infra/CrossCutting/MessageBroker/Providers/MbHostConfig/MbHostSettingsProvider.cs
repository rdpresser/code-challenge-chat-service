namespace Jobsity.CodeChallenge.Chat.UI.Infra.CrossCutting.MessageBroker.Providers.MbHostConfig
{
    public class MbHostSettingsProvider
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string VirtualHost { get; set; }
        public string HostName { get; set; }
        public int DefaultPort { get; set; }
        public bool UseSsl { get; set; }
    }
}
