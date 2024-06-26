﻿using RabbitMQ.Client;

namespace Company.Sherad
{
    public abstract class Connection
    {
        public IModel CreateConnection()
        {
            var factory = new ConnectionFactory { HostName = "localhost" };
            var connection = factory.CreateConnection();
            return connection.CreateModel();
        }

        public IModel QueueDeclare(IModel channel, string queue, Dictionary<string, object> arguments = null)
        {
            channel.QueueDeclare(queue: queue,
                                 durable: false,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: arguments);

            return channel;
        }

        public IModel ExchangeDeclare(IModel channel, string exchange)
        {
            channel.ExchangeDeclare(exchange: exchange, type: ExchangeType.Fanout);

            return channel;
        }
    }
}
