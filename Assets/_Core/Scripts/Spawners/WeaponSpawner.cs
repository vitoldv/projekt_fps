using _Core;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSpawner : SpawnerBase<WeaponSpawner>
{
    [SerializeField] private BulletTrace bulletTracePrefab;
    [SerializeField] private ProjectileBase bfgProjectile;
    private List<BulletTrace> bulletTracePool = new List<BulletTrace>();
    private List<ProjectileBase> bfgProjectilePool = new List<ProjectileBase>();

    public static BulletTrace SpawnBulletTrace()
    {
        return inst.Spawn(inst.bulletTracePool, inst.bulletTracePrefab);
    }

    public static ProjectileBase SpawnBFGProjectile(Vector3 position)
    {
        var projectile = inst.Spawn(inst.bfgProjectilePool, inst.bfgProjectile);
        projectile.transform.position = position;
        projectile.transform.rotation = Quaternion.identity;
        return projectile;
    }
}
