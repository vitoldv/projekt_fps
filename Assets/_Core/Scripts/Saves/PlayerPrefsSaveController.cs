using UnityEngine;

namespace _Core
{
    public class PlayerPrefsSaveController : ISaveController
    {
        public static string GLOBAL_AUDIO_VOLUME = nameof(GLOBAL_AUDIO_VOLUME);
        public static string MUSIC_VOLUME = nameof(MUSIC_VOLUME);
        public static string EFFECTS_VOLUME = nameof(EFFECTS_VOLUME);

        public void Save<T>(string key, T value)
        {
            string jsonString = JsonUtility.ToJson(value);
            PlayerPrefs.SetString(key, jsonString);
        }

        public T Load<T>(string key)
        {
            string jsonString = PlayerPrefs.GetString(key);
            return JsonUtility.FromJson<T>(jsonString);
        }

        public void Delete<T>(string key)
        {
            PlayerPrefs.DeleteKey(key);
        }
    }
}