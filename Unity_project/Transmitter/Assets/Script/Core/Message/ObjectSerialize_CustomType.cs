using System;
using System.IO;
using System.Collections.Generic;
using Transmitter.Serialize.Tool;

//此檔案由SerializableFactory 自動生成 為配合效能最大化 請勿在意腳本封裝性
namespace Transmitter.Serialize
{
    public class ObjectSerialize_CustomType : ObjectSerialize_Base
    {
        public override void Init ()
        {
            base.Init ();
            serializeFunctionTable.Add ("Transmitter.Serialize.Apple", AppleConvertToBuffer);
            serializeFunctionTable.Add ("Transmitter.Serialize.Tree", TreeConvertToBuffer);
            serializeFunctionTable.Add ("Transmitter.Serialize.GunType", GunTypeConvertToBuffer);
        }

        byte[] AppleConvertToBuffer(Object msg)
        {
            Apple apple = (Apple)msg;
            return apple.GetByteArray ();
        }

        byte[] TreeConvertToBuffer(Object msg)
        {
            Tree tree = (Tree)msg;
            return tree.GetByteArray ();
        }

        byte[] GunTypeConvertToBuffer(Object msg)
        {
            GunType gunType = (GunType)msg;
            ushort m_gunType_Value = (ushort)gunType;
            return BitConverter.GetBytes (m_gunType_Value);
        }
    }
}
