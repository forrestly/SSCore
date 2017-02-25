# SSCore
dotnet core version of SuperSocket. Pick up socket communication logic from SuperSocket, and exclude command concept.

## usage

1. Create SocketServerBase object
2. Add handler for new client connection.
3. Start the socket server object.
4. Implement the handler of new client connection.
5. Add handler for receiving message.

## Example


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
