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
                

            client.Send(System.Text.Encoding.UTF8.GetBytes("hello world!"));
            Console.WriteLine("sent message.");
            var buffer = new byte[1024];
            client.Receive(buffer);
            Console.WriteLine("received message.");
            Console.WriteLine(System.Text.Encoding.UTF8.GetString(buffer, 0, 12));

            var key = Console.ReadKey();

            if (key.KeyChar.Equals('q'))
                break;
        }
    }
}