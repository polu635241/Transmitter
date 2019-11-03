using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Transmitter.Model
{
    public class UserIdentity
    {
        string ip;
        string token;

        public static UserIdentity CreateByDefaultFormat(string ip, string token)
        {
            UserIdentity userIdentity = new UserIdentity();
            userIdentity.ip = ip;
            userIdentity.token = token;
            return userIdentity;
        }

        UserIdentity() { }

        public byte[] GetBuffer()
        {
            byte[] buffer = null;
            MemoryStream memoryStream = null;
            BinaryWriter binaryWriter = null;

            try
            {
                memoryStream = new MemoryStream();
                binaryWriter = new BinaryWriter(memoryStream);

                //寫入IP
                binaryWriter.Write(ip);

                //寫入Token
                binaryWriter.Write(token);

                binaryWriter.Flush();
                buffer = memoryStream.ToArray();

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            finally
            {
                memoryStream?.Dispose();
                binaryWriter?.Dispose();
            }
            return buffer;
        }
    }
}
