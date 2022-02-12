using ServerCore;
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

            Connector connector = new Connector();
            connector.Connect(endPoint, SessionManager.Instance.Generate, 500);

            while (true)
            {
                try
                {
                    SessionManager.Instance.SendForEach();
                }
                catch(Exception e)
                {
                    Console.WriteLine(e.ToString());
                }

                Thread.Sleep(1000);
            }
        }
    }
}
