using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace RabbitMQ.Subscriber
{
    public static class Exchanges
    {

        public static void FanoutExhangeConsumer()
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

        public static void DirectExchangeConsumer()
        {
            var factory = new ConnectionFactory()
            {
                HostName = "localhost",
                UserName = "guest",
                Password = "guest",
            };


            using var connection = factory.CreateConnection();

            var channel = connection.CreateModel();





            channel.BasicQos(0, 1, false);

            var consumer = new EventingBasicConsumer(channel);

            var queueName = "direct-queue-Critical";

            channel.BasicConsume(queueName, false, consumer);

            Console.WriteLine("Loglar dinleniyor.");

            consumer.Received += (object? sender, BasicDeliverEventArgs e) =>
            {
                var message = Encoding.UTF8.GetString(e.Body.ToArray());


                Console.WriteLine("Gelen Mesaj : " + message);

                //File.AppendAllText("log-critical.txt", message + "\n");

                channel.BasicAck(e.DeliveryTag, false); // bunu tanımlamamızın sebebi channel.BasicConsume(2.parametre false yaptık.)




            };

            Console.ReadLine();
        }

        public static void TopicExchangeConsumer()
        {
            var factory = new ConnectionFactory()
            {
                HostName = "localhost",
                UserName = "guest",
                Password = "guest",
            };


            using var connection = factory.CreateConnection();

            var channel = connection.CreateModel();





            channel.BasicQos(0, 1, false);

            var consumer = new EventingBasicConsumer(channel);

            var queueName = channel.QueueDeclare().QueueName;

            // Ortasında Error yazanı bulmaya çalışıyoruz.
            // "info.#" = başında info olsun sonunda ne olursa olsun diyoruz.

            var routeKey = "*.Error.*";

            channel.QueueBind(queueName, "logs-topic", routeKey);

            channel.BasicConsume(queueName, false, consumer);

            Console.WriteLine("Loglar dinleniyor.");

            consumer.Received += (object? sender, BasicDeliverEventArgs e) =>
            {
                var message = Encoding.UTF8.GetString(e.Body.ToArray());


                Console.WriteLine("Gelen Mesaj : " + message);

                //File.AppendAllText("log-critical.txt", message + "\n");

                channel.BasicAck(e.DeliveryTag, false); // bunu tanımlamamızın sebebi channel.BasicConsume(2.parametre false yaptık.)




            };

            Console.ReadLine();
        }

        public static void HeaderExchangeConsumer()
        {
            var factory = new ConnectionFactory()
            {
                HostName = "localhost",
                UserName = "guest",
                Password = "guest",
            };


            using var connection = factory.CreateConnection();

            var channel = connection.CreateModel();



            var consumer = new EventingBasicConsumer(channel);

            var queueName = channel.QueueDeclare().QueueName;

            Dictionary<string, object> headers = new Dictionary<string, object>();

            headers.Add("format", "pdf");
            headers.Add("shape", "a4");
            // all dersek mutlaka key value değerleri komple eşleşmesi laızm
            // any dersek herhangi biri eşleşse yeter.
            headers.Add("x-match", "all");

            channel.QueueBind(queueName, "header-exchange", string.Empty, headers);

            channel.BasicConsume(queueName, false, consumer);

            Console.WriteLine("Loglar Dinleniyor");

            consumer.Received += (object? sender, BasicDeliverEventArgs e) =>
            {

                var message = Encoding.UTF8.GetString(e.Body.ToArray());
                Product? product = JsonSerializer.Deserialize<Product>(message);

                Console.WriteLine($"Gelen message :{ product?.Id}-{product?.Name}");
            };

            Console.ReadLine();
        }


    }
}
