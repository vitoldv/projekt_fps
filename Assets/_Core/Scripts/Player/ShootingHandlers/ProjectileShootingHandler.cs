using Assets._Core.Scripts.Player.ShootingParameters;
using UnityEngine;

namespace Assets._Core.Scripts.Player.ShootingHandlers
{
    // Shooting handler for weapons that launch projectile
    public class ProjectileShootingHandler : ShootingHandlerBase
    {
        protected ProjectileBase projectilePrefab;

        public ProjectileShootingHandler(ProjectileShootingParameters shootingParameters)
            : base(shootingParameters)
        {
            this.projectilePrefab = shootingParameters.projectilePrefab;
        }

        protected override void Shoot()
        {
            var shootPoint = camera.transform.position;
            var shootDir = camera.transform.forward;
            var projectile = GameObject.Instantiate(projectilePrefab, camera.transform.position + camera.transform.forward * 2, Quaternion.identity);
            projectile.Init(shootDir, 2);
        }
    }
}
