using System;
using System.IO;
using System.Text;
using System.Threading;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace UdemyRabbitMQ.Consumer
{
    public enum LogNames
    {
        Critical = 1,
        Error = 2
    }

    class Program
    {
        static void Main(string[] args)
        {
            var factory = new ConnectionFactory { HostName = "localhost" };//rabbitmq ile factory baglantısı için instance açtık
            //factory.Uri = new Uri("http://localhost:15672");

            using var connection = factory.CreateConnection();
            using (var channel = connection.CreateModel())
            {
                //channel.QueueDeclare("hello", false, false, false, null);
                ////consumer tarafı ile publisher burada aynı olmalı yoksa kuyruktaki mesajları alamayız!!
                ////durable:parametre memoryde mi tutsun yoksa diskte mi, exclusive:bir kanal mı bağlansın yoksa başka kanallar da bağlanabilsin mi ,autoDelete:kuyrukta eleman kalmayınca silinsin mi?

                //var consumer = new EventingBasicConsumer(channel);

                //channel.BasicConsume("hello", true, consumer);
                ////autoack: mesajın doğru ya da yanlış işlenmesine bağlı olmadan kuyruktan silinip silinmeyecegini belirler

                //consumer.Received += (model, ea) =>
                //{
                //    var message = Encoding.UTF8.GetString(ea.Body.ToArray());//kuyruktaki mesajı byte olarak aldık ve stringe cevirdik

                //    Console.WriteLine($"Mesaj alındı: {message}");

                //};

                /*EXCHANGE OLMAYAN*/ /*Daha sağlam bi rabbit mq publisher'a ait consumer oluşturduk*/
                //channel.QueueDeclare("task_queue", durable: true, false, false, null);

                /*EXCHANGE-> FANOUT*/
                //channel.ExchangeDeclare("logs", durable: true, type: ExchangeType.Fanout);
                //var queueName = channel.QueueDeclare().QueueName;//her instance'de (her ayaga kalktığında) ayrı bir kuyruk ismi oluşsun diye
                //channel.QueueBind(queue: queueName, exchange: "logs", routingKey: "");


                /*EXCHANGE-> DIRECT*/
                channel.ExchangeDeclare("direct-exhange", durable: true, type: ExchangeType.Direct);
                var queueName = channel.QueueDeclare().QueueName;//her instance'de (her ayaga kalktığında) ayrı bir kuyruk ismi oluşsun diye

                foreach (var item in Enum.GetNames(typeof(LogNames)))
                {
                    channel.QueueBind(queue: queueName, exchange: "direct-exhange", routingKey: item);
                }    

                channel.BasicQos(prefetchSize: 0, prefetchCount: 1, false);
                //prefectcount: bir mesaj gelsin ve doğru şekilde halletim dedikten sonra diğerini gönder.İdeal olan 1'dir.
                //global:tüm consumerları ilgilendirsin mi ilgilendirmesin mi.

                Console.WriteLine("Critical ve Error Logları bekliyorum");

                var consumer = new EventingBasicConsumer(channel);
                channel.BasicConsume(queueName, autoAck: false, consumer);

                consumer.Received += (model, ea) =>
                {
                    var log = Encoding.UTF8.GetString(ea.Body.ToArray());
                    Console.WriteLine($"Log alındı:{log}");

                        //farklı konfigurasyon kısmını simüle etmek için yaptık
                        int time = int.Parse(GetMessage(args));
                    Thread.Sleep(time);
                    File.AppendAllText("logs_critical_error.txt", log+"\n");//direct için
                    Console.WriteLine($"Loglama bitti");
                    channel.BasicAck(ea.DeliveryTag, multiple: false);//mesajı başarıyla işledim artık kuyruktan silebilirsin.
                 };
            }

            Console.WriteLine("Çıkış yapmak için tıklayınız");
            Console.ReadLine();


        }

        private static string GetMessage(string[] args)
        {
            return args[0].ToString();
        }
    }
}
