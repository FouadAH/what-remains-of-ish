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
    bool isNewGame;

    public static GameLoader Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        currentSceneIndex = gameObject.scene.buildIndex;
        DontDestroyOnLoad(this);
    }

    public void OnStartLoading()
    {
        loadingStartEvent.Raise();
    }

    public void OnLoadNewGame()
    {
        isNewGame = true;
        loadingStartEvent.Raise();
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void StartLoadingScene()
    {
        SceneManager.UnloadSceneAsync(currentSceneIndex).completed += CurrentSceneUnload;
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
            SceneManager.LoadSceneAsync(playerData.lastSavepointLevelIndex.Value, LoadSceneMode.Additive).completed += LoadLevelComplete;
        }
    }

    private void LoadLevelComplete(AsyncOperation obj)
    {
        if (obj.isDone)
        {
            SceneManager.SetActiveScene(SceneManager.GetSceneByBuildIndex(playerData.lastSavepointLevelIndex.Value));

            spawnPlayerEvent.Raise();
            loadInitialScene.Raise();
            loadingEndEvent.Raise();
        }
    }
}
