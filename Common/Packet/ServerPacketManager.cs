using Server;
using ServerCore;
using System;
using System.Collections.Generic;

class ServerPacketManager
{
    ServerPacketManager()
    {
        Register();
    }

    static ServerPacketManager _instance;
    public static ServerPacketManager Instance
    {
        get
        {
            if (_instance == null)
                _instance = new ServerPacketManager();
            return _instance;
        }
    }

    Dictionary<ushort, Action<PacketSession, ArraySegment<byte>>> _onRecv = new Dictionary<ushort, Action<PacketSession, ArraySegment<byte>>>();
    Dictionary<ushort, Action<PacketSession, IPacket>> _handler = new Dictionary<ushort, Action<PacketSession, IPacket>>();
        
    public void Register()
    {
        _onRecv.Add((ushort)PacketID.C_Chat, MakePacket<C_Chat>);
        _handler.Add((ushort)PacketID.C_Chat, PacketHandler.C_ChatHandler);
	
    }

    public void OnRecvPacket(PacketSession session, ArraySegment<byte> buffer)
    {
        ushort count = 0;
        ushort size = BitConverter.ToUInt16(buffer.Array, buffer.Offset);
        count += sizeof(ushort);
        ushort id = BitConverter.ToUInt16(buffer.Array, buffer.Offset + count);
        count += sizeof(ushort);

        Action<PacketSession, ArraySegment<byte>> action = null;
        if (_onRecv.TryGetValue(id, out action))
            action.Invoke(session, buffer);
    }

    void MakePacket<T>(PacketSession session, ArraySegment<byte> buffer) where T : IPacket, new()
    {
        T pkt = new T();
        pkt.Read(buffer);

        Action<PacketSession, IPacket> action = null;
        if (_handler.TryGetValue(pkt.Protocol, out action))
            action.Invoke(session, pkt);
    }
}
