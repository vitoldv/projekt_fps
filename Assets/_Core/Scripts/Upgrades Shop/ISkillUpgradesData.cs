using System;
using System.Collections.Generic;
using UnityEngine;

namespace _Core.Upgrades
{
    public interface ISkillUpgradesData
    {
        public int UnlockCost { get; }
        public string SkillName { get; }
        public Sprite IconImage { get; }
        public List<ISkillUpgradeLevel> UpgradeLevels { get; }
    }
}
