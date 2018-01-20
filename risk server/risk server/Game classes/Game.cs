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

        public Game(List<User> users)
        {
            Random r = new Random();
            _players = new List<User>(users);

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
                i = 0;
                while (i<limit)
                {
                    t = _territories.ElementAt(r.Next(42)).Value;
                    if (t.GetUser() == null)
                    {
                        t.SetUser(p);
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
        public void RemovePlayer(User u)
        {
            _players.Remove(u);
            SetUsersOnMap();
            SendInitMessage();
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
                    _territories.ElementAt(i).Value.SetAmount(int.Parse(msg[i]));
                }

                if (done)
                    done = (_territories.ElementAt(i).Value.GetAmount() != 0);
            }

            if (done)
            {
                Console.WriteLine("THE TURN SHOULD START NOW.");
                //StartTurn();
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
                message += Helper.GetPaddedNumber(pair.Key.Length, 2) + pair.Key;
                message += Helper.GetPaddedNumber(pair.Value.GetAmount(), 2);
            }
        }
    }
}
