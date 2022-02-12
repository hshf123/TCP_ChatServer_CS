using Server;
using ServerCore;
using System;
using System.Net;

class ClientSession : PacketSession
{
    public int SessionId { get; set; }
    public ChatRoom Room { get; set; }

    public override void OnConnected(EndPoint endPoint)
    {
        Program.Room.Push(() => { Program.Room.Enter(this); });
    }

    public override void OnDisConnected(EndPoint endPoint)
    {
        SessionManager.Instance.Remove(this);
        if (Room != null)
        {
            ChatRoom room = Room;
            room.Push(() => { room.Leave(this); });
            Room = null;
        }
    }

    public override void OnRecvPacket(ArraySegment<byte> buffer)
    {
        ServerPacketManager.Instance.OnRecvPacket(this, buffer);
    }

    public override void OnSend(int numOfBytes)
    {
        // Console.WriteLine($"SendByte : {numOfBytes}");
    }
}