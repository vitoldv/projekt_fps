using _Core;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

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

    private void OnCollisionEnter(Collision collision)
    {
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
