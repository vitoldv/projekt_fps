using _Core.Common;
using _Core.Player;
using UnityEngine;

namespace _Core.Enemies
{
    public class InsectProjectile : ProjectileBase
    {
        protected override void OnHit(Collider collider)
        {
            if(collider.gameObject.TryGetComponent<PlayerController>(out var playerController))
            {
                playerController.ReceiveDamage(this.damage);
            }
            base.OnHit(collider);
        }
    }
}
