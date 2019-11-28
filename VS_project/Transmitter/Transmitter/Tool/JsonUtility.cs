using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transmitter.Model;
using Newtonsoft.Json;

namespace Transmitter.Tool
{
    /// <summary>
    /// 封裝化 可依據後續需求 修改 序列化 反序列化的工具
    /// </summary>
    public static class JsonUtility
    {
        public static string ToJson(Object target)
        {
            return JsonConvert.SerializeObject(target);
        }

        public static T FromJson<T>(string jsonStr)
        {
            return JsonConvert.DeserializeObject<T>(jsonStr);
        }
    }
}
