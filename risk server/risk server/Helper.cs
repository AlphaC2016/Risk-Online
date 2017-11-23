using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace risk_server
{
    static class Helper
    {
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

        public const int MSG_TYPE_CODE_LENGTH = 3;

        public const string SIGN_IN_SUCCESS = "1020";
        public const string SIGN_IN_WRONG_DETAILS = "1021";
        public const string SIGN_IN_USER_IS_ALREADY_CONNECTED = "1022";

        public const string SIGN_UP_SUCCESS = "1040";
        public const string SIGN_UP_PASS_ILLEGAL = "1041";
        public const string SIGN_UP_USERNAME_ALREADY_EXISTS = "1042";
        public const string SIGN_UP_USERNAME_ILLEGAL = "1043";
        public const string SIGN_UP_OTHER = "1044";

        public const string CREATE_ROOM_SUCCESS = "1140";
        public const string CREATE_ROOM_FAIL = "1141";

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

        public const string DB_PATH = "MyDatabase.db";

        public static void SendData(string message, TcpClient client)
        {
            NetworkStream stream = client.GetStream();

            byte[] data = new ASCIIEncoding().GetBytes(message);
            stream.Write(data, 0, data.Length);
            stream.Flush();
            Console.WriteLine("MESSAGE " + message + " HAS BEEN SENT TO CLIENT ON " + GetIp(client));
        }


        private static string RecvData(int size, TcpClient client)
        {
            byte[] data = new byte[size + 1];
            client.GetStream().Read(data, 0, size);
            string ans = new ASCIIEncoding().GetString(data);
            return ans.Replace("\0", "");
        }

        public static string GetPaddedNumber(int num, int size)
        {
            return num.ToString().PadLeft(size, '0');
        }

        public static string GetMessageTypeCode(TcpClient client)
        {
            return RecvData(Helper.MSG_TYPE_CODE_LENGTH, client);
        }

        public static int GetIntPartFromSocket(TcpClient client, int size)
        {
            return int.Parse(RecvData(size, client));
        }

        public static string GetStringPartFromSocket(TcpClient client, int size)
        {
            return RecvData(size, client);
        }

        public static string GetIp(TcpClient client)
        {
            return ((IPEndPoint)client.Client.RemoteEndPoint).Address.ToString();
        }
    }
}
