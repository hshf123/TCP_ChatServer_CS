using ServerCore;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading;

public enum PacketID
{
    UserInfoReq = 1,

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
            this.testInt = BitConverter.ToInt32(read.Slice(count, read.Length - count));
            count += sizeof(int);
            this.testShort = BitConverter.ToInt16(read.Slice(count, read.Length - count));
            count += sizeof(short);
            this.testFloat = BitConverter.ToSingle(read.Slice(count, read.Length - count));
            count += sizeof(float);
        }
    }

    public List<TestInfo> testInfos = new List<TestInfo>();

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
        testInfos.Clear();
        ushort testInfoLen = BitConverter.ToUInt16(read.Slice(count, read.Length - count));
        count += sizeof(ushort);
        for (int i = 0; i < testInfoLen; i++)
        {
            TestInfo testInfo = new TestInfo();
            testInfo.Read(read, ref count);
            testInfos.Add(testInfo);
        }
    }

    public override ArraySegment<byte> Write()
    {
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);
        Span<byte> span = new Span<byte>(segment.Array, segment.Offset, segment.Count);

        ushort count = 0;
        bool success = true;

        count += sizeof(ushort);
        success &= BitConverter.TryWriteBytes(span.Slice(count, span.Length - count), (ushort)PacketID.UserInfoReq);
        count += sizeof(ushort);
        success &= BitConverter.TryWriteBytes(span.Slice(count, span.Length - count), userId);
        count += sizeof(long);
        ushort userNameLen = (ushort)Encoding.Unicode.GetByteCount(this.userName);
        success &= BitConverter.TryWriteBytes(span.Slice(count, span.Length - count), userNameLen);
        count += sizeof(ushort);
        Array.Copy(Encoding.Unicode.GetBytes(this.userName), 0, segment.Array, count, userNameLen);
        count += userNameLen;
        success &= BitConverter.TryWriteBytes(span.Slice(count, span.Length - count), (ushort)testInfos.Count);
        count += sizeof(ushort);
        foreach (TestInfo testInfo in testInfos)
            testInfo.Write(span, ref count);

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

        switch ((PacketID)id)
        {
            case PacketID.UserInfoReq:
                {
                    UserInfoReq packet = new UserInfoReq();
                    packet.Read(buffer);
                    Console.WriteLine($"UserInfoReq : {packet.userId}, UserName : {packet.userName}");
                    foreach (UserInfoReq.TestInfo test in packet.testInfos)
                        Console.WriteLine($"Int : {test.testInt} Short : {test.testShort} Float : {test.testFloat}");
                }
                break;
        }
    }

    public override void OnSend(int numOfBytes)
    {
    }
}