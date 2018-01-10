using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace risk_project
{
    class ReceivedMessage
    {
        string code;
        List<string> args;

        public ReceivedMessage(int flags=0)
        {
            code = GetMessageTypeCode(flags);
            args = GetArgsFromData(flags);
        }

        public string GetCode() { return code; }
        public List<string> GetArgs() { return args; }

        public string this[int index]
        {
            get { return args[index]; }
        }


        private string GetMessageTypeCode(int flags)
        {
            return Comms.RecvData(3, flags);
        }


        private List<string> GetArgsFromData(int flags)
        {
            List<string> ans = new List<string>();

            int i, amount, size;

            if (code == Comms.SIGN_IN_RES)
            {
                ans.Add(Comms.RecvData(1, flags));
            }
                    

            else if (code == Comms.SIGN_UP_RES)                                                                        //104
            {
                ans.Add(Comms.RecvData(1, flags));
            }


            else if (code == Comms.GET_ROOMS_RES)                                                                   //106
            {
                amount = int.Parse(Comms.RecvData(4, flags));

                for (i = 0; i < amount; i++)
                {
                    ans.Add(Comms.RecvData(4, flags)); //getting the room id
                    size = int.Parse(Comms.RecvData(2, flags));
                    ans.Add(Comms.RecvData(size, flags));
                }
            }


            else if (code == Comms.GET_USERS_RES)                                                                      //108
            {
                amount = int.Parse(Comms.RecvData(1, flags));

                for (i = 0; i < amount; i++)
                {
                    size = int.Parse(Comms.RecvData(2, flags));
                    ans.Add(Comms.RecvData(size, flags));
                }
            }


            else if (code == Comms.JOIN_ROOM_RES)                                                                      //110
            {
                string code = Comms.RecvData(1, flags);
                ans.Add(code);
            }

            else if (code == Comms.CREATE_ROOM_RES)                                                                       //114
            {
                ans.Add(Comms.RecvData(1, flags));
                ans.Add(Comms.RecvData(4, flags));
            }

            else if (code == Comms.START_GAME_RES)
            {
                //No Values!
            }

            else if (code == Comms.INIT_MAP)                                                                           //119
            {
                string temp;
                amount = int.Parse(Comms.RecvData(1, flags));
                ans.Add(amount.ToString());
                string[] users = new string[amount];

                for (i=0; i<amount; i++)
                {
                    size = int.Parse(Comms.RecvData(2, flags));
                    temp = Comms.RecvData(size, flags);
                    users[i] = temp;
                    ans.Add(temp);
                }

                for (i=0; i<Helper.TERRITORY_AMOUNT; i++)
                {
                    temp = users[int.Parse(Comms.RecvData(1, flags))];
                    ans.Add(temp);
                }
            }

            else if (code == Comms.LEADERBOARDS_RES)                                                                   //124
            {
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
            }


            else
            {
                throw new Exception("UNSUPPORTED MESSAGE TYPE!");
            }

            return ans;
        }
    }
}
