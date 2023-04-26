using System;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public event Action StartGameClicked;

    [SerializeField] private Button startButton;
    [SerializeField] private Button settingsButton;
    [SerializeField] private Button exitButton;

    private void Awake()
    {
        SubscribeToElements();
    }

    private void SubscribeToElements()
    {
        startButton.onClick.AddListener(OnStartButtonClick);
        settingsButton.onClick.AddListener(OnSettingsButtonClick);
        exitButton.onClick.AddListener(OnExitButtonClick);
    }

    private void UnsubscribeFromElements()
    {
        startButton.onClick.RemoveListener(OnStartButtonClick);
        settingsButton.onClick.RemoveListener(OnSettingsButtonClick);
        exitButton.onClick.RemoveListener(OnExitButtonClick);
    }

    private void Update()
    {
        
    }

    private void OnStartButtonClick()
    {
        StartGameClicked?.Invoke();
    }

    private void OnSettingsButtonClick()
    {

    }

    private void OnExitButtonClick()
    {

    }
}
