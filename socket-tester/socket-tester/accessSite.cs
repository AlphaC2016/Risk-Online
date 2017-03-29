using System;
using System.Net;
using System.IO;
using System.Text;
using System.Net.Sockets;
using System.Collections;
using System.Diagnostics;
using System.Management;

//IMPORTANT!!!!!! to download a vid, send http://www.youtubeinmp3.com/fetch/?video=<enter the link>

namespace socket_tester
{
    public static class socket_handler
    {
        static string downloadPrefix = "/fetch/?video=";

        public static Socket accessSite(string url)
        {
	        Socket sc = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
	
	        IPAddress[] IPOptions = Dns.GetHostAddresses(url);
	    
	        IPEndPoint endPoint = new IPEndPoint(IPOptions[0], 80);

            sc.Connect(endPoint);

            return sc;
        }

        public static void getSong(string youtubeLink, Socket sc)
        {
            string getMessage = "GET "; //constructing the HTTP get packet.
            getMessage += downloadPrefix;
            getMessage += youtubeLink;
            getMessage += " HTTP/1.1\r\n";

            getMessage += "Accept: */*\r\n";
            getMessage += "Accept-Language: en-US,en;q=0.7,he;q=0.3\r\n";
            getMessage += "User-Agent: Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 6.3; WOW64; Trident/7.0; .NET4.0C; .NET4.0E; .NET CLR 2.0.50727; .NET CLR 3.0.30729; .NET CLR 3.5.30729)\r\n";
            getMessage += "Host: www.youtubeinmp3.com\r\n";
            getMessage += "Connection: Keep-Alive\r\n";

            Console.WriteLine(getMessage);

            byte[] buf = Encoding.Default.GetBytes(getMessage);
            sc.Send(buf, 0, buf.Length, 0);

            byte[] response = new byte[1024];

            int rec = sc.Receive(buf, 0, buf.Length, 0);

            Array.Resize<byte>(ref buf, rec);

            Console.WriteLine(System.Text.Encoding.Default.GetString(buf));
        }
    }
}



