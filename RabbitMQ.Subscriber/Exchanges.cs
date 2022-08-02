using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RabbitMQ.Subscriber
{
    public static class Exchanges
    {

        public static void FanoutExhangeConsumerRun()
        {
            var factory = new ConnectionFactory()
            {
                HostName = "localhost",
                UserName = "guest",
                Password = "guest",
            };


            using var connection = factory.CreateConnection();

            var channel = connection.CreateModel();

            var randomQueueName = channel.QueueDeclare().QueueName;

            // Bind etmek ile declare etmek arasında ne fark var açıklayalım.
            // Bind ettiğimizde ilgili kuyruğun işi bittikden sonra kendisini siler.
            // Declare ettiğimizde ise kalmaya devam eder.
            channel.QueueBind(randomQueueName, "logs-fanout", "", null);



            channel.BasicQos(0, 1, false);

            var consumer = new EventingBasicConsumer(channel);

            channel.BasicConsume(randomQueueName, false, consumer);

            Console.WriteLine("Loglar dinleniyor.");

            consumer.Received += (object? sender, BasicDeliverEventArgs e) =>
            {
                var message = Encoding.UTF8.GetString(e.Body.ToArray());


                Console.WriteLine("Gelen Mesaj : " + message);
                channel.BasicAck(e.DeliveryTag, false); // bunu tanımlamamızın sebebi channel.BasicConsume(2.parametre false yaptık.)




            };

            Console.ReadLine();
        }
    }
}
