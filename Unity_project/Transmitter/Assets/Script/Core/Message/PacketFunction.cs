using System;
using System.IO;
using System.Collections.Generic;
using Transmitter.Serialize;

//此檔案由SerializableFactory 自動生成 為配合效能最大化 請勿在意腳本封裝性
namespace Transmitter.Serialize
{
    public partial class Apple : Message
    {
        public override byte[] GetByteArray()
        {
            byte[] buffer = new byte[1024];
            MemoryStream memoryStream = new MemoryStream(buffer);
            BinaryWriter binaryWriter = new BinaryWriter(memoryStream);

            binaryWriter.Write (id);

            binaryWriter.Write (value);

            byte[] result = memoryStream.ToArray ();

            binaryWriter.Close ();
            memoryStream.Close ();

            return result;
        }

        public override void Init (byte[] msg)
        {
            if (hasInit)
                return;

            hasInit = true;
            MemoryStream memoryStream = new MemoryStream(msg);
            BinaryReader binaryReader = new BinaryReader (memoryStream);

            id = binaryReader.ReadInt32 ();

            value = binaryReader.ReadSingle ();

            binaryReader.Close ();
            memoryStream.Close ();
        }
    }

    public partial class Tree : Message
    {
        public override byte[] GetByteArray()
        {
            byte[] buffer = new byte[1024];
            MemoryStream memoryStream = new MemoryStream(buffer);
            BinaryWriter binaryWriter = new BinaryWriter(memoryStream);

            if (apples != null)
            {
                binaryWriter.Write((short)apples.Count);

                for (int i = 0; i < apples.Count; i++)
                {
                    Apple m_applesItem = apples[i];
                    byte[] m_applesItemBuffer = m_applesItem.GetByteArray();
                    binaryWriter.Write((ushort)m_applesItemBuffer.Length);
                    binaryWriter.Write(m_applesItemBuffer);
                }
            }
            else
            {
                binaryWriter.Write ((short)-1);
            }

            if (root != null)
            {
                binaryWriter.Write((short)root.Count);

                for (int i = 0; i < root.Count; i++)
                {
                    int m_rootItem = root[i];
                    binaryWriter.Write (m_rootItem);
                }
            }
            else
            {
                binaryWriter.Write ((short)-1);
            }

            if (ownerWeapons != null)
            {
                binaryWriter.Write((short)ownerWeapons.Count);

                for (int i = 0; i < ownerWeapons.Count; i++)
                {
                    GunType m_ownerWeaponsItem = ownerWeapons[i];
                    ushort m_ownerWeaponsItem_value = (ushort)m_ownerWeaponsItem;
                    binaryWriter.Write(m_ownerWeaponsItem_value);
                }
            }
            else
            {
                binaryWriter.Write ((short)-1);
            }

            ushort presetWeapon_value = (ushort)presetWeapon;
            binaryWriter.Write(presetWeapon_value);

            byte[] result = memoryStream.ToArray ();

            binaryWriter.Close ();
            memoryStream.Close ();

            return result;
        }

        public override void Init (byte[] msg)
        {
            if (hasInit)
                return;

            hasInit = true;
            MemoryStream memoryStream = new MemoryStream(msg);
            BinaryReader binaryReader = new BinaryReader (memoryStream);

            int applesReapeatedCount = binaryReader.ReadInt16();
            if (applesReapeatedCount != -1)
            {

                apples = new List<Apple>();
                for (int i = 0; i < applesReapeatedCount; i++)
                {
                    Apple apples_item;
                    ushort apples_itemBufferCount = binaryReader.ReadUInt16();
                    byte[] apples_itemBuffer = binaryReader.ReadBytes(apples_itemBufferCount);
                    apples_item = new Apple ();
                    apples_item.Init (apples_itemBuffer);
                    apples.Add(apples_item);
                }
            }
            else
            {
                apples = null;
            }

            int rootReapeatedCount = binaryReader.ReadInt16();
            if (rootReapeatedCount != -1)
            {

                root = new List<int>();
                for (int i = 0; i < rootReapeatedCount; i++)
                {
                    int root_item;
                    root_item = binaryReader.ReadInt32 ();
                    root.Add(root_item);
                }
            }
            else
            {
                root = null;
            }

            int ownerWeaponsReapeatedCount = binaryReader.ReadInt16();
            if (ownerWeaponsReapeatedCount != -1)
            {

                ownerWeapons = new List<GunType>();
                for (int i = 0; i < ownerWeaponsReapeatedCount; i++)
                {
                    GunType ownerWeapons_item;
                    ushort ownerWeapons_item_value = binaryReader.ReadUInt16 ();
                    ownerWeapons_item = (GunType)ownerWeapons_item_value;
                    ownerWeapons.Add(ownerWeapons_item);
                }
            }
            else
            {
                ownerWeapons = null;
            }

            ushort presetWeapon_value = binaryReader.ReadUInt16 ();
            presetWeapon = (GunType)presetWeapon_value;

            binaryReader.Close ();
            memoryStream.Close ();
        }
    }
}
