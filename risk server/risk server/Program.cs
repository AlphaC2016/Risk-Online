﻿using System;
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
        }
    }
}
