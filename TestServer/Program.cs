using SSCore;
using System;
using System.Collections.Generic;
using System.Net.Sockets;

class Program
{
    static void Main(string[] args)
    {
        SocketServerBase server = new SocketServerBase();
        server.ReadCompleted += Server_ReadCompleted;
        server.Start();

        Console.WriteLine("enter any key to exit.");
        Console.ReadKey();
    }

    private static void Server_ReadCompleted(Socket client, object state)
    {
        SocketAsyncEventArgs arg = state as SocketAsyncEventArgs;

        string received = System.Text.Encoding.UTF8.GetString(arg.Buffer, arg.Offset, arg.BytesTransferred);

        Console.WriteLine(received);

        AsyncSocketSession session = arg.UserToken as AsyncSocketSession;

        session.Send("received.");
    }
}