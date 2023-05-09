using UnityEngine;

namespace _Core.Arena
{
    [RequireComponent(typeof(Collider))]
    public class ArenaDoor : MonoBehaviour
    {
        private Collider collider;

        private void Awake()
        {
            collider = GetComponent<Collider>();
        }

        public void Open()
        {
            // TODO Play animation
            collider.enabled = false;
        }

        public void Close()
        {
            // TODO Play animation
            collider.enabled = true;
        }
    }
}

