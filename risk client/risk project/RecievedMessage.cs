using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace risk_project
{
    class RecievedMessage
    {
        string code;
        List<string> args;

        public RecievedMessage()
        {
            code = GetMessageTypeCode();
            args = GetArgsFromData();
        }

        public string GetCode() { return code; }
        public List<string> GetArgs() { return args; }

        public string this[int index]
        {
            get { return args[index]; }
        }


        private string GetMessageTypeCode()
        {
            return Comms.RecvData(3);
        }


        private List<string> GetArgsFromData()
        {
            List<string> ans = new List<string>();

            int i, amount, size;

            if (code == Comms.SIGN_IN_RES)
            {
                ans.Add(Comms.RecvData(1));
            }//102
                    



            else if (code == Comms.SIGN_UP_RES)                                                                        //104
                ans.Add(Comms.RecvData(1));



            else if (code == Comms.ACTIVE_ROOMS_RES)                                                                   //106
            {
                amount = int.Parse(Comms.RecvData(4));

                for (i = 0; i < amount; i++)
                {
                    ans.Add(Comms.RecvData(4)); //getting the room id
                    size = int.Parse(Comms.RecvData(2));
                    ans.Add(Comms.RecvData(size));
                }
            }

            else if (code == Comms.GET_USERS_RES)                                                                      //108
            {
                amount = int.Parse(Comms.RecvData(1));

                for (i = 0; i < amount; i++)
                {
                    size = int.Parse(Comms.RecvData(2));
                    ans.Add(Comms.RecvData(size));
                }
            }

            else if (code == Comms.JOIN_ROOM_RES)                                                                      //110
                {
                    string code = Comms.RecvData(1);
                    ans.Add(code);

                    if (code == "0")
                    {
                        ans.Add(Comms.RecvData(2));
                        ans.Add(Comms.RecvData(2));
                    }
                }

            else if (code == Comms.NEW_ROOM_RES)                                                                       //114
            {
                ans.Add(Comms.RecvData(1));
                ans.Add(Comms.RecvData(4));
            }
                

            else if (code == Comms.LEADERBOARDS_RES)
            {
                amount = 8;
                for (i = 0; i < amount; i++)
                {
                    size = int.Parse(Comms.RecvData(2));
                    if (size == 0)
                    {
                        ans.Add("-----------");
                        ans.Add("-----------");
                    }
                    else
                    {
                        ans.Add(Comms.RecvData(size));
                        ans.Add(Comms.RecvData(2));
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
