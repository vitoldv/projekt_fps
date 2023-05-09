using _Core;
using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewWeaponUpgradeData", menuName = "Weapon/Weapon Upgrade Data", order = 1)]
public class WeaponUpgradeData : ScriptableObject
{
    [Serializable]
    public struct WeaponUpgradeLevel
    {
        public int cost;
        public float newReloadTimeValue;
        public float newMaxCapacityValue;
        public int newDamageValue;
    }

    public string title;
    public Sprite image;
    public int unlockCost;
    public WeaponType weaponType;
    public List<WeaponUpgradeLevel> upgradeLevels;        
}
