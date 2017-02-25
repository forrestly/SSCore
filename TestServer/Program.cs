using SSCore;
using System;
using System.Collections.Generic;
using System.Net.Sockets;

class Program
{
    static void Main(string[] args)
    {
        SocketServerBase server = new SocketServerBase();
        server.NewClientAccepted += Server_NewClientAccepted;
        server.Start();

        Console.WriteLine("enter any key to exit.");
        Console.ReadKey();
    }

    private static void Server_NewClientAccepted(Socket client, ISocketSession session)
    {
        Console.WriteLine("----- new client ------------");
        AsyncSocketSession ass = session as AsyncSocketSession;

        ass.SetReceiveHandler(arg =>
        {
            Console.WriteLine("----- new receive ------------");
            string received = System.Text.Encoding.UTF8.GetString(arg.Buffer, arg.Offset, arg.BytesTransferred);
            Console.WriteLine(received);

            ass.Send(received);
        });
    }
}