using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Core.UI
{
    public class PauseMenu : Screen
    {
        public event Action ReturnToMenuClicked;
        public event Action ResumeGameClicked;

        [SerializeField] private Button resumeButton;
        [SerializeField] private Button returnToMenuButton;

        private void Awake()
        {
            resumeButton.onClick.AddListener(OnResumeButtonClick);
            returnToMenuButton.onClick.AddListener(OnReturnToMenuButtonClick);
        }

        private void OnResumeButtonClick()
        {
            ResumeGameClicked?.Invoke();
        }

        private void OnReturnToMenuButtonClick()
        {
            ReturnToMenuClicked?.Invoke();
        }
    }
}

