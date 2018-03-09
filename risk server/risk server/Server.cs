using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using risk_server.Game_classes;

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
            Console.WriteLine("BOOTING UP...");
            InitSocket();
            _connectedUsers = new Dictionary<TcpClient, User>();
            _roomsList = new Dictionary<int, Room>();
            _queRecMessages = new Queue<RecievedMessage>();
            _db = new Database();
            _roomIdSequence = 0;
        }

        ~Server()
        {
            _socket.Stop();
        }


        //--------------------------SERVER INIT FUNCTIONS---------------------------
        public void Serve()
        {
            _socket.Start();
            Console.WriteLine("LISTENING...");
            Thread t = new Thread(HandleRecievedMessages);
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
            Console.WriteLine("ACCEPTED CLIENT FROM " + Helper.GetIp(client)+'\n');
            Thread t = new Thread(ClientHandler);
            t.Start(client);
        }

        private RecievedMessage BuildRecievedMessage(TcpClient client, string msgCode)
        {
            List<string> values = new List<string>();

            int size, i;

            Console.WriteLine("BUILDING MESSAGE " + msgCode + " FROM SOCKET " + Helper.GetIp(client)+'\n');


            switch (msgCode)
            {
                case Helper.SIGN_IN:
                    // username
                    size = Helper.GetIntPartFromSocket(client, 2);
                    values.Add(Helper.GetStringPartFromSocket(client, size));

                    // password
                    size = Helper.GetIntPartFromSocket(client, 2);
                    values.Add(Helper.GetStringPartFromSocket(client, size));
                    break;

                case Helper.SIGN_OUT:
                    // No values
                    break;

                case Helper.SIGN_UP:
                    // username
                    size = Helper.GetIntPartFromSocket(client, 2);
                    values.Add(Helper.GetStringPartFromSocket(client, size));

                    // password
                    size = Helper.GetIntPartFromSocket(client, 2);
                    values.Add(Helper.GetStringPartFromSocket(client, size));
                    break;

                case Helper.GET_ROOMS:
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

                case Helper.CREATE_ROOM:
                    // Room name
                    size = Helper.GetIntPartFromSocket(client, 2);
                    values.Add(Helper.GetStringPartFromSocket(client, size));

                    // No. of players
                    values.Add(Helper.GetStringPartFromSocket(client, 1));
                    break;

                case Helper.CLOSE_ROOM:
                    // No Values!
                    break;

                case Helper.START_GAME:
                    // No Values!
                    break;

                case Helper.FORCES_INIT:
                    for (i=0; i<Helper.TERRITORY_AMOUNT; i++)
                    {
                        values.Add(Helper.GetStringPartFromSocket(client, 2));
                    }
                    break;

                case Helper.SEND_REINFORCEMENTS:
                    size = int.Parse(Helper.GetStringPartFromSocket(client, 2));
                    for (i=0; i<size; i++)
                    {
                        values.Add(Helper.GetStringPartFromSocket(client, 2));
                        values.Add(Helper.GetStringPartFromSocket(client, 2));
                    }
                    break;

                case Helper.MOVE_FORCES:
                    values.Add(Helper.GetStringPartFromSocket(client, 2));
                    values.Add(Helper.GetStringPartFromSocket(client, 2));
                    values.Add(Helper.GetStringPartFromSocket(client, 2));
                    values.Add(Helper.GetStringPartFromSocket(client, 2));
                    break;

                case Helper.END_TURN:
                    // No Values!
                    break;

                case Helper.START_BATTLE:
                    values.Add(Helper.GetStringPartFromSocket(client, 2));
                    values.Add(Helper.GetStringPartFromSocket(client, 2));
                    break;

                case Helper.ROLL_DICE:
                    // No Values!
                    break;

                case Helper.BATTLE_RETREAT:
                    // No Values!
                    break;

                case Helper.VICTORY_MOVE_FORCES:
                    values.Add(Helper.GetStringPartFromSocket(client, 2));
                    values.Add(Helper.GetStringPartFromSocket(client, 2));
                    break;

                case Helper.QUIT_GAME:
                    // No Values!
                    break;

                case Helper.SEND_MESSAGE:
                    size = Helper.GetIntPartFromSocket(client, 2);
                    values.Add(Helper.GetStringPartFromSocket(client, size));
                    break;

                case Helper.LEADERBOARDS:
                    // No Values!
                    break;

                default:
                    throw new Exception("UNKNOWN MESSAGE CODE!");
            }

            RecievedMessage newMsg = new RecievedMessage(client, msgCode, values);

            newMsg.SetUser(GetUserBySocket(client));

            return newMsg;
        }


        /*--------------------------------------------------------------------------HANDLERS--------------------------------------------------------------------------*/

        //-------------------SIGN HANDLERS-----------------------------------------
        private void HandleSignIn(RecievedMessage msg)
        {
            string username = msg[0];
            string password = msg[1];

            try
            {
                if (!_db.IsUserAndPassMatch(username, password))
                {
                    Console.WriteLine("SIGN IN: USERNAME AND PASSWORD DO NOT MATCH FROM SOCKET "+Helper.GetIp(msg.GetSocket())+'\n');
                    Helper.SendData(Helper.SIGN_IN_WRONG_DETAILS.ToString(), msg.GetSocket());
                    return;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Helper.SendData(Helper.SIGN_IN_WRONG_DETAILS.ToString(), msg.GetSocket());
                return;
            }

            if (GetUserByName(username) != null)
            {
                Console.WriteLine("SIGN IN: USER IS ALREADY CONNECTED FROM SOCKET " + Helper.GetIp(msg.GetSocket()));
                Helper.SendData(Helper.SIGN_IN_USER_IS_ALREADY_CONNECTED.ToString(), msg.GetSocket());
                return;
            }

            User newUser = new User(username, msg.GetSocket());
            Helper.SendData(Helper.SIGN_IN_SUCCESS.ToString(), msg.GetSocket());
            Console.WriteLine("SIGN IN: SUCCESSFUL FROM SOCKET " +Helper.GetIp(msg.GetSocket()));
            _connectedUsers.Add(msg.GetSocket(), newUser);
        }

        private bool HandleSignUp(RecievedMessage msg)
        {
            string username = msg[0];
            string password = msg[1];
            TcpClient client = msg.GetSocket();
            try
            {
                if (_db.DoesUserExist(username))
                {
                    Helper.SendData(Helper.SIGN_UP_USERNAME_ALREADY_EXISTS.ToString(), client);
                    return false;
                }

                _db.AddNewUser(username, password);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Data);
                Helper.SendData(Helper.SIGN_UP_OTHER.ToString(), client);
            }

            Helper.SendData(Helper.SIGN_UP_SUCCESS.ToString(), msg.GetSocket());
            _connectedUsers.Add(client, new User(username, client));
            return true;
        }

        private void HandleSignOut(RecievedMessage msg)
        {
            if (msg.GetUser() != null)
            {
                HandleQuitGame(msg);
                HandleCloseRoom(msg);
                HandleLeaveRoom(msg);
                _connectedUsers.Remove(msg.GetUser().GetSocket());
            }
        }


        //------------------ROOM HANDLERS-----------------------------------------

        private bool HandleCreateRoom(RecievedMessage msg)
        {
            User user;
            if ((user = msg.GetUser()) != null)
            {
                bool success = user.CreateRoom(++_roomIdSequence, msg[0], int.Parse(msg[1]));
                
                if (success)
                {
                    _roomsList.Add(_roomIdSequence, user.GetRoom());
                    user.Send(Helper.CREATE_ROOM_SUCCESS+Helper.GetPaddedNumber(_roomIdSequence, 4));
                    return true;
                }
                else
                {
                    _roomIdSequence--;
                    user.Send(Helper.CREATE_ROOM_FAIL);
                    return false;
                }
            }
            return false;
        }

        private bool HandleCloseRoom(RecievedMessage msg)
        {

            User user = msg.GetUser();
            if (user!=null)
            {
                Room rm = user.GetRoom();
                if (rm != null)
                {
                    int code = user.CloseRoom();

                    if (code != -1)
                    {
                        _roomsList.Remove(code);
                        return true;
                    }
                }
            }
            return false;
        }

        private bool HandleJoinRoom(RecievedMessage msg)
        {
            User user = msg.GetUser();
            if (user != null)
            {
                int roomId = int.Parse(msg[0]);
                Room rm = GetRoomById(roomId);

                if (rm == null || !user.JoinRoom(rm))
                {
                
                    user.Send(Helper.JOIN_ROOM_NOT_EXIST_OR_OTHER.ToString());
                }
            }
            return false;
        }

        private bool HandleLeaveRoom(RecievedMessage msg)
        {
            if (msg.GetUser() != null && msg.GetUser().GetRoom() != null)
            {
                msg.GetUser().LeaveRoom();
                return true;
            }
            return false;
        }

        private void HandleGetUsersInRoom(RecievedMessage msg)
        {
            User user = msg.GetUser();
            if (user != null)
            {
                Room room = GetRoomById(int.Parse(msg[0]));
                if (room != null)
                {
                    user.Send(room.GetUserListMessage());
                }
                else
                {
                    user.Send(Helper.GET_USERS_OF_ROOM_FAIL.ToString());
                }
            }
        }

        private void HandleGetRooms(RecievedMessage msg)
        {
            User user = msg.GetUser();
            if (user != null)
            {
                int num = _roomsList.Count();

                string ans = Helper.GET_ROOMS_SUCCESS.ToString() + Helper.GetPaddedNumber(num, 4);

                foreach (KeyValuePair<int, Room> pair in _roomsList)
                {
                    ans += Helper.GetPaddedNumber(pair.Key, 4);
                    ans += Helper.GetPaddedNumber(pair.Value.GetName().Length, 2);
                    ans += pair.Value.GetName();
                }
                user.Send(ans);
            }
        }

        //----------------------GAME HANDLERS--------------------------------------

        private void HandleStartGame(RecievedMessage msg)
        {
            Room room = msg.GetUser().GetRoom();
            room.SendMessage(Helper.START_GAME_RES);
            Game gm = new Game(room.GetUsers());
            _roomsList.Remove(room.GetId());
        }

        private void HandleQuitGame(RecievedMessage msg)
        {
            if (msg.GetUser() != null)
            {
                Game gm = msg.GetUser().GetGame();
                if (gm != null)
                    gm.RemovePlayer(msg.GetUser());
            }
        }

        private void HandleForcesInit(RecievedMessage msg)
        {
            msg.GetUser().GetGame().HandleInitialReinforcements(msg);
        }
        
        private void HandleTurnReinforcements(RecievedMessage msg)
        {
            msg.GetUser().GetGame().HandleTurnRerinforcements(msg);
        }

        private void HandleMoveForces(RecievedMessage msg)
        {
            msg.GetUser().GetGame().HandleMoveForces(msg);
        }

        private void HandleEndTurn(RecievedMessage msg)
        {
            msg.GetUser().GetGame().HandleEndTurn(msg);
        }

        private void HandleStartBattle(RecievedMessage msg)
        {
            msg.GetUser().GetGame().HandleAttack(msg);
        }

        private void HandleRollDice(RecievedMessage msg)
        {
            Game gm = msg.GetUser().GetGame();
            User u = gm.HandleRollDice(msg);

            if (u != null)
            {
                _db.AddVictory(u.GetUsername());
                
            }

        }

        private void HandleBattleRetreat(RecievedMessage msg)
        {
            msg.GetUser().GetGame().EndBattle(false);
        }

        private void HandleVictoryMoveForces(RecievedMessage msg)
        {
            msg.GetUser().GetGame().HandleVictoryMoveForces(msg);
        }

        //------------------LEADERBOARDS HANDLER(S)--------------------------------

        private void HandleGetLeaderboards(RecievedMessage msg)
        {
            //[row, column]
            string[,] data = _db.GetLeaderboards();
            int i, j;
            string message = Helper.LEADERBOARDS_RES;

            for (i=0; i<data.GetLength(0); i++)
            {
                if (data[i,0] != null)
                {
                    message += Helper.GetPaddedNumber(data[i, 0].Length, 2);
                    message += data[i, 0];
                }
                else
                {
                    message += "00";
                }
                

                if (data[i,1] != null)
                {
                    message += Helper.GetPaddedNumber(int.Parse(data[i, 1]), 2);
                }
                else
                {
                    message += "00";
                }
            }
            msg.GetUser().Send(message);
            
        }

        //------------------------MESSAGE HANDLERS---------------------------------
        
        private void HandleUserMessage(RecievedMessage msg)
        {
            Game gm = msg.GetUser().GetGame();
            string content = Helper.RECEIVE_MESSAGE;
            string username = msg.GetUser().GetUsername();

            content += Helper.GetPaddedNumber(username.Length, 2) + username;
            content += Helper.GetPaddedNumber(msg[0].Length, 2) + msg[0];

            gm.HandleUserMessage(content);
        }

        //--------------------------CLIENT HANDLERS--------------------------------
        private void ClientHandler(object data)
        {
            TcpClient client = data as TcpClient;
            RecievedMessage msg;
            string code;
            try
            {
                code = Helper.GetMessageTypeCode(client);
            }
            catch (Exception e)
            {
                code = "";
            }
            try
            {
                while (code != "" && code != Helper.EXIT_APP)
                {
                    msg = BuildRecievedMessage(client, code);
                    msg.SetUser(GetUserBySocket(msg.GetSocket()));

                    lock (_queRecMessages)
                    {
                        _queRecMessages.Enqueue(msg);
                        Console.WriteLine("INSERTED NEW MESSAGE FROM IP {0}, CODE {1}", Helper.GetIp(client), code);
                    }
                    try
                    {
                        code = Helper.GetMessageTypeCode(client);
                    }
                    catch (Exception e)
                    {
                        code = "";
                    }
                }
                throw new Exception("OK");
            }
            catch (Exception e)
            {
                if (e.Message != "OK")
                {
                    Console.WriteLine("EXCEPTION FROM MESSAGE, IP {0} CODE {1}", client, code);
                    Console.WriteLine("FUNCTION: {0} MESSAGE: {1}", e.TargetSite, e.Message);
                }
                else
                {
                    msg = new RecievedMessage(client, Helper.EXIT_APP);
                    msg.SetUser(GetUserBySocket(msg.GetSocket()));
                    lock (_queRecMessages)
                    {
                        _queRecMessages.Enqueue(msg);
                        Console.WriteLine("INSERTED NEW MESSAGE FROM IP {0}, CODE {1}", Helper.GetIp(client), code);
                    }
                }
            }
        }

        private void HandleRecievedMessages()
        {
            RecievedMessage msg = null;
            while (true)
            {
                if (_queRecMessages.Count>0)
                {
                    lock(_queRecMessages)
                    {
                        msg = _queRecMessages.Dequeue();
                    }
                    Router(msg);
                }
            }
        }

        private void Router(RecievedMessage msg)
        {
            string messageCode = msg.GetMessageCode();

            switch (messageCode)
            {
                case Helper.SIGN_IN:
                    Console.WriteLine("router :: entering SignIn");
                    HandleSignIn(msg);
                    break;

                case Helper.SIGN_OUT:
                    Console.WriteLine("router :: entering SignOut");
                    HandleSignOut(msg);
                    break;

                case Helper.SIGN_UP:
                    Console.WriteLine("router :: entering SignUp");
                    if (!HandleSignUp(msg))
                    {
                        Console.WriteLine("router :: ErrorSignUp");
                    }
                    break;

                case Helper.GET_ROOMS:
                    Console.WriteLine("router :: entering GetRooms");
                    HandleGetRooms(msg);
                    break;

                case Helper.JOIN_ROOM:
                    Console.WriteLine("router :: entering JoinRoom");
                    HandleJoinRoom(msg);
                    break;

                case Helper.GET_USERS:
                    Console.WriteLine("router :: entering UsersInRoom");
                    HandleGetUsersInRoom(msg);
                    break;

                case Helper.CREATE_ROOM:
                    Console.WriteLine("router :: entering CreateRoom");
                    HandleCreateRoom(msg);
                    break;

                case Helper.LEAVE_ROOM:
                    Console.WriteLine("router :: entering LeaveRoom");
                    HandleLeaveRoom(msg);
                    break;

                case Helper.CLOSE_ROOM:
                    Console.WriteLine("router :: entering CloseRoom");
                    HandleCloseRoom(msg);
                    break;

                case Helper.START_GAME:
                    Console.WriteLine("router :: entering StartGame");
                    HandleStartGame(msg);
                    break;

                case Helper.FORCES_INIT:
                    Console.WriteLine("Router :: entering HandleForcesInit");
                    HandleForcesInit(msg);
                    break;

                case Helper.SEND_REINFORCEMENTS:
                    Console.WriteLine("router :: entering HandleTurnReinforcements");
                    HandleTurnReinforcements(msg);
                    break;

                case Helper.MOVE_FORCES:
                    Console.WriteLine("router :: entering HandleMoveForces");
                    HandleMoveForces(msg);
                    break;

                case Helper.END_TURN:
                    Console.WriteLine("router :: entering HandleEndTurn");
                    HandleEndTurn(msg);
                    break;

                case Helper.START_BATTLE:
                    Console.WriteLine("router :: entering HandleStartBattle");
                    HandleStartBattle(msg);
                    break;

                case Helper.ROLL_DICE:
                    Console.WriteLine("router :: entering HandleRollDice");
                    HandleRollDice(msg);
                    break;

                case Helper.BATTLE_RETREAT:
                    Console.WriteLine("routerr :: entering HandleBattleRetreat");
                    HandleBattleRetreat(msg);
                    break;

                case Helper.VICTORY_MOVE_FORCES:
                    Console.WriteLine("router :: entering HandleVictoryMoveForces");
                    HandleVictoryMoveForces(msg);
                    break;

                case Helper.SEND_MESSAGE:
                    Console.WriteLine("router :: entering HandleUserMessage");
                    HandleUserMessage(msg);
                    break;

                case Helper.QUIT_GAME:
                    Console.WriteLine("router :: entering HandleQuitGame");
                    HandleQuitGame(msg);
                    break;

                case Helper.LEADERBOARDS:
                    Console.WriteLine("router :: entering Leaderboards");
                    HandleGetLeaderboards(msg);
                    break;

                case Helper.EXIT_APP:
                    Console.WriteLine("router :: entering SafeDeleteUser");
                    this.SafeDeleteUser(msg);
                    break;

                default:
                    break;
            }
        }

        //--------------------------GETTERS----------------------------------------
        private User GetUserBySocket(TcpClient client)
        {
            try
            {
                return _connectedUsers[client];
            }
            catch (Exception e)
            {
                return null;
            }
        }

        private User GetUserByName(string username)
        {
            foreach (User user in _connectedUsers.Values)
            {
                if (user != null && user.GetUsername() == username)
                    return user;
            }
            return null;
        }

        private Room GetRoomById(int id)
        {
            return _roomsList[id];
        }

        private void SafeDeleteUser(RecievedMessage msg)
        {
            TcpClient sc = msg.GetSocket();
            HandleSignOut(msg);
            sc.Close();
        }

    }
}
