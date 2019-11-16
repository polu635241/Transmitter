using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Transmitter.DataStruct
{
    public static class Consts
    {
        public static class NetworkEvents
        {
            public const short GameMessage = 1001;
            public const short AddUser = 1002;
            public const short RemoveUser = 1003;


            public const short NewUserReq = 1004;
            public const short NewUserRes = 1004;
        }
    }
}
