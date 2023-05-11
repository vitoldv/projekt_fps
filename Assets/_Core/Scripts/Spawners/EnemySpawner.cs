using System.Collections.Generic;
using UnityEngine;
using _Core.Enemies;

namespace _Core.Spawners
{
    public class EnemySpawner : SpawnerBase<EnemySpawner>
    {
        [SerializeField] private WalkingEnemy walkingEnemyPrefab;
        [SerializeField] private FlyingEnemy flyingEnemyPrefab;
        [SerializeField] private InsectProjectile insectProjectilePrefab;

        private List<WalkingEnemy> walkingEnemyTool = new List<WalkingEnemy>();
        private List<FlyingEnemy> flyingEnemyPool = new List<FlyingEnemy>();
        private List<InsectProjectile> insectProjectiles = new List<InsectProjectile>();

        public static T SpawnEnemy<T>() where T : EnemyBase
        {
            T enemy = null;
            if (typeof(T) == typeof(WalkingEnemy))
            {
                enemy = inst.Spawn(inst.walkingEnemyTool, inst.walkingEnemyPrefab) as T;
            }
            if (typeof(T) == typeof(FlyingEnemy))
            {
                enemy = inst.Spawn(inst.flyingEnemyPool, inst.flyingEnemyPrefab) as T;
            }
            return enemy;
        }

        public static InsectProjectile SpawnInsectProjectile(Vector3 position)
        {
            var projectile = inst.Spawn(inst.insectProjectiles, inst.insectProjectilePrefab);
            projectile.transform.position = position;
            projectile.transform.rotation = Quaternion.identity;
            return projectile;
        }
    }
}