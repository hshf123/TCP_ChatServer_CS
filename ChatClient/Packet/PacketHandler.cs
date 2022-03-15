using ServerCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Client
{
    class PacketHandler
    {
        public static void S_ChatHandler(PacketSession session, IPacket packet)
        {
            S_Chat pkt = packet as S_Chat;

            if(pkt.userId==1)
                Console.WriteLine($"{pkt.userId} : {pkt.chat}");
        }
    }
}
