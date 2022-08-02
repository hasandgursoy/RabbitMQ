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

        // Direct exhange için :D
        public enum LogNames
        {
            Critical = 1,
            Error = 2,
            Warning = 3,
            Info = 4
        }

        public static void DirectExchange()
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
            channel.ExchangeDeclare("logs-direct", durable: true, type: ExchangeType.Direct);

            Enum.GetNames(typeof(LogNames)).ToList().ForEach(x =>
            {
                var routeKey = $"route -{x}";
                var queueName = $"direct-queue-{x}";
                channel.QueueDeclare(queueName,true,false,false);
                // İlgili queue'nin ilgili exchange'nin route yapısını oluşturuyouruz. (1)
                // Kuyruğa bind işlemi sırasında exhange ve route veriyoruz. sonra bu yapıya basicPublish de olduğu gibi mesaj gönderiyoruz.
                channel.QueueBind(queueName, "logs-direct", routeKey,null); 
            });



            Enumerable.Range(1, 50).ToList().ForEach(x =>
            {
                LogNames log = (LogNames) new Random().Next(1,5);

                string message = $"Log-Type : {log}";
                var messageBody = Encoding.UTF8.GetBytes(message);

                var routeKey = $"route -{log}";

                // İlgili exhange'in route'una messajımızı gönderiyoruz.
                channel.BasicPublish("logs-direct", routeKey, null, messageBody);

                Console.WriteLine($"Mesaj gönderilmiştir : {message}");
            });


            Console.ReadLine();
        }

    }
}
