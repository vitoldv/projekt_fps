using UnityEngine;
using System.Linq;
using System;
using _Core.Common;
using System.Collections;
using System.Collections.Generic;
using _Core.Spawners;
using _Core.Enemies;
using _Core.Collectibles;
using Random = UnityEngine.Random;
using _Core.Player;

namespace _Core.Arena
{
    public class ArenaManager : MonoBehaviour
    {
        public event Action OnAllEnemiesDefeated;
        public event Action OnActivePhaseStarted;
        public event Action OnActivePhaseEnded;
        public event Action OnArenaFinished;

        private int currentWave;
        private ArenaWaveDescription currentArenaDesctiption;

        [SerializeField] private float waveCooldown;

        // Current arena data
        [SerializeField] private List<ArenaWaveDescription> wavesDescription;
        [SerializeField] private Transform playerSpawnPoint;
        [SerializeField] private EntryRoomTriggerZone entryRoomTriggerZone;
        [SerializeField] private ExitRoomTriggerZone exitRoomTriggerZone;
        [SerializeField] private ArenaDoor entryDoor;
        [SerializeField] private ArenaDoor exitDoor;
        [Header("Collectibles Params")]
        [SerializeField] private float bigHealthPackDropChance;
        [SerializeField] private float midHealthPackDropChance;
        [SerializeField] private float smallHealthPackDropChance;
        [SerializeField] private float healthDropCoefficient;
        [SerializeField] private float ammoDropCoefficient;

        [SerializeField] private UpgradesMenuUIController arenaUpgradesUI;

        public UpgradesMenuUIController UpgradesUI => arenaUpgradesUI;

        private int currentWaveTotalEnemiesCount => currentArenaDesctiption.walkingEnemiesCount + currentArenaDesctiption.flyeingEnemiesCount + currentArenaDesctiption.tankEnemiesCount;
        private int currentWaveDefeatedEnemiesCount => waveEnemiesDefeated[0] + waveEnemiesDefeated[1] + waveEnemiesDefeated[2];

        public Transform PlayerInitialTransform => playerSpawnPoint;

        private bool isActivePhase;
        private int[] waveEnemiesDefeated = new int[3];

        private void Awake()
        {
            var spawnPoints = GameObject.FindGameObjectsWithTag(Tags.EnemySpawnPoint).Select(go => go.GetComponent<EnemySpawnPoint>());
            entryRoomTriggerZone.PlayerLeftTriggerZone += OnPlayerLeftEntryRoom;
            exitRoomTriggerZone.PlayerEnterTriggerZone += OnPlayerEnterExitRoom;
        }

        private void Update()
        {
            if (isActivePhase)
            {

            }
        }

        private void StartWave(int waveNum)
        {
            // walking enemies spawn
            var enemiesToSpawn = wavesDescription[waveNum].walkingEnemiesCount;
            var countOfSpawnPoints = wavesDescription[waveNum].walkingEnemiesSpawnPoints.Count;
            if (enemiesToSpawn % countOfSpawnPoints == 0)
            {
                var enemiesForPoint = enemiesToSpawn / countOfSpawnPoints;
                foreach (var spawnPoint in wavesDescription[waveNum].walkingEnemiesSpawnPoints)
                {
                    for (int i = 0; i < enemiesForPoint; i++)
                    {
                        var enemy = EnemySpawner.SpawnEnemy<WalkingEnemy>();
                        enemy.Init(GameManager.PlayerController.transform);
                        enemy.Defeated += OnEnemyDefeated;
                        spawnPoint.PlaceEnemy(enemy);
                    }
                }
            }
            for (int i = 0; i < waveEnemiesDefeated.Length; i++)
            {
                waveEnemiesDefeated[i] = 0;
            }
        }

        private bool IsWaveEnded()
        {
            return waveEnemiesDefeated[0] == wavesDescription[currentWave].walkingEnemiesCount
                && waveEnemiesDefeated[1] == wavesDescription[currentWave].flyeingEnemiesCount
                && waveEnemiesDefeated[2] == wavesDescription[currentWave].tankEnemiesCount;
        }

        private IEnumerator C_StartNewWaveInSeconds(int waveNum, float seconds)
        {
            yield return new WaitForSeconds(seconds);
            StartWave(waveNum);
        }

        private void StartAcrivePhase()
        {
            entryDoor?.Close();
            isActivePhase = true;
            OnActivePhaseStarted?.Invoke();
            currentWave = 1;
            StartWave(currentWave);
        }

        private void FinishActivePhase()
        {
            exitDoor?.Open();
            isActivePhase = true;
            OnActivePhaseEnded?.Invoke();
        }

        private void OnEnemyDefeated(EnemyBase enemy)
        {
            if (enemy is WalkingEnemy)
            {
                waveEnemiesDefeated[0]++;
            }
            if (enemy is FlyingEnemy)
            {
                waveEnemiesDefeated[1]++;
            }
            if (enemy is TankEnemy)
            {
                waveEnemiesDefeated[2]++;
            }

            SpawnCollectiblesOnEnemyIfRequired(enemy);

            if (IsWaveEnded())
            {
                currentWave++;
                if(currentWave == wavesDescription.Count)
                {
                    FinishActivePhase();
                }
                else
                {
                    StartCoroutine(C_StartNewWaveInSeconds(currentWave, waveCooldown));
                }
            }

        }

        private void SpawnCollectiblesOnEnemyIfRequired(EnemyBase enemy)
        {
            if (ShouldDropHealthCollectible(GameManager.PlayerController.MaxHP, GameManager.PlayerController.CurrentHP, out var healthCollSize))
            {
                var healthCollectible = CollectibleSpawner.SpawnHealthCollectible(healthCollSize);
                healthCollectible.transform.position = enemy.transform.position;
            }
            if (ShouldDropAmmoCollectible(GameManager.PlayerController.GetWeaponsAmmoInfo(), out WeaponType weapon, out AmmoCollectible.Size ammoSize))
            {
                var ammoCollectible = CollectibleSpawner.SpawnAmmoCollectible(weapon, ammoSize);
                ammoCollectible.transform.position = enemy.transform.position;
            }
        }

        private void OnPlayerLeftEntryRoom()
        {
            if(!isActivePhase)
            {
                StartAcrivePhase();
            }            
        }

        private void OnPlayerEnterExitRoom()
        {
            if(isActivePhase)
            {
                FinishArena();
            }
        }

        private void FinishArena()
        {
            print("ARENA FINISHED");
        }

        public bool ShouldDropHealthCollectible(float maxPlayerHealth, float currentPlayerHealth, out HealthCollectible.Size healthSize)
        {
            healthSize = 0;
            float healthPercentage = (float)currentPlayerHealth / maxPlayerHealth;
            float healthPackDropChance = (1.0f - healthPercentage) * healthDropCoefficient;
            if(Random.Range(0f, 1f) <= healthPackDropChance)
            {
                var healthPackSizeChance = Random.Range(0f, 1f);
                if(healthPackSizeChance < smallHealthPackDropChance)
                {
                    healthSize = HealthCollectible.Size.SMALL;
                }
                else if (healthPackSizeChance < smallHealthPackDropChance + midHealthPackDropChance)
                {
                    healthSize = HealthCollectible.Size.MID;
                }
                else if (healthPackSizeChance < smallHealthPackDropChance + midHealthPackDropChance + bigHealthPackDropChance)
                {
                    healthSize = HealthCollectible.Size.BIG;
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        //[SerializeField] private float maxPlayerHealth;
        //[SerializeField] private float currentPlayerHealth;
        //[SerializeField] private int enemiesDefeated;
        //[SerializeField] private int totalEnemies;

        //[ContextMenu("Test")]
        //public void Test()
        //{
        //    print("TRY");
        //    if(ShouldDropHealthBonus(maxPlayerHealth, currentPlayerHealth, enemiesDefeated, totalEnemies, out var res))
        //    {
        //        print($"Drop {res.ToString()}");
        //    }
        //}

        public bool ShouldDropAmmoCollectible(Dictionary<WeaponType, ShootingHandlerState> weaponsState, out WeaponType weapon, out AmmoCollectible.Size ammoSize)
        {
            ammoSize = 0;
            float weaponChance = Random.Range(0f, 1f);
            if(weaponChance <= 0.2f)
            {
                weapon = WeaponType.Railgun;
            }
            else if (weaponChance <= 0.4f)
            {
                weapon = WeaponType.BFG;
            }
            else if (weaponChance <= 0.6f)
            {
                weapon = WeaponType.Shotgun;
            }
            else if (weaponChance <= 0.8f)
            {
                weapon = WeaponType.Rifle;
            }
            else
            {
                weapon = WeaponType.Pistol;
            }
            var weaponState = weaponsState[weapon];
            
            var ammoPercentage = weaponState.CurrentAmmoAmountTotal / weaponState.GunShopCapacity * 3;
            var ammoPackDropChance = (1.0f - ammoPercentage) * ammoDropCoefficient;

            if (Random.Range(0f, 1f) <= ammoPackDropChance)
            {
                var ammoPackSizeChance = Random.Range(0f, 1f);
                if (ammoPackSizeChance < smallHealthPackDropChance)
                {
                    ammoSize = AmmoCollectible.Size.SMALL;
                }
                else if (ammoPackSizeChance < smallHealthPackDropChance + midHealthPackDropChance)
                {
                    ammoSize = AmmoCollectible.Size.MID;
                }
                else if (ammoPackSizeChance < smallHealthPackDropChance + midHealthPackDropChance + bigHealthPackDropChance)
                {
                    ammoSize = AmmoCollectible.Size.BIG;
                }
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}