using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace risk_project
{
    class RecievedMessage
    {
        int code;
        List<string> args;

        public RecievedMessage()
        {
            code = GetMessageTypeCode();
            args = GetArgsFromData();
        }

        public int GetCode() { return code; }
        public List<string> GetArgs() { return args; }

        public string this[int index]
        {
            get { return args[index]; }
        }


        private int GetMessageTypeCode()
        {
            return int.Parse(Comms.RecvData(3));
        }


        private List<string> GetArgsFromData()
        {
            List<string> ans = new List<string>();

            int i, amount, size;

            switch (code)
            {
                case Comms.SIGN_IN_RES:                                                                        //102
                    ans.Add(Comms.RecvData(1));
                    break;


                case Comms.SIGN_UP_RES:                                                                        //104
                    ans.Add(Comms.RecvData(1));
                    break;


                case Comms.ACTIVE_ROOMS_RES:                                                                   //106
                    amount = int.Parse(Comms.RecvData(4));

                    for (i = 0; i < amount; i++)
                    {
                        ans.Add(Comms.RecvData(4)); //getting the room id
                        size = int.Parse(Comms.RecvData(2));
                        ans.Add(Comms.RecvData(size));
                    }
                    break;


                case Comms.GET_USERS_RES:                                                                      //108
                    amount = int.Parse(Comms.RecvData(1));

                    for (i = 0; i < amount; i++)
                    {
                        size = int.Parse(Comms.RecvData(2));
                        ans.Add(Comms.RecvData(size));
                    }
                    break;


                case Comms.JOIN_ROOM_RES:                                                                      //110
                    string code = Comms.RecvData(1);
                    ans.Add(code);

                    if (code == "0")
                    {
                        ans.Add(Comms.RecvData(2));
                        ans.Add(Comms.RecvData(2));
                    }
                    break;


                case Comms.LEAVE_ROOM_RES:                                                                     //112
                    //NO ARGS!
                    break;


                case Comms.NEW_ROOM_RES:                                                                       //114
                    ans.Add(Comms.RecvData(1));
                    break;


                case Comms.CLOSE_ROOM_RES:                                                                     //116
                    //NO ARGS!
                    break;
            }

            return ans;
        }
    }
}
