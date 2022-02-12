using ServerCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Server
{
    class PacketHandler
    {
        public static void C_ChatHandler(PacketSession session, IPacket packet)
        {
            ClientSession cs = session as ClientSession;
            C_Chat pkt = packet as C_Chat;

            if (cs.Room == null)
                return;

            cs.Room.BroadCast(cs, pkt.chat);
        }
    }
}
