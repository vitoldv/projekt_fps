using _Core.Saves;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace _Core.UI
{
    public class MainMenuUIController : MonoBehaviour
    {
        public event Action<GameSaveFileData> GameStarted;

        [Header("Elements")]
        [SerializeField] private Button startNewGameButton;
        [SerializeField] private Button loadGameButton;
        [SerializeField] private Button settingsButton;
        [SerializeField] private Button exitButton;
        [Header("Screens")]
        [SerializeField] private SavesScreen savesScreen;

        private GameSaveFileData[] gameSaveFiles;
        private Screen currentVisibleScreen;

        private void Awake()
        {
            SubscribeToElements();
        }

        private void Start()
        {
            gameSaveFiles = SaveManager.LoadGameSaveFiles();
            savesScreen.Init(gameSaveFiles);
            savesScreen.SaveSelected += OnSaveSelected;
        }

        private void SubscribeToElements()
        {
            startNewGameButton.onClick.AddListener(OnStartNewGameButtonClick);
            loadGameButton.onClick.AddListener(OnLoadGameButtonClick);
            settingsButton.onClick.AddListener(OnSettingsButtonClick);
            exitButton.onClick.AddListener(OnExitButtonClick);
        }

        private void UnsubscribeFromElements()
        {
            startNewGameButton.onClick.RemoveListener(OnStartNewGameButtonClick);
            loadGameButton.onClick.RemoveListener(OnLoadGameButtonClick);
            settingsButton.onClick.RemoveListener(OnSettingsButtonClick);
            exitButton.onClick.RemoveListener(OnExitButtonClick);
        }

        private void OnSaveSelected(Guid saveFileGuid)
        {
            GameStarted?.Invoke(gameSaveFiles.First(f => f.guid == saveFileGuid));
        }



        private void OnStartNewGameButtonClick()
        {
            var newGameSaveFile = new GameSaveFileData();
            newGameSaveFile.nextArena = 0;
            GameStarted?.Invoke(newGameSaveFile);
        }

        private void OnLoadGameButtonClick()
        {
            ShowButtons(false);
            currentVisibleScreen?.Hide();
            currentVisibleScreen = savesScreen;
            savesScreen.Show();
        }

        private void OnSettingsButtonClick()
        {

        }

        private void OnExitButtonClick()
        {

        }

        private void ShowButtons(bool show)
        {
            startNewGameButton.gameObject.SetActive(show);
            loadGameButton.gameObject.SetActive(show);
            settingsButton.gameObject.SetActive(show);
            exitButton.gameObject.SetActive(show);
        }
    }
}