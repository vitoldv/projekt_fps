using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace _Core
{
    public class MyToggle : MonoBehaviour
    {
        [SerializeField] private Toggle _toggle;
        

        public void Initialize(bool initialValue, UnityAction<bool> onValueChanged)
        {
            _toggle.SetIsOnWithoutNotify(initialValue);
            _toggle.onValueChanged.AddListener(onValueChanged);
        }


        public void SetToggleGroup(ToggleGroup toggleGroup)
        {
            _toggle.group = toggleGroup;
        }
    }
}