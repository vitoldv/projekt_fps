using UnityEngine;

namespace _Core.Player
{
    public interface IShootingTarget
    {
        public void OnHit(Vector3 hitPoint, float damage, DamageType damageType);
    }
}

