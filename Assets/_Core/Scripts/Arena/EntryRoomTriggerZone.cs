using System;
using UnityEngine;

namespace _Core.Arena
{
    public  class EntryRoomTriggerZone : MonoBehaviour
    {
        public event Action PlayerLeftTriggerZone;

        private void OnTriggerExit(Collider other)
        {
            if(other.CompareTag(Tags.Player))
            {
                PlayerLeftTriggerZone?.Invoke();
            }
        }
    }
}
