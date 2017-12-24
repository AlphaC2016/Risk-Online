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

        public Game(List<User> users)
        {
            _players = new List<User>(users);

            InitMap();
        }

        private void InitMap()
        {
            BuildMap();
            SetUsersOnMap();
            SendInitMessage();
        }

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

        private bool findUser(Territory t, string name)
        {
            return (t.GetUser().GetUsername() == name);
        }

        public void SendMessage(string message)
        {
            foreach (User user in _players)
            {
                user.Send(message);
            }
        }
    }
}
