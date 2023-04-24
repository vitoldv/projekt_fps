using System.Collections.Generic;
using UnityEngine;

namespace _Core
{
    public abstract class SpawnerBase<T> : Singleton<T> where T: MonoBehaviour
    {
        protected delegate bool Condition<in T>(T obj);

        protected override void Initialize() { }

        protected T Spawn<T, U>(List<T> pool, T prefab, Condition<T> condition = null) 
            where T: MonoBehaviour, IPoolableObject
            where U: T
        {
            T obj = null;
            foreach (var poolObj in pool)
            {
                if (poolObj is U && !poolObj.gameObject.activeInHierarchy)
                {
                    if (condition != null)
                    {
                        if (!condition(poolObj))
                        {
                            continue;
                        }
                    }
                    obj = poolObj;
                    break;
                }
            }
            if (obj == null)
            {
                obj = Instantiate(prefab);
                pool.Add(obj);
            }
            obj.Enable();
            return obj;       
        }
        protected T Spawn<T>(List<T> pool, T prefab, Condition<T> condition = null) where T : MonoBehaviour, IPoolableObject
        {
            return Spawn<T, T>(pool, prefab, condition);
        }

        protected static void DisableAllObjectsInPool<T>(List<T> pool) where T: IPoolableObject
        {
            foreach (var obj in pool)
            {
                if (obj.IsActive())
                {
                    obj.Disable();
                }
            }
        }
    }
}