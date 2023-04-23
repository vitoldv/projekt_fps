using UnityEngine;

namespace Assets._Core.Scripts.Player.ShootingHandlers
{
    public class AutoGunShootingHandler : IShootingHandler
    {
        private ShootingParameters shootingParameters;
        public BulletTrace BulletTrace;
        public int BulletsLeft;
        private Camera camera;
        public void Init(ShootingParameters param, Camera camera)
        {
            shootingParameters = param;
            this.camera = camera;
        }

        public void Shoot()
        {
            var shootPoint = camera.transform.position - new Vector3(0f, 0.2f, 0f);
            var shootDir = camera.transform.forward;
            var trace = GameObject.Instantiate(BulletTrace);
            
            RaycastHit hit;
            var ray = new Ray(camera.transform.position, shootDir);
            Vector3 bulletReachPoint;
            if (Physics.Raycast(ray, out hit, Mathf.Infinity))
            {
                bulletReachPoint = hit.point;
                if (hit.collider.TryGetComponent<IShootingTarget>(out var target) && target != null)
                {
                    target.OnHit(hit.point);
                }
            }
            else
            {
                bulletReachPoint = default;
            }
            trace.Init(shootPoint, shootDir, bulletReachPoint);
        }
    }
}
