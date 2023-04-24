using Assets._Core.Scripts.Player.ShootingParameters;
using UnityEngine;

namespace Assets._Core.Scripts.Player.ShootingHandlers
{
    // Shooting handler for weapons that instantly hit the target in crosshair after shot
    public class FractionShootingHandler : ShootingHandlerBase
    {
        // Some kind of different effect
        //protected BulletTrace bulletTracePrefab;
        protected float scatterConeAngle = 3f;

        public FractionShootingHandler(FractionShootingParameters shootingParameters)
            : base(shootingParameters)
        {
            //this.bulletTracePrefab = bulletTracePrefab;
            this.scatterConeAngle = shootingParameters.scatterConeAngle;
        }

        protected override void Shoot()
        {

            // Calculate the direction of the bullet based on the scatter cone angle
            Quaternion scatterConeRotation = Quaternion.Euler(Random.Range(-scatterConeAngle, scatterConeAngle),
                                                              Random.Range(-scatterConeAngle, scatterConeAngle),
                                                              0f);

            var shootPoint = camera.transform.position;
            var shootDir = camera.transform.forward;

            RaycastHit hit;
            Vector3 bulletReachPoint;
            var ray = new Ray(camera.transform.position, shootDir);
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

            // SOME OTHER EFFECT
            // Create a trace for bullet
            //var trace = GameObject.Instantiate(bulletTracePrefab);
            //trace.Init(shootPoint, shootDir, bulletReachPoint);
        }
    }
}
