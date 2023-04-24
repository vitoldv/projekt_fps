using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets._Core.Scripts.Player.ShootingParameters
{
    public class ShootingParametersBase
    {
        public float roundsPerMinute;
        public float reloadTime;
        public int gunShopCapacity;
        public PlayerController playerController;
        public Camera camera;
    }

    public class HitScanShootingParameters : ShootingParametersBase
    {
        public BulletTrace bulletTracePrefab;
    }
    
    public class FractionShootingParameters : ShootingParametersBase
    {
        // some effect for shotgun shot
        public float scatterConeAngle;
    }

    public class ProjectileShootingParameters : ShootingParametersBase
    {
        public ProjectileBase projectilePrefab;
    }
}
