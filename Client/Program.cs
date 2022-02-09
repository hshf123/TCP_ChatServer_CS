using ServerCore;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Client
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
                while (true)
                {
                    // Send
                    ChatPacket packet = new ChatPacket() { size = 4, packetId = 0 };

                    ArraySegment<byte> openSegment = SendBufferHelper.Open(4096);
                    byte[] buffer = BitConverter.GetBytes(packet.size);
                    byte[] buffer2 = BitConverter.GetBytes(packet.packetId);
                    Array.Copy(buffer, 0, openSegment.Array, openSegment.Offset, buffer.Length);
                    Array.Copy(buffer2, 0, openSegment.Array, openSegment.Offset + buffer.Length, buffer2.Length);
                    ArraySegment<byte> sendBuff = SendBufferHelper.Close(packet.size);
                    Send(sendBuff);

                    Thread.Sleep(1000);
                }
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

            while (true)
            {

            }
        }
    }
}
