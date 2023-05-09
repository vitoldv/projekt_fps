using _Core;
using _Core.Common;
using _Core.Player;
using _Core.Upgrades;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpgradesMenuUIController : MonoBehaviour
{
    public event Action<WeaponType> WeaponUnlocked;
    public event Action<WeaponUpgradeData, int> WeaponUpgraded;

    [Header("References")]
    [SerializeField] private LayoutGroup weaponsLayout;
    [SerializeField] private LayoutGroup skillsLayout;
    [SerializeField] private List<WeaponUpgradeData> weaponUpgrades;
    [SerializeField] private DashUpgradeData dashUpgradeData;
    [SerializeField] private QuakeUpgradeData quakeUpgradeData;
    [Header("Prefabs")]
    [SerializeField] private WeaponUpgradeUIController weaponUpgradeUiControllerPrefab;
    [SerializeField] private SkillUpgradeUIController skillUpgradeUiControllerPrefab;

    private List<WeaponUpgradeUIController> weaponUpgradeUIControllers = new List<WeaponUpgradeUIController>();
    private List<SkillUpgradeUIController> skillUpgradeUIControllers = new List<SkillUpgradeUIController>();
    private PlayerController playerReference;

    public void Init(PlayerController player)
    {
        playerReference = player;        
    }

    private void Start()
    {
        // WEAPONS INIT
        foreach (var upgradeData in weaponUpgrades)
        {
            CreateWeaponUIController(upgradeData);
        }
        CreateSkillUIController(dashUpgradeData);
        CreateSkillUIController(quakeUpgradeData);
    }

    private void CreateWeaponUIController(WeaponUpgradeData upgradeData)
    {
        var uiController = Instantiate(weaponUpgradeUiControllerPrefab, weaponsLayout.transform);
        uiController.Init(upgradeData, 0, false);
        uiController.WeaponUnlocked += OnWeaponUnlocked;
        uiController.LevelPurchased += OnWeaponUpgradePurchased;
        weaponUpgradeUIControllers.Add(uiController);
    }

    private void CreateSkillUIController(ISkillUpgradesData upgradeData)
    {
        var uiController = Instantiate(skillUpgradeUiControllerPrefab, weaponsLayout.transform);
        uiController.Init(upgradeData, 0, false);
        uiController.SkillUnlocked += OnSkillUnlocked;
        uiController.SkillLevelPurchased += OnSkillUpgraded; 
        skillUpgradeUIControllers.Add(uiController);
    }

    private void OnSkillUnlocked(ISkillUpgradesData upgradesData)
    {
        var saveData = GameManager.CurrentPlayerProgressionData;
        if (upgradesData is DashUpgradeData)
        {
            playerReference.IsDashEnabled = true;            
            saveData.isDashPurchased = true;
        }
        if (upgradesData is QuakeUpgradeData)
        {
            playerReference.IsQuakeEnabled = true;
            saveData.isQuakePurchased = true;
        }
        GameManager.CurrentPlayerProgressionData = saveData;
    }

    private void OnSkillUpgraded(ISkillUpgradeLevel upgradeLevelData, int level)
    {
        var saveData = GameManager.CurrentPlayerProgressionData;
        if (upgradeLevelData is DashUpgradeLevel dashUpgradeLevel)
        {
            playerReference.dashCooldown = dashUpgradeLevel.ReloadTime;
            playerReference.dashSpeed = dashUpgradeLevel.Speed;
            saveData.dashLevelPurchased = level;
        }
        if (upgradeLevelData is QuakeUpgradeLevel quakeUpgradeLevel)
        {
            playerReference.quakeCooldown = quakeUpgradeLevel.ReloadTime;
            playerReference.quakeDamage = quakeUpgradeLevel.Damage;
            playerReference.quakeRadius = quakeUpgradeLevel.Radius;
            saveData.quakeLevelPurchased = level;
        }
        GameManager.CurrentPlayerProgressionData = saveData;
    }

    private void OnWeaponUnlocked(WeaponUpgradeData upgradeData)
    {
        WeaponUnlocked?.Invoke(upgradeData.weaponType);
    }

    private void OnWeaponUpgradePurchased(WeaponUpgradeData upgradeData, int upgradeLevel)
    {
        WeaponUpgraded?.Invoke(upgradeData, upgradeLevel);
    }
}
