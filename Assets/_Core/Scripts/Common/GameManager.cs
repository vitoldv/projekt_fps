using UnityEngine;
using UnityEngine.SceneManagement;
using _Core.Interfaces;
using _Core.Player;
using _Core.Arena;
using _Core.Saves;
using _Core.UI;
using System.Text;
using System.Reflection;
using System;
using System.IO;
using _Core.Upgrades;
using System.Collections.Generic;

namespace _Core.Common
{
    public class GameManager : Singleton<GameManager>
    {
        private string MainMenuSceneName = "MainMenuScene";

        public static GameLayers GameLayers => inst.layers;
        [SerializeField] private GameLayers layers;
        [SerializeField] private CollectiblesConfig collectiblesConfig;
        [SerializeField] private DashUpgradeData dashUpgradeData;
        [SerializeField] private QuakeUpgradeData quakeUpgradeData;
        [SerializeField] private List<WeaponUpgradeData> weaponUpgrades;

        public WeaponsConfiguration DefaultWeaponConfiguration;
        public static WeaponsConfiguration CurrentWeaponConfiguration { get; private set; }
        public static CollectiblesConfig CollectiblesConfig => inst.collectiblesConfig;
        public static PlayerProgressionData CurrentPlayerProgressionData
        {
            get => inst.currentGameSaveFileData.playerProgressionData;
            set => inst.currentGameSaveFileData.playerProgressionData = value;
        }           

        public static PlayerController PlayerController { get; private set; }
        public static int RewardPoints
        {
            get => inst.currentGameSaveFileData.rewardPoints;
            set => inst.currentGameSaveFileData.rewardPoints = value;
        }

        [Header("Prefabs")]
        [SerializeField] private PlayerController playerControllerPrefab;
        [SerializeField] private HUD hudPrefab;
        [SerializeField] private PauseMenu pauseMenuPrefab;

        private GameSaveFileData currentGameSaveFileData;
        private ArenaManager currentArenaManager;
        private MainMenuUIController mainMenu;
        private HUD hud;
        private PauseMenu pauseMenu;

        private bool isGameActive;

        protected override void Initialize()
        {
            if (SceneManager.GetActiveScene().name == MainMenuSceneName)
            {
                mainMenu = FindFirstObjectByType<MainMenuUIController>();
                mainMenu.GameStarted += OnGameStartInitiated;
            }
            else
            {
                CurrentWeaponConfiguration = DefaultWeaponConfiguration;
            }
        }

        private void Start()
        {
            ArenaLoader.ArenaLoaded += OnArenaLoaded;
        }

        private void Update()
        {
            if(isGameActive)
            {
                if(Input.GetKeyDown(KeyCode.Escape))
                {
                    if (pauseMenu.IsShown())
                    {
                        ResumeGame();
                    }
                    else
                    {
                        PauseGame();
                    }                    
                }                
            }
        }

        private void OnGameStartInitiated(GameSaveFileData gameSaveFileGuid)
        {
            OnSaveFileSelected(gameSaveFileGuid);
        }

        private void OnSaveFileSelected(GameSaveFileData gameSaveFile)
        {
            currentGameSaveFileData = gameSaveFile;
            StartGame(currentGameSaveFileData);
        }

        private void StartGame(GameSaveFileData gameSaveFile)
        {
            mainMenu.GameStarted -= OnGameStartInitiated;
            StartArena(gameSaveFile.nextArena);
        }

        private void OnArenaLoaded(int arenaIndex)
        {
            currentArenaManager = GameObject.FindGameObjectWithTag(Tags.ArenaManager).GetComponent<ArenaManager>();
            currentArenaManager.Init(arenaIndex);
            currentArenaManager.UpgradesUI.Init(PlayerController);
            currentArenaManager.UpgradesUI.UpgradePurchased += OnUpgradePurchased;         
            currentArenaManager.ArenaFinished += OnArenaFinished;

            pauseMenu = Instantiate(pauseMenuPrefab);
            pauseMenu.ReturnToMenuClicked += ReturnToMainMenu;
            pauseMenu.ResumeGameClicked += ResumeGame;
            pauseMenu.Hide();

            CreatePlayer();
            CreateHUD();

            isGameActive = true;

            HideCursor();
        }

        private void OnArenaFinished(int arenaIndex, int rewardPoints)
        {
            RewardPoints += rewardPoints;
            currentArenaManager.UpgradesUI.UpgradePurchased -= OnUpgradePurchased;
            currentArenaManager.ArenaFinished -= OnArenaFinished;
            pauseMenu.ReturnToMenuClicked -= ReturnToMainMenu;
            pauseMenu.ResumeGameClicked -= ResumeGame;

            // it was the last arena
            if (arenaIndex == ArenaLoader.TotalArenaCount - 1)
            {
                currentGameSaveFileData.nextArena = 0;
                SaveManager.SaveGameSaveFile(currentGameSaveFileData);
                ReturnToMainMenu();
            }
            else
            {
                currentGameSaveFileData.nextArena = arenaIndex + 1;
                SaveManager.SaveGameSaveFile(currentGameSaveFileData);
                StartArena(arenaIndex + 1);
            }

            isGameActive = false;
        }

        private void StartArena(int arenaIndex)
        {
            ArenaLoader.LoadArena(arenaIndex);
        }

        private void PauseGame()
        {
            Time.timeScale = 1f;
            PlayerController.IsFrozen = true;
            pauseMenu.Show();
            ShowCursor();
        }

        private void ResumeGame()
        {
            pauseMenu.Hide();
            HideCursor();
            PlayerController.IsFrozen = false;
            Time.timeScale = 1f;
        }

        private void ReturnToMainMenu()
        {
            SceneManager.LoadScene(MainMenuSceneName);
            ShowCursor();
        }

        private void CreatePlayer()
        {
            var player = Instantiate(playerControllerPrefab);
            player.Init(currentGameSaveFileData.playerProgressionData);
            var initialTransform = currentArenaManager.PlayerInitialTransform;
            player.transform.SetPositionAndRotation(initialTransform.position, initialTransform.rotation);
            PlayerController = player;
            ApplyUpgradesToPlayer();
        }

        private void CreateHUD()
        {
            hud = Instantiate(hudPrefab);
            hud.Init(PlayerController);
            hud.Show();
        }

        public static void ApplyUpgradesToPlayer()
        {
            // Unlocking skills
            PlayerController.IsDoubleJumpEnabled = inst.currentGameSaveFileData.playerProgressionData.isDashPurchased;
            PlayerController.IsDashEnabled = inst.currentGameSaveFileData.playerProgressionData.isDashPurchased;
            PlayerController.IsQuakeEnabled = inst.currentGameSaveFileData.playerProgressionData.isQuakePurchased;
            // Unlocking weapons 
            PlayerController.UnlockedWeapons = inst.currentGameSaveFileData.playerProgressionData.weaponPurchased;
            // Applying skills levels
            // Dash
            var dashLevel = inst.currentGameSaveFileData.playerProgressionData.dashLevelPurchased;
            if(dashLevel > 0)
            {
                var dashLevelData = (DashUpgradeLevel)inst.dashUpgradeData.UpgradeLevels[dashLevel - 1];
                PlayerController.DashCooldown = dashLevelData.ReloadTime;
                PlayerController.DashSpeed = dashLevelData.Speed;
            }
            // Quake
            var quakeLevel = inst.currentGameSaveFileData.playerProgressionData.quakeLevelPurchased;
            if(quakeLevel > 0)
            {
                var quakeLevelData = (QuakeUpgradeLevel)inst.quakeUpgradeData.UpgradeLevels[quakeLevel - 1];
                PlayerController.QuakeCooldown = quakeLevelData.ReloadTime;
                PlayerController.QuakeDamage = quakeLevelData.Damage;
                PlayerController.QuakeRadius = quakeLevelData.Radius;
            }
            // Applying weapon levels
            foreach (var weaponUpgrade in inst.weaponUpgrades)
            {
                if(PlayerController.UnlockedWeapons.HasFlag(weaponUpgrade.weaponType))
                {
                    var level = inst.currentGameSaveFileData.playerProgressionData.GetWeaponLevelForType(weaponUpgrade.weaponType);
                    if(level > 0)
                    {
                        PlayerController.UpgradeWeapon(weaponUpgrade.upgradeLevels[level - 1], weaponUpgrade.weaponType);
                    }                    
                }
            }
        }

        private void OnUpgradePurchased()
        {
            ApplyUpgradesToPlayer();
        }

        private void ShowCursor()
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }

        private void HideCursor()
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }

        #region ZAPISKA_HELPER

        [ContextMenu("wow")]
        private void wow()
        {
            GenerateClassFiles("_Core.Arena", @"C:\Users\VITOLD\Desktop\test\");
        }

        public static void GenerateClassFiles(string namespaceName, string directoryPath)
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            Type[] types = assembly.GetTypes();

            foreach (Type type in types)
            {                
                if (type.Namespace != null && type.Namespace.Contains("_Core"))
                {
                    if (type.Name.Contains("<") || type.Name.Contains(">"))
                    {
                        continue;
                    }
                    StringBuilder sb = new StringBuilder();

                    sb.AppendLine($"Type: {type.FullName}");

                    if (type.IsEnum)
                    {
                        sb.AppendLine("Values:");
                        Array enumValues = Enum.GetValues(type);
                        foreach (object enumValue in enumValues)
                        {
                            sb.AppendLine($"{enumValue}");
                        }
                    }
                    else
                    {
                        sb.AppendLine("Fields:");
                        FieldInfo[] fields = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly);
                        foreach (FieldInfo field in fields)
                        {
                            sb.AppendLine($"{GetProtectionLevel(field)}{field.Name} : {IfSingleThenFloat(field.FieldType.Name)}");
                        }

                        sb.AppendLine("Properties:");
                        PropertyInfo[] properties = type.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly);
                        foreach (PropertyInfo property in properties)
                        {
                            sb.AppendLine($"{GetProtectionLevel(property)}{property.Name} : {IfSingleThenFloat(property.PropertyType.Name)}");
                        }

                        sb.AppendLine("Events:");
                        EventInfo[] events = type.GetEvents(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly);
                        foreach (EventInfo evt in events)
                        {
                            MethodInfo delegateMethod = evt.EventHandlerType.GetMethod("Invoke");
                            string delegateParams = string.Join(", ", Array.ConvertAll(delegateMethod.GetParameters(), p => $"{IfSingleThenFloat(p.ParameterType.Name)} {p.Name}"));
                            sb.AppendLine($"{GetProtectionLevel(evt)}{evt.Name} : {evt.EventHandlerType.Name}<{delegateParams}>");
                        }

                        sb.AppendLine("Methods:");
                        MethodInfo[] methods = type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly);
                        foreach (MethodInfo method in methods)
                        {
                            if(method.Name.Contains("add_") || method.Name.Contains("remove_") || method.Name.Contains("get_") || method.Name.Contains("set_"))
                            {
                                continue;
                            }
                            string methodParameters = string.Join(", ", Array.ConvertAll(method.GetParameters(), p => $"{IfSingleThenFloat(p.ParameterType.Name)}"));
                            sb.AppendLine($"{GetProtectionLevel(method)}{method.Name}({methodParameters}) : {IfSingleThenFloat(method.ReturnType.Name)}");
                        }
                    }

                    print(type.Name);

                    string filePath = Path.Combine(directoryPath, $"{type.Name}.txt");
                    File.WriteAllText(filePath, sb.ToString());
                }
            }
        }

        public static string IfSingleThenFloat(string s)
        {
            string res = "";
            if (s == "Boolean") res = "bool";
            if (s == "Single") res = "float";
            if (s == "Int32") res = "int";
            if (s == "Void") res = "void";
            if (s == "String") res = "string";
            if (s == "Object") res = "object";
            if (res == "") res = s;
            return res;
        }

        public static string GetProtectionLevel(FieldInfo memberInfo)
        {
            string accessibilityLevel = "";
            if (memberInfo.IsPublic)
                accessibilityLevel = "+";
            else if (memberInfo.IsPrivate)
                accessibilityLevel = "-";
            else if (memberInfo.IsFamily)
                accessibilityLevel = "#";
            else if (memberInfo.IsAssembly)
                accessibilityLevel = "#";
            return accessibilityLevel;
        }

        public static string GetProtectionLevel(MethodInfo memberInfo)
        {
            string accessibilityLevel = "";
            if (memberInfo.IsPublic)
                accessibilityLevel = "+";
            else if (memberInfo.IsPrivate)
                accessibilityLevel = "-";
            else if (memberInfo.IsFamily)
                accessibilityLevel = "#";
            else if (memberInfo.IsAssembly)
                accessibilityLevel = "#";
            return accessibilityLevel;
        }

        public static string GetProtectionLevel(PropertyInfo memberInfo)
        {
            bool isPublic = memberInfo.GetGetMethod()?.IsPublic ?? memberInfo.GetSetMethod()?.IsPublic ?? false;
            return isPublic ? "+" : "-";
        }
        public static string GetProtectionLevel(EventInfo memberInfo)
        {
            bool isPublic = memberInfo.GetAddMethod()?.IsPublic ?? memberInfo.GetRemoveMethod()?.IsPublic ?? false;
            return isPublic ? "+" : "-";
        }

        #endregion
    }
}