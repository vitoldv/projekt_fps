using _Core.Collectibles;
using _Core.Common;
using System.Collections.Generic;
using UnityEngine;

namespace _Core.Spawners
{
    public class CollectibleSpawner : SpawnerBase<CollectibleSpawner>
    {
        [SerializeField] private HealthCollectible healthCollectiblePrefab;
        [SerializeField] private AmmoCollectible ammoCollectiblePreafab;


        private List<HealthCollectible> healthCollectiblePool = new List<HealthCollectible>();
        private List<AmmoCollectible> ammoCollectiblePool = new List<AmmoCollectible>();

        public static HealthCollectible SpawnHealthCollectible(HealthCollectible.Size healthSize)
        {
            var health = inst.Spawn(inst.healthCollectiblePool, inst.healthCollectiblePrefab);
            health.Init(healthSize);
            return health;
        }

        public static AmmoCollectible SpawnAmmoCollectible(WeaponType weaponType, AmmoCollectible.Size ammoSize)
        {
            var ammoCollectible = inst.Spawn(inst.ammoCollectiblePool, inst.ammoCollectiblePreafab);
            ammoCollectible.Init(ammoSize, weaponType);
            return ammoCollectible;
        }
    }

}

