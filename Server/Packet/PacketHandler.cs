using Google.Protobuf;
using Google.Protobuf.Protocol;
using Server.Chat;
using ServerCore;
using System;
using System.Collections.Generic;
using System.Text;

class PacketHandler
{
    public static void C_ChatHandler(PacketSession session, IMessage packet)
    {
        ClientSession cs = session as ClientSession;
        C_Chat pkt = packet as C_Chat;

        ChatRoom room = cs.User.Room;
        room.Broadcast(pkt);
    }
}
