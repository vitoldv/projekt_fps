using UnityEngine;
using _Core.Arena;
using _Core;
using System.Linq;
using System;

public class ArenaManager : MonoBehaviour
{
    public event Action OnAllEnemiesDefeated;
    public event Action OnPlayerLeaveEntryZone;
    public event Action OnPlayerEnterExitZone;

    public string ArenaName;
    public int enemiesDefeated;

    // Current arena data
    [SerializeField] private Transform playerSpawnPoint;
    [SerializeField] private RepeatedSpawnPoint[] repeatedEnemySpawnPoints;
    [SerializeField] private OneTimeSpawnPoint[] oneTimeEnemySpawnPoints;
    [SerializeField] private EntryRoomTriggerZone entryRoomTriggerZone;
    [SerializeField] private ExitRoomTriggerZone exitRoomTriggerZone;

    public Transform PlayerInitialTransform => playerSpawnPoint;

    private bool isGameStarted;

    private void Awake()
    {
        var spawnPoints = GameObject.FindGameObjectsWithTag(Tags.SpawnPoints).Cast<EnemySpawnPoint>().ToList();
        repeatedEnemySpawnPoints = spawnPoints.OfType<RepeatedSpawnPoint>().ToArray();
        oneTimeEnemySpawnPoints = spawnPoints.OfType<OneTimeSpawnPoint>().ToArray();
        entryRoomTriggerZone.PlayerLeftTriggerZone += OnPlayerLeftEntryRoom;
        exitRoomTriggerZone.PlayerEnterTriggerZone += OnPlayerEnterExitRoom;
    }

    private void Update()
    {
        if(isGameStarted)
        {
            SpawnEnemies();
        }
    }

    private void SpawnEnemies()
    {
        // TODO: Implement logic that would spawn enemies throught the arena
    }

    private void OnPlayerLeftEntryRoom()
    {
        // TODO: Start active actions
        OnPlayerLeaveEntryZone?.Invoke();
    }

    private void OnPlayerEnterExitRoom()
    {
        // TODO: Start active actions
        OnPlayerEnterExitZone?.Invoke();
    }
}
