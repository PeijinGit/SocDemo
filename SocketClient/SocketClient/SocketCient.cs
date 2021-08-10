using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SocketClient
{
    public class SocketClient
    {
        private string _ip = string.Empty;
        private int _port = 0;
        private Socket _socket = null;
        private byte[] buffer = new byte[1024 * 1024 * 2];

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="ip">Server IP</param>
        /// <param name="port">Server Port</param>
        public SocketClient(string ip, int port)
        {
            this._ip = ip;
            this._port = port;
        }
        public SocketClient(int port)
        {
            this._ip = "127.0.0.1";
            this._port = port;
        }

        /// <summary>
        /// Strat connection
        /// </summary>
        public void StartClient()
        {
            try
            {
                //1.0 Instance Socket(IP4,Sream,TCP)
                _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                //2.0 Create IP obj
                IPAddress address = IPAddress.Parse(_ip);
                //3.0 Build IP + PORT
                IPEndPoint endPoint = new IPEndPoint(address, _port);
                //4.0 Build Connection
                _socket.Connect(endPoint);
                Console.WriteLine("Connection From Server");
                //5.0 Receive Data
                int length = _socket.Receive(buffer);
                Console.WriteLine("Receive From Server{0},Message:{1}", _socket.RemoteEndPoint.ToString(), Encoding.UTF8.GetString(buffer, 0, length));
                Thread thread = new Thread(() => {
                    while (true) 
                    {
                        int length = _socket.Receive(buffer);
                        Console.WriteLine("New Thread Receive Server{0},Message:{1}", _socket.RemoteEndPoint.ToString(), Encoding.UTF8.GetString(buffer, 0, length));
                    }
                });
                //thread.Start();
                
                TestReceiveAsync();
                //6.0 Send To Server
                for (int i = 0; i < 10; i++)
                {
                    Thread.Sleep(2000);
                    string sendMessage = string.Format("From Client,Current Time{0}", DateTime.Now.ToString());
                    //_socket.Send(Encoding.UTF8.GetBytes(sendMessage));

                    TestSendAsync(sendMessage);
                    Console.WriteLine("SendtoServer:{0}", sendMessage);
                }
            }
            catch (Exception ex)
            {
                _socket.Shutdown(SocketShutdown.Both);
                _socket.Close();
                Console.WriteLine(ex.Message);
            }
            Console.WriteLine("Finish Sending");
            Console.ReadKey();
        }

        public void TestSendAsync(string sendMsg)
        {
            sendMsg = "ASYNC!!";
            var socketEventArgs = new SocketAsyncEventArgs();
            socketEventArgs.RemoteEndPoint = _socket.RemoteEndPoint;
            socketEventArgs.UserToken = null;
            socketEventArgs.Completed += OnSendCompleted;
            byte[] byteMessage = Encoding.UTF8.GetBytes(sendMsg);
            socketEventArgs.SetBuffer(byteMessage, 0, byteMessage.Length);
            _socket.SendAsync(socketEventArgs);
        }

        public void TestReceiveAsync()
        {
            var socketEventArgs = new SocketAsyncEventArgs();
            socketEventArgs.RemoteEndPoint = _socket.RemoteEndPoint;
            socketEventArgs.UserToken = null;
            socketEventArgs.Completed += OnReceiveCompleted;
            socketEventArgs.SetBuffer(buffer, 0, buffer.Length);
            _socket.ReceiveAsync(socketEventArgs);
        }
        public void OnSendCompleted(object sender, SocketAsyncEventArgs e)
        {
            if (e.SocketError != SocketError.Success) return;
            var socket = sender as Socket;
            byte[] sendBuffer = e.Buffer;
            
            //int bytesRec = socket.Receive(sendBuffer);
            //var  response = Encoding.ASCII.GetString(sendBuffer, 0, bytesRec);
            //Console.WriteLine("Response: " + response);
        }
        public void OnReceiveCompleted(object sender, SocketAsyncEventArgs e)
        {
            if (e.SocketError != SocketError.Success) return;
            var socket = sender as Socket;
            byte[] recBuffer = e.Buffer;
            var res = Encoding.UTF8.GetString(buffer, 0, recBuffer[0]);
            //int bytesRec = socket.Receive(sendBuffer);
            //var  response = Encoding.ASCII.GetString(sendBuffer, 0, bytesRec);
            //Console.WriteLine("Response: " + response);
        }
    }
}