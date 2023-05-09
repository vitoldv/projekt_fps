using _Core.Common;
using _Core.Player;
using _Core.Spawners;
using UnityEngine;

namespace _Core.Collectibles
{
    public abstract class CollectibleBase : MonoBehaviour, IPoolableObject
    {
        protected abstract void ApplyEffect(PlayerController player);

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag(Tags.Player))
            {
                var player = other.gameObject.GetComponent<PlayerController>();
                ApplyEffect(player);
                Disable();
            }
        }

        public void Disable()
        {
            gameObject.SetActive(false);
        }

        public void Enable()
        {
            gameObject.SetActive(true);
        }

        public bool IsActive()
        {
            return gameObject.activeSelf;
        }
    }
}

