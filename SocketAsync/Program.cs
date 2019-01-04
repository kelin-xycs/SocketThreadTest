using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Net;
using System.Net.Sockets;

using System.Threading;

namespace SocketAsync
{
    class Program
    {

        private static int myProt = 9527;   //端口
        static Socket serverSocket;

        static void Main(string[] args)
        {

            IPAddress ip = IPAddress.Parse("127.0.0.1");
            serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            serverSocket.Bind(new IPEndPoint(ip, myProt));  //绑定IP地址：端口
            serverSocket.Listen(20000);    //设定最多20000个排队连接请求
            Console.WriteLine("启动监听{0}成功", serverSocket.LocalEndPoint.ToString());
            //通过Clientsoket发送数据
            //Thread myThread = new Thread(ListenClientConnect);
            //myThread.Start();


            SocketAsyncEventArgs e = new SocketAsyncEventArgs();

            e.Completed += Accept_Completed;


            serverSocket.AcceptAsync(e);

            Console.ReadLine();

        }

        static void Accept_Completed(object sender, SocketAsyncEventArgs e)
        {
            
            SocketAsyncEventArgs e2 = new SocketAsyncEventArgs();

            byte[] b = new byte[2];

            e2.SetBuffer(b, 0, 2);

            e2.Completed += Receive_Completed;

            e2.AcceptSocket = e.AcceptSocket;

            e.AcceptSocket.ReceiveAsync(e2);


            SocketAsyncEventArgs e3 = new SocketAsyncEventArgs();

            e3.Completed += Accept_Completed;


            serverSocket.AcceptAsync(e3);
        }

        static void Receive_Completed(object sender, SocketAsyncEventArgs e)
        {
            byte[] b = new byte[2];

            b[0] = e.Buffer[0];
            b[1] = e.Buffer[1];

            SocketAsyncEventArgs e2 = new SocketAsyncEventArgs();

            e2.SetBuffer(b, 0, 2);

            e.AcceptSocket.SendAsync(e2);

            SocketAsyncEventArgs e3 = new SocketAsyncEventArgs();

            e3.Completed += Receive_Completed;

            e3.AcceptSocket = e.AcceptSocket;
            e3.SetBuffer(new byte[2], 0, 2);

            e.AcceptSocket.ReceiveAsync(e3);
        }

    }
}
