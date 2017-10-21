using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace risk_server
{
    class User
    {
        private string _username;
        private Room _currRoom;
        private Game _currGame;
        TcpClient _socket;

        public User(string name, TcpClient socket)
        {
            _username = name;
            _socket = socket;
            _currGame = null;
            _currRoom = null;
        }

        public void Send(string message)
        {
            Helper.SendData(message, _socket);
        }

        void SetGame(Game game)
        {
            _currGame = game;
            _currRoom = null;
        }


        public string GetUsername() { return _username; }

        public TcpClient GetSocket() { return _socket; }

        public Game GetGame() { return _currGame; }

        public Room GetRoom() { return _currRoom; }


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

        public bool JoinRoom(Room newRoom)
        {
            if (_currRoom != null) return false;

            if (!newRoom.JoinRoom(this)) return false;

            _currRoom = newRoom;

            return true;
        }


        public void LeaveRoom()
        {
            _currRoom.LeaveRoom(this);
            _currRoom = null;
        }

        public int CloseRoom()
        {
            if (_currRoom == null) return -1;

            int ans = _currRoom.GetId();
            _currRoom.CloseRoom(this);
            _currRoom = null;
            return ans;
        }

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
