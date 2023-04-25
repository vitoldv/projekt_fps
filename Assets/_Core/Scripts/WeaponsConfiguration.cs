using _Core;
using Assets._Core.Scripts.Player.ShootingParameters;
using UnityEngine;

[CreateAssetMenu(fileName = "NewWeaponConfig", menuName = "Weapon/Weapon configuration", order = 1)]
public class WeaponsConfiguration : ScriptableObject
{
    [Header("Pistol")]
    public float pistolRoundsPerMinute;
    public float pistolReloadTime;
    public int pistolCapacity;
    public int pistolDamage;
    public BulletTrace pistolBulletTracePrefab;
    [Header("Rifle")]
    public float rifleRoundsPerMinute;
    public float rifleReloadTime;
    public int rifleCapacity;
    public int rifleDamage;
    public BulletTrace rifleBulletTracePrefab;
    [Header("Shotgun")]
    public float shotgunRoundsPerMinute;
    public float shotgunReloadTime;
    public int shotgunCapacity;
    public int shotgunDamage;
    public float shotgunScatterConeAngle;
    [Header("BFG")]
    public float bfgRoundsPerMinute;
    public float bfgReloadTime;
    public int bfgCapacity;
    public float bfgDamage;
    public float projectileSpeed;    
    public ProjectileBase bfgProjectilePrefab;
    public float explosionRadius;
    [Header("Railgun")]
    public float railgunRoundsPerMinute;
    public float railgunReloadTime;
    public int railgunCapacity;
    public int railgunDamage;
    public BulletTrace railgunBulletTracePrefab;

    public ShootingHandlerArgs GetShootingHandlerArgsForWeapon(WeaponType weapon)
    {
        switch (weapon)
        {
            case WeaponType.Pistol:
                return new HitScanShootingHandlerArgs
                {
                    roundsPerMinute = pistolRoundsPerMinute,
                    reloadTime = pistolReloadTime,
                    gunShopCapacity = pistolCapacity,
                    bulletTracePrefab = pistolBulletTracePrefab,
                    damage = pistolDamage
                };

            case WeaponType.Rifle:
                return new HitScanShootingHandlerArgs
                {
                    roundsPerMinute = rifleRoundsPerMinute,
                    reloadTime = rifleReloadTime,
                    gunShopCapacity = rifleCapacity,
                    bulletTracePrefab = rifleBulletTracePrefab,
                    damage = rifleDamage
                };

            case WeaponType.Shotgun:
                return new FractionShootingHandlerArgs
                {
                    roundsPerMinute = pistolRoundsPerMinute,
                    reloadTime = pistolReloadTime,
                    gunShopCapacity = pistolCapacity,
                    scatterConeAngle = shotgunScatterConeAngle,
                    damage = shotgunDamage,
                    bulletTracePrefab = rifleBulletTracePrefab
                };

            case WeaponType.BFG:
                return new ProjectileShootingHandlerArgs
                {
                    roundsPerMinute = bfgRoundsPerMinute,
                    reloadTime = bfgReloadTime,
                    gunShopCapacity = bfgCapacity,
                    projectilePrefab = bfgProjectilePrefab,
                    projectileSpeed = projectileSpeed,
                    damage = bfgDamage,
                    explosionRadius = explosionRadius
                };

            case WeaponType.Railgun:
                return new HitScanShootingHandlerArgs
                {
                    roundsPerMinute = railgunRoundsPerMinute,
                    reloadTime = railgunReloadTime,
                    gunShopCapacity = railgunCapacity,
                    damage = railgunDamage,
                    bulletTracePrefab = railgunBulletTracePrefab
                };

            default:  return null;
        }
    }
}
