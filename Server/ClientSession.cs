using ServerCore;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace Server
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

    class ClientSession : PacketSession
    {
        public override void OnConnected(EndPoint endPoint)
        {
        }

        public override void OnDisConnected(EndPoint endPoint)
        {
        }

        public override void OnRecvPacket(ArraySegment<byte> buffer)
        {
            ushort count = 0;
            ushort size = BitConverter.ToUInt16(buffer.Array, buffer.Offset);
            count += sizeof(ushort);
            ushort id = BitConverter.ToUInt16(buffer.Array, buffer.Offset + count);
            count += sizeof(ushort);

            switch((PacketID)id)
            {
                case PacketID.UserID:
                    {
                        UserInfoReq packet = new UserInfoReq();
                        packet.Read(buffer);
                        Console.WriteLine($"UserInfoReq : {packet.userId}");
                    }
                    break;
            }
        }

        public override void OnSend(int numOfBytes)
        {
        }
    }
}
