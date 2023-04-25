using UnityEngine;

public class HealthCollectible : CollectibleBase
{
    [SerializeField] private float healAmount;

    protected override void ApplyEffect(PlayerController player)
    {
        player.Heal(healAmount);
    }
}
