using System.Collections.Generic;
using UnityEngine;
using _Core.Upgrades;
using System.Linq;

[CreateAssetMenu(fileName = "NewDashUpgradeData", menuName = "Upgrades/Dash Upgrade Data", order = 1)]
public class DashUpgradeData : ScriptableObject, ISkillUpgradesData
{
    [SerializeField] private int unlockCost;
    [SerializeField] private string skillName;
    [SerializeField] private Sprite image;
    [SerializeField] private List<DashUpgradeLevel> upgradeLevels;

    public int UnlockCost  => unlockCost;
    public string SkillName  => skillName;
    public Sprite IconImage  => image;
    public List<ISkillUpgradeLevel> UpgradeLevels => upgradeLevels.Cast<ISkillUpgradeLevel>().ToList();
}
