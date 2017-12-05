using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using RabbitMQ.Client;
using System.Diagnostics;
using RabbitMQ.Client.Events;

namespace PingPongMessaging_cmd
{
    class RMQ
    {
        public ConnectionFactory connectionFactory;
        public IConnection connection;
        public IModel channel;

        public void InitRMQConnection(string host = "localhost", int port = 5672, string user = "guest", string pass = "guest")
        {
            connectionFactory = new ConnectionFactory();
            connectionFactory.HostName = host;
            //connectionFactory.Port = port;
            connectionFactory.UserName = user;
            connectionFactory.Password = pass;
        }

        public void CreateRMQConnection()
        {
            connection = connectionFactory.CreateConnection();
            Console.WriteLine("Connection " + (connection.IsOpen ? "Successful!" : "Failed!"));
        }

        public void CreateRMQChannel(string queue_name,string routingKey = "", string exchange_name = "")
        {
            if (connection.IsOpen)
            {
                channel = connection.CreateModel();
                Console.WriteLine("Channel " + (channel.IsOpen ? "Created!" : "Broken!"));
            }

            if (channel.IsOpen)
            {

                channel.QueueDeclare(queue: queue_name,
                                     durable: true,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);
                Console.WriteLine("Queue Declared.");

            }
        }

        public void WaitingMessage(string queue_name)
        {
            if (channel.IsOpen)
            {
                channel.QueueDeclare(queue: queue_name,
                                     durable: true,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);

                var consumer = new EventingBasicConsumer(channel);
                string tujuan, inputMsg;

                Console.WriteLine("Type 'q' to quit, 's' to send message, or any other key to start listening.");
                string cmd = Console.ReadLine();
                

                consumer.Received += (model, ea) =>
                {
                    var body = ea.Body;
                    var message = Encoding.UTF8.GetString(body);
                    Console.WriteLine(" [rx] received: {0}", message);
                    Console.WriteLine("Type 'q' to quit, press another key to send message");
                    cmd = Console.ReadLine();
                    if (cmd == "q")
                    {
                        Disconnect();
                    }
                    else
                    {
                        Console.Write("Write the target & message to send, or type 'exit' to close session.\n>>");
                        Console.Write(">> target: ");
                        tujuan = Console.ReadLine();
                        Console.Write(">> message: ");
                        inputMsg = Console.ReadLine();
                        SendMessage(tujuan, inputMsg);
                    }
                    
                };
                channel.BasicConsume(queue: queue_name,
                                     autoAck: true,
                                     consumer: consumer);
                if (cmd == "q")
                {
                    Disconnect();
                }else if(cmd == "s")
                {
                    Console.Write("Write the target & message to send, or type 'exit' to close session.\n>>");
                    Console.Write(">> target: ");
                    tujuan = Console.ReadLine();
                    Console.Write(">> message: ");
                    inputMsg = Console.ReadLine();
                    SendMessage(tujuan, inputMsg);
                }
            }
        }

        public void SendMessage(string tujuan, string msg = "send")
        {
            byte[] responseBytes = Encoding.UTF8.GetBytes(msg);// konversi pesan dalam bentuk string menjadi byte
            
            channel.BasicPublish(exchange: "",
                                    routingKey: tujuan,
                                    basicProperties: null,
                                    body: responseBytes);
            Console.WriteLine(" [tx]: '"+msg+"' sent.");
        }

        public void Disconnect()
        {
            channel.Close();
            channel = null;
            Console.WriteLine("Channel closed!");
            if (connection.IsOpen)
            {
                connection.Close();
            }

            Console.WriteLine("Connection closed!");
            connection.Dispose();
            connection = null;
        }
    }

    class Program
    {

        static void Main(string[] args)
        {
            RMQ rmq = new RMQ();
            Console.WriteLine("Press any key to initialise RMQ parameters.");
            Console.ReadKey();
            rmq.InitRMQConnection(); // inisialisasi parameter (secara default) untuk koneksi ke server RMQ
            Console.WriteLine("Press any key to create RMQ connections.");
            Console.ReadKey();
            rmq.CreateRMQConnection(); // memulai koneksi dengan RMQ
            Console.Write("Write a short name for you identity in the RMQ.\n>> ");
            string queue_name = Console.ReadLine();
            rmq.CreateRMQChannel(queue_name);
            
            rmq.WaitingMessage(queue_name);
            
        }        
    }
}
