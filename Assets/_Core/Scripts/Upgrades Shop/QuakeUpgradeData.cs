using System.Collections.Generic;
using System;
using UnityEngine;
using _Core.Upgrades;
using System.Linq;

[CreateAssetMenu(fileName = "NewQuakeUpgradeData", menuName = "Upgrades/Quake Upgrade Data", order = 1)]
public class QuakeUpgradeData : ScriptableObject, ISkillUpgradesData
{
    [SerializeField] private int unlockCost;
    [SerializeField] private string skillName;
    [SerializeField] private Sprite iconImage;
    [SerializeField] private List<QuakeUpgradeLevel> upgradeLevels;

    public int UnlockCost => unlockCost;
    public string SkillName => skillName;
    public Sprite IconImage => iconImage;
    public List<ISkillUpgradeLevel> UpgradeLevels => upgradeLevels.Cast<ISkillUpgradeLevel>().ToList();
}
