using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;


namespace RabbitMQ.Subscriber
{
    public  class BasicRabbitMQConsumer
    {
        public static void BasicRabbitMQConsumerRun()
        {
            var factory = new ConnectionFactory()
            {
                HostName = "localhost",
                UserName = "guest",
                Password = "guest",
            };


            using var connection = factory.CreateConnection();

            var channel = connection.CreateModel();

            // durable = eğer true yaparsak restart atıldığında memoryde ki veri kaybolmaz. false yaparsak kaybolur.
            // exculisve = başka kanallardan ulaşılmasını istiyorsak false yapmamız lazım. yoksa burda oluşturduğumuz kanal üzerinden okuyabiliriz sadece.
            // autoDelete = son subscriber da bağlanmayı bırakınca kuyruk otomatik olarak silinir eğer true yaparsak bu istenmeyen bir durum o yüzden false yapacağız.

            // Bu kuyruğu declare etmiştik ancak yinede dursun hataya sebeb olmaz ancka kuyruk ismi aynı olacak ve özellikleri de aynı olacak.
            //channel.QueueDeclare("hello-queue", true, false, false);


            // ilk parametre kaç mesaj alacağı , ikinci parametre subscriberlara kaçar kaçar dağıtacağı.
            // üçüncü parametre subscriberlara verilen değeri tek seferde bölmesi örnek 5 tane mesaj gidecek bir subscriber ' 3 diğerine 2 gönderir true yaparsak.
            channel.BasicQos(0, 1, false);

            var consumer = new EventingBasicConsumer(channel);

            // Autoack = true olursa mesaj doğru işlense yanlışda islense kuyrukdan silinir. False olursa kendimiz haber vericez.
            channel.BasicConsume("hello-queue", false, consumer);


            consumer.Received += (object? sender, BasicDeliverEventArgs e) =>
            {
                var message = Encoding.UTF8.GetString(e.Body.ToArray());
                Thread.Sleep(1000);

                Console.WriteLine("Gelen Mesaj : " + message);
                // BasicAck de ilk parametre işlenen mesajı sil artık diyoruz.
                // 2. parametrede başka mesajlar varsa onların bilgisini göster diyoruz.
                channel.BasicAck(e.DeliveryTag, false); // bunu tanımlamamızın sebebi channel.BasicConsume(2.parametre false yaptık.)




            };

            Console.ReadLine();
        }

    }
}
