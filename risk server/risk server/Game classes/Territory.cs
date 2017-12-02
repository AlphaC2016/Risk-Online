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
        private int _amount;
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
    }
}
