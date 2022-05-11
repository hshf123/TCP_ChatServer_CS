using Google.Protobuf;
using Google.Protobuf.Protocol;
using System;
using System.Collections.Generic;
using System.Text;

namespace Server.Chat
{
    public class ChatRoom
    {
        object _lock = new object();
        public int RoomId { get; set; }

        Dictionary<int, User> _users = new Dictionary<int, User>();

        public void EnterRoom(User user)
        {
            if (user == null)
                return;

            lock(_lock)
            {
                _users.Add(user.Id, user);
                user.Room = this;
                // 방의 인원 수 전송
                {
                    S_EnterUser enterPacket = new S_EnterUser();
                    enterPacket.UserCount = _users.Count;
                    foreach(User u in _users.Values)
                        u.Session.Send(enterPacket);
                }
            }
        }

        public void LeaveRoom(User user)
        {
            if (user == null)
                return;

            lock (_lock)
            {
                if (_users.Remove(user.Id, out user) == false)
                    return;
                user.Room = null;

                // 방의 인원 수 전송
                {
                    S_EnterUser enterPacket = new S_EnterUser();
                    enterPacket.UserCount = _users.Count;
                    foreach (User u in _users.Values)
                        u.Session.Send(enterPacket);
                }
            }
        }

        public void Broadcast(IMessage message)
        {
            C_Chat packet = message as C_Chat;
            S_Chat pkt = new S_Chat();
            pkt.UserName = packet.UserName;
            pkt.Chat = packet.Chat;

            lock(_lock)
            {
                foreach (User user in _users.Values)
                    user.Session.Send(pkt);
            }
        }
    }
}
