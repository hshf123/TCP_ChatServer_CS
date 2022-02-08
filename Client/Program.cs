using ServerCore;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Client
{
    class ChatSession : Session
    {
        public override void OnConnected(EndPoint endPoint)
        {
            Console.WriteLine($"OnConnected : {endPoint.ToString()}");
            try
            {
                while (true)
                {
                    // Send
                    byte[] sendBuffer = new byte[1024];
                    sendBuffer = Encoding.UTF8.GetBytes("서버로 전송!");
                    Send(sendBuffer);

                    Thread.Sleep(1000);
                }
            }
            catch(Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        public override void OnDisConnected(EndPoint endPoint)
        {
            Console.WriteLine($"OnDisConnected : {endPoint.ToString()}");
        }

        public override int OnRecv(ArraySegment<byte> buffer)
        {
            int recvLen = buffer.Count;
            string recvData = Encoding.UTF8.GetString(buffer.Array, buffer.Offset, buffer.Count);
            Console.WriteLine(recvData);
            return recvLen;
        }

        public override void OnSend(int numOfBytes)
        {
            Console.WriteLine($"Send : {numOfBytes}");
        }
    }

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
            connector.Connect(endPoint, () => { return new ChatSession(); });

            while(true)
            {

            }
        }
    }
}
