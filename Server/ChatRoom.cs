using ServerCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Server
{
    class ChatRoom
    {
        object _lock = new object();
        List<ClientSession> _sessions = new List<ClientSession>();

        public void Enter(ClientSession session)
        {
            lock(_lock)
            {
                _sessions.Add(session);
                session.Room = this;
            }
        }

        public void Leave(ClientSession session)
        {
            lock(_lock)
            {
                _sessions.Remove(session);
            }
        }

        public void BroadCast(ClientSession session, string chat)
        {
            S_Chat pkt = new S_Chat();
            pkt.userId = session.SessionId;
            pkt.chat = chat;
            ArraySegment<byte> segment = pkt.Write();

            lock(_lock)
            {
                foreach (ClientSession cs in _sessions)
                    cs.Send(segment);
            }
        }
    }
}
