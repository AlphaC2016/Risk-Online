using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace risk_server.Game_classes
{
    class Game
    {
        List<User> _players;
        Dictionary<string, Territory> _territories;
        Dictionary<User, int> _territoryCount;

        int currAttackerIndex;

        Territory src, dst;
        bool srcRolled, dstRolled;

        public Game(List<User> users)
        {
            Random r = new Random();
            _players = new List<User>(users);
            _territoryCount = new Dictionary<User, int>();

            foreach (User p in _players)
            {
                p.SetGame(this);
            }

            currAttackerIndex = r.Next(_players.Count);

            InitMap();
        }

        /// <summary>
        /// This function activates all the initialization functions.
        /// </summary>
        private void InitMap()
        {
            BuildMap();
            SetUsersOnMap();
            SendInitMessage();
        }

        /// <summary>
        /// This function takes the data from the mapdata.csv file and builds the basic data structure.
        /// </summary>
        private void BuildMap()
        {
            List<string>[] data = Helper.GetMapData();
            int i, j;
            _territories = new Dictionary<string, Territory>();

            for (i = 0; i < data.Length; i++)
            {
                _territories.Add(data[i][0], new Territory(data[i][0]));
            }

            i = 0;
            foreach (Territory t in _territories.Values)
            {
                for (j = 1; j < data[i].Count; j++)
                {
                    t.AddAdj(_territories[data[i][j]]);
                }
                i++;
            }
        }

        /// <summary>
        /// This function randomly assigns the territories to the players.
        /// </summary>
        private void SetUsersOnMap()
        {
            Random r = new Random();
            int i;
            Territory t;
            int limit = 42 / _players.Count;

            foreach (User p in _players)
            {
                _territoryCount.Add(p, 0);
                i = 0;
                while (i<limit)
                {
                    t = _territories.ElementAt(r.Next(42)).Value;
                    if (t.GetUser() == null)
                    {
                        t.SetUser(p);
                        _territoryCount[p]++;
                        i++;
                    }
                }
                
            }
        }

        /// <summary>
        /// This function sends the 119 message to all the players.
        /// </summary>
        private void SendInitMessage()
        {
            string message = Helper.INIT_MAP;

            message += _players.Count;
            foreach (User p in _players)
            {
                message += Helper.GetPaddedNumber(p.GetUsername().Length, 2);
                message += p.GetUsername();
            }

            int i;
            foreach (Territory t in _territories.Values)
            {
                string temp = t.GetUser().GetUsername();
                for (i=0; i<_players.Count; i++)
                {
                    if (temp == _players[i].GetUsername())
                    {
                        message += i;
                    }
                }
            }
            SendMessage(message);
        }
        /// <summary>
        /// This function removes a player from the game.
        /// </summary>
        /// <param name="u">The player to be removed.</param>
        public User RemovePlayer(User u)
        {
            _players.Remove(u);

            _territoryCount.Clear();
            foreach (Territory t in _territories.Values)
                t.SetUser(null);

            if (_players.Count != 0)
            {
                SetUsersOnMap();
                SendInitMessage();
            }

            if (_players.Count == 1)
                return CheckEndGame(_players[0]);

            return null;
        }

        /// <summary>
        /// This function checks whether a user owns a specific territory.
        /// </summary>
        /// <param name="t">The territory to be checked.</param>
        /// <param name="name">The wanted user.</param>
        /// <returns>True if the user owns it, false otherwise.</returns>
        private bool findUser(Territory t, string name)
        {
            return (t.GetUser().GetUsername() == name);
        }

        /// <summary>
        /// This function sends a message to all the players.
        /// </summary>
        /// <param name="message">The message to be sent.</param>
        private void SendMessage(string message)
        {
            foreach (User user in _players)
            {
                user.Send(message);
            }
        }


        /// <summary>
        /// This function handles a player's initial reinforcements demand.
        /// </summary>
        /// <param name="msg">The player's message.</param>
        public void HandleInitialReinforcements(RecievedMessage msg)
        {
            bool done = true;
            for (int i=0; i<Helper.TERRITORY_AMOUNT; i++)
            {
                if (msg[i] != "00")
                {
                    _territories.ElementAt(i).Value.Amount = int.Parse(msg[i]);
                }

                if (done)
                    done = (_territories.ElementAt(i).Value.Amount != 0);
            }

            if (done)
            {
                Console.WriteLine("THE TURN SHOULD START NOW.");
                StartTurn();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void HandleUserMessage(string message)
        {
            SendMessage(message);
        }

        private void StartTurn()
        {
            SendUpdate();
            string message = Helper.START_TURN;
            string curr = _players[currAttackerIndex].GetUsername();

            message += Helper.GetPaddedNumber(curr.Length, 2) + curr;
            SendMessage(message);
        }

        public void SendUpdate()
        {
            string message = Helper.UPDATE_MAP;
            foreach (var pair in _territories)
            {
                message += Helper.GetPaddedNumber(pair.Value.GetUser().GetUsername().Length, 2) + pair.Value.GetUser().GetUsername();
                message += Helper.GetPaddedNumber(pair.Value.Amount, 2);
            }
            SendMessage(message);
        }

        public void HandleTurnRerinforcements(RecievedMessage msg)
        {
            int index, value;
            for (int i=0; i<msg.Length; i+=2)
            {
                index = int.Parse(msg[i]);
                value = int.Parse(msg[i + 1]);

                _territories.ElementAt(index).Value.Amount = value;
            }
            SendUpdate();
        }

        public void HandleMoveForces(RecievedMessage msg)
        {
            int index1 = int.Parse(msg[0]);
            int index2 = int.Parse(msg[1]);
            int amount1 = int.Parse(msg[2]);
            int amount2 = int.Parse(msg[3]);
            Territory t1 = _territories.ElementAt(index1).Value;
            Territory t2 = _territories.ElementAt(index2).Value;

            string message = Helper.MOVE_FORCES_RES;

            if (AreConnected(t1, t2))
            {
                t1.Amount = amount1;
                t2.Amount = amount2;
                message += "0";
            }
            else
            {
                message += "1";
            }
            msg.GetUser().Send(message);
        }

        public void HandleAttack(RecievedMessage msg)
        {
            
            src = _territories.ElementAt(int.Parse(msg[0])).Value;
            dst = _territories.ElementAt(int.Parse(msg[1])).Value;

            if (src.Amount < 2)
            {
                msg.GetUser().Send(Helper.START_BATTLE_RES + "1");
            }
            else if (src.GetUser() != msg.GetUser())
            {
                msg.GetUser().Send(Helper.START_BATTLE_RES + "2");
            }
            else if (dst.GetUser() == msg.GetUser())
            {
                msg.GetUser().Send(Helper.START_BATTLE_RES + "3");
            }
            else if (!src.IsAdj(dst))
            {
                msg.GetUser().Send(Helper.START_BATTLE_RES + "4");
            }
            else
            {
                string message = Helper.START_BATTLE_RES + "0" + msg[0] + msg[1];
                SendMessage(message);
            }

            if (src.Amount == 0)
                EndBattle(false);
            else if (dst.Amount == 0)
                EndBattle(true);
        }

        public User HandleRollDice(RecievedMessage msg)
        {
            if (msg.GetUser() == src.GetUser())
            {
                srcRolled = true;
            }
            else if (msg.GetUser() == dst.GetUser())
            {
                dstRolled = true;
            }

            if (srcRolled && dstRolled)
            {
                Random r = new Random();
                string message = Helper.ROLL_DICE_RES;

                int atkCount = Math.Min(3, src.Amount - 1);
                int defCount = Math.Min(2, dst.Amount);

                int[] atk = new int[3];
                int[] def = new int[2];
                int i;

                for (i=0; i<3; i++)
                {
                    if (i < atkCount)
                        atk[i] = r.Next(1, 7);
                    else
                        atk[i] = 0;
                }

                for (i = 0; i < 2; i++)
                {
                    if (i < defCount)
                        def[i] = r.Next(1, 7);
                    else
                        def[i] = 0;
                }

                Array.Sort(atk);
                Array.Reverse(atk);
                Array.Sort(def);
                Array.Reverse(def);

                for (i=0; i< 2; i++)
                {
                    if (atk[i] > 0 && def[i] > 0)
                    {
                        if (atk[i] > def[i])
                            dst.Amount--;
                        else
                            src.Amount--;
                    }
                }

                foreach (int x in atk)
                    message += x;

                foreach (int x in def)
                    message += x;

                message += Helper.GetPaddedNumber(src.Amount, 2);
                message += Helper.GetPaddedNumber(dst.Amount, 2);

                src.GetUser().Send(message);
                dst.GetUser().Send(message);

                dstRolled = srcRolled = false;

                if (dst.Amount < 1)
                    return EndBattle(true);
                else if (src.Amount < 2)
                    return EndBattle(false);
            }
            return null;
        }

        public void HandleVictoryMoveForces(RecievedMessage msg)
        {
            src.Amount = int.Parse(msg[0]);
            dst.Amount = int.Parse(msg[1]);

            _territoryCount[src.GetUser()]++;
            _territoryCount[dst.GetUser()]--;

            src.SetUser(dst.GetUser());

            if (_territoryCount[dst.GetUser()] == Helper.TERRITORY_AMOUNT)
            {
                string message = Helper.END_GAME;
                string username = src.GetUser().GetUsername();
                message += Helper.GetPaddedNumber(username.Length, 2);
                message += username;

                SendMessage(message);
            }

            src = dst = null;

            SendUpdate();
        }

        public User EndBattle(bool success)
        {
            SendUpdate();
            string message = Helper.END_BATTLE;
            message += Convert.ToInt32(!success);
            SendMessage(message);

            if (!success)
                src = dst = null;
            else
            {
                _territoryCount[dst.GetUser()]--;
                _territoryCount[src.GetUser()]++;
                dst.SetUser(src.GetUser());
                return CheckEndGame(src.GetUser());
            }
            return null;
        }

        private User CheckEndGame(User user)
        {
            string message;
            if (_territoryCount[user] == Helper.TERRITORY_AMOUNT)
            {
                string name = user.GetUsername();
                message = Helper.END_GAME;
                message += Helper.GetPaddedNumber(name.Length, 2);
                message += name;
                SendMessage(message);

                foreach (User u in _players)
                    u.LeaveGame();

                return user;
            }
            return null;
        }

        public void HandleEndTurn(RecievedMessage msg)
        {
            do
            {
                currAttackerIndex = (currAttackerIndex + 1) % _players.Count;
            } while (_territoryCount[_players[currAttackerIndex]] == 0);
            StartTurn();
        }

        private bool AreConnected(Territory t1, Territory t2, int count = 10)
        {
            if (count > 0)
            {
                if (t1.IsAdj(t2) && t1.GetUser() == t2.GetUser())
                    return true;

                else
                {
                    foreach (Territory t in t1.GetAdj())
                    {
                        if (t.GetUser() == t1.GetUser() && AreConnected(t, t2, count - 1))
                            return true;
                    }
                    return false;
                }
            }
            return false;
        }
    }
}
