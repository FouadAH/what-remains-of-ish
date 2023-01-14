using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Audio;

public class GameLoader : MonoBehaviour
{
    public GameEvent loadInitialScene;
    int currentSceneIndex;

    private void Awake()
    {
        currentSceneIndex = gameObject.scene.buildIndex;
    }
    public void StartGame()
    {
        GameManager.instance.GetComponent<Animator>().Play("Fade_Out");
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
        GameManager.instance.isLoading = true;
        GameManager.instance.anim.Play("Fade_Out");
        SceneManager.UnloadSceneAsync(currentSceneIndex).completed += CurrentSceneUnload;
        yield return null;
    }

    private void LoadBaseSceneComplete(AsyncOperation obj)
    {
        if (obj.isDone)
        {
            GameManager.instance.InitialSpawn();
            SceneManager.LoadSceneAsync(GameManager.instance.playerData.lastSavepointLevelIndex.Value, LoadSceneMode.Additive).completed += LoadLevelComplete;
        }
    }

    private void LoadLevelComplete(AsyncOperation obj)
    {
        if (obj.isDone)
        {
            SceneManager.SetActiveScene(SceneManager.GetSceneByBuildIndex(GameManager.instance.playerData.lastSavepointLevelIndex.Value));
            loadInitialScene.Raise();
            GameManager.instance.isLoading = false;
            GameManager.instance.anim.Play("Fade_in");
        }
    }

    private void CurrentSceneUnload(AsyncOperation obj)
    {
        if (obj.isDone)
        {
            SceneManager.LoadSceneAsync("PlayerScene", LoadSceneMode.Additive).completed += LoadBaseSceneComplete;
        }
    }


    private IEnumerator Loading()
    {
        GameManager.instance.isLoading = true;
        GameManager.instance.anim.Play("Fade_Out");
        SceneManager.UnloadSceneAsync(currentSceneIndex).completed += CurrentSceneUnload;

        yield return null;
    }
}
