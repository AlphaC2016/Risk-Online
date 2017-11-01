using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Net.Sockets;
using Windows.Networking.Sockets;
using Windows.Networking;
using Windows.Storage.Streams;
using System.Runtime.InteropServices.WindowsRuntime;
using System.IO;

namespace risk_project
{
    static class Comms
    {
        private static StreamSocket sc;

        static Comms()
        {
            initSocket();
        }

        private static async void initSocket()
        {
            sc = new StreamSocket();
            HostName serverHost = new HostName("127.0.0.1");
            string port = "9736";
            await sc.ConnectAsync(serverHost, port);
        }

        public static  void SendData(string message)
        {
            StreamWriter writer = new StreamWriter(sc.OutputStream.AsStreamForWrite());
            writer.Write(message);
            writer.Flush();
        }

        public static string RecvData(int size)
        {
            Stream streamIn = sc.InputStream.AsStreamForRead();
            StreamReader reader = new StreamReader(streamIn);
            char[] buf = new char[size + 1];
            reader.Read(buf, 0, size);
            return new string(buf);
        }
    }
}
