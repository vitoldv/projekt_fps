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


namespace _Core.Common
{
    public class GameManager : Singleton<GameManager>
    {
        private string MainMenuSceneName = "MainMenuScene";

        public static GameLayers GameLayers => inst.layers;
        [SerializeField] private GameLayers layers;
        [SerializeField] private CollectiblesConfig collectiblesConfig;
        public WeaponsConfiguration DefaultWeaponConfiguration;
        public static WeaponsConfiguration CurrentWeaponConfiguration { get; private set; }
        public static CollectiblesConfig CollectiblesConfig => inst.collectiblesConfig;
        public static PlayerProgressionData CurrentPlayerProgressionData
        {
            get => inst.currentGameSaveFileData.playerProgressionData;
            set => inst.currentGameSaveFileData.playerProgressionData = value;
        }           

        public static PlayerController PlayerController { get; private set; }
        public static int PlayerPoints = 100;


        [Header("Prefabs")]
        [SerializeField] private PlayerController playerControllerPrefab;
        [SerializeField] private HUD hudPrefab;

        private GameSaveFileData currentGameSaveFileData;
        private ArenaManager currentArenaManager;
        private MainMenuUIController mainMenu;
        private HUD hud;
        

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

        private void InitializeForMainMenu()
        {

        }

        private void Start()
        {
            ArenaLoader.ArenaLoaded += OnArenaLoaded;
        }

        private void OnGameStartInitiated(GameSaveFileData gameSaveFileGuid)
        {
            print("game started");
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
            ArenaLoader.LoadArena(gameSaveFile.nextArena);
        }

        private void OnArenaLoaded(string arenaName)
        {
            print("arena loaded");
            currentArenaManager = GameObject.FindGameObjectWithTag(Tags.ArenaManager).GetComponent<ArenaManager>();
            currentArenaManager.UpgradesUI.Init(PlayerController);
            currentArenaManager.UpgradesUI.WeaponUnlocked += OnWeaponUnlocked;
            currentArenaManager.UpgradesUI.WeaponUpgraded += OnWeaponUpgraded;
            CreatePlayer();
            CreateHUD();
        }

        private void CreatePlayer()
        {
            var player = Instantiate(playerControllerPrefab);
            player.Init(currentGameSaveFileData.playerProgressionData);
            var initialTransform = currentArenaManager.PlayerInitialTransform;
            print($"Initial transform {initialTransform.position}, {initialTransform.rotation.eulerAngles}");
            player.gameObject.transform.position= initialTransform.position;
            player.gameObject.transform.rotation = initialTransform.rotation;
            //player.transform.SetPositionAndRotation(initialTransform.position, initialTransform.rotation);
            print($"Initial transform {player.transform.position}, {player.transform.rotation.eulerAngles}");            
            PlayerController = player;
        }

        private void CreateHUD()
        {
            hud = Instantiate(hudPrefab);
            hud.Init(PlayerController);
            hud.Show();
        }

        private void OnWeaponUnlocked(WeaponType weaponPurchased)
        {
            inst.currentGameSaveFileData.playerProgressionData.weaponPurchased |= weaponPurchased;
            PlayerController.UnlockWeapon(weaponPurchased);
        }

        private void OnWeaponUpgraded(WeaponUpgradeData upgradeData, int upgradeLevel)
        {
            switch (upgradeData.weaponType)
            {
                case WeaponType.Pistol:
                    inst.currentGameSaveFileData.playerProgressionData.pistolLevelPurchased = upgradeLevel;
                    break;
                case WeaponType.Rifle:
                    inst.currentGameSaveFileData.playerProgressionData.riflelLevelPurchased = upgradeLevel;
                    break;
                case WeaponType.Shotgun:
                    inst.currentGameSaveFileData.playerProgressionData.shotgunLevelPurchased = upgradeLevel;
                    break;
                case WeaponType.BFG:
                    inst.currentGameSaveFileData.playerProgressionData.bfgLevelPurchased = upgradeLevel;
                    break;
                case WeaponType.Railgun:
                    inst.currentGameSaveFileData.playerProgressionData.railgunLevelPurchased = upgradeLevel;
                    break;
                default:
                    break;
            }
            PlayerController.UpgradeWeapon(upgradeData.upgradeLevels[upgradeLevel], upgradeData.weaponType);
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