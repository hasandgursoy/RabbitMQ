


using RabbitMQ.Client;
using System.Text;

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
channel.QueueDeclare("hello-queue",true, false, false);

// 50 tane mesaj gönderiyoruz bu 50 tane mesajı birden fazla subscriber ile dinlemeye çalışalım.
Enumerable.Range(1, 50).ToList().ForEach(x =>
{
    string message = $"Message{x}";
    var messageBody = Encoding.UTF8.GetBytes(message);

    // ilk parametresi olan exchance kullanmıyorsak string.Empty verip geçiyoruz.
    // Ve exhange kullanmadığımız için 2. parametreye yani route-key kuyruğumuzun ismini veriyoruz. 
    // Publish edildikden sonra mesajımız bir consume işlemi olana kadar tüketilmeyi bekler.
    channel.BasicPublish(string.Empty, "hello-queue", null, messageBody);

    Console.WriteLine($"Mesaj gönderilmiştir : {message}");
});

// RabbitMQ ' ya mesajlar bit dizini olarak gider bu şekilde pdf,image vs herşeyi gönderebiliriz.

Console.ReadLine();
