using _Core.Saves;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Core.UI
{
    public class SaveView : MonoBehaviour
    {
        public event Action<Guid> Selected;

        [SerializeField] private Button selectButton;
        [SerializeField] private TextMeshProUGUI arenaNumberText;
        [SerializeField] private TextMeshProUGUI lastSaveDateText;

        private Guid saveGuid;

        private void Awake()
        {
            selectButton.onClick.AddListener(OnSelectButtonClick);
        }

        public void Init(GameSaveFileData saveFileData)
        {
            this.saveGuid = saveFileData.guid;
            lastSaveDateText.text = saveFileData.lastSaveDate.ToString();
            arenaNumberText.text = saveFileData.nextArena.ToString();
        }

        private void OnSelectButtonClick()
        {
            Selected?.Invoke(saveGuid);
        }
    }

}
