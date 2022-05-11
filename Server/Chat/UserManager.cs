using System;
using System.Collections.Generic;
using System.Text;

namespace Server.Chat
{
    class UserManager
    {
        public static UserManager Instance { get; } = new UserManager();

        object _lock = new object();
        Dictionary<int, User> _users = new Dictionary<int, User>();
        int _userId = 1;

        public User Add()
        {
            User user = new User();

            lock (_lock)
            {
                user.Id = _userId;
                _users.Add(_userId, user);
                _userId++;
            }
            return user;
        }

        public bool Remove(int userId)
        {
            lock (_lock)
            {
                return _users.Remove(userId);
            }
        }

        public User Find(int userId)
        {
            lock (_lock)
            {
                User user = null;
                if (_users.TryGetValue(userId, out user))
                    return user;
                return null;
            }
        }
    }
}
