using ServerCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Server
{
    class ChatRoom : IJobQueue
    {
        List<ClientSession> _sessions = new List<ClientSession>();
        JobQueue _jobQueue = new JobQueue();
        List<ArraySegment<byte>> _pendingList = new List<ArraySegment<byte>>();

        public void Enter(ClientSession session)
        {
            _sessions.Add(session);
            session.Room = this;

            int userCount = _sessions.Count;
            S_EnterUser packet = new S_EnterUser();
            packet.userCount = userCount;
            _pendingList.Add(packet.Write());
        }

        public void Leave(ClientSession session)
        {
            _sessions.Remove(session);
        }

        public void BroadCast(ClientSession session, C_Chat packet)
        {
            S_Chat pkt = new S_Chat();
            pkt.userId = session.SessionId;
            pkt.userName = packet.userName;
            pkt.chat = packet.chat;
            ArraySegment<byte> segment = pkt.Write();

            _pendingList.Add(segment);
        }

        public void Flush()
        {
            foreach (ClientSession cs in _sessions)
                cs.Send(_pendingList);

            _pendingList.Clear();
        }

        public void Push(Action job)
        {
            _jobQueue.Push(job);
        }
    }
}
