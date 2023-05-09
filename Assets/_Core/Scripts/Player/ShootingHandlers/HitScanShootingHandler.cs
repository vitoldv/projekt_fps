using _Core.Spawners;
using UnityEngine;

namespace _Core.Player
{
    // Shooting handler for weapons that instantly hit the target in crosshair after shot
    public class HitScanShootingHandler : ShootingHandlerBase
    {
        private HitScanShootingHandlerArgs hitScanParams => base.baseParams as HitScanShootingHandlerArgs;

        public HitScanShootingHandler(HitScanShootingHandlerArgs shootingParameters) 
            : base(shootingParameters)
        {
        }

        protected override void Shoot()
        {
            var shootPoint = playerController.Camera.transform.position;
            var shootDir = playerController.Camera.transform.forward;
            RaycastHit hit;
            Vector3 bulletReachPoint;
            var ray = new Ray(playerController.Camera.transform.position, shootDir);
            if (Physics.Raycast(ray, out hit, Mathf.Infinity))
            {
                bulletReachPoint = hit.point;
                if (hit.collider.TryGetComponent<IShootingTarget>(out var target) && target != null)
                {
                    target.OnHit(hit.point, hitScanParams.damage, DamageType.Shot);
                }
            }
            else
            {
                bulletReachPoint = default;
            }
            // Create a trace for bullet
            var trace = WeaponSpawner.SpawnBulletTrace();
            trace.Init(shootPoint, shootDir, bulletReachPoint);
        }
    }
}
