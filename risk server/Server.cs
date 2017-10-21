﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace risk_server
{
    class Server
    {
        private TcpListener _socket;
        private Dictionary<TcpClient, User> _connectedUsers;
        private Dictionary<int, Room> _roomsList;
        private Database _db;
        private Queue<RecievedMessage> _queRecMessages;

        private static int _roomIdSequence;

        //----------------------CONSTRUCTORS AND DESTRUCTORS------------------------
        public Server()
        {
            InitSocket();
            _connectedUsers = new Dictionary<TcpClient, User>();
            _roomsList = new Dictionary<int, Room>();
            _db = new Database();
            _roomIdSequence++;
        }

        ~Server()
        {
            _socket.Stop();
        }


        //--------------------------SERVER INIT FUNCTIONS---------------------------
        public void Serve()
        {
            _socket.Start();
            Thread t = new Thread(this.HandleRecievedMessages);
            t.Start();

            while (true)
            {
                Console.WriteLine("waiting for new client...");
                Accept();
            }
        }

        private void InitSocket()
        {
            _socket = new TcpListener(IPAddress.Parse("0.0.0.0"), 3000);
        }

        private void Accept()
        {
            TcpClient client = _socket.AcceptTcpClient();
            Thread t = new Thread(ClientHandler);
            t.Start(client);
        }


        private RecievedMessage BuildRecievedMessage(TcpClient client, int msgCode)
        {
            List<string> values = new List<string>();

            int sizes;

            //cout << "Building message " << msgCode << " from socket " << client;


            switch (msgCode)
            {
                case Helper.SIGN_IN:
                    // username
                    sizes = Helper.GetIntPartFromSocket(client, 2);
                    values.Add(Helper.GetStringPartFromSocket(client, sizes));

                    // password
                    sizes = Helper.GetIntPartFromSocket(client, 2);
                    values.Add(Helper.GetStringPartFromSocket(client, sizes));
                    break;

                case Helper.SIGN_OUT:
                    // No values
                    break;

                case Helper.SIGN_UP:
                    // username
                    sizes = Helper.GetIntPartFromSocket(client, 2);
                    values.Add(Helper.GetStringPartFromSocket(client, sizes));

                    // password
                    sizes = Helper.GetIntPartFromSocket(client, 2);
                    values.Add(Helper.GetStringPartFromSocket(client, sizes));

                    // email
                    sizes = Helper.GetIntPartFromSocket(client, 2);
                    values.Add(Helper.GetStringPartFromSocket(client, sizes));
                    break;

                case Helper.ACTIVE_ROOMS:
                    // No Values!
                    break;

                case Helper.GET_USERS:
                    // Room ID (even that it is a number it comes as a string)
                    values.Add(Helper.GetStringPartFromSocket(client, 4));
                    break;

                case Helper.JOIN_ROOM:
                    // Room ID (even that it is a number it comes as a string)
                    values.Add(Helper.GetStringPartFromSocket(client, 4));
                    break;

                case Helper.LEAVE_ROOM:
                    // No Values!
                    break;

                case Helper.NEW_ROOM:
                    // Room name
                    sizes = Helper.GetIntPartFromSocket(client, 2);
                    values.Add(Helper.GetStringPartFromSocket(client, sizes));

                    // No. of players
                    values.Add(Helper.GetStringPartFromSocket(client, 1));

                    // No. of questions
                    values.Add(Helper.GetStringPartFromSocket(client, 2));

                    // Time to answer
                    values.Add(Helper.GetStringPartFromSocket(client, 2));
                    break;

                case Helper.CLOSE_ROOM:
                    // No Values!
                    break;

                case Helper.START_GAME:
                    // No Values!
                    break;

                case Helper.ANSWER:
                    // Answer No.
                    values.Add(Helper.GetStringPartFromSocket(client, 1));

                    // Answer time
                    values.Add(Helper.GetStringPartFromSocket(client, 2));
                    break;

                case Helper.LEAVE_GAME:
                    // No Values!
                    break;

                case Helper.BEST_SCORES:
                    // No Values!
                    break;

                case Helper.PERSONAL_STATUS:
                    // No Values!
                    break;

                default:
                    throw new Exception("UNKNOWN MESSAGE CODE!");
            }

            RecievedMessage newMsg = new RecievedMessage(client, msgCode, values);

            newMsg.SetUser(GetUserBySocket(client));

            return newMsg;
        }




        //--------------------------------HANDLERS---------------------------------

        private User HandleSignIn(RecievedMessage msg)
        {
            string username = msg.GetValues()[0];
            string password = msg.GetValues()[1];

            try
            {
                if (!_db.isUserAndPassMatch(username, password))
                {
                    Helper.SendData(Helper.SIGN_IN_WRONG_DETAILS.ToString(), msg.GetSocket());
                    return null;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Helper.SendData(Helper.SIGN_IN_WRONG_DETAILS.ToString(), msg.GetSocket());
                return null;
            }

            if (Get)


        }

        private void ClientHandler(object data)
        {
            TcpClient client = data as TcpClient;

            int code = Helper.GetMessageTypeCode(client);

            while (code != 0 && code != Helper.EXIT_APP)
            {
                RecievedMessage msg = BuildRecievedMessage(client, code);

                lock (_queRecMessages)
                {
                    _queRecMessages.Enqueue(msg);
                }
            }
        }

        private void HandleRecievedMessages()
        {

        }

        //--------------------------GETTERS----------------------------------------
        private User GetUserBySocket(TcpClient client)
        {
            return _connectedUsers[client];
        }

        private User GetUserByName(string username)
        {
            foreach (User user in _connectedUsers.Values)
            {
                if (user != null && user.)
            }
        }
    }
}
