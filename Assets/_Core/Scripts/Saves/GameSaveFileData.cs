using _Core;
using System;

namespace _Core.Saves
{
    [Serializable]
    public struct GameSaveFileData
    {
        public PlayerProgressionData playerProgressionData;
        public int nextArena;
        public int rewardPoints;
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

        public int GetWeaponLevelForType(WeaponType weapon)
        {
            switch (weapon)
            {
                case WeaponType.Pistol: return pistolLevelPurchased;
                case WeaponType.Rifle: return riflelLevelPurchased;
                case WeaponType.Shotgun: return shotgunLevelPurchased;
                case WeaponType.BFG: return bfgLevelPurchased;
                case WeaponType.Railgun: return railgunLevelPurchased;
                default: return -1;
            }
        }
    }
}
