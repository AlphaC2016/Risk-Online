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
using Windows.ApplicationModel.Core;
using Windows.UI.Popups;

namespace risk_project
{
    /// <summary>
    /// This static class handles all back-end communiction with the server.
    /// It includes all protocol codes and send/receive functions.
    /// </summary>
    static class Comms
    {
        //private static StreamSocket sc;
        private static StreamReader reader = null;
        private static StreamSocket sc;
        private const string CONFIG_PATH = @"Assets/Data/config.txt";

        private static string ip = "192.168.1.34";
        private static string port = "3000";

        //these are the protocol code values. a matching list is found on the server.
        public const string SIGN_IN = "200";
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
        public const string LEAVE_ROOM_RES = "112";
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

        public const string MSG_TYPE_CODE_LENGTH = "3";


        

        //RESPONSE VERBOSE MEANINGS

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

        public const string LOGIN_SUCCESS = "0";
        public const string LOGIN_WRONG_DETAILS = "1";
        public const string LOGIN_ALREADY_CONNECTED = "2";

        public const string FORGOT_PASS_SUCCESS = "0";
        public const string FORGOT_PASS_EMAIL_DOES_NOT_EXIST = "1";
        public const string FORGOT_PASS_NO_PARAMS = "2";
        public const string FORGOT_PASS_OTHER = "3";


        private static HostName serverHost;

        /// <summary>
        /// This function handles the initial connection to the server.
        /// </summary>
        public static void InitSocket()
        {
            sc = new StreamSocket();
            
            serverHost = new HostName(ip);

            Connect();
        }

        private async static void Connect()
        {
            CancellationTokenSource cts = new CancellationTokenSource();
            try
            {
                cts.CancelAfter(2000);
                await sc.ConnectAsync(serverHost, port).AsTask(cts.Token);
            }
            catch (Exception)
            {
                MessageDialog dialog = new MessageDialog("Error: Cannot connect to server.");
                dialog.Commands.Add(new UICommand("Try again", new UICommandInvokedHandler(CommandInvokedHandler)));
                dialog.Commands.Add(new UICommand("Exit App", new UICommandInvokedHandler(CommandInvokedHandler)));
                await dialog.ShowAsync();
            }
        }

        private static void CommandInvokedHandler(IUICommand command)
        {
            if (command.Label == "Exit App")
                CoreApplication.Exit();
            else
            {
                Connect();
            }
        }
        /// <summary>
        /// This function sends data to the server.
        /// </summary>
        /// <param name="message">The content of the message.</param>
        public static void SendData(string message)
        {
            lock (sc)
            {
                StreamWriter writer = new StreamWriter(sc.OutputStream.AsStreamForWrite());
                writer.Write(message);
                writer.Flush();
            }
        }


        /// <summary>
        /// This function recieves data from the server, while locking / not locking the socket according to flags.
        /// </summary>
        /// 
        /// Why lock, or NOT lock, you ask?
        /// In normal activity mode, locking the socket while receiving works better in order to avoid messages getting mixed up.
        /// However, while room browsing / in game, you cannot know when a message will pop up, or when you'll need to send - 
        /// so using a lock will actually jam the whole thing.
        /// 
        /// <param name="size">The size of the message to be received in bytes.</param>
        /// <param name="flags">Indicate whtether to lock the socket or not. 0 means locked, anything else means not locked.</param>
        /// <returns>Returns the data from the server.</returns>
        public static string RecvData(int size, int flags)
        {
            if (flags == 0)
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
            else
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

        /// <summary>
        /// This function turns an int into a string of a given size, with 0s to pad in case of free space.
        /// </summary>
        /// <param name="num">the number to be turned into a string.</param>
        /// <param name="size">the size of the target string.</param>
        /// <returns>Returns the wanted string.</returns>
        public static string GetPaddedNumber(int num, int size)
        {
            return num.ToString().PadLeft(size, '0');
        }

        /// <summary>
        /// Takes an int and turns it into a strin of a defined length.
        /// </summary>
        /// <param name="num">the number to be parsed.</param>
        /// <param name="size"></param>
        /// <returns></returns>
        public static string GetPaddedNumber(string num, int size)
        {
            return num.PadLeft(size, '0');
        }

        public static string PrepString(string str)
        {
            return GetPaddedNumber(str.Length, 2) + str;
        }

        public static void Reset()
        {
            reader = null;
        }
    }
}
