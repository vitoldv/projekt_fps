using _Core.Player;
using System.Collections.Generic;
using UnityEngine;

namespace _Core.Spawners
{
    public class WeaponSpawner : SpawnerBase<WeaponSpawner>
    {
        [SerializeField] private BulletTrace bulletTracePrefab;
        [SerializeField] private BfgProjectile bfgProjectile;
        private List<BulletTrace> bulletTracePool = new List<BulletTrace>();
        private List<BfgProjectile> bfgProjectilePool = new List<BfgProjectile>();

        public static BulletTrace SpawnBulletTrace()
        {
            return inst.Spawn(inst.bulletTracePool, inst.bulletTracePrefab);
        }

        public static BfgProjectile SpawnBFGProjectile(Vector3 position)
        {
            var projectile = inst.Spawn(inst.bfgProjectilePool, inst.bfgProjectile);
            projectile.transform.position = position;
            projectile.transform.rotation = Quaternion.identity;
            return projectile;
        }
    }
}