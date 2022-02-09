using ServerCore;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Server
{
    class ChatPacket
    {
        public ushort size;
        public ushort packetId;
    }

    public class ChatSession : PacketSession
    {
        public override void OnConnected(EndPoint endPoint)
        {
            Console.WriteLine($"OnConnected : {endPoint.ToString()}");
            try
            {
                //// Send
                //ChatPacket packet = new ChatPacket() { size = 4, packetId = 0 };

                //ArraySegment<byte> openSegment = SendBufferHelper.Open(4096);
                //byte[] buffer = BitConverter.GetBytes(packet.size);
                //byte[] buffer2 = BitConverter.GetBytes(packet.packetId);
                //Array.Copy(buffer, 0, openSegment.Array, openSegment.Offset, buffer.Length);
                //Array.Copy(buffer2, 0, openSegment.Array, openSegment.Offset + buffer.Length, buffer2.Length);
                //ArraySegment<byte> sendBuff = SendBufferHelper.Close(buffer.Length + buffer2.Length);
                //Send(sendBuff);

                Thread.Sleep(1000);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        public override void OnDisConnected(EndPoint endPoint)
        {
            Console.WriteLine($"OnDisConnected : {endPoint.ToString()}");
        }

        public override void OnRecvPacket(ArraySegment<byte> buffer)
        {
            ushort count = 0;
            ushort size = BitConverter.ToUInt16(buffer.Array, buffer.Offset);
            count += sizeof(ushort);
            ushort packetId = BitConverter.ToUInt16(buffer.Array, buffer.Offset + count);
            Console.WriteLine($"Size : {size} / PacketID : {packetId}");
        }

        public override void OnSend(int numOfBytes)
        {
            Console.WriteLine($"Send : {numOfBytes}");
        }
    }

    class Program
    {
        static Listener _listener = new Listener();

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

            _listener.Init(endPoint, () => { return new ChatSession(); });
            Console.WriteLine("연결 대기중...");

            while (true)
            {

            }
        }
    }
}
