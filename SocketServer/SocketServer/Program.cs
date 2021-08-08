using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using SocketUtil;

namespace SocketUtil
{
    class Program
    {
        public  static Socket socketServer = new Socket(AddressFamily.InterNetwork,SocketType.Stream,ProtocolType.Tcp);
        private static byte[] buffer = new byte[1024 * 1024 * 2];
        static void Main(string[] args)
        {
            SocketServer server = new SocketServer(8888);
            server.StartListen();
            Console.ReadKey();
        }

    }
}
