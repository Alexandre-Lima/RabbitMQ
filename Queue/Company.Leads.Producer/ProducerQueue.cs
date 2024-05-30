using System.Text;
using Company.Sherad;
using Newtonsoft.Json;
using RabbitMQ.Client;

namespace Queue.Company.Leads.Producer
{
    public class ProducerQueue : Connection
    {
        public void Exec()
        {
            var channel = QueueDeclare(CreateConnection(), "cadastro-dados");
            SendMessageQueue(channel, 10);

            channel = QueueDeclare(CreateConnection(), "processamento-pagamento");
            SendMessageQueue(channel, 10);

            Console.WriteLine(" Press [enter] to exit.");
            Console.ReadLine();
        }

        private static void SendMessageQueue(IModel channel, int total)
        {
            var count = 0;

            while (count < total)
            {
                var consumer = new Consumer
                {
                    ConsumerId = count++,
                    Name = $"Consumer [{channel.CurrentQueue}] - [ {count} ]",
                    CreateDate = DateTime.Now,
                    CardHolder = $"Consumer {count}",
                    CardNumber = $"{count * 11111111}",
                    CardExpiry = $"20/3{count}",
                };

                var send = JsonConvert.SerializeObject(consumer);

                channel.BasicPublish(
                          exchange: string.Empty,
                          routingKey: channel.CurrentQueue,
                          basicProperties: null,
                          body: Encoding.UTF8.GetBytes(send)
                );
            }
        }
    }
}
