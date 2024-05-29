using System.Text;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Company.Sherad;

namespace Company.Register.Customer.Consumer
{
    public class CustomerConsumer : Connection
    {
        public void Exec()
        {
            const string queueName = "cadastro-dados";
            var channel = QueueDeclare(CreateConnection(), queueName);

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (model, ea) =>
            {
                var result = Encoding.UTF8.GetString(ea.Body.ToArray());
                var message = JsonConvert.DeserializeObject<Sherad.Consumer>(result);
                Console.WriteLine(string.Format("\nId:{0}\nName:{1}\n\rDate Cadastro {2}", message.ConsumerId, message.Name, message.CreateDate));
            };

            channel.BasicConsume(queue: queueName,
                                 autoAck: true,
                                 consumer: consumer);

            Console.WriteLine(" Press [enter] to exit.");
            Console.ReadLine();
        }
    }
}