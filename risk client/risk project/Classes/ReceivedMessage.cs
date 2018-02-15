using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace risk_project
{
    /// <summary>
    /// This class represents a received message from te server.
    /// </summary>
    class ReceivedMessage
    {
        string code; //the message code.
        List<string> args; //the message's arguments.


        /// <summary>
        /// The standard constructor for a received message. the message itself is received in the "Get" functions.
        /// </summary>
        /// <param name="flags">Indicates whether to lock the socket or not. 0 in regular communications, 1 in room page or in-game.</param>
        public ReceivedMessage(int flags=0)
        {
            code = GetMessageTypeCode(flags);
            args = GetArgsFromData(flags);
        }

        public string GetCode() { return code; }
        public List<string> GetArgs() { return args; }


        /// <summary>
        /// This function allows random-access into the message arguments without digging into the fields themselves.
        /// </summary>
        /// <param name="index">the index of the requested argument.</param>
        /// <returns>The requested argument.</returns>
        public string this[int index]
        {
            get { return args[index]; }
        }


        /// <summary>
        /// This function gets the message code of the incoming message.
        /// </summary>
        /// <param name="flags">Flag use described in general class descripion.</param>
        /// <returns>Returns the message code.</returns>
        private string GetMessageTypeCode(int flags)
        {
            return Comms.RecvData(3, flags);
        }


        /// <summary>
        /// This function gets all the arguments from the server, according to each message's struture.
        /// </summary>
        /// <param name="flags">Flag use described in general class descripion.</param>
        /// <returns>Returns a list that contains all arguments.</returns>
        private List<string> GetArgsFromData(int flags)
        {
            List<string> ans = new List<string>();

            int i, amount, size;

            switch (code)
            {
                case Comms.SIGN_IN_RES:
                    ans.Add(Comms.RecvData(1, flags));
                    break;

                case Comms.SIGN_UP_RES:
                    ans.Add(Comms.RecvData(1, flags));
                    break;

                case Comms.GET_USERS_RES:
                    amount = int.Parse(Comms.RecvData(1, flags));
                    for (i=0; i<amount; i++)
                    {
                        size = int.Parse(Comms.RecvData(2, flags));
                        ans.Add(Comms.RecvData(size, flags));
                    }
                    break;

                case Comms.GET_ROOMS_RES:
                    amount = int.Parse(Comms.RecvData(4, flags));

                    for (i = 0; i < amount; i++)
                    {
                        ans.Add(Comms.RecvData(4, flags)); //getting the room id
                        size = int.Parse(Comms.RecvData(2, flags));
                        ans.Add(Comms.RecvData(size, flags));
                    }
                    break;

                case Comms.JOIN_ROOM_RES:
                    string code = Comms.RecvData(1, flags);
                    ans.Add(code);
                    break;

                case Comms.CREATE_ROOM_RES:
                    ans.Add(Comms.RecvData(1, flags));
                    ans.Add(Comms.RecvData(4, flags));
                    break;

                case Comms.CLOSE_ROOM_RES:
                    // No Values!
                    break;

                case Comms.START_GAME_RES:
                    // No Values!
                    break;

                case Comms.INIT_MAP:
                    string temp;
                    amount = int.Parse(Comms.RecvData(1, flags));
                    ans.Add(amount.ToString());
                    string[] users = new string[amount];

                    for (i = 0; i < amount; i++)
                    {
                        size = int.Parse(Comms.RecvData(2, flags));
                        temp = Comms.RecvData(size, flags);
                        users[i] = temp;
                        ans.Add(temp);
                    }

                    for (i = 0; i < Helper.TERRITORY_AMOUNT; i++)
                    {
                        temp = users[int.Parse(Comms.RecvData(1, flags))];
                        ans.Add(temp);
                    }
                    break;

                case Comms.UPDATE_MAP:
                    for (i=0; i<Helper.TERRITORY_AMOUNT; i++)
                    {
                        size = int.Parse(Comms.RecvData(2, flags));
                        ans.Add(Comms.RecvData(size, flags));
                        ans.Add(Comms.RecvData(2, flags));
                    }
                    break;

                case Comms.LEADERBOARDS_RES:
                    amount = 8;
                    for (i = 0; i < amount; i++)
                    {
                        size = int.Parse(Comms.RecvData(2, flags));
                        if (size == 0)
                        {
                            ans.Add("-----------");
                            ans.Add("-----------");
                        }
                        else
                        {
                            ans.Add(Comms.RecvData(size, flags));
                            ans.Add(Comms.RecvData(2, flags));
                        }
                    }
                    break;

                case Comms.START_TURN:
                    size = int.Parse(Comms.RecvData(2, flags));
                    ans.Add(Comms.RecvData(size, flags));
                    break;

                case Comms.MOVE_FORCES_RES:
                    ans.Add(Comms.RecvData(1, flags));
                    break;

                case Comms.START_BATTLE_RES:
                    ans.Add(Comms.RecvData(1, flags));
                    ans.Add(Comms.RecvData(2, flags));
                    ans.Add(Comms.RecvData(2, flags));
                    break;

                case Comms.ROLL_DICE_RES:
                    for (i=0; i<5; i++)
                    {
                        ans.Add(Comms.RecvData(1, flags));
                    }
                    ans.Add(Comms.RecvData(2, flags));
                    ans.Add(Comms.RecvData(2, flags));
                    break;

                case Comms.END_BATTLE:
                    ans.Add(Comms.RecvData(1, flags));
                    break;

                case Comms.RECEIVE_MESSAGE:
                    size = int.Parse(Comms.RecvData(2, flags));
                    ans.Add(Comms.RecvData(size, flags));
                    size = int.Parse(Comms.RecvData(2, flags));
                    ans.Add(Comms.RecvData(size, flags));
                    break;

                default:
                    throw new Exception("UNSUPPORTED MESSAGE TYPE!");
            }
            return ans;
        }
    }
}
