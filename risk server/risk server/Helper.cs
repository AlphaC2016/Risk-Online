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
        public const string GET_ROOMS = "205";
        public const string GET_ROOMS_RES = "106";
        public const string GET_USERS = "207";
        public const string GET_USERS_RES = "108";
        public const string JOIN_ROOM = "209";
        public const string JOIN_ROOM_RES = "110";
        public const string LEAVE_ROOM = "211";
        public const string ACK = "1120";
        public const string CREATE_ROOM = "213";
        public const string CREATE_ROOM_RES = "114";
        public const string CLOSE_ROOM = "215";
        public const string CLOSE_ROOM_RES = "116";
        public const string START_GAME = "217";
        public const string START_GAME_RES = "118";
        public const string INIT_MAP = "119";
        public const string QUIT_GAME = "220";
        public const string FORCES_INIT = "221";
        public const string START_TURN = "122";
        public const string SEND_REINFORCEMENTS = "223";
        public const string MOVE_FORCES = "224";
        public const string MOVE_FORCES_RES = "125";
        public const string END_TURN = "226";
        public const string START_BATTLE = "227";
        public const string START_BATTLE_RES = "128";
        public const string BATTLE_RETREAT = "229";
        public const string ROLL_DICE = "230";
        public const string ROLL_DICE_RES = "131";
        public const string END_BATTLE = "132";
        public const string VICTORY_MOVE_FORCES = "233";
        public const string UPDATE_MAP = "134";
        public const string END_GAME = "135";
        public const string SEND_MESSAGE = "236";
        public const string RECEIVE_MESSAGE = "137";
        public const string LEADERBOARDS = "238";
        public const string LEADERBOARDS_RES = "139";
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

        public const int TERRITORY_AMOUNT = 42;

        public const string DB_PATH = "../../MyDatabase.db";

        public static void SendData(string message, TcpClient client)
        {
            try
            {
                NetworkStream stream = client.GetStream();

                byte[] data = new ASCIIEncoding().GetBytes(message);
                stream.Write(data, 0, data.Length);
                stream.Flush();
                Console.WriteLine("MESSAGE " + message + " HAS BEEN SENT TO CLIENT ON " + GetIp(client));
            }
            catch (Exception)
            {
            }
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

        public static List<string>[] GetMapData()
        {
            string[] rawData = Properties.Resources.mapdata.Replace("\r\n", "\n").Split('\n');
            List<string>[] ans = new List<string>[rawData.Length];

            for (int i = 0; i < rawData.Length; i++)
            {
                ans[i] = new List<string>(rawData[i].Split(','));
            }

            return ans;
        }
    }
}
