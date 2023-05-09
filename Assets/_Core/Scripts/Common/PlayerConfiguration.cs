using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewPlayerConfig", menuName = "Player/Player configuration", order = 1)]
public class PlayerConfiguration : ScriptableObject
{
    [Serializable]
    public struct WeaponProgressionLevel
    {
        public int maxCapacity;
        public int reloadTime;
    }

    [Serializable]
    public struct DashProgressionLevel
    {
        public int speed;
        public int reloadTime;
    }

    [Serializable]
    public struct QuakeProgressionLevel
    {
        public int damage;
        public int reloadTime;
    }

    public List<WeaponProgressionLevel> pistolProgressionLevels;
    public List<WeaponProgressionLevel> rifleProgressionLevels;
    public List<WeaponProgressionLevel> shotgunProgressionLevels;
    public List<WeaponProgressionLevel> bfgProgressionLevels;
    public List<WeaponProgressionLevel> railgunProgressionLevels;
    public List<DashProgressionLevel> dashProgressionLevels;
    public List<QuakeProgressionLevel> quakeProgressionLevels;
}
