using UnityEngine;

namespace _Core.Player
{
    public class ShootingHandlerArgs
    {
        public int initialAmmoAmount;
        public float roundsPerMinute;
        public float reloadTime;
        public float damage;
        public int gunShopCapacity;        
        public PlayerController playerController;
    }

    public class HitScanShootingHandlerArgs : ShootingHandlerArgs
    {
        public BulletTrace bulletTracePrefab;
    }
    
    public class FractionShootingHandlerArgs : ShootingHandlerArgs
    {
        // some effect for shotgun shot
        public float scatterConeAngle;
        public int numberOfFractions;
        public BulletTrace bulletTracePrefab;
    }

    public class ProjectileShootingHandlerArgs : ShootingHandlerArgs
    {
        public float projectileSpeed;
        public float explosionRadius;
        public BfgProjectile projectilePrefab;
    }
}
