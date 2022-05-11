using Google.Protobuf;
using Google.Protobuf.Protocol;
using Server;
using Server.Chat;
using ServerCore;
using System;
using System.Net;

public class ClientSession : PacketSession
{
    public int SessionId { get; set; }
    public User User { get; set; }

    public void Send(IMessage packet)
    {
        string msgName = packet.Descriptor.Name.Replace("_", string.Empty);
        MsgId msgId = (MsgId)Enum.Parse(typeof(MsgId), msgName);

        ushort size = (ushort)packet.CalculateSize(); // 사이즈
        byte[] sendBuffer = new byte[size + 4];
        Array.Copy(BitConverter.GetBytes((ushort)(size + 4)), 0, sendBuffer, 0, sizeof(ushort));
        Array.Copy(BitConverter.GetBytes((ushort)msgId), 0, sendBuffer, 2, sizeof(ushort));
        Array.Copy(packet.ToByteArray(), 0, sendBuffer, 4, size);

        Send(new ArraySegment<byte>(sendBuffer));
    }

    public override void OnConnected(EndPoint endPoint)
    {
        User = UserManager.Instance.Add();
        User.Session = this;
        RoomManager.Instance.Find(1).EnterRoom(User);
    }

    public override void OnDisConnected(EndPoint endPoint)
    {
        SessionManager.Instance.Remove(this);
        RoomManager.Instance.Find(1).LeaveRoom(User);
    }

    public override void OnRecvPacket(ArraySegment<byte> buffer)
    {
        PacketManager.Instance.OnRecvPacket(this, buffer);
    }

    public override void OnSend(int numOfBytes)
    {

    }
}