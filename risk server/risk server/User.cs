using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using risk_server.Game_classes;

namespace risk_server
{

    class User
    {
        private string _username;
        private Room _currRoom;
        private Game _currGame;
        TcpClient _socket;

        /// <summary>
        /// the default constructor.
        /// </summary>
        /// <param name="name">the user's name.</param>
        /// <param name="socket">the user's comm socekt.</param>
        public User(string name, TcpClient socket)
        {
            _username = name;
            _socket = socket;
            _currGame = null;
            _currRoom = null;
        }

        /// <summary>
        /// this function sends a message to the user.
        /// </summary>
        /// <param name="message">the message.</param>
        public void Send(string message)
        {
            Helper.SendData(message, _socket);
        }

        /*simple setter.*/
        public void SetGame(Game game)
        {
            _currGame = game;
            _currRoom = null;
        }

        /* simple get functions.*/
        public string GetUsername() { return _username; }

        public TcpClient GetSocket() { return _socket; }

        public Game GetGame() { return _currGame; }

        public Room GetRoom() { return _currRoom; }

        /// <summary>
        /// this function tries to create a room controlled by the current user.
        /// </summary>
        /// <param name="roomId">the room's id.</param>
        /// <param name="name">the room's name.</param>
        /// <param name="maxUsers">the max amount of users in the room.</param>
        /// <returns></returns>
        public bool CreateRoom(int roomId, string name, int maxUsers)
        {
            if (_currRoom != null)
            {
                Send(Helper.CREATE_ROOM_FAIL.ToString());
                return false;
            }

            _currRoom = new Room(roomId, this, name, maxUsers);
            return true;
        }

        /// <summary>
        /// this function adds the user to a room.
        /// </summary>
        /// <param name="newRoom">the new room for the user.</param>
        /// <returns>returns true if worked, false otherwise.</returns>
        public bool JoinRoom(Room newRoom)
        {
            if (_currRoom != null) return false;

            if (!newRoom.JoinRoom(this)) return false;

            _currRoom = newRoom;

            return true;
        }

        /// <summary>
        /// this function removes the user from his room.
        /// </summary>
        public void LeaveRoom()
        {
            _currRoom.LeaveRoom(this);
            _currRoom = null;
        }

        /// <summary>
        /// this function attempts to close the room the user is in.
        /// </summary>
        /// <returns>returns the room code if worked, -1 otherwise.</returns>
        public int CloseRoom()
        {
            if (_currRoom == null) return -1;

            int ans = _currRoom.GetId();
            ans = _currRoom.CloseRoom(this);
            _currRoom = null;
            return ans;
        }

        /// <summary>
        /// this function removes the user from the current game.
        /// </summary>
        /// <returns>true unless the room's empty.</returns>
        public bool LeaveGame()
        {
            if (_currGame == null) return false;

            //currGame.LeaveGame(this);
            _currGame = null;
            return true;
        }

        public void ClearRoom() { _currRoom = null; }
    }
}
