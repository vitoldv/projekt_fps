//using System;
//using UnityEngine;
//using _Core.Common;
//using _Core.Spawners;

//namespace _Core.Sound
//{
//    [RequireComponent(typeof(AudioSource))]
//    public class Sound : MonoBehaviour, IPoolableObject
//    {
//        public string soundKey;
//        private AudioSource _audioSource;
//        private bool _isInitialized;
//        private Action<string> onStopCallback;

//        public float Volume
//        {
//            get => _audioSource.volume;
//            set => _audioSource.volume = value;
//        }
        
//        private void Awake()
//        {
//            _audioSource = GetComponent<AudioSource>();
//        }
        
//        private void Update()
//        {
//            CheckForEnd();
//        }

//        public void InitializeAndPlay(SoundParameters soundParameters, Action<string> onStop)
//        {
//            soundKey = soundParameters.Key;
//            onStopCallback = onStop;
//            _audioSource.clip = soundParameters.AudioClip;
//            _audioSource.loop = soundParameters.IsLooped;
//            _isInitialized = true;
//            _audioSource.Play();
//        }

//        public void Stop()
//        {
//            Disable();
//        }

//        private void CheckForEnd()
//        {
//            if (_isInitialized)
//            {
//                if (!_audioSource.isPlaying)
//                {
//                    Disable();
//                }
//            }
//        }

//        public void Enable()
//        {
//            gameObject.SetActive(true);
//        }

//        public void Disable()
//        {
//            gameObject.SetActive(false);
//        }

//        private void OnDisable()
//        {
//            _isInitialized = false;
//            onStopCallback.Invoke(soundKey);
//        }

//        public bool IsActive()
//        {
//            return gameObject.activeInHierarchy;
//        }

//    }
//}