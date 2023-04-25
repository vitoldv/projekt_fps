using _Core;
using System.Collections.Generic;
using UnityEngine;

public class CollectibleSpawner : SpawnerBase<CollectibleSpawner>
{
    [SerializeField] private HealthCollectible healthCollectiblePrefab;
    [SerializeField] private AmmoCollectible ammoCollectiblePreafab;

    private List<HealthCollectible> healthCollectiblePool = new List<HealthCollectible>();
    private List<AmmoCollectible> ammoCollectiblePool = new List<AmmoCollectible>();

    public static HealthCollectible SpawnHealthCollectible()
    {
        return _instance.Spawn(_instance.healthCollectiblePool, _instance.healthCollectiblePrefab);
    }

    public static AmmoCollectible SpawnAmmoCollectible(int ammoAmount, WeaponType weaponType)
    {
        var ammoCollectible = _instance.Spawn(_instance.ammoCollectiblePool, _instance.ammoCollectiblePreafab);
        ammoCollectible.ammoAmount = ammoAmount;
        ammoCollectible.weaponType = weaponType;
        return ammoCollectible;
    }

    public WeaponType a;
    public int ammo;
    public Transform p;

    [ContextMenu("Piska")]
    public void P()
    {
        var c = SpawnAmmoCollectible(ammo, a);
        c.transform.position = p.position;
    }  
    
    [ContextMenu("Huiska")]
    public void S()
    {
        var c = SpawnHealthCollectible();
        c.transform.position = p.position;
    }
}
