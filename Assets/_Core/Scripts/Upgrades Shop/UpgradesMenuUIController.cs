using _Core;
using _Core.Common;
using _Core.Player;
using _Core.Upgrades;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UpgradesMenuUIController : MonoBehaviour
{
    public event Action UpgradePurchased;

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
            saveData.isDashPurchased = true;
        }
        if (upgradesData is QuakeUpgradeData)
        {
            saveData.isQuakePurchased = true;
        }
        GameManager.CurrentPlayerProgressionData = saveData;
        UpgradePurchased?.Invoke();
    }

    private void OnSkillUpgraded(ISkillUpgradeLevel upgradeLevelData, int level)
    {
        var saveData = GameManager.CurrentPlayerProgressionData;
        if (upgradeLevelData is DashUpgradeLevel dashUpgradeLevel)
        {
            saveData.dashLevelPurchased = level;
        }
        if (upgradeLevelData is QuakeUpgradeLevel quakeUpgradeLevel)
        {
            saveData.quakeLevelPurchased = level;
        }
        GameManager.CurrentPlayerProgressionData = saveData;
        UpgradePurchased?.Invoke();
    }

    private void OnWeaponUnlocked(WeaponUpgradeData upgradeData)
    {
        var saveData = GameManager.CurrentPlayerProgressionData;
        saveData.weaponPurchased |= upgradeData.weaponType;
        GameManager.CurrentPlayerProgressionData = saveData;
        UpgradePurchased?.Invoke();
    }

    private void OnWeaponUpgradePurchased(WeaponUpgradeData upgradeData, int upgradeLevel)
    {
        var saveData = GameManager.CurrentPlayerProgressionData;
        switch (upgradeData.weaponType)
        {
            case WeaponType.Pistol:
                saveData.pistolLevelPurchased = upgradeLevel;
                break;
            case WeaponType.Rifle:
                saveData.riflelLevelPurchased = upgradeLevel;
                break;
            case WeaponType.Shotgun:
                saveData.shotgunLevelPurchased = upgradeLevel;
                break;
            case WeaponType.BFG:
                saveData.bfgLevelPurchased = upgradeLevel;
                break;
            case WeaponType.Railgun:
                saveData.railgunLevelPurchased = upgradeLevel;
                break;
            default:
                break;
        }
        GameManager.CurrentPlayerProgressionData = saveData;
        UpgradePurchased?.Invoke();
    }
}
