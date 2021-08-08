using System;

namespace SocketClient
{
    class Program
    {
        static void Main(string[] args)
        {
            SocketClient client = new SocketClient(8888);
            client.StartClient();
            Console.ReadKey();

        }
    }
}
