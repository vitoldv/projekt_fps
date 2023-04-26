using UnityEngine;
using _Core;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System.Collections;
using System;
using System.Linq;

public class ArenaManager : Singleton<ArenaManager>
{
    public static event Action<string> ArenaLoaded;
    public static event Action ArenaInitialized;

    [SerializeField] private List<string> arenaNames;

    public string currentArenaName;

    // Current arena data
    private Transform playerSpawnPoint;
    private List<Transform> enemySpawnPoints;
    private bool isGameStarted;
    

    protected override void Initialize()
    {
        
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

    public static void LoadArena(int arenaId)
    {
        inst.StartCoroutine(inst.LoadSceneAsync(inst.arenaNames[arenaId]));
    }

    public static void InitializeCurrentArena(PlayerController player)
    {
        inst.playerSpawnPoint = GameObject.FindGameObjectWithTag("PlayerSpawnPoint").transform;
        inst.enemySpawnPoints = GameObject.FindGameObjectsWithTag("EnemySpawnPoint").Select(o => o.transform).ToList();
        player.transform.position = inst.playerSpawnPoint.position;
    }

    public static void UnloadArena()
    {
        inst.StartCoroutine(inst.UnloadSceneAsync(inst.currentArenaName));
    }

    private IEnumerator UnloadSceneAsync(string sceneName)
    {
        AsyncOperation asyncUnload = SceneManager.UnloadSceneAsync(sceneName);

        while (!asyncUnload.isDone)
        {
            yield return null;
        }
    }

    private IEnumerator LoadSceneAsync(string sceneName)
    {      
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);

        while (!asyncLoad.isDone)
        {
            // Update progress bar or loading text
            // You can also use asyncLoad.progress to get the loading progress

            yield return null;
        }

        currentArenaName = sceneName;
        ArenaLoaded?.Invoke(sceneName);
    }


}
