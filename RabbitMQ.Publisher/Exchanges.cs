using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RabbitMQ.Publisher
{
    public static class Exchanges
    {

        public static void FanoutExhangeRun()
        {
            var factory = new ConnectionFactory()
            {
                HostName = "localhost",
                UserName = "guest",
                Password = "guest",
            };


            using var connection = factory.CreateConnection();

            var channel = connection.CreateModel();

            // Publisher tarfından kuyruğu oluşturmayacağız bu sefer gerçek dünya senaryolarında tabiki de oluşturulur ancak biz suan oluşturmayacağız.
            //channel.QueueDeclare("hello-queue", true, false, false);

            // Declare ettiğimiz exchange yapısını subscriber tarafında da yapabiliriz. ama burda declare t
            channel.ExchangeDeclare("logs-fanout", durable: true, type: ExchangeType.Fanout);

            Enumerable.Range(1, 50).ToList().ForEach(x =>
             {
                 string message = $"Log{x}";
                 var messageBody = Encoding.UTF8.GetBytes(message);

                // Yukarıda declare ettiğimiz exchange 'i vericez burda. string.Empty yerine.
                 channel.BasicPublish("logs-fanout", "", null, messageBody);

                 Console.WriteLine($"Mesaj gönderilmiştir : {message}");
             });


            Console.ReadLine();
        }

    }
}
