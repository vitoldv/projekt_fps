using Assets._Core.Scripts.Player.ShootingParameters;

namespace Assets._Core.Scripts.Player.ShootingHandlers
{
    // Shooting handler for weapons that launch projectile
    public class ProjectileShootingHandler : ShootingHandlerBase
    {
        protected ProjectileBase projectilePrefab;
        protected float projectileSpeed;

        public ProjectileShootingHandler(ProjectileShootingHandlerArgs shootingParameters)
            : base(shootingParameters)
        {
            this.projectilePrefab = shootingParameters.projectilePrefab;
        }

        protected override void Shoot()
        {
            var shootPoint = camera.transform.position;
            var shootDir = camera.transform.forward;
            var projectile = WeaponSpawner.SpawnBFGProjectile(camera.transform.position + camera.transform.forward * 2);
            projectile.Init(shootDir, 2);
        }
    }
}
