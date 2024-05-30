using System.Text;
using Company.Sherad;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Company.Payment.Processing.Consumer
{
    public class PaymentProcessingConsumer : Connection
    {
        public void Exec()
        {
            Dictionary<string, object> args = new()
            {
                { "x-dead-letter-exchange", "exchange-deadletter" },
                { "x-dead-letter-routing-key", "routing-key-deadletter" }
            };

            const string queueName = "processamento-pagamento";
            var channel = QueueDeclare(CreateConnection(), queueName, args);

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (model, ea) =>
            {
                var result = Encoding.UTF8.GetString(ea.Body.ToArray());
                var message = JsonConvert.DeserializeObject<Sherad.Consumer>(result);

                if (message!.CardNumber.Equals("0000"))
                {
                    channel.BasicNack(ea.DeliveryTag, false, false);
                }
                else
                {

                    Console.WriteLine(string.Format("\nCardHolder:{0}\nCardNumber:{1}\n\rCardExpiry {2}", message.CardHolder, message.CardNumber, message.CardExpiry));
                    channel.BasicAck(ea.DeliveryTag, false);
                }
            };

            channel.BasicConsume(queue: queueName,
                       autoAck: false,
                       consumer: consumer);

            Console.WriteLine(" Press [enter] to exit.");
            Console.ReadLine();
        }
    }
}