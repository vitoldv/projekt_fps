using _Core;
using UnityEngine;

public class AmmoCollectible : CollectibleBase
{
    public WeaponType weaponType;
    public int ammoAmount;

    protected override void ApplyEffect(PlayerController player)
    {
        player.GetAmmo(ammoAmount, weaponType);
    }
}
