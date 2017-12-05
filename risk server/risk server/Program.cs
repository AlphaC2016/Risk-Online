using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace risk_server
{
    class Program
    {
        static void Main(string[] args)
        {
            Server s = new Server();
            s.Serve();
            //List<User> users = new List<User>();
            //users.Add(new User("alpha", null));
            //users.Add(new User("bravo", null));
            //Game_classes.Game gm = new Game_classes.Game(users);

        }
    }
}
