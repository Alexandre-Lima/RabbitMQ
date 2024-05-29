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
            const string queueName = "processamento-pagamento";
            var channel = QueueDeclare(CreateConnection(), queueName);

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (model, ea) =>
            {
                var result = Encoding.UTF8.GetString(ea.Body.ToArray());
                var message = JsonConvert.DeserializeObject<Sherad.Consumer>(result);
                Console.WriteLine(string.Format("\nCardHolder:{0}\nCardNumber:{1}\n\rCardExpiry {2}", message.CardHolder, message.CardNumber, message.CardExpiry));
            };

            channel.BasicConsume(queue: queueName,
                       autoAck: true,
                       consumer: consumer);

            Console.WriteLine(" Press [enter] to exit.");
            Console.ReadLine();
        }
    }
}