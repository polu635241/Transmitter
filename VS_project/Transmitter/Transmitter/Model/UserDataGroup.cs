using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Transmitter.Model
{
    [Serializable]
    public class UserDataGroup
    {
        /// <summary>
        /// 現存玩家的資料
        /// </summary>
        public List<UserData> UserDatas;

        /// <summary>
        /// 新連線進來的玩家的udid
        /// </summary>
        public ushort NewUserUDID;
    }
}
