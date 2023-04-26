using UnityEngine;
using _Core;
using System;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.SceneManagement;

public class ArenaLoader : Singleton<ArenaLoader>
{
    public static event Action<string> ArenaLoaded;
    
    public string currentArenaName;
    [SerializeField] private List<string> arenaNames;

    public static void LoadArena(int arenaId)
    {
        inst.StartCoroutine(inst.LoadSceneAsync(inst.arenaNames[arenaId]));
    }

    public static void UnloadArena()
    {
        inst.StartCoroutine(inst.UnloadSceneAsync(inst.currentArenaName));
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

    private IEnumerator UnloadSceneAsync(string sceneName)
    {
        AsyncOperation asyncUnload = SceneManager.UnloadSceneAsync(sceneName);

        while (!asyncUnload.isDone)
        {
            yield return null;
        }
    }
}
