//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//namespace _Core.Sound
//{
//    public class SoundManager : MonoBehaviour
//    {
//        private static SoundManager _instance;
//        private const string SoundDataKey = "SoundDataKey";
//        [SerializeField] private List<SoundParameters> _sfxSounds;
//        [SerializeField] private List<SoundParameters> _musicSounds;
//        private float _sfxVolume;
//        private AudioSource _musicSource;
//        private List<Sound> _activeSFX;
        
//        public static float SFXVolume
//        {
//            get => _instance._sfxVolume;
//            private set => _instance._sfxVolume = value;
//        }
//        public static float MusicVolume
//        {
//            get => _instance._musicSource.volume;
//            private set => _instance._musicSource.volume = value;
//        }
//        public static float GlobalVolume
//        {
//            get => AudioListener.volume;
//            private set => AudioListener.volume = value;
//        }
            
//        private void Awake()
//        {
//            CreateSingleton();
//            InitializeManager();
//        }

//        private void Start()
//        {
//            LoadSoundData();
//        }

//        private void CreateSingleton()
//        {
//            if (_instance != null)
//            {
//                Destroy(gameObject);
//            }
//            else
//            {
//                _instance = this;
//                DontDestroyOnLoad(gameObject);
//            }
//        }

//        private void InitializeManager()
//        {
//            _activeSFX = new List<Sound>();
//            _musicSource = gameObject.AddComponent<AudioSource>();
//        }
        
//        public static void PlaySFX(string sfxKey, GameObject sourceObject)
//        {
//            SoundParameters soundParameters = FindSFXSound(sfxKey);
//            if (soundParameters.IsOneAtTime)
//            {
//                if (_instance._activeSFX.Find(x => x.soundKey == soundParameters.Key) != null)
//                {
//                    return;
//                }
//            }
//            //Sound sound = SpawnManager.SpawnSound();
//            //sound.Volume = SFXVolume;
//            //sound.InitializeAndPlay(soundParameters, _instance.OnSoundStop);
//            //_instance._activeSFX.Add(sound);
//        }
//        public static void StopSFX(string soundKey, float fadeTime = 0)
//        {
//            Sound sound = _instance._activeSFX.Find(s => s.soundKey == soundKey);
//            if (sound != null)
//            {
//                if (fadeTime == 0)
//                {
//                    sound.Stop();
//                }
//                else
//                {
//                    _instance.StartCoroutine(StopSFXWithFade(sound, fadeTime));
//                }    
//            }
//        }
//        private static IEnumerator StopSFXWithFade(Sound sfxSound, float fadeTime)
//        {
//            for (float i = 0f; i < fadeTime; i+=Time.deltaTime)
//            {
//                sfxSound.Volume = _instance._sfxVolume - i / fadeTime;
//                yield return null;
//            }
//            sfxSound.Stop();
//        }

//        public static void PlayMusic(string musicKey, float fadeTime = 0)
//        {
//            SoundParameters musicParameters = FindMusicSound(musicKey);
//            _instance._musicSource.clip = musicParameters.AudioClip;
//            _instance._musicSource.loop = musicParameters.IsLooped;
//            if (fadeTime == 0)
//            {
//                _instance._musicSource.Play();
//            }
//            else
//            {
//                _instance.StartCoroutine(PlayMusicWithFade(fadeTime));
//            }
//        }
//        private static IEnumerator PlayMusicWithFade(float fadeTime)
//        {
//            float finalVolume = MusicVolume;
//            MusicVolume = 0;
//            _instance._musicSource.Play();
//            for (float i = 0f; i < fadeTime; i+=Time.deltaTime)
//            {
//                MusicVolume = i/fadeTime * finalVolume;
//                yield return null;
//            }
//            MusicVolume = finalVolume;
//        }
//        public static void StopMusic(float fadeTime = 0)
//        {
//            if (fadeTime == 0)
//            {
//                _instance._musicSource.Stop();
//            }
//            else
//            {
//                _instance.StartCoroutine(StopMusicWithFade(fadeTime));
//            }
//        }
//        private static IEnumerator StopMusicWithFade(float fadeTime)
//        {
//            float initialVolume = MusicVolume;
//            for (float i = 0f; i < fadeTime; i+=Time.deltaTime)
//            {
//                MusicVolume = initialVolume - i / fadeTime;
//                yield return null;
//            }
//            _instance._musicSource.Stop();
//            MusicVolume = initialVolume;
//        }
        
//        private void OnSoundStop(string soundKey)
//        {
//            int index = _activeSFX.FindIndex(s => s.soundKey == soundKey);
//            _activeSFX.RemoveAt(index);
//        }

//        private static SoundParameters FindSFXSound(string key)
//        {
//            return _instance._sfxSounds.Find(s => s.Key == key);
//        }
//        private static SoundParameters FindMusicSound(string key)
//        {
//            return _instance._musicSounds.Find(s => s.Key == key);
//        }

//        private static void LoadSoundData()
//        {
//            SoundData soundData = SaveManager.Load<SoundData>(SoundDataKey);
//            if (soundData == null)
//            {
//                SFXVolume = 1;
//                MusicVolume = 1;
//                GlobalVolume = 1;
//                SaveSoundData();
//            }
//            else
//            {
//                SFXVolume = soundData.sfxVolume;
//                MusicVolume = soundData.musicVolume;
//                GlobalVolume = soundData.globalVolume;
//            }
//        }
//        private static void SaveSoundData()
//        {
//            SoundData soundData = new SoundData
//            {
//                globalVolume = GlobalVolume,
//                sfxVolume = SFXVolume,
//                musicVolume = MusicVolume
//            };
//            SaveManager.Save(SoundDataKey, soundData);
//        }

//        public static void EnableMusic()
//        {
//            SetMusicVolume(1);
//        }
//        public static void MuteMusic()
//        {
//            SetMusicVolume(0);
//        }
//        private static void SetMusicVolume(float value)
//        {
//            MusicVolume = value;
//            SaveSoundData();
//        }
//        public static void EnableSFX()
//        {
//            SetSFXVolume(1);
//        }
//        public static void MuteSFX()
//        {
//            SetSFXVolume(0);
//        }
//        private static void SetSFXVolume(float value)
//        {
//            SFXVolume = value;
//            foreach (Sound sound in _instance._activeSFX)
//            {
//                sound.Volume = _instance._sfxVolume;
//            }
//            SaveSoundData();
//        }

//        public static void EnableGlobalSound()
//        {
//            SetGlobalSound(1);
//        }
//        public static void MuteGlobalSound()
//        {
//            SetGlobalSound(0);
//        }
//        private static void SetGlobalSound(float value)
//        {
//            GlobalVolume = value;
//            SaveSoundData();
//        }

//        public static void ResetSoundData()
//        {
//            SaveManager.Delete<SoundData>(SoundDataKey);
//            LoadSoundData();
//            UIManager.UpdateSettingsScreen();
//        }
//    }
//}