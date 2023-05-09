using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace _Core
{
    [RequireComponent(typeof(Button))]
    public class MyButton: MonoBehaviour
    {
        [SerializeField] private Button _button;
        [SerializeField] private string _soundOnClick;

        private void Awake()
        {
            _button = GetComponent<Button>();
            _button.onClick.AddListener(OnClick);
        }

        public void SubscribeOnClick(UnityAction onClickCallback)
        {
            _button.onClick.AddListener(onClickCallback);
        }

        private void OnClick()
        {
            //SoundManager.PlaySFX(_soundOnClick);
        }

        public void DisableButton()
        {
            _button.interactable = false;
        }
    }
}