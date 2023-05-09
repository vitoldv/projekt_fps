using UnityEngine;

namespace _Core.Player
{
    // Shooting handler for weapons that instantly hit the target in crosshair after shot
    public class FractionShootingHandler : ShootingHandlerBase
    {
        // Some kind of different effect
        //protected BulletTrace bulletTracePrefab;
        private FractionShootingHandlerArgs fracParams => base.baseParams as FractionShootingHandlerArgs;


        public FractionShootingHandler(FractionShootingHandlerArgs shootingParameters)
            : base(shootingParameters)
        {
            //this.bulletTracePrefab = bulletTracePrefab;
            //this.fracParams.scatterConeAngle = shootingParameters.scatterConeAngle;
            //this.fracParams.bulletTracePrefab = shootingParameters.bulletTracePrefab;
        }

        protected override void Shoot()
        {
            for (int i = 0; i < fracParams.numberOfFractions; i++)
            {
                // Calculate the direction of the bullet based on the scatter cone angle
                Quaternion scatterConeRotation = Quaternion.Euler(Random.Range(-fracParams.scatterConeAngle, fracParams.scatterConeAngle),
                                                                  Random.Range(-fracParams.scatterConeAngle, fracParams.scatterConeAngle),
                                                                  0f);

                var shootPoint = playerController.Camera.transform.position;
                var shootDir = scatterConeRotation * playerController.Camera.transform.forward;

                RaycastHit hit;
                Vector3 bulletReachPoint;
                var ray = new Ray(playerController.Camera.transform.position, shootDir);
                if (Physics.Raycast(ray, out hit, Mathf.Infinity))
                {
                    bulletReachPoint = hit.point;
                    if (hit.collider.TryGetComponent<IShootingTarget>(out var target) && target != null)
                    {
                        target.OnHit(hit.point, fracParams.damage, DamageType.Shot);
                    }
                }
                else
                {
                    bulletReachPoint = default;
                }

                // SOME OTHER EFFECT
                // Create a trace for bullet
                var trace = GameObject.Instantiate(fracParams.bulletTracePrefab);
                trace.Init(shootPoint, shootDir, bulletReachPoint);
            }
        }
    }
}
