using ServerCore;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading;

public enum PacketID
{
    C_Chat = 1,
	S_Chat = 2,
	S_EnterUser = 3,
	
}

interface IPacket
{
	ushort Protocol { get; }
	ArraySegment<byte> Write();
	void Read(ArraySegment<byte> segment);
}


class C_Chat : IPacket
{
    public string userName;
	public string chat;

    public ushort Protocol { get { return (ushort)PacketID.C_Chat; } }

    public void Read(ArraySegment<byte> segment)
    {
        ReadOnlySpan<byte> read = new ReadOnlySpan<byte>(segment.Array, segment.Offset, segment.Count);
        ushort count = 0;

        count += sizeof(ushort);
        count += sizeof(ushort);
        ushort userNameLen = BitConverter.ToUInt16(read.Slice(count, read.Length - count));
		count += sizeof(ushort);
		this.userName = Encoding.Unicode.GetString(read.Slice(count, userNameLen));
		count += userNameLen;
		ushort chatLen = BitConverter.ToUInt16(read.Slice(count, read.Length - count));
		count += sizeof(ushort);
		this.chat = Encoding.Unicode.GetString(read.Slice(count, chatLen));
		count += chatLen;
    }

    public ArraySegment<byte> Write()
    {
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);
        Span<byte> span = new Span<byte>(segment.Array, segment.Offset, segment.Count);

        ushort count = 0;
        bool success = true;

        count += sizeof(ushort);
        success &= BitConverter.TryWriteBytes(span.Slice(count, span.Length - count), (ushort)PacketID.C_Chat);
        count += sizeof(ushort);
        ushort userNameLen = (ushort)Encoding.Unicode.GetByteCount(this.userName);
		success &= BitConverter.TryWriteBytes(span.Slice(count, span.Length - count), userNameLen);
		count += sizeof(ushort);
		Array.Copy(Encoding.Unicode.GetBytes(this.userName), 0, segment.Array, count, userNameLen);
		count += userNameLen;
		ushort chatLen = (ushort)Encoding.Unicode.GetByteCount(this.chat);
		success &= BitConverter.TryWriteBytes(span.Slice(count, span.Length - count), chatLen);
		count += sizeof(ushort);
		Array.Copy(Encoding.Unicode.GetBytes(this.chat), 0, segment.Array, count, chatLen);
		count += chatLen;

        success &= BitConverter.TryWriteBytes(span, count);

        if (success == false)
            return null;

        return SendBufferHelper.Close(count);
    }
}

class S_Chat : IPacket
{
    public int userId;
	public string userName;
	public string chat;

    public ushort Protocol { get { return (ushort)PacketID.S_Chat; } }

    public void Read(ArraySegment<byte> segment)
    {
        ReadOnlySpan<byte> read = new ReadOnlySpan<byte>(segment.Array, segment.Offset, segment.Count);
        ushort count = 0;

        count += sizeof(ushort);
        count += sizeof(ushort);
        this.userId = BitConverter.ToInt32(read.Slice(count, read.Length - count));
		count += sizeof(int);
		ushort userNameLen = BitConverter.ToUInt16(read.Slice(count, read.Length - count));
		count += sizeof(ushort);
		this.userName = Encoding.Unicode.GetString(read.Slice(count, userNameLen));
		count += userNameLen;
		ushort chatLen = BitConverter.ToUInt16(read.Slice(count, read.Length - count));
		count += sizeof(ushort);
		this.chat = Encoding.Unicode.GetString(read.Slice(count, chatLen));
		count += chatLen;
    }

    public ArraySegment<byte> Write()
    {
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);
        Span<byte> span = new Span<byte>(segment.Array, segment.Offset, segment.Count);

        ushort count = 0;
        bool success = true;

        count += sizeof(ushort);
        success &= BitConverter.TryWriteBytes(span.Slice(count, span.Length - count), (ushort)PacketID.S_Chat);
        count += sizeof(ushort);
        success &= BitConverter.TryWriteBytes(span.Slice(count, span.Length - count), userId);
		count += sizeof(int);
		ushort userNameLen = (ushort)Encoding.Unicode.GetByteCount(this.userName);
		success &= BitConverter.TryWriteBytes(span.Slice(count, span.Length - count), userNameLen);
		count += sizeof(ushort);
		Array.Copy(Encoding.Unicode.GetBytes(this.userName), 0, segment.Array, count, userNameLen);
		count += userNameLen;
		ushort chatLen = (ushort)Encoding.Unicode.GetByteCount(this.chat);
		success &= BitConverter.TryWriteBytes(span.Slice(count, span.Length - count), chatLen);
		count += sizeof(ushort);
		Array.Copy(Encoding.Unicode.GetBytes(this.chat), 0, segment.Array, count, chatLen);
		count += chatLen;

        success &= BitConverter.TryWriteBytes(span, count);

        if (success == false)
            return null;

        return SendBufferHelper.Close(count);
    }
}

class S_EnterUser : IPacket
{
    public int userCount;

    public ushort Protocol { get { return (ushort)PacketID.S_EnterUser; } }

    public void Read(ArraySegment<byte> segment)
    {
        ReadOnlySpan<byte> read = new ReadOnlySpan<byte>(segment.Array, segment.Offset, segment.Count);
        ushort count = 0;

        count += sizeof(ushort);
        count += sizeof(ushort);
        this.userCount = BitConverter.ToInt32(read.Slice(count, read.Length - count));
		count += sizeof(int);
    }

    public ArraySegment<byte> Write()
    {
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);
        Span<byte> span = new Span<byte>(segment.Array, segment.Offset, segment.Count);

        ushort count = 0;
        bool success = true;

        count += sizeof(ushort);
        success &= BitConverter.TryWriteBytes(span.Slice(count, span.Length - count), (ushort)PacketID.S_EnterUser);
        count += sizeof(ushort);
        success &= BitConverter.TryWriteBytes(span.Slice(count, span.Length - count), userCount);
		count += sizeof(int);

        success &= BitConverter.TryWriteBytes(span, count);

        if (success == false)
            return null;

        return SendBufferHelper.Close(count);
    }
}

