using UnityEngine;

namespace _Core.Interfaces
{
    public abstract class Singleton<T> : MonoBehaviour where T: MonoBehaviour
    {
        protected bool isInitialized;
        protected static T inst;

        private void Awake()
        {
            CreateSingleton();
            Initialize();
        }
        
        private void CreateSingleton()
        {
            if (inst != null)
            {
                Destroy(gameObject);
            }
            else
            {
                inst = this as T;
                DontDestroyOnLoad(gameObject);
            }
        }

        protected virtual void Initialize()
        {
            isInitialized = true;
        }
    }
}