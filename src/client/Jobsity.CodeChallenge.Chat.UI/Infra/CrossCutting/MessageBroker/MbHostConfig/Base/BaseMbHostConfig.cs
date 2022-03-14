using Jobsity.CodeChallenge.Chat.UI.Infra.CrossCutting.MessageBroker.Interfaces.MbHostConfig;
using Jobsity.CodeChallenge.Chat.UI.Infra.CrossCutting.MessageBroker.Providers.MbHostConfig;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Security.Authentication;

namespace Jobsity.CodeChallenge.Chat.UI.Infra.CrossCutting.MessageBroker.MbHostConfig.Base
{
    public abstract class BaseMbHostConfig : IMbHostConfig, IDisposable
    {
        private readonly MbHostSettingsProvider _settings;
        private readonly ILogger<BaseMbHostConfig> _logger;

        private bool disposedValue;
        private ConnectionFactory _factory;
        private IConnection _connection;
        private IModel _channel;

        protected abstract string ClientProvidedNameType { get; }
        protected static string AssemblyName => Assembly.GetExecutingAssembly().GetName().Name.Replace(".Infra.CrossCutting.MessageBroker", "");
        protected static string EnvironmentName => Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

        public BaseMbHostConfig(IOptions<MbHostSettingsProvider> settings,
                                ILogger<BaseMbHostConfig> logger)
        {
            _settings = settings?.Value;
            _logger = logger;
        }

        private ConnectionFactory GetFactory
        {
            get
            {
                if (_factory == null)
                {
                    _factory = new ConnectionFactory()
                    {
                        ClientProperties = new Dictionary<string, object>
                        {
                            { "connection_name", $"{AssemblyName} - {ClientProvidedNameType}" },
                            { "product", $"{AssemblyName} - {ClientProvidedNameType}" },
                            { "version", "v1" },
                            { "platform", "Docker" },
                            { "copyright", "rodrigo.presser@gmail.com" },
                            { "information", $"{AssemblyName} - {ClientProvidedNameType}" }
                        },
                        ClientProvidedName = $"{AssemblyName} - {ClientProvidedNameType}",
                        HostName = _settings.HostName,
                        Port = _settings.DefaultPort,
                        UserName = _settings.UserName ?? ConnectionFactory.DefaultUser,
                        Password = _settings.Password ?? ConnectionFactory.DefaultPass,
                        VirtualHost = _settings.VirtualHost ?? ConnectionFactory.DefaultVHost,
                        DispatchConsumersAsync = true,
                        Ssl = new SslOption() //Only for SSL Connections
                        {
                            Enabled = _settings.UseSsl,
                            ServerName = _settings.HostName,
                            Version = SslProtocols.Tls12
                        }
                    };
                }

                return _factory;
            }
        }

        private IConnection GetConnection
        {
            get
            {
                if (_connection == null)
                {
                    _connection = GetFactory.CreateConnection();

                    _logger.LogInformation($"++> Connection open for '{nameof(BaseMbHostConfig)}' with Type: {ClientProvidedNameType}");
                }

                return _connection;
            }
        }

        public IModel Channel
        {
            get
            {
                if (_channel == null)
                {
                    _channel = GetConnection.CreateModel();
                }

                return _channel;
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                    try
                    {
                        _channel?.Dispose();
                        _channel = null;

                        _connection?.Dispose();
                        _connection = null;
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Cannot dispose RabbitMQ channel or connection");
                    }
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }


        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~MbHostSettings()
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
