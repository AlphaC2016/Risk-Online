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
        List<string> values;

        public int GetMessageTypeCode()
        {
            return Comms.GetIntPartFromSocket(3);
        }

        public List<string> GetValues()
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

/*protocol:
* 131-num-namesize-name-victories
* num: 1 digit
* namesize: 2 bytes
* name: size bytes
* victories: 2 bytes
*/
                case Comms.GET_LEADERBOARDS:
                    amount = Comms.GetIntPartFromSocket(1);
                    for (i=0; i<amount*2; i++)
                    {
                        size = Comms.GetIntPartFromSocket(2);
                        ans.Add(Comms.RecvData(size));
                        ans.Add(Comms.RecvData(2));
                    }
                    break;
            }

            return ans;
        }
    }
}
