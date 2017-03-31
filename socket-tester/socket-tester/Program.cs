using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

//http://www.youtubeinmp3.com/fetch/?video=https://www.youtube.com/watch?v=b8-tXG8KrWs

namespace socket_tester
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                //Socket sc = SocketHandler.accessSite("www.youtubeinmp3.com");
                SocketHandler.getSong("https://www.youtube.com/watch?v=b8-tXG8KrWs");
                
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}
