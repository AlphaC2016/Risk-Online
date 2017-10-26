﻿using System;
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

        public const int SIGN_IN = 200; //protocol values
        public const int SIGN_OUT = 201;
        public const int SIGN_IN_RES = 102;
        public const int SIGN_UP = 203;
        public const int SIGN_UP_RES = 104;
        public const int ACTIVE_ROOMS = 205;
        public const int ACTIVE_ROOMS_RES = 106;
        public const int GET_USERS = 207;
        public const int GET_USERS_RES = 108;
        public const int JOIN_ROOM = 209;
        public const int JOIN_ROOM_RES = 110;
        public const int LEAVE_ROOM = 211;
        public const int LEAVE_ROOM_RES = 1120;
        public const int NEW_ROOM = 213;
        public const int NEW_ROOM_RES = 114;
        public const int CLOSE_ROOM = 215;
        public const int CLOSE_ROOM_RES = 116;
        public const int START_GAME = 217;
        public const int SEND_QUESTIONS = 118;
        public const int ANSWER = 219;
        public const int ANSWER_RES = 120;
        public const int END_GAME = 121;
        public const int LEAVE_GAME = 222;
        public const int BEST_SCORES = 223;
        public const int BEST_SCORES_RES = 124;
        public const int PERSONAL_STATUS = 225;
        public const int PERSONAL_STATUS_RES = 126;
        public const int FORGOT = 227;
        public const int FORGOT_RES = 128;
        public const int EXIT_APP = 299;

        public const int MSG_TYPE_CODE_LENGTH = 3;

        public const int SIGN_IN_SUCCESS = 1020;
        public const int SIGN_IN_WRONG_DETAILS = 1021;
        public const int SIGN_IN_USER_IS_ALREADY_CONNECTED = 1022;

        public const int SIGN_UP_SUCCESS = 1040;
        public const int SIGN_UP_PASS_ILLEGAL = 1041;
        public const int SIGN_UP_USERNAME_ALREADY_EXISTS = 1042;
        public const int SIGN_UP_USERNAME_ILLEGAL = 1043;
        public const int SIGN_UP_OTHER = 1044;

        public const int CREATE_ROOM_SUCCESS = 1140;
        public const int CREATE_ROOM_FAIL = 1141;

        public const int JOIN_ROOM_SUCCESS = 1100;
        public const int JOIN_ROOM_FULL = 1101;
        public const int JOIN_ROOM_NOT_EXIST_OR_OTHER = 1102;

        public const int GET_ROOMS_SUCCESS = 106;

        public const int GET_USERS_OF_ROOM_SUCCESS = 108;
        public const int GET_USERS_OF_ROOM_FAIL = 1080;

        public const int CREATE_GAME_SUCCESS = 118;
        public const int CREATE_GAME_FAIL = 1180;

        //RESPONSE VERBOSE MEANINGS
        public const int LOGIN_SUCCESS = 0;
        public const int LOGIN_WRONG_DETAILS = 1;
        public const int LOGIN_ALREADY_CONNECTED = 2;

        public const int FORGOT_PASS_SUCCESS = 0;
        public const int FORGOT_PASS_EMAIL_DOES_NOT_EXIST = 1;
        public const int FORGOT_PASS_NO_PARAMS = 2;
        public const int FORGOT_PASS_OTHER = 3;

        public const string DB_PATH = "MyDatabase.db";

        public static void SendData(string message, TcpClient client)
        {
            NetworkStream stream = client.GetStream();

            byte[] data = new ASCIIEncoding().GetBytes(message);
            stream.Write(data, 0, data.Length);
            stream.Flush();
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

        public static int GetMessageTypeCode(TcpClient client)
        {
            return int.Parse(RecvData(Helper.MSG_TYPE_CODE_LENGTH, client));
        }

        public static int GetIntPartFromSocket(TcpClient client, int size)
        {
            return int.Parse(RecvData(size, client));
        }

        public static string GetStringPartFromSocket(TcpClient client, int size)
        {
            return RecvData(size, client);
        }
    }
}
