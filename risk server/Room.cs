using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace risk_server
{
    class Room
    {
        private List<User> _users;
        User _admin;
        int _maxUsers;
        string _name;
        int _id;

        public Room(int id, User admin, string name, int maxUsers)
        {
            _id = id;
            _admin = admin;
            _name = name;
            _maxUsers = maxUsers;

            _users = new List<User>();
            _users.Add(admin);
        }


        private void SendMessage(string message)
        {
            foreach (User user in _users)
            {
                user.Send(message);
            }
        }

        private void SendMessage(User excludeUser, string message)
        {
            foreach (User user in _users)
            {
                if (user != excludeUser)
                    user.Send(message);
            }
        }


        public List<User> GetUsers() { return _users; }

        public int GetId() { return _id; }

        public string GetName() { return _name; }

        public string GetUserListMessage()
        {
            string ans = Helper.GET_USERS_OF_ROOM_SUCCESS.ToString() + Helper.GetPaddedNumber(_users.Count, 1);

            foreach (User user in _users)
            {
                string username = user.GetUsername();
                ans += Helper.GetPaddedNumber(username.Length, 2) + username;
            }
            return ans;
        }

        public bool JoinRoom(User user)
        {
            if (_users.Count == _maxUsers)
            {
                user.Send(Helper.JOIN_ROOM_FULL.ToString());
                return false;
            }

            user.Send(Helper.JOIN_ROOM_SUCCESS.ToString());
            _users.Add(user);
            SendMessage(GetUserListMessage());
            return true;
        }

        public void LeaveRoom(User user)
        {
            if (_users.Contains<User>(user))
            {
                _users.Remove(user);
                user.Send(Helper.LEAVE_ROOM_RES.ToString());
                SendMessage(GetUserListMessage());
            }
        }

        public int CloseRoom(User user)
        {
            if (user != _admin)
            {
                return -1;
            }
            SendMessage(Helper.CLOSE_ROOM_RES.ToString());

            foreach (User u in _users)
            {
                if (u!=_admin)
                {
                    u.ClearRoom();
                }
            }
            return _id;
        }
    }
}
