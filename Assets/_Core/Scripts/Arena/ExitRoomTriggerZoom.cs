using System;
using UnityEngine;

namespace _Core.Arena
{
    public class ExitRoomTriggerZone : MonoBehaviour
    {
        public event Action PlayerEnterTriggerZone;

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag(Tags.Player))
            {
                PlayerEnterTriggerZone?.Invoke();
            }
        }
    }
}
