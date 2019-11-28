using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Transmitter.Model
{
    [Serializable]
    public class UserData
    {
        public ushort Udid;
        public string Token;

        public static UserData Create(ushort udid, string token)
        {
            UserData userData = new UserData();
            userData.Udid = udid;
            userData.Token = token;
            return userData;
        }

        UserData() { }
    }
}
