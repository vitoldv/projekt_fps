using _Core.Common;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class TankAttackCollider : MonoBehaviour
{
    public bool IsPlayerIn { get; private set; }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag(Tags.Player))
        {
            IsPlayerIn = true;
        }       
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(Tags.Player))
        {
            IsPlayerIn = false;
        }
    }
}
