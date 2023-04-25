using Assets._Core.Scripts.Player;
using UnityEngine;

public interface IShootingTarget
{
    public void OnHit(Vector3 hitPoint, float damage, DamageType damageType);
}
