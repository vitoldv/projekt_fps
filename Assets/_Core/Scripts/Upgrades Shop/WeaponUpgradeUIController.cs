using _Core.Common;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WeaponUpgradeUIController : MonoBehaviour
{
    public event Action<WeaponUpgradeData, int> LevelPurchased;
    public event Action<WeaponUpgradeData> WeaponUnlocked;

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
    private WeaponUpgradeData upgradeData;

    private WeaponUpgradeData.WeaponUpgradeLevel currentLevel => upgradeData.upgradeLevels[levelsPurchased];

    private void Awake()
    {
        upgradeButton.onClick.AddListener(OnUpgradeButtonClick);
        unlockButton.onClick.AddListener(OnUnlockButtonClick);
    }

    public void Init(WeaponUpgradeData weaponUpgradeData, int levelsPurchased, bool isUnlocked)
    {
        this.levelsPurchased = levelsPurchased;
        this.isUnlocked = isUnlocked;
        upgradeData = weaponUpgradeData;
        itemImage.sprite = weaponUpgradeData.image;
        titleText.text = weaponUpgradeData.title;
        if(isUnlocked)
        {
            unlockButton.gameObject.SetActive(false);
        }
        else
        {
            unlockButton.gameObject.SetActive(true);
            unlockText.text = $"UNLOCK {upgradeData.title.ToString().ToUpper()}";
            unlockCost.text = $"{upgradeData.unlockCost}";
        }
        SetViewDataForCurrentLevel();
    }

    private void OnUpgradeButtonClick()
    {
        print("On upgrade");
        if (currentLevel.cost <= GameManager.PlayerPoints)
        {
            print("On upgrade succ");
            GameManager.PlayerPoints -= currentLevel.cost;
            LevelPurchased?.Invoke(upgradeData, levelsPurchased);
            levelsPurchased++;
            SetViewDataForCurrentLevel();            
        }
    }

    private void OnUnlockButtonClick()
    {
        print("On unlock");
        if (currentLevel.cost <= GameManager.PlayerPoints)
        {
            print("On unlock succ");
            GameManager.PlayerPoints -= currentLevel.cost;
            isUnlocked = true;
            unlockButton.gameObject.SetActive(false);
            SetViewDataForCurrentLevel();
            WeaponUnlocked?.Invoke(upgradeData);
        }
    }

    private void SetViewDataForCurrentLevel()
    {
        cellBar.SetCells(levelsPurchased);
        if (levelsPurchased < upgradeData.upgradeLevels.Count)
        {
            costText.text = currentLevel.cost.ToString();
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

    private string GetDescriptionForLevel(WeaponUpgradeData.WeaponUpgradeLevel level)
    {
        return $"New reload time: {level.newReloadTimeValue}. New max capacity {level.newMaxCapacityValue}. Cost {level.cost}";
    }
}
