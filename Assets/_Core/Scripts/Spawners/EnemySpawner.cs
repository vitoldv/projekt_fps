using _Core;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : SpawnerBase<EnemySpawner>
{
    public Transform target;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private EnemyBase enemyPrefab;
    private List<EnemyBase> _enemyPool = new List<EnemyBase>();

    public static EnemyBase SpawnEnemy(Transform transform = null)
    {
        EnemyBase knife = _instance.Spawn(_instance._enemyPool, _instance.enemyPrefab);
        return knife;
    }

    public static void DisableAllEnemies()
    {
        DisableAllObjectsInPool(_instance._enemyPool);
    }

    [ContextMenu("Spawn enemy")]
    public void SpawnEnemyAtRandomPoint()
    {
        var enemy = SpawnEnemy();
        enemy.Initialize(target);
        enemy.transform.position = spawnPoint.position;
    }
}
