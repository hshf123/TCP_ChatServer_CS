using System;
using System.Collections.Generic;
using System.Text;

namespace ChatClient
{
    class SessionManager
    {
        static SessionManager _session = new SessionManager();
        public static SessionManager Instance { get { return _session; } }

        List<ServerSession> _sessions = new List<ServerSession>();
        object _lock = new object();

        public void SendForEach(C_Chat packet)
        {
            lock(_lock)
            {
                foreach(ServerSession session in _sessions)
                {
                    ArraySegment<byte> segment = packet.Write();

                    session.Send(segment);
                }
            }
        }

        public ServerSession Generate()
        {
            lock(_lock)
            {
                ServerSession session = new ServerSession();
                _sessions.Add(session);
                return session;
            }
        }
    }
}
