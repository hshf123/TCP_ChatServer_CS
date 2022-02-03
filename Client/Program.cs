using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Client
{
    class Program
    {
        static void Main(string[] args)
        {
            // 1) 소켓 준비
            // 2) 서버 주소로 Connect
            // 소켓을 통해 Session소켓과 패킷 송수신 가능!

            string host = Dns.GetHostName();
            IPHostEntry ipHost = Dns.GetHostEntry(host);
            IPAddress ipAddr = ipHost.AddressList[0];
            IPEndPoint endPoint = new IPEndPoint(ipAddr, 194);

            Socket socket = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            try
            {
                socket.Connect(endPoint);

                while(true)
                {
                    // Send
                    byte[] sendBuffer = new byte[1024];
                    sendBuffer = Encoding.UTF8.GetBytes("서버로 전송!");
                    socket.Send(sendBuffer);

                    // Recieve
                    byte[] recvBuffer = new byte[1024];
                    int recvLen = socket.Receive(recvBuffer);
                    string recvData = Encoding.UTF8.GetString(recvBuffer, 0, recvLen);
                    Console.WriteLine(recvData);
                    Thread.Sleep(1000);
                }
            }
            catch(Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
    }
}
