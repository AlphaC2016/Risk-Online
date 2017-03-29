using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace socket_tester
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Socket sc = socket_handler.accessSite("www.youtubeinmp3.com");
                socket_handler.getSong("https://www.youtube.com/watch?v=b8-tXG8KrWs", sc);
                //http://www.youtubeinmp3.com/fetch/?video=https://www.youtube.com/watch?v=b8-tXG8KrWs
                sc.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

        }
    }
}
