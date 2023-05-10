using _Core.Common;
using _Core.Upgrades;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkillUpgradeUIController : MonoBehaviour
{
    public event Action<ISkillUpgradeLevel, int> SkillLevelPurchased;
    public event Action<ISkillUpgradesData> SkillUnlocked;

    // upgrade elements
    [SerializeField] private Image itemImage;
    [SerializeField] private TextMeshProUGUI descText;
    [SerializeField] private TextMeshProUGUI costText;
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI titleOnUnlockText;
    [SerializeField] private CellBar cellBar;
    [SerializeField] private Button upgradeButton;
    // unlock elements
    [SerializeField] private Button unlockButton;
    [SerializeField] private TextMeshProUGUI unlockText;
    [SerializeField] private TextMeshProUGUI unlockCost;

    private int levelsPurchased;
    private bool isUnlocked;
    private ISkillUpgradesData upgradeData;

    private ISkillUpgradeLevel currentLevel => upgradeData.UpgradeLevels[levelsPurchased];

    private void Awake()
    {
        upgradeButton.onClick.AddListener(OnUpgradeButtonClick);
        unlockButton.onClick.AddListener(OnUnlockButtonClick);
    }

    public void Init(ISkillUpgradesData weaponUpgradeData, int levelsPurchased, bool isUnlocked)
    {
        this.levelsPurchased = levelsPurchased;
        this.isUnlocked = isUnlocked;
        upgradeData = weaponUpgradeData;
        itemImage.sprite = weaponUpgradeData.IconImage;
        titleText.text = weaponUpgradeData.SkillName;
        if (isUnlocked)
        {
            unlockButton.gameObject.SetActive(false);
        }
        else
        {
            unlockButton.gameObject.SetActive(true);
            unlockText.text = $"UNLOCK {upgradeData.SkillName.ToUpper()}";
            unlockCost.text = $"{upgradeData.UnlockCost}";
        }
        SetViewDataForCurrentLevel();
    }

    private void OnUpgradeButtonClick()
    {
        print("On upgrade");
        if (currentLevel.Cost <= GameManager.RewardPoints)
        {
            print("On upgrade succ");
            GameManager.RewardPoints -= currentLevel.Cost;
            SkillLevelPurchased?.Invoke(currentLevel, levelsPurchased);
            levelsPurchased++;
            SetViewDataForCurrentLevel();
        }
    }

    private void OnUnlockButtonClick()
    {
        print("On unlock");
        if (currentLevel.Cost <= GameManager.RewardPoints)
        {
            print("On unlock succ");
            GameManager.RewardPoints -= currentLevel.Cost;
            isUnlocked = true;
            unlockButton.gameObject.SetActive(false);
            SetViewDataForCurrentLevel();
            SkillUnlocked?.Invoke(upgradeData);
        }
    }

    private void SetViewDataForCurrentLevel()
    {
        cellBar.SetCells(levelsPurchased);
        if (levelsPurchased < upgradeData.UpgradeLevels.Count)
        {
            costText.text = currentLevel.Cost.ToString();
            descText.text = GetDescriptionForLevel(currentLevel);
            cellBar.SetCells(levelsPurchased);
        }
        else
        {
            unlockButton.gameObject.SetActive(false);
            costText.text = "FULL";
            descText.text = string.Empty;
        }
    }

    private string GetDescriptionForLevel(ISkillUpgradeLevel levelData)
    {
        string res = string.Empty;
        if(levelData is DashUpgradeLevel dashUpgradeData)
        {
            res = $"New reload time: {dashUpgradeData.ReloadTime}. New speed {dashUpgradeData.Speed}.";
        }
        if (levelData is QuakeUpgradeLevel quakeUpgradeData)
        {
            res = $"New reload time: {quakeUpgradeData.ReloadTime}. New damage {quakeUpgradeData.Damage}. Radius {quakeUpgradeData.Radius}";
        }
        return res;
    }
}
