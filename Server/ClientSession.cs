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
        public string userName;

        public UserInfoReq()
        {
            this.packetId = (ushort)PacketID.UserID;
        }

        public override void Read(ArraySegment<byte> segment)
        {
            ReadOnlySpan<byte> read = new ReadOnlySpan<byte>(segment.Array, segment.Offset, segment.Count);
            ushort count = 0;

            count += sizeof(ushort);
            count += sizeof(ushort);
            this.userId = BitConverter.ToInt64(read.Slice(count, read.Length - count));
            count += sizeof(long);
            ushort userNameLen = BitConverter.ToUInt16(read.Slice(count, read.Length - count));
            count += sizeof(ushort);
            this.userName = Encoding.Unicode.GetString(read.Slice(count, userNameLen));
        }

        public override ArraySegment<byte> Write()
        {
            ArraySegment<byte> segment = SendBufferHelper.Open(4096);
            Span<byte> span = new Span<byte>(segment.Array, segment.Offset, segment.Count);

            ushort count = 0;
            bool success = true;

            count += sizeof(ushort);
            success &= BitConverter.TryWriteBytes(span.Slice(count, span.Length - count), this.packetId);
            count += sizeof(ushort);
            success &= BitConverter.TryWriteBytes(span.Slice(count, span.Length - count), this.userId);
            count += sizeof(long);

            ushort userNameLen = (ushort)Encoding.Unicode.GetByteCount(this.userName);
            success &= BitConverter.TryWriteBytes(span.Slice(count, span.Length - count), userNameLen);
            count += sizeof(ushort);
            Array.Copy(Encoding.Unicode.GetBytes(this.userName), 0, segment.Array, count, userNameLen);
            count += userNameLen;

            //ushort userNameLen = (ushort)Encoding.Unicode.GetBytes(this.userName, 0, this.userName.Length, segment.Array, segment.Offset + count + sizeof(ushort));

            success &= BitConverter.TryWriteBytes(span, count);

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
                        Console.WriteLine($"UserInfoReq : {packet.userId}, UserName : {packet.userName}");
                    }
                    break;
            }
        }

        public override void OnSend(int numOfBytes)
        {
        }
    }
}
