using System.Text;
using Company.Sherad;
using Newtonsoft.Json;
using RabbitMQ.Client;

namespace DeadLetter.Company.Leads.Producer
{
    public class ProducerDeadLetter : Connection
    {
        public void Exec()
        {
            var connection = CreateConnection();

            // Criação de exchange dead-letter
            var exchange = "exchange-deadletter";
            var channel = ExchangeDeclare(CreateConnection(), exchange);

            // Criação de fila dead-letter
            var channelDeadLetter = QueueDeclare(connection, "fila-deadletter");

            // Associação de fila dead-letter a exchange dead-letter
            channel.QueueBind("fila-deadletter", "exchange-deadletter", "");

            Dictionary<string, object> args = new()
            {
                { "x-dead-letter-exchange", "exchange-deadletter" },
                { "x-dead-letter-routing-key", "routing-key-deadletter" }
            };

            var channelPagamento = QueueDeclare(connection, "processamento-pagamento", args);

            var consumer = new Consumer
            {
                ConsumerId = 9999,
                Name = $"Consumer ProducerDeadLetter",
                CreateDate = DateTime.Now,
                CardHolder = $"ProducerDeadLetter",
                CardNumber = $"0000",
                CardExpiry = $"20/33",
            };

            var send = JsonConvert.SerializeObject(consumer);

            channelPagamento.BasicPublish(
                      exchange: string.Empty,
                      routingKey: channelPagamento.CurrentQueue,
                      basicProperties: null,
                      body: Encoding.UTF8.GetBytes(send)
            );


            Console.WriteLine(" Press [enter] to exit.");
            Console.ReadLine();
        }
    }
}
