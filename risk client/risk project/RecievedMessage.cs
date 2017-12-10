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

        public RecievedMessage(int flags=0)
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


        private List<string> GetArgsFromData()
        {
            List<string> ans = new List<string>();

            int i, amount, size;

            if (code == Comms.SIGN_IN_RES)
            {
                ans.Add(Comms.RecvData(1, flags));
            }//102
                    



            else if (code == Comms.SIGN_UP_RES)                                                                        //104
                ans.Add(Comms.RecvData(1, flags));



            else if (code == Comms.ACTIVE_ROOMS_RES)                                                                   //106
            {
                amount = int.Parse(Comms.RecvData(4, flags));

                for (i = 0; i < amount; i++)
                {
                    ans.Add(Comms.RecvData(4)); //getting the room id
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

                    if (code == "0")
                    {
                        ans.Add(Comms.RecvData(2, flags));
                        ans.Add(Comms.RecvData(2, flags));
                    }
                }

            else if (code == Comms.NEW_ROOM_RES)                                                                       //114
            {
                ans.Add(Comms.RecvData(1, flags));
                ans.Add(Comms.RecvData(4, flags));
            }
                

            else if (code == Comms.LEADERBOARDS_RES)
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
