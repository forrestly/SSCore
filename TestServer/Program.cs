using SSCore;
using System;
using System.Collections.Generic;

class Program
{
    static void Main(string[] args)
    {
        Dictionary<string, string> config = new Dictionary<string, string>
        {
            { "ip", "127.0.0.1" },
            { "port", "2020" }
        };
        SocketServerBase server = new SocketServerBase(config);

        server.Start();

        Console.ReadKey();
    }
}