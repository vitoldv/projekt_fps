using UnityEngine;
using _Core;
using Assets._Core.Scripts;
using Assets._Core.Scripts.Saves;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager>
{
    private string MainMenuSceneName = "MainMenuScene";

    public static Layers GameLayers;
    public Layers layers;

    public WeaponsConfiguration DefaultWeaponConfiguration;
    public static WeaponsConfiguration CurrentWeaponConfiguration { get; private set; }
    
    public PlayerController PlayerController { get; private set; }
    
    [Header("Prefabs")]
    [SerializeField] private PlayerController playerControllerPrefab;
    [SerializeField] private HUD hudPrefab;
    
    private GameSaveFileData currentGameSaveFileData;
    private ArenaManager currentArenaManager;
    private MainMenu mainMenu;
    private HUD hud;

    protected override void Initialize()
    {
        if(SceneManager.GetActiveScene().name == MainMenuSceneName)
        {
            mainMenu = FindFirstObjectByType<MainMenu>();
            mainMenu.StartGameClicked += OnGameStartInitiated;
        }
        else
        {
            GameLayers = layers;
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

    private void OnGameStartInitiated()
    {
        var gameFile = SaveManager.LoadGame();
        OnSaveFileSelected(gameFile);
    }

    private void OnSaveFileSelected(GameSaveFileData gameSaveFile)
    {
        currentGameSaveFileData = gameSaveFile;
        StartGame(currentGameSaveFileData);
    }

    private void StartGame(GameSaveFileData gameSaveFile)
    {
        print("start game");
        // temporarily here
        mainMenu.StartGameClicked -= OnGameStartInitiated;
        ArenaLoader.LoadArena(gameSaveFile.nextArena);
    }

    private void OnArenaLoaded(string arenaName)
    {
        print("arena loaded");
        currentArenaManager = GameObject.FindGameObjectWithTag(Tags.ArenaManager).GetComponent<ArenaManager>();
        CreatePlayer();
        CreateHUD();
    }

    private void CreatePlayer()
    {
        var player = Instantiate(playerControllerPrefab);
        player.Init(currentGameSaveFileData.playerProgressionData);
        var initialTransform = currentArenaManager.PlayerInitialTransform;
        player.transform.SetPositionAndRotation(initialTransform.position, initialTransform.rotation);
        PlayerController = player;
    }

    private void CreateHUD()
    {
        hud = Instantiate(hudPrefab);
        hud.Init(PlayerController);
        hud.Show();
    }
}
