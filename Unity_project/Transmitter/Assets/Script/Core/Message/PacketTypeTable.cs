using System;
using System.IO;
using System.Collections.Generic;
using Transmitter.Serialize.Tool;

//此檔案由SerializableFactory 自動生成 為配合效能最大化 請勿在意腳本封裝性
namespace Transmitter.Serialize
{
    [System.Serializable]
    public partial class Apple : Message
    {
        public int id;

        public float value;
    }

    [System.Serializable]
    public partial class Tree : Message
    {
        public List<Apple> apples;

        public List<int> root;

        public List<GunType> ownerWeapons;

        public GunType presetWeapon;
    }

    public enum GunType
    {
        ShotGun,
        Pistol,
        Submachine,
        Sniper_Rifle
    }
}
