using ServerCore;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading;

namespace Client
{
    public enum PacketID
    {
        UserID = 1,
    }

    public abstract class ChatPacket
    {
        public ushort size;
        public ushort packetId;

        public abstract ArraySegment<byte> Write();
        public abstract void Read(ArraySegment<byte> segment);
    }

    class UserInfoReq : ChatPacket
    {
        public long userId;

        public UserInfoReq()
        {
            this.packetId = (ushort)PacketID.UserID;
        }

        public override void Read(ArraySegment<byte> segment)
        {
            ushort count = 0;

            count += sizeof(ushort);
            count += sizeof(ushort);
            this.userId = BitConverter.ToInt64(new ReadOnlySpan<byte>(segment.Array, segment.Offset + count, segment.Count - count));
            count += sizeof(long);
        }

        public override ArraySegment<byte> Write()
        {
            ArraySegment<byte> segment = SendBufferHelper.Open(4096);

            ushort count = 0;
            bool success = true;

            count += sizeof(ushort);
            success &= BitConverter.TryWriteBytes(new Span<byte>(segment.Array, segment.Offset + count, segment.Count - count), this.packetId);
            count += sizeof(ushort);
            success &= BitConverter.TryWriteBytes(new Span<byte>(segment.Array, segment.Offset + count, segment.Count - count), this.userId);
            count += sizeof(long);
            success &= BitConverter.TryWriteBytes(new Span<byte>(segment.Array, segment.Offset, segment.Count), count);

            if (success == false)
                return null;

            return SendBufferHelper.Close(count);
        }
    }

    class ServerSession : PacketSession
    {
        public override void OnConnected(EndPoint endPoint)
        {
            Console.WriteLine($"OnConnected : {endPoint.ToString()}");
            try
            {
                while (true)
                {
                    // Send
                    UserInfoReq packet = new UserInfoReq() { userId = 1798 };

                    ArraySegment<byte> segment = packet.Write();
                    if (segment != null)
                        Send(segment);

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
            
        }

        public override void OnSend(int numOfBytes)
        {
            Console.WriteLine($"Send : {numOfBytes}");
        }
    }
}
