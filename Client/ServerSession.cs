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
        public string userName;

        public struct TestInfo
        {
            public int testInt;
            public short testShort;
            public float testFloat;

            public bool Write(Span<byte> span, ref ushort count)
            {
                bool success = true;

                success &= BitConverter.TryWriteBytes(span.Slice(count, span.Length - count), testInt);
                count += sizeof(int);
                success &= BitConverter.TryWriteBytes(span.Slice(count, span.Length - count), testShort);
                count += sizeof(short);
                success &= BitConverter.TryWriteBytes(span.Slice(count, span.Length - count), testFloat);
                count += sizeof(float);

                return success;
            }

            public void Read(ReadOnlySpan<byte> read, ref ushort count)
            {
                testInt = BitConverter.ToInt32(read.Slice(count, read.Length - count));
                count += sizeof(int);
                testShort = BitConverter.ToInt16(read.Slice(count, read.Length - count));
                count += sizeof(short);
                testFloat = BitConverter.ToSingle(read.Slice(count, read.Length - count));
                count += sizeof(float);
            }
        }

        public List<TestInfo> list = new List<TestInfo>();

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
            count += userNameLen;

            ushort listLen = BitConverter.ToUInt16(read.Slice(count, read.Length - count));
            count += sizeof(ushort);
            list.Clear();
            for (int i = 0; i < listLen; i++)
            {
                TestInfo testInfo = new TestInfo();
                testInfo.Read(read, ref count);
                list.Add(testInfo);
            }
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

            success &= BitConverter.TryWriteBytes(span.Slice(count, span.Length - count), (ushort)list.Count);
            count += sizeof(ushort);
            foreach (TestInfo testInfo in list)
                testInfo.Write(span, ref count);

            success &= BitConverter.TryWriteBytes(span, count);

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
                    UserInfoReq packet = new UserInfoReq() { userId = 1798, userName = "LeafC" };
                    for (int i = 0; i < 4; i++)
                    {
                        UserInfoReq.TestInfo testInfo = new UserInfoReq.TestInfo() { testInt = (i + 1) * 1000, testShort = (short)i, testFloat = 1.5f };
                        packet.list.Add(testInfo);
                    }


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
