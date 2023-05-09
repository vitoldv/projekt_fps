using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using _Core.Saves;
using System;

namespace _Core.UI
{
    public class SavesScreen : Screen
    {
        public event Action<Guid> SaveSelected;

        [SerializeField] private LayoutGroup layout;
        [SerializeField] private SaveView saveViewPrefab;

        public void Init(GameSaveFileData[] gameSaveFileDatas)
        {
            for (int i = 0; i < gameSaveFileDatas.Length; i++)
            {
                GameSaveFileData gameSaveFile = gameSaveFileDatas[i];
                var saveView = Instantiate(saveViewPrefab, layout.transform);
                saveView.Init(gameSaveFile);
                saveView.Selected += OnSaveSelected;
            }
        }

        public void OnSaveSelected(Guid saveGuid)
        {
            SaveSelected?.Invoke(saveGuid);
        }
    }
}


