using Server;
using Server.Chat;
using ServerCore;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Server
{
    class Program
    {
        static Listener _listener = new Listener();

        static void Main(string[] args)
        {
            RoomManager.Instance.Add();

            // 1) Listener 소켓 준비
            // 2) Bind(서버주소/Port를 소켓에 연동)
            // 3) Listen
            // 4) Accept
            // 클라이언트 세션을 통해 클라이언트와 송수신 가능!

            string host = Dns.GetHostName();
            IPHostEntry ipHost = Dns.GetHostEntry(host);
            IPAddress ipAddr = ipHost.AddressList[0];
            IPEndPoint endPoint = new IPEndPoint(ipAddr, 194);

            _listener.Init(endPoint, SessionManager.Instance.Generate);
            Console.WriteLine("연결 대기중...");

            while (true)
            {
                Thread.Sleep(250);
            }
        }
    }
}
