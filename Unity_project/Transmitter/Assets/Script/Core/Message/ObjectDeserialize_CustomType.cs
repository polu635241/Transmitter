using System;
using System.IO;
using System.Collections.Generic;
using Transmitter.Serialize.Tool;

//此檔案由SerializableFactory 自動生成 為配合效能最大化 請勿在意腳本封裝性
namespace Transmitter.Serialize
{
    public class ObjectDeserialize_CustomType : ObjectDeserialize_Base
    {
        public override void Init ()
        {
            base.Init ();
            deserializeFunctionTable.Add ("Transmitter.Serialize.Apple", ConvertToApple);
            deserializeFunctionTable.Add ("Transmitter.Serialize.Tree", ConvertToTree);
            deserializeFunctionTable.Add ("Transmitter.Serialize.GunType", ConvertToGunType);
        }

        object ConvertToApple(byte[] msg)
        {
            Apple apple = new Apple ();
            apple.Init (msg);
            return apple;
        }

        object ConvertToTree(byte[] msg)
        {
            Tree tree = new Tree ();
            tree.Init (msg);
            return tree;
        }

        object ConvertToGunType(byte[] msg)
        {
            ushort m_gunType_Value = BitConverter.ToUInt16 (msg, 0);
            return (GunType)m_gunType_Value;
        }
    }
}
