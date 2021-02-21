using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Text;

namespace UdemyRabbitMQ.Publisher
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var factory = new ConnectionFactory { HostName = "localhost" };

            using (var connection = factory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    channel.ExchangeDeclare("header-exchange", durable: true, type: ExchangeType.Headers);

                    var property = channel.CreateBasicProperties();

                    Dictionary<string, object> headers = new Dictionary<string, object>();

                    headers.Add("fformat5", "pdf");
                    headers.Add("shape", "a4");

                    property.Headers = headers;

                    channel.BasicPublish("header-exchange", string.Empty, property, Encoding.UTF8.GetBytes("header mesajım"));
                    
                }

                Console.WriteLine("Çıkış yapmak tıklayınız..");
                Console.ReadLine();
            }
        }

        private static string GetMessage(string[] args)
        {
            return args[0].ToString();
        }
    }
}