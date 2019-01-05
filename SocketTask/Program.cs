using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace SocketTask
{
    class Program
    {
        private static int myProt = 9527;   //端口

        static void Main(string[] args)
        {
            var ip = IPAddress.Parse("127.0.0.1");
            var serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            serverSocket.Bind(new IPEndPoint(ip, myProt));  //绑定IP地址：端口
            serverSocket.Listen(20000);    //设定最多20000个排队连接请求
            Console.WriteLine("启动监听{0}成功", serverSocket.LocalEndPoint.ToString());

            ListenClientConnect(serverSocket).Wait();
        }

        private static async Task ListenClientConnect(Socket serverSocket)
        {
            while (true)
            {
                Socket clientSocket = await Task.Factory.FromAsync(serverSocket.BeginAccept, serverSocket.EndAccept, serverSocket);
                Receive(clientSocket);

                //  用 线程池

                //ThreadPool.QueueUserWorkItem(Receive, clientSocket);

                //Thread receiveThread = new Thread(Receive);
                //receiveThread.Start(clientSocket);
            }
        }

        private static async Task Receive(Socket clientSocket)
        {
            byte[] b = new byte[2];

            //while (true)
            //{
            try
            {
                var receiveNumber = await Task.Factory.FromAsync(
                    (cb, o) => clientSocket.BeginReceive(b, 0, 2, SocketFlags.None, cb, o)
                    , clientSocket.EndReceive, null);
                await Task.Factory.FromAsync((cb, o) => clientSocket.BeginSend(b, 0, b.Length, SocketFlags.None, cb, o), clientSocket.EndSend, null);

                Receive(clientSocket);
                
                //通过clientSocket接收数据
                //int receiveNumber = myClientSocket.Receive(b);
                //myClientSocket.Send(b);

                //ThreadPool.QueueUserWorkItem(Receive, clientSocket);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                clientSocket.Shutdown(SocketShutdown.Both);
                clientSocket.Close();
                //myClientSocket.Shutdown(SocketShutdown.Both);
                //myClientSocket.Close();
                //break;
            }
            //}
        }
    }
}
