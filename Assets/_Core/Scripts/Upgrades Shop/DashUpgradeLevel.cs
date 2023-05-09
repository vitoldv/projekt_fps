using System;
using UnityEngine;

namespace _Core.Upgrades
{
    [Serializable]
    public struct DashUpgradeLevel : ISkillUpgradeLevel
    {
        [SerializeField] private int cost;
        [SerializeField] private int reloadTime;
        [SerializeField] private float speed;

        public int Cost => cost;
        public int ReloadTime => reloadTime;
        public float Speed => speed;
    }
}
