using System;
using UnityEngine;

namespace _Core.Upgrades
{
    [Serializable]
    public struct QuakeUpgradeLevel : ISkillUpgradeLevel
    {
        [SerializeField] private int cost;
        [SerializeField] private int reloadTime;
        [SerializeField] private float damage;        
        [SerializeField] private float radius;

        public int Cost => cost;
        public int ReloadTime => reloadTime;
        public float Damage => damage;
        public float Radius => radius;
    }
}
