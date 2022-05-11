using System;
using System.Collections.Generic;
using System.Text;

namespace Server.Chat
{
    public class User
    {
        public ChatRoom Room { get; set; }
        public ClientSession Session { get; set; }
        public int Id { get; set; }
    }
}
