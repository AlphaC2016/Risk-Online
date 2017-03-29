using System;
using System.Net;
using System.IO;
using System.Text;
using System.Net.Sockets;
using System.Collections;
using System.Diagnostics;
using System.Management;

public static Socket accessSite(string url)
{
	Socket sc = new Socket(AddressFamily.Internetwork, SocketType.Stream, ProtocolType.Tcp);
	
	IPAddress siteIP = Dns.GetHostAddress(url)[0];
	
	IPEndPoint endPoint = new IPEndPoint(siteIP, 80);
}


