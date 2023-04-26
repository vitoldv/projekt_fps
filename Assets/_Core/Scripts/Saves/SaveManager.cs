using Assets._Core.Scripts.Saves;
using System.IO;
using System.Xml.Serialization;
using System;
using UnityEngine;

namespace _Core
{
    public class SaveManager : Singleton<SaveManager>
    {
        private ISaveController _saveController = new PlayerPrefsSaveController();

        /// <summary>
        /// ///////////////////////////////////
        /// </summary>

        private string SAVE_FILES_FOLDER_PATH;

        protected override void Initialize()
        {
            inst.SAVE_FILES_FOLDER_PATH = Application.persistentDataPath + "/gameSave.xml";
        }

        //[ContextMenu("SaveData")]
        //public void SaveData()
        //{
        //    GameSaveFileData data = new GameSaveFileData
        //    {
        //        playerProgressionData = new PlayerProgressionData
        //        {
        //            isDashPurchased = true,
        //            isQuakePurchased = false,
        //            dashLevelPurchased = 2,
        //            quakeLevelPurchased = 0,
        //            weaponPurchased = WeaponType.Pistol | WeaponType.Rifle | WeaponType.BFG,
        //            pistolLevelPurchased = 1,
        //            riflelLevelPurchased = 0,
        //            shotgunLevelPurchased = 0,
        //            bfgLevelPurchased = 0,
        //            railgunLevelPurchased = 0
        //        },
        //        nextArena = 3
        //    };
        //    SaveGame(data);

        //}

        //[ContextMenu("LoadData")]
        //public void LoadData()
        //{
        //    GameSaveFileData loadedData = LoadGame();
        //}


        public static void SaveGame(GameSaveFileData saveData)
        {
            // Create the XML serializer
            XmlSerializer serializer = new XmlSerializer(typeof(GameSaveFileData));

            // Create the stream to write the file
            FileStream fileStream = new FileStream(inst.SAVE_FILES_FOLDER_PATH, FileMode.Create);

            // Serialize the data to the file
            serializer.Serialize(fileStream, saveData);

            // Close the file stream
            fileStream.Close();
        }

        public static GameSaveFileData LoadGame()
        {
            // Check if the file exists
            if (!File.Exists(inst.SAVE_FILES_FOLDER_PATH))
            {
                Debug.LogWarning("No save file found.");
                return new GameSaveFileData();
            }

            // Create the XML serializer
            XmlSerializer serializer = new XmlSerializer(typeof(GameSaveFileData));

            // Create the stream to read the file
            FileStream fileStream = new FileStream(inst.SAVE_FILES_FOLDER_PATH, FileMode.Open);

            // Deserialize the data from the file
            GameSaveFileData loadedData = (GameSaveFileData)serializer.Deserialize(fileStream);

            // Close the file stream
            fileStream.Close();

            // Fill any new variables with default values using reflection
            FillNewVariablesWithDefaultValues(loadedData);

            // Return the loaded data
            return loadedData;
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