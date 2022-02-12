using System;
using System.Collections.Generic;
using System.Text;

namespace PacketGenerator
{
    class PacketFormat
    {
        #region GenPackets.cs
        // {0} 패킷목록 {1} genPackets
        public static string fileFormat =
@"using ServerCore;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading;

public enum PacketID
{{
    {0}
}}

interface IPacket
{{
	ushort Protocol {{ get; }}
	ArraySegment<byte> Write();
	void Read(ArraySegment<byte> segment);
}}

{1}
";
        // {0} 클래스 이름, {1} 번호
        public static string fileEnumFormat =
@"{0} = {1},";

        // {0} 클래스이름 {1}멤버변수 {2} Read {3} Write
        public static string packetFormat =
 @"
class {0} : IPacket
{{
    {1}

    public ushort Protocol {{ get {{ return (ushort)PacketID.{0}; }} }}

    public void Read(ArraySegment<byte> segment)
    {{
        ReadOnlySpan<byte> read = new ReadOnlySpan<byte>(segment.Array, segment.Offset, segment.Count);
        ushort count = 0;

        count += sizeof(ushort);
        count += sizeof(ushort);
        {2}
    }}

    public ArraySegment<byte> Write()
    {{
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);
        Span<byte> span = new Span<byte>(segment.Array, segment.Offset, segment.Count);

        ushort count = 0;
        bool success = true;

        count += sizeof(ushort);
        success &= BitConverter.TryWriteBytes(span.Slice(count, span.Length - count), (ushort)PacketID.{0});
        count += sizeof(ushort);
        {3}

        success &= BitConverter.TryWriteBytes(span, count);

        if (success == false)
            return null;

        return SendBufferHelper.Close(count);
    }}
}}
";

        // {0} 데이터형식 {1} 변수명
        public static string memberFormat =
@"public {0} {1};";

        // {0} 변수명 {1} ToInt~ {2} 데이터형식
        public static string readFormat =
@"this.{0} = BitConverter.{1}(read.Slice(count, read.Length - count));
count += sizeof({2});";

        // {0} 변수명 {1} 데이터형식
        public static string writeFormat =
@"success &= BitConverter.TryWriteBytes(span.Slice(count, span.Length - count), {0});
count += sizeof({1});";

        // {0} 변수명 {}
        public static string readStringFormat =
@"ushort {0}Len = BitConverter.ToUInt16(read.Slice(count, read.Length - count));
count += sizeof(ushort);
this.{0} = Encoding.Unicode.GetString(read.Slice(count, {0}Len));
count += {0}Len;";

        // {0} 변수 이름
        public static string writeStringFormat =
@"ushort {0}Len = (ushort)Encoding.Unicode.GetByteCount(this.{0});
success &= BitConverter.TryWriteBytes(span.Slice(count, span.Length - count), {0}Len);
count += sizeof(ushort);
Array.Copy(Encoding.Unicode.GetBytes(this.{0}), 0, segment.Array, count, {0}Len);
count += {0}Len;";

        // {0} 리스트이름 {1} 리스트이름(첫번째 소문자) {2} 멤버변수 {3} write {4} read
        public static string memberListFormat =
@"public struct {0}
{{
    {2}

    public bool Write(Span<byte> span, ref ushort count)
    {{
        bool success = true;

        {3}

        return success;
    }}

    public void Read(ReadOnlySpan<byte> read, ref ushort count)
    {{
        {4}
    }}
}}

public List<{0}> {1}s = new List<{0}>();";

        // {0} 구조체 이름 {1} 리스트 이름
        public static string readListFormat =
@"{1}s.Clear();
ushort {1}Len = BitConverter.ToUInt16(read.Slice(count, read.Length - count));
count += sizeof(ushort);
for (int i = 0; i < {1}Len; i++)
{{
    {0} {1} = new {0}();
    {1}.Read(read, ref count);
    {1}s.Add({1});
}}";
        // {0} 구조체 이름 {1} 리스트 이름
        public static string writeListFormat =
@"success &= BitConverter.TryWriteBytes(span.Slice(count, span.Length - count), (ushort){1}s.Count);
count += sizeof(ushort);
foreach ({0} {1} in {1}s)
    {1}.Write(span, ref count);";

        // {0} 변수 이름 {1} 변수 형식
        public static string readByteFormat =
@"this.{0} = ({1})segment.Array[segment.Offset + count];
count += sizeof({1});";

        // {0} 변수 이름 {1} 변수 형식
        public static string writeByteFormat =
@"segment.Array[segment.Offset + count] = (byte)this.{0};
count += sizeof({1});";
        #endregion

        #region PacketManager

        // {0} Register
        public static string clientPacketManagerFormat =
@"using Client;
using ServerCore;
using System;
using System.Collections.Generic;

class ClientPacketManager
{{
    ClientPacketManager()
    {{
        Register();
    }}

    static ClientPacketManager _instance;
    public static ClientPacketManager Instance
    {{
        get
        {{
            if (_instance == null)
                _instance = new ClientPacketManager();
            return _instance;
        }}
    }}

    Dictionary<ushort, Action<PacketSession, ArraySegment<byte>>> _onRecv = new Dictionary<ushort, Action<PacketSession, ArraySegment<byte>>>();
    Dictionary<ushort, Action<PacketSession, IPacket>> _handler = new Dictionary<ushort, Action<PacketSession, IPacket>>();
        
    public void Register()
    {{
        {0}
    }}

    public void OnRecvPacket(PacketSession session, ArraySegment<byte> buffer)
    {{
        ushort count = 0;
        ushort size = BitConverter.ToUInt16(buffer.Array, buffer.Offset);
        count += sizeof(ushort);
        ushort id = BitConverter.ToUInt16(buffer.Array, buffer.Offset + count);
        count += sizeof(ushort);

        Action<PacketSession, ArraySegment<byte>> action = null;
        if (_onRecv.TryGetValue(id, out action))
            action.Invoke(session, buffer);
    }}

    void MakePacket<T>(PacketSession session, ArraySegment<byte> buffer) where T : IPacket, new()
    {{
        T pkt = new T();
        pkt.Read(buffer);

        Action<PacketSession, IPacket> action = null;
        if (_handler.TryGetValue(pkt.Protocol, out action))
            action.Invoke(session, pkt);
    }}
}}
";
        public static string serverPacketManagerFormat =
@"using Server;
using ServerCore;
using System;
using System.Collections.Generic;

class ServerPacketManager
{{
    ServerPacketManager()
    {{
        Register();
    }}

    static ServerPacketManager _instance;
    public static ServerPacketManager Instance
    {{
        get
        {{
            if (_instance == null)
                _instance = new ServerPacketManager();
            return _instance;
        }}
    }}

    Dictionary<ushort, Action<PacketSession, ArraySegment<byte>>> _onRecv = new Dictionary<ushort, Action<PacketSession, ArraySegment<byte>>>();
    Dictionary<ushort, Action<PacketSession, IPacket>> _handler = new Dictionary<ushort, Action<PacketSession, IPacket>>();
        
    public void Register()
    {{
        {0}
    }}

    public void OnRecvPacket(PacketSession session, ArraySegment<byte> buffer)
    {{
        ushort count = 0;
        ushort size = BitConverter.ToUInt16(buffer.Array, buffer.Offset);
        count += sizeof(ushort);
        ushort id = BitConverter.ToUInt16(buffer.Array, buffer.Offset + count);
        count += sizeof(ushort);

        Action<PacketSession, ArraySegment<byte>> action = null;
        if (_onRecv.TryGetValue(id, out action))
            action.Invoke(session, buffer);
    }}

    void MakePacket<T>(PacketSession session, ArraySegment<byte> buffer) where T : IPacket, new()
    {{
        T pkt = new T();
        pkt.Read(buffer);

        Action<PacketSession, IPacket> action = null;
        if (_handler.TryGetValue(pkt.Protocol, out action))
            action.Invoke(session, pkt);
    }}
}}
";

        // {0} 패킷이름
        public static string registerManagerFormat =
@"_onRecv.Add((ushort)PacketID.{0}, MakePacket<{0}>);
        _handler.Add((ushort)PacketID.{0}, PacketHandler.{0}Handler);";

        #endregion
    }
}
