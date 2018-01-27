using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace risk_server.Game_classes
{
    class Territory
    {
        private string _name;
        private User _owner;
        public int Amount { get; set; }
        Dictionary<string, Territory> _adj;

        public string GetName() { return _name; }

        public Territory(string name)
        {
            _name = name;
            _adj = new Dictionary<string, Territory>();
        }

        public void AddAdj(Territory newTerritory)
        {
            _adj.Add(newTerritory.GetName(), newTerritory);
        }

        public User GetUser() { return _owner; }
        public void SetUser(User owner) { _owner = owner; }
        public IEnumerable<Territory> GetAdj() { return _adj.Values; }

        public bool IsAdj(Territory other)
        {
            bool found = false;

            for (int i=0; i<_adj.Count && !found; i++)
            {
                found = (_adj.ElementAt(i).Value == other);
            }

            return found;
        }
    }
}
