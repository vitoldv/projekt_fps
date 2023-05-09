using _Core;
using System;

namespace _Core.Saves
{
    [Serializable]
    public struct GameSaveFileData
    {
        public PlayerProgressionData playerProgressionData;
        public int nextArena;
        public DateTime lastSaveDate;
        public Guid guid;
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
