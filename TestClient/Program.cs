using System;
using System.Net.Sockets;

class Program
{
    static void Main(string[] args)
    {
        StartAsync();
    }

    static void StartAsync()
    {

        Socket client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        try
        {
            client.Connect("127.0.0.1", 2020);
        }
        catch (Exception ex)
        {
            throw ex;
        }
        while (true)
        {
            if (!client.Connected)
            {
                client.Dispose();
                client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                client.Connect("127.0.0.1", 2020);
            }


            try
            {
                client.Send(System.Text.Encoding.UTF8.GetBytes("hello world!"));
            }
            catch (Exception)
            {
                Console.WriteLine("send error.");
            }
            Console.WriteLine("sent message.");
            var buffer = new byte[128];
            try
            {
                client.Receive(buffer);
            }
            catch (Exception)
            {
                Console.WriteLine("receive error.");
            }
            Console.WriteLine("received message.");
            Console.WriteLine(System.Text.Encoding.UTF8.GetString(buffer, 0, 12));

            var key = Console.ReadKey();

            if (key.KeyChar.Equals('q'))
                break;
        }
    }
}