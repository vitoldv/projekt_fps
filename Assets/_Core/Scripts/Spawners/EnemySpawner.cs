using _Core;
using Assets._Core.Scripts.Enemies;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : SpawnerBase<EnemySpawner>
{
    public Transform target;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private WalkingEnemy enemyPrefab;
    [SerializeField] private FlyingEnemy flyingEnemyPrefab;
    private List<WalkingEnemy> enemyPool = new List<WalkingEnemy>();
    private List<FlyingEnemy> flyingEnemyPool = new List<FlyingEnemy>();

    public static T SpawnEnemy<T>(Transform transform = null) where T: EnemyBase
    {
        T enemy = null;
        if(typeof(T) == typeof(WalkingEnemy))
        {
            enemy = inst.Spawn(inst.enemyPool, inst.enemyPrefab) as T;
        }
        if(typeof(T) == typeof(FlyingEnemy))
        {
            enemy = inst.Spawn(inst.flyingEnemyPool, inst.flyingEnemyPrefab) as T;
        }        
        return enemy;
    }

    [ContextMenu("Spawn enemy")]
    public void SpawnEnemyAtRandomPoint()
    {
        var enemy = SpawnEnemy<WalkingEnemy>();
        enemy.Init(target);
        enemy.transform.position = spawnPoint.position;
    }
}
