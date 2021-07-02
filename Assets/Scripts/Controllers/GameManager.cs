using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Class responsible for ascynchronis scene loading and unloading, as well as holding, saving and loading player data 
/// </summary>
public class GameManager : MonoBehaviour
{
    public static GameManager instance { get; private set; }
    public GameObject player;
    [HideInInspector] public GameObject boomerangLauncher;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            Application.targetFrameRate = 60;
        }
        else
        {
            Destroy(gameObject);
        }

        boomerangLauncher = player.GetComponentInChildren<BoomerangLauncher>().gameObject;
    }

    private void Start()
    {
        //boomerangLauncher = player.GetComponentInChildren<BoomerangLauncher>().gameObject;
    }

    public float health = 10000;
    public float maxHealth = 10000;

    public int currency;
    public Camera playerCamera;

    public Vector2 lastCheckpointPos;
    public int lastCheckpointLevelIndex;

    public CameraController cameraController;
    public Animator anim;

    private int levelToUnload;
    private int levelToLoad;
    public int currentScene;
    public bool loading = false;

    public Vector3 playerPosition;
    public Vector3 dronePosition;

    private void OnDrawGizmos()
    {
        if(lastCheckpointPos != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(lastCheckpointPos, 2);
        }
    }
    
    public void LoadScene(int levelToUnload, int levelToLoad)
    {
        anim.SetTrigger("FadeOut");
        loading = true;
        currentScene = levelToLoad;
        player.GetComponent<Player>().enabled = false;

        this.levelToLoad = levelToLoad;
        this.levelToUnload = levelToUnload;
    }

    public void OnFadeComplete()
    {
        SceneManager.LoadSceneAsync(levelToLoad, LoadSceneMode.Additive).completed += LoadScene_completed;
    }

    private void LoadScene_completed(AsyncOperation obj)
    {
        if (obj.isDone)
        {
            loading = false;
            player.GetComponent<Player>().enabled = true;
            SceneManager.UnloadSceneAsync(levelToUnload).completed += UnloadScene_completed;
        }
    }

    private void UnloadScene_completed(AsyncOperation obj)
    {
        if (obj.isDone)
        {
            StartCoroutine(FadeIn());
            SceneManager.SetActiveScene(SceneManager.GetSceneByBuildIndex(levelToLoad));
            AstarPath.active.Scan();
        }
    }

    IEnumerator FadeIn()
    {
        yield return new WaitForSecondsRealtime(0.2f);
        anim.SetTrigger("FadeIn");
    }

    /////////////////////////////////////////////////////////////////
    ///                 SAVING AND LOADING                       ////
    /////////////////////////////////////////////////////////////////                 

    public void SaveGame()
    {
        GameDataController.SaveGame();
    }

    public void LoadGame()
    {
        PlayerData data = GameDataController.LoadData();

        health = data.health;
        maxHealth = data.maxHealth;
        currency = data.currency;

        Vector3 playerPosition;
        playerPosition.x = data.playerPosition[0];
        playerPosition.y = data.playerPosition[1];
        playerPosition.z = data.playerPosition[2];

        Vector2 checkpointPosition;
        checkpointPosition.x = data.lastCheckpointPos[0];
        checkpointPosition.y = data.lastCheckpointPos[1];
        lastCheckpointPos = checkpointPosition;
        
        lastCheckpointLevelIndex = data.lastCheckpointLevelIndex;

        currentScene = data.currentScene;
    }
}
