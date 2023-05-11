using UnityEngine;
using UnityEngine.UI;

namespace _Core.UI
{
    public class SettingsScreen : Screen
    {
        [SerializeField] private Slider globalVolumeSlider;
        [SerializeField] private Slider musicVolumeSlider;
        [SerializeField] private Slider effectsVolumeSlider;
        [SerializeField] private Button backButton;

        private void Awake()
        {
            globalVolumeSlider.onValueChanged.AddListener(OnGlobalVolumeSliderValueChanged);
            musicVolumeSlider.onValueChanged.AddListener(OnMusicVolumeSliderValueChanged);
            effectsVolumeSlider.onValueChanged.AddListener(OnEffectsVolumeSliderValueChanged);
            backButton.onClick.AddListener(OnBackButtonClick);
        }

        private void OnGlobalVolumeSliderValueChanged(float value)
        {
            // TODO 
        }

        private void OnMusicVolumeSliderValueChanged(float value)
        {
            // TODO 
        }

        private void OnEffectsVolumeSliderValueChanged(float value)
        {
            // TODO 
        }

        private void OnBackButtonClick()
        {

        }
    }

    
}

