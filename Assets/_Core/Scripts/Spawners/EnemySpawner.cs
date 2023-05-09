using System.Collections.Generic;
using UnityEngine;
using _Core.Enemies;

namespace _Core.Spawners
{
    public class EnemySpawner : SpawnerBase<EnemySpawner>
    {
        [SerializeField] private WalkingEnemy enemyPrefab;
        [SerializeField] private FlyingEnemy flyingEnemyPrefab;
        private List<WalkingEnemy> walkingEnemy = new List<WalkingEnemy>();
        private List<FlyingEnemy> flyingEnemyPool = new List<FlyingEnemy>();

        public static T SpawnEnemy<T>() where T : EnemyBase
        {
            T enemy = null;
            if (typeof(T) == typeof(WalkingEnemy))
            {
                enemy = inst.Spawn(inst.walkingEnemy, inst.enemyPrefab) as T;
            }
            if (typeof(T) == typeof(FlyingEnemy))
            {
                enemy = inst.Spawn(inst.flyingEnemyPool, inst.flyingEnemyPrefab) as T;
            }
            return enemy;
        }
    }
}