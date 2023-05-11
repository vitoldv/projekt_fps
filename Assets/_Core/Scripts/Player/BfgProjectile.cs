using _Core.Common;
using UnityEngine;

namespace _Core.Player
{
    public  class BfgProjectile : ProjectileBase
    {
        [SerializeField] private float explosionRadius;

        protected override void OnHit(Collider collider)
        {
            Explode();
            base.OnHit(collider);
        }

        private void Explode()
        {
            // play some effect
            var colliders = Physics.OverlapSphere(transform.position, explosionRadius, onDestroyLayers);
            foreach (var collider in colliders)
            {
                if (collider.gameObject.TryGetComponent<IShootingTarget>(out var shootingTarget))
                {
                    Vector3 hitPoint = collider.ClosestPoint(transform.position);
                    float distanceToObject = Vector3.Distance(hitPoint, transform.position);
                    // the damage is less the further the object from epicenter
                    float actualDamage = damage * (1 - distanceToObject / explosionRadius);
                    shootingTarget.OnHit(hitPoint, actualDamage, DamageType.Explosion);
                }
            }
        }
    }
}
