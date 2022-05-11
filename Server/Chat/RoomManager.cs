using System;
using System.Collections.Generic;
using System.Text;

namespace Server.Chat
{
    public class RoomManager
    {
        public static RoomManager Instance { get; } = new RoomManager();

        object _lock = new object();
        Dictionary<int, ChatRoom> _rooms = new Dictionary<int, ChatRoom>();
        int _roomId = 1;

        public ChatRoom Add()
        {
            ChatRoom room = new ChatRoom();

            lock (_lock)
            {
                room.RoomId = _roomId;
                _rooms.Add(_roomId, room);
                _roomId++;
            }
            return room;
        }

        public bool Remove(int roomId)
        {
            lock (_lock)
            {
                return _rooms.Remove(roomId);
            }
        }

        public ChatRoom Find(int roomId)
        {
            lock (_lock)
            {
                ChatRoom room = null;
                if (_rooms.TryGetValue(roomId, out room))
                    return room;
                return null;
            }
        }
    }
}
