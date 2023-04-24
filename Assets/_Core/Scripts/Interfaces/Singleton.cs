using UnityEngine;

namespace _Core
{
    public abstract class Singleton<T> : MonoBehaviour where T: MonoBehaviour
    {
        protected static T _instance;

        private void Awake()
        {
            CreateSingleton();
            Initialize();
        }
        
        private void CreateSingleton()
        {
            if (_instance != null)
            {
                Destroy(gameObject);
            }
            else
            {
                _instance = this as T;
                DontDestroyOnLoad(gameObject);
            }
        }

        protected abstract void Initialize();
    }
}