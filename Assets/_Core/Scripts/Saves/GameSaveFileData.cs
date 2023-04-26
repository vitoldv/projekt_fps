using _Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using UnityEngine;

namespace Assets._Core.Scripts.Saves
{
    [Serializable]
    public struct GameSaveFileData
    {
        public PlayerProgressionData playerProgressionData;
        public int nextArena;
    }

    [Serializable]
    public struct PlayerProgressionData
    {
        public bool isDashPurchased;
        public bool isQuakePurchased;
        public bool isDoubleJumpPurchased;

        public int dashLevelPurchased;
        public int quakeLevelPurchased;

        public WeaponType weaponPurchased;
        public int pistolLevelPurchased;
        public int riflelLevelPurchased;
        public int shotgunLevelPurchased;
        public int bfgLevelPurchased;
        public int railgunLevelPurchased;
    }
}
