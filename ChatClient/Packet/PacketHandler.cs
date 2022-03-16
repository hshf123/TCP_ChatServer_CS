using ServerCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace ChatClient
{
    class PacketHandler
    {
        public static void S_ChatHandler(PacketSession session, IPacket packet)
        {
            S_Chat pkt = packet as S_Chat;

            Form1.Form.WriteMessage(pkt.userName, pkt.chat);
        }
    }
}
