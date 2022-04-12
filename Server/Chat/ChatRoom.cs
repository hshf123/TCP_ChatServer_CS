using System;
using System.Collections.Generic;
using System.Text;

namespace Server.Chat
{
    class ChatRoom
    {
        object _lock = new object();
        public int RoomId { get; set; }

        Dictionary<int, User> _users = new Dictionary<int, User>();

        public void EnterRoom(User user)
        {

        }

        public void LeaveRoom(User user)
        {

        }
    }
}
