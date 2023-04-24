﻿using Assets._Core.Scripts.Player.ShootingParameters;
using UnityEngine;

namespace Assets._Core.Scripts.Player.ShootingHandlers
{
    // Shooting handler for weapons that instantly hit the target in crosshair after shot
    public class HitScanShootingHandler : ShootingHandlerBase
    {
        protected BulletTrace bulletTracePrefab;

        public HitScanShootingHandler(HitScanShootingParameters shootingParameters) 
            : base(shootingParameters)
        {
            this.bulletTracePrefab = shootingParameters.bulletTracePrefab;
        }

        protected override void Shoot()
        {
            Debug.Log("sdf");
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
            // Create a trace for bullet
            var trace = GameObject.Instantiate(bulletTracePrefab);
            trace.Init(shootPoint, shootDir, bulletReachPoint);
        }
    }
}