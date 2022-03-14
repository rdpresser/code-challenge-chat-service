using Jobsity.CodeChallenge.Chat.UI.Infra.CrossCutting.Hubs;
using Jobsity.CodeChallenge.Chat.UI.Infra.CrossCutting.RabbitMq.Interfaces;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;

namespace Jobsity.CodeChallenge.Chat.UI.Infra.CrossCutting.RabbitMq.Services
{
    public class RabbitMqService : IRabbitMqService
    {
        protected readonly ConnectionFactory _factory;
        protected readonly IConnection _connection;
        protected readonly IModel _channel;

        protected readonly IServiceProvider _serviceProvider;
        private const string _contentType = "application/json";
        private const string _exchangeName = "Ex.TestQueue.Chat";
        private const string _queueName = "Q.TestQueue";

        public RabbitMqService(IServiceProvider serviceProvider)
        {
            // Opens the connections to RabbitMQ
            _factory = new ConnectionFactory
            {
                HostName = "rabbitmq",
                Port = 5672,
                UserName = "guest",
                Password = "guest"
            };
            _connection = _factory.CreateConnection();
            _channel = _connection.CreateModel();

            _serviceProvider = serviceProvider;
        }

        private void QueueSetupBind()
        {
            _channel.BasicQos(0, 1, false);

            _channel.ExchangeDeclare(exchange: _exchangeName,
                type: "direct",
                durable: true,
                autoDelete: false);

            _channel.QueueDeclare(queue: _queueName,
                                     durable: true,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);


            _channel.QueueBind(queue: _queueName,
                               exchange: _exchangeName,
                               routingKey: _queueName);
        }

        public virtual void Connect()
        {
            // Declare a RabbitMQ Queue
            //_channel.QueueDeclare(queue: "TestQueue", durable: true, exclusive: false, autoDelete: false);
            QueueSetupBind();

            var consumer = new EventingBasicConsumer(_channel);

            // When we receive a message from SignalR
            consumer.Received += delegate (object model, BasicDeliverEventArgs ea)
            {
                // Get the ChatHub from SignalR (using DI)
                var chatHub = _serviceProvider.GetRequiredService<IHubContext<ChatHub>>();

                var body = ea.Body.ToArray();

                //TODO: Send an object to serialize the userName Properly in the original message event from rabbitMq
                var message = Encoding.UTF8.GetString(body);

                // Send message to all users in SignalR
                chatHub.Clients.All.SendAsync("ReceiveMessage", "user", message);

            };

            // Consume a RabbitMQ Queue
            _channel.BasicConsume(queue: _queueName, autoAck: true, consumer: consumer);
        }

        public virtual void ProduceMessage(string message)
        {
            var body = Encoding.UTF8.GetBytes(message);

            //using var accquisiton = _mbHostProducer.Channel.Accquire();

            var properties = _channel.CreateBasicProperties();
            properties.AppId = "Producer-TestQueue";
            properties.ContentType = _contentType;
            properties.Persistent = true;
            properties.DeliveryMode = 2;

            _channel.BasicPublish(exchange: _exchangeName,
                                 routingKey: _queueName,
                                 basicProperties: properties,
                                 body: body);
        }
    }
}