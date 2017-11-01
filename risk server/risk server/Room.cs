using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace risk_server
{
    /// <summary>
    /// This class represents a game room, that contains a group of users before they play a game.
    /// </summary>

    /* Fields:
     * _users = the list of users in the room.
     * _admin = the administrator of the room (whomever built it)
     * _maxUsers = the maximal amount of users in the room.
     * _name = the name of the room.
     * _id = a field used to access the room in the server.
     */
    class Room
    {
        private List<User> _users;
        User _admin;
        int _maxUsers;
        string _name;
        int _id;

        /// <summary>
        /// the default constructor.
        /// </summary>
        /// <param name="id">the room's id.</param>
        /// <param name="admin">the administrator of the room.</param>
        /// <param name="name">the name of the room.</param>
        /// <param name="maxUsers">the max amount of users in this room.</param>
        public Room(int id, User admin, string name, int maxUsers)
        {
            _id = id;
            _admin = admin;
            _name = name;
            _maxUsers = maxUsers;

            _users = new List<User>();
            _users.Add(admin);
        }

        /// <summary>
        /// This function sends a message to all users in th room -
        /// a neater alternative to using a NetworkStream directly.
        /// </summary>
        /// <param name="message">the message to be sent.</param>
        private void SendMessage(string message)
        {
            foreach (User user in _users)
            {
                user.Send(message);
            }
        }

        /// <summary>
        /// This function works exacty the same as the original, but allows you to exclude a user from the list.
        /// </summary>
        /// <param name="excludeUser">the user to be excluded.</param>
        /// <param name="message">the </param>
        private void SendMessage(User excludeUser, string message)
        {
            foreach (User user in _users)
            {
                if (user != excludeUser)
                    user.Send(message);
            }
        }


        //returns all users.
        public List<User> GetUsers() { return _users; }

        //returns the room's id.
        public int GetId() { return _id; }

        //returns the room's name.
        public string GetName() { return _name; }


        /// <summary>
        /// this function generates a 108 message that contains all current users.
        /// </summary>
        /// <returns>the 108 message.</returns>
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

        /// <summary>
        /// this function attempts to add a user to the room.
        /// </summary>
        /// <param name="user">the user to be added.</param>
        /// <returns>returns whether the operation worked.</returns>
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

        /// <summary>
        /// this function removes a user from the room.
        /// </summary>
        /// <param name="user">the user to be removed.</param>
        public void LeaveRoom(User user)
        {
            if (_users.Contains<User>(user))
            {
                _users.Remove(user);
                user.Send(Helper.LEAVE_ROOM_RES.ToString());
                SendMessage(GetUserListMessage());
            }
        }

        /// <summary>
        /// this function shuts the room down.
        /// </summary>
        /// <param name="user">the user who initiated the shutdown.</param>
        /// <returns>returns the room's id code or -1 if failed.</returns>
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
