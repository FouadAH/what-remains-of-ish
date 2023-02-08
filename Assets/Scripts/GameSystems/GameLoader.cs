using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Audio;

public class GameLoader : MonoBehaviour
{
    [Header("Events")]
    public GameEvent loadInitialScene;
    public GameEvent loadingStartEvent;
    public GameEvent loadingEndEvent;
    public GameEvent spawnPlayerEvent;

    [Header("Data")]
    public PlayerDataSO playerData;

    int currentSceneIndex;

    private void Awake()
    {
        currentSceneIndex = gameObject.scene.buildIndex;
    }
    public void StartGame()
    {
        StartCoroutine(NewGame());
    }

    public void LoadEvent()
    {
        StartCoroutine(Loading());
    }

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Quit");
    }

    private IEnumerator NewGame()
    {
        loadingStartEvent.Raise();
        SceneManager.UnloadSceneAsync(currentSceneIndex).completed += CurrentSceneUnload;
        yield return null;
    }

    private void CurrentSceneUnload(AsyncOperation obj)
    {
        if (obj.isDone)
        {
            SceneManager.LoadSceneAsync("PlayerScene", LoadSceneMode.Additive).completed += LoadBaseSceneComplete;
        }
    }

    private void LoadBaseSceneComplete(AsyncOperation obj)
    {
        if (obj.isDone)
        {
            //GameManager.instance.InitialSpawn();
            spawnPlayerEvent.Raise();
            SceneManager.LoadSceneAsync(playerData.lastSavepointLevelIndex.Value, LoadSceneMode.Additive).completed += LoadLevelComplete;
        }
    }

    private void LoadLevelComplete(AsyncOperation obj)
    {
        if (obj.isDone)
        {
            SceneManager.SetActiveScene(SceneManager.GetSceneByBuildIndex(playerData.lastSavepointLevelIndex.Value));
            loadInitialScene.Raise();
            loadingEndEvent.Raise();
        }
    }

    private IEnumerator Loading()
    {
        loadingStartEvent.Raise();
        SceneManager.UnloadSceneAsync(currentSceneIndex).completed += CurrentSceneUnload;

        yield return null;
    }
}
