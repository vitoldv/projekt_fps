using System.IO;
using System.Xml.Serialization;
using System;
using UnityEngine;
using _Core.Interfaces;
using System.Collections.Generic;

namespace _Core.Saves
{
    public class SaveManager : Singleton<SaveManager>
    {
        private string saveFilesFolderLocation;

        protected override void Initialize()
        {
            inst.saveFilesFolderLocation = Path.Combine(Application.persistentDataPath, "saves");
        }

        //FOR TESTING
        //public GameSaveFileData testData;
        //public bool isN;

        //[ContextMenu("Test")]
        //public void Test()
        //{
        //    SaveGameSaveFile(testData, isN);
        //}

        //[ContextMenu("Test2")]
        //public void Test2()
        //{
        //    var files = LoadGameSaveFiles();
        //    foreach (var item in files)
        //    {
        //        print(Print(item));
        //    }
        //}

        //private string Print(GameSaveFileData saveFileData)
        //{
        //    return "Player Progression Data:\n" +
        //        "- isDashPurchased: " + saveFileData.playerProgressionData.isDashPurchased + "\n" +
        //        "- isQuakePurchased: " + saveFileData.playerProgressionData.isQuakePurchased + "\n" +
        //        "- isDoubleJumpPurchased: " + saveFileData.playerProgressionData.isDoubleJumpPurchased + "\n" +
        //        "- dashLevelPurchased: " + saveFileData.playerProgressionData.dashLevelPurchased + "\n" +
        //        "- quakeLevelPurchased: " + saveFileData.playerProgressionData.quakeLevelPurchased + "\n" +
        //        "- weaponPurchased: " + saveFileData.playerProgressionData.weaponPurchased + "\n" +
        //        "- pistolLevelPurchased: " + saveFileData.playerProgressionData.pistolLevelPurchased + "\n" +
        //        "- riflelLevelPurchased: " + saveFileData.playerProgressionData.riflelLevelPurchased + "\n" +
        //        "- shotgunLevelPurchased: " + saveFileData.playerProgressionData.shotgunLevelPurchased + "\n" +
        //        "- bfgLevelPurchased: " + saveFileData.playerProgressionData.bfgLevelPurchased + "\n" +
        //        "- railgunLevelPurchased: " + saveFileData.playerProgressionData.railgunLevelPurchased + "\n\n" +
        //        "Next Arena: " + saveFileData.nextArena + "\n" +
        //        "Last Save Date: " + saveFileData.lastSaveDate + "\n" +
        //        "GUID: " + saveFileData.guid;
        //}

        public static void SaveGameSaveFile(GameSaveFileData saveData)
        {
            if (!Directory.Exists(inst.saveFilesFolderLocation))
            {
                Directory.CreateDirectory(inst.saveFilesFolderLocation);
            }

            if (saveData.guid == Guid.Empty)
            {
                saveData.guid = Guid.NewGuid();
            }
            saveData.lastSaveDate = DateTime.Now;

            var path = Path.Combine(inst.saveFilesFolderLocation, $"gamesave_{saveData.guid.ToString()}.xml");
            path = path.Replace("\\", "/");
            XmlSerializer serializer = new XmlSerializer(typeof(GameSaveFileData));
            FileStream fileStream = new FileStream(path, FileMode.Create);
            serializer.Serialize(fileStream, saveData);
            fileStream.Close();
        }

        public static GameSaveFileData[] LoadGameSaveFiles()
        {
            if(!Directory.Exists(inst.saveFilesFolderLocation))
            {
                Directory.CreateDirectory(inst.saveFilesFolderLocation);
                Debug.LogWarning("No save file found.");
                return null;
            }

            var saveFilesPaths = Directory.GetFiles(inst.saveFilesFolderLocation);
            var gameSaveFileDatas = new List<GameSaveFileData>();

            foreach (var saveFilePath in saveFilesPaths)
            {
                XmlSerializer serializer = new XmlSerializer(typeof(GameSaveFileData));
                FileStream fileStream = new FileStream(saveFilePath, FileMode.Open);
                GameSaveFileData loadedData = (GameSaveFileData)serializer.Deserialize(fileStream);
                fileStream.Close();
                FillNewVariablesWithDefaultValues(loadedData);
                gameSaveFileDatas.Add(loadedData);
            }

            // Return the loaded data
            return gameSaveFileDatas.ToArray();
        }

        private static void FillNewVariablesWithDefaultValues(object obj)
        {
            // Get the type of the object
            Type objectType = obj.GetType();
            // Get the fields of the object
            var fields = objectType.GetFields();
            // Loop through each field
            foreach (var field in fields)
            {
                // Check if the field is a struct or class
                if (field.FieldType.IsValueType || field.FieldType == typeof(string))
                {
                    // Check if the field is null
                    if (field.GetValue(obj) == null)
                    {
                        // Set the field to the default value
                        field.SetValue(obj, Activator.CreateInstance(field.FieldType));
                    }
                }
                else
                {
                    // Recursively fill any new variables with default values in nested objects
                    FillNewVariablesWithDefaultValues(field.GetValue(obj));
                }
            }
        }
    }
}