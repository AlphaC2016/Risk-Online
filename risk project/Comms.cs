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

        public static async void Send(string message)
        {
            IBuffer buf = Encoding.ASCII.GetBytes(message).AsBuffer();
            await sc.OutputStream.WriteAsync(buf);
            await sc.OutputStream.FlushAsync();
        }

        public static async Task<string> Recieve()
        {
            Stream streamIn = sc.InputStream.AsStreamForRead();
            StreamReader reader = new StreamReader(streamIn);
            return await reader.ReadLineAsync();
        }
    }
}
