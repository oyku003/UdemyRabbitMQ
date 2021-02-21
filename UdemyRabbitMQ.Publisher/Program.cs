using RabbitMQ.Client;
using System;
using System.Text;

namespace UdemyRabbitMQ.Publisher
{
    public enum LogNames
    {
        Critical = 1,
        Error = 2,
        Info = 3,
        Warning = 4
    }
    class Program
    {
        static void Main(string[] args)
        {
            var factory = new ConnectionFactory { HostName="localhost"};//rabbitmq ile factory baglantısı için instance açtık
            //factory.Uri = new Uri("http://localhost:15672");

            using (var connection = factory.CreateConnection())
            {
                using (var channel= connection.CreateModel())
                {
                    //channel.QueueDeclare("hello", false, false, false, null);//durable:parametre memoryde mi tutsun yoksa diskte mi, exclusive:bir kanal mı bağlansın yoksa başka kanallar da bağlanabilsin mi ,autoDelete:kuyrukta eleman kalmayınca silinsin mi?

                    //string message = "Hello word";

                    //mesaj gönderilirken byte olarak gönderilmelidir.

                    //var bodyByte = Encoding.UTF8.GetBytes(message);//mesajlar byte array olmak zorunda
                    //channel.BasicPublish("", routingKey: "hello", null, bodyByte);//default exchange kullanılıyorsa (exchange'in boş geçilmesi durumu) yukarda belirttiğimiz que (hello) ile burada belirttiğimiz basicpublis içindeki route key parametresi aynı olmak zorunda

                    /*Daha sağlam bi rabbit mq publisherı oluşturduk*/

                    //channel.QueueDeclare("task_queue", durable: true, false, false, null);//durable =true vererek kuyrugu sağlama aldık//exchange kullanılmadığı zaman kullanılabilir.

                    /*EXHANGE-> FANOUT*/
                    //channel.ExchangeDeclare("logs",durable:true, type: ExchangeType.Fanout);
                    //  string message = GetMessage(args);


                    /*EXHANGE-> DIRECT*/
                    channel.ExchangeDeclare("direct-exhange",durable:true, type: ExchangeType.Direct);

                    Array logName = Enum.GetValues(typeof(LogNames));

                    for (int i = 1; i < 11; i++)
                    {
                        Random rnm = new Random();
                        LogNames log = (LogNames)logName.GetValue(rnm.Next(logName.Length));

                        //var body = Encoding.UTF8.GetBytes($"{message}-{i}");
                        var body = Encoding.UTF8.GetBytes($"log={log}");
                        var properties = channel.CreateBasicProperties();//mesajı sağlama aldık
                        properties.Persistent = true;//mesajı sağlama aldık
                        //channel.BasicPublish("", routingKey: "task_queue", properties, body: body); exhagne olmayan kısım için gecerli
                        channel.BasicPublish("direct-exhange", routingKey: log.ToString(), properties, body: body);

                        Console.WriteLine($"Log mesajı gönderilmiştir.{log}");
                    }
                   
                }

                Console.WriteLine("Çıkış yapmak için tıklayınız");
                Console.ReadLine();                
            }

        }

        private static string GetMessage(string[] args)
        {
            return args[0].ToString();
        }
    }
}
