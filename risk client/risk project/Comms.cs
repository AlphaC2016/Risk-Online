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
        //private static StreamSocket sc;
        private static StreamReader reader = null;
        private static StreamSocket sc;

        private const string CONFIG_PATH = "config.txt";

        public const string SIGN_IN = "200"; //protocol values
        public const string SIGN_OUT = "201";
        public const string SIGN_IN_RES = "102";
        public const string SIGN_UP = "203";
        public const string SIGN_UP_RES = "104";
        public const string ACTIVE_ROOMS = "205";
        public const string ACTIVE_ROOMS_RES = "106";
        public const string GET_USERS = "207";
        public const string GET_USERS_RES = "108";
        public const string JOIN_ROOM = "209";
        public const string JOIN_ROOM_RES = "110";
        public const string LEAVE_ROOM = "211";
        public const string LEAVE_ROOM_RES = "1120";
        public const string NEW_ROOM = "213";
        public const string NEW_ROOM_RES = "114";
        public const string CLOSE_ROOM = "215";
        public const string CLOSE_ROOM_RES = "116";
        public const string START_GAME = "217";
        public const string SEND_QUESTIONS = "118";
        public const string ANSWER = "219";
        public const string ANSWER_RES = "120";
        public const string END_GAME = "121";
        public const string LEAVE_GAME = "222";
        public const string LEADERBOARDS = "223";
        public const string LEADERBOARDS_RES = "124";
        public const string PERSONAL_STATUS = "225";
        public const string PERSONAL_STATUS_RES = "126";
        public const string FORGOT = "227";
        public const string FORGOT_RES = "128";
        public const string EXIT_APP = "299";

        public const string MSG_TYPE_CODE_LENGTH = "3";

        public const string SIGN_IN_SUCCESS = "0";
        public const string SIGN_IN_WRONG_DETAILS = "1";
        public const string SIGN_IN_USER_IS_ALREADY_CONNECTED = "2";

        public const string SIGN_UP_SUCCESS = "0";
        public const string SIGN_UP_PASS_ILLEGAL = "1";
        public const string SIGN_UP_USERNAME_ALREADY_EXISTS = "2";
        public const string SIGN_UP_USERNAME_ILLEGAL = "3";
        public const string SIGN_UP_OTHER = "4";

        public const string CREATE_ROOM_SUCCESS = "0";
        public const string CREATE_ROOM_FAIL = "1";

        public const string JOIN_ROOM_SUCCESS = "1100";
        public const string JOIN_ROOM_FULL = "1101";
        public const string JOIN_ROOM_NOT_EXIST_OR_OTHER = "1102";

        public const string GET_ROOMS_SUCCESS = "106";

        public const string GET_USERS_OF_ROOM_SUCCESS = "108";
        public const string GET_USERS_OF_ROOM_FAIL = "1080";

        public const string CREATE_GAME_SUCCESS = "118";
        public const string CREATE_GAME_FAIL = "1180";

        //RESPONSE VERBOSE MEANINGS
        public const string LOGIN_SUCCESS = "0";
        public const string LOGIN_WRONG_DETAILS = "1";
        public const string LOGIN_ALREADY_CONNECTED = "2";

        public const string FORGOT_PASS_SUCCESS = "0";
        public const string FORGOT_PASS_EMAIL_DOES_NOT_EXIST = "1";
        public const string FORGOT_PASS_NO_PARAMS = "2";
        public const string FORGOT_PASS_OTHER = "3";

        public static async void InitSocket()
        {
            sc = new StreamSocket();
            HostName serverHost = new HostName("127.0.0.1");
            string port = "3000";
            await sc.ConnectAsync(serverHost, port);
        }

        public static void SendData(string message)
        {
            lock (sc)
            {
                StreamWriter writer = new StreamWriter(sc.OutputStream.AsStreamForWrite());
                writer.Write(message);
                writer.Flush();
                reader = null;
            }
        }

        public static string RecvData(int size)
        {
            lock (sc)
            {
                    if (reader == null)
                    {
                        Stream streamIn = sc.InputStream.AsStreamForRead();
                        reader = new StreamReader(streamIn);
                    }
                    char[] buf = new char[size];
                    reader.Read(buf, 0, size);
                    return new string(buf);
            }
            
        }

        public static string GetPaddedNumber(int num, int size)
        {
            return num.ToString().PadLeft(size, '0');
        }
    }
}
