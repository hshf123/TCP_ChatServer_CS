using ServerCore;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Server
{
    class Program
    {
        static Listener _listener = new Listener();

        static void OnAcceptHandler(Socket clientSocket)
        {
            try
            {
                while (true)
                {
                    // Recieve
                    byte[] recvBuffer = new byte[1024];
                    int recvLen = clientSocket.Receive(recvBuffer);
                    string recvData = Encoding.UTF8.GetString(recvBuffer, 0, recvLen);
                    Console.WriteLine(recvData);

                    // Send
                    byte[] sendBuffer = new byte[1024];
                    sendBuffer = Encoding.UTF8.GetBytes($"\"{recvData}\"이라는 문장을 받았습니다!");
                    clientSocket.Send(sendBuffer);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        static void Main(string[] args)
        {
            // 1) Listener 소켓 준비
            // 2) Bind(서버주소/Port를 소켓에 연동)
            // 3) Listen
            // 4) Accept
            // 클라이언트 세션을 통해 클라이언트와 송수신 가능!

            string host = Dns.GetHostName();
            IPHostEntry ipHost = Dns.GetHostEntry(host);
            IPAddress ipAddr = ipHost.AddressList[0];
            IPEndPoint endPoint = new IPEndPoint(ipAddr, 194);

            _listener.Init(endPoint, OnAcceptHandler);
            Console.WriteLine("연결 대기중...");

            while(true)
            {

            }
        }
    }
}
