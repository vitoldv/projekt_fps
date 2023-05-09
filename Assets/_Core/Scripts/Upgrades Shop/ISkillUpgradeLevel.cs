using System;

namespace _Core.Upgrades
{
    public interface ISkillUpgradeLevel
    {
        public int Cost { get; }
        public int ReloadTime { get; }
    }
}
