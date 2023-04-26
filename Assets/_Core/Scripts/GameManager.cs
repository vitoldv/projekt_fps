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
    
    [HideInInspector] public PlayerController PlayerController;
    [SerializeField] private PlayerController playerControllerPrefab;

    private GameSaveFileData currentGameSaveFileData;

    [SerializeField] private HUD hudPrefab;

    private MainMenu mainMenuObject;
    private HUD hud;

    protected override void Initialize()
    {
        if(SceneManager.GetActiveScene().name == MainMenuSceneName)
        {
            mainMenuObject = FindFirstObjectByType<MainMenu>();
            mainMenuObject.StartGameClicked += OnGameStartInitiated;
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
        ArenaManager.ArenaLoaded += OnArenaLoaded;
        ArenaManager.ArenaInitialized += OnArenaInitialized;
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
        // temporarily here
        print("start game");
        mainMenuObject.StartGameClicked -= OnGameStartInitiated;
        ArenaManager.LoadArena(gameSaveFile.nextArena);
    }

    private void OnArenaLoaded(string arenaName)
    {
        print("arena loaded");
        var player = Instantiate(playerControllerPrefab);
        player.Init(currentGameSaveFileData.playerProgressionData);
        PlayerController = player;
        ArenaManager.InitializeCurrentArena(player);
    }

    private void OnArenaInitialized()
    {
        // HUD creation
        hud = Instantiate(hudPrefab);
        hud.Init(PlayerController);
        hud.Show();
    }
}
