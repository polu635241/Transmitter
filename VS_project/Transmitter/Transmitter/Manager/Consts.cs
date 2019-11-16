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
            public const ushort GameMessage = 1001;
            public const ushort AddUser = 1002;
            public const ushort RemoveUser = 1003;


            public const ushort NewUserReq = 1004;
            public const ushort NewUserRes = 1004;
        }
    }
}
