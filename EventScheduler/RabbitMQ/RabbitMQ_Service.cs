using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;



namespace EventScheduler.RabbitMQ
{
    public class RabbitMQ_Service
    {
        //  private readonly IConfiguration _configuration;
        private readonly RabbitMQConfiguration _rabbitMQConfig;
        private readonly IConnection _connection;
        private readonly IModel _channel;
        private readonly IServiceProvider _serviceProvider;
        public RabbitMQ_Service(IOptions<RabbitMQConfiguration> options)
        {

            _rabbitMQConfig = options.Value;

            var factory = new ConnectionFactory()
            {
                HostName = _rabbitMQConfig.HostName,
                UserName = _rabbitMQConfig.UserName,
                Password = _rabbitMQConfig.Password,
                VirtualHost = _rabbitMQConfig.VirtualHost
            };
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
        }

        // For Sending Message
        public void SendMessage(string queueName, string message)
        {
            _channel.QueueDeclare(queue: queueName,
                                  durable: false,
                                  exclusive: false,
                                  autoDelete: false,
                                  arguments: null);

            var body = Encoding.UTF8.GetBytes(message);

            _channel.BasicPublish(exchange: "",
                                 routingKey: queueName,
                                 basicProperties: null,
                                 body: body);
        }

        // For Consuming Message
        

            // Modified ConsumeMessages method with a callback parameter
            public void ConsumeMessages(string queueName, Action<string> messageReceivedCallback)
            {
                _channel.QueueDeclare(queue: queueName,
                                      durable: false,
                                      exclusive: false,
                                      autoDelete: false,
                                      arguments: null);

                var consumer = new EventingBasicConsumer(_channel);

                consumer.Received += (model, ea) =>
                {
                    var body = ea.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);

                    // Call the callback method with the received message for processing
                    messageReceivedCallback(message);
                    
                    
                };

                _channel.BasicConsume(queue: queueName,
                                      autoAck: true,
                                      consumer: consumer);
            
            }


        public void Dispose()
        {
            _channel?.Close();
            _connection?.Close();
        }

    }

}
