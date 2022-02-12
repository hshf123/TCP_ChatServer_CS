using Client;
using ServerCore;
using System;
using System.Net;
using System.Threading;

class ServerSession : PacketSession
{
    public override void OnConnected(EndPoint endPoint)
    {
        
    }

    public override void OnDisConnected(EndPoint endPoint)
    {
        Console.WriteLine($"OnDisConnected : {endPoint.ToString()}");
    }

    public override void OnRecvPacket(ArraySegment<byte> buffer)
    {
        ClientPacketManager.Instance.OnRecvPacket(this, buffer);
    }

    public override void OnSend(int numOfBytes)
    {
        // Console.WriteLine($"Send : {numOfBytes}");
    }
}
