using System.Text;
using Company.Sherad;
using Newtonsoft.Json;
using RabbitMQ.Client;

namespace Company.Leads.Producer
{
    public class ProducerExchange : Connection
    {
        public void Exec()
        {
            var exchange = "exchange-cadastro";
            var channel = ExchangeDeclare(CreateConnection(), exchange);

            channel.QueueBind(
                  queue: "cadastro-dados",
                  exchange: exchange,
                  routingKey: string.Empty);

            channel.QueueBind(
                  queue: "processamento-pagamento",
                  exchange: exchange,
                  routingKey: string.Empty);

            SendMessageExchange(channel, exchange, 50);

            Console.WriteLine(" Press [enter] to exit.");
            Console.ReadLine();
        }

        private static int SendMessageExchange(IModel channel, string exchange, int total)
        {
            var count = 0;
            while (count < total)
            {
                var consumer = GetConsumer(channel, ref count);
                var send = JsonConvert.SerializeObject(consumer);
                var body = Encoding.UTF8.GetBytes(send);

                channel.BasicPublish(
                     exchange: exchange,
                     routingKey: string.Empty,
                     basicProperties: null,
                     body: body);

            }
            return count;
        }

        private static Consumer GetConsumer(IModel channel, ref int count)
        {
            return new Consumer
            {
                ConsumerId = count++,
                Name = $"Consumer [{channel.CurrentQueue}] - [ {count} ]",
                CreateDate = DateTime.Now,
                CardHolder = $"Consumer {count}",
                CardNumber = $"{count * 11111111}",
                CardExpiry = $"20/3{count}",
            };
        }
    }
}
