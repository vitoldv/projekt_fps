using _Core.Spawners;

namespace _Core.Player
{
    // Shooting handler for weapons that launch projectile
    public class ProjectileShootingHandler : ShootingHandlerBase
    {
        private ProjectileShootingHandlerArgs projectileParams => base.baseParams as ProjectileShootingHandlerArgs;


        public ProjectileShootingHandler(ProjectileShootingHandlerArgs shootingParameters)
            : base(shootingParameters)
        {
        }

        protected override void Shoot()
        {
            var shootPoint = playerController.Camera.transform.position;
            var shootDir = playerController.Camera.transform.forward;
            var projectile = WeaponSpawner.SpawnBFGProjectile(playerController.Camera.transform.position + playerController.Camera.transform.forward * 2);
            projectile.Init(shootDir, 2, projectileParams.damage, projectileParams.explosionRadius);
        }
    }
}
