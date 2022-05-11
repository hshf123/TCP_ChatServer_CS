using ChatClient;
using Google.Protobuf;
using Google.Protobuf.Protocol;
using ServerCore;
using System;
using System.Collections.Generic;
using System.Text;

class PacketHandler
{
    public static void S_ChatHandler(PacketSession session, IMessage packet)
    {
        S_Chat pkt = packet as S_Chat;

        Form1.Form.WriteMessage(pkt.UserName, pkt.Chat);
    }

    public static void S_EnterUserHandler(PacketSession session, IMessage packet)
    {
        S_EnterUser pkt = packet as S_EnterUser;

        Form1.Form.UserCount(pkt.UserCount);
    }
}
