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
    public Transform playerCurrentPosition;

    [Header("Player Settings")]
    public Vector3 initialPlayerPosition;
    public float health = 5;
    public float maxHealth = 5;

    public int healingPodAmount = 3;
    public int healingAmountPerPod = 2;

    int healthShardsNeeded = 3;
    public int healthShardAmount = 0;

    public int currency;
    public Camera playerCamera;

    [Header("Player Debug Settings")]
    public bool hasInfiniteLives = false;

    [Header("Checkpoints")]
    public Vector2 lastCheckpointPos;
    public int lastCheckpointLevelIndex;
    public string lastCheckpointLevelPath;


    public Vector2 lastSavepointPos;
    public int lastSavepointLevelIndex;
    public string lastSavepointLevelPath;


    public CameraController cameraController;
    public Animator anim;

    private int levelToUnload;
    private int levelToLoad;

    private string levelToUnloadPath;
    private string levelToLoadPath;

    public int currentScene;
    public string currentScenePath;

    public bool loading = false;

    AstarPath astarPath;

    [Header("Player Abilities")]
    public bool hasDashAbility = true;
    public bool hasWallJump = false;
    public bool hasTeleportAbility = false;
    public bool hasSprintAbility = false;

    [Header("Game State")]
    public bool isRespawning = false;
    bool isLoading = false;
    public bool isPaused = false;

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

        DontDestroyOnLoad(this);
    }

    private void Start()
    {
        astarPath = FindObjectOfType<AstarPath>();
        anim = gameObject.GetComponent<Animator>();

        PlayerPrefs.DeleteAll();
    }

    public int AddHealthShard()
    {
        healthShardAmount++;
        int remainder = (healthShardAmount % healthShardsNeeded);
        if (remainder == 0)
        {
            maxHealth++;
        }
        return remainder;
    }

    public void Respawn()
    {
        Debug.Log("Hard Respawn");
        if (!isRespawning)
        {
            isRespawning = true;
            isLoading = true;
            StartCoroutine(HardRespawnRoutine());
            LoadScenePath(SceneManager.GetActiveScene().path, lastCheckpointLevelPath);
        }
    }

    public void SoftRespawn()
    {
        Debug.Log("Soft Respawn");
        StartCoroutine(SoftRespawnRoutine());
    }

    IEnumerator SoftRespawnRoutine()
    {
        anim.Play("Fade_Out");
        player.GetComponent<Player>().enabled = false;
        yield return new WaitForSecondsRealtime(1f);

        anim.Play("Fade_in");

        player.GetComponent<Player>().enabled = true;
        player.transform.position = lastCheckpointPos;
        playerCamera.transform.position = player.transform.position;
        player.GetComponentInChildren<BoomerangLauncher>().canFire = true;
    }

    IEnumerator HardRespawnRoutine()
    {
        while (isLoading)
        {
            player.GetComponent<Player>().enabled = false;
            yield return null;
        }

        player.transform.position = lastSavepointPos;
        playerCamera.transform.position = player.transform.position;
        player.GetComponentInChildren<BoomerangLauncher>().canFire = true;
        player.GetComponent<Player>().enabled = true;

        isRespawning = false;
    }

    public void LoadScene(int levelToUnload, int levelToLoad)
    {
        loading = true;
        currentScene = levelToLoad;
        player.GetComponent<Player>().enabled = false;

        this.levelToLoad = levelToLoad;
        this.levelToUnload = levelToUnload;

        levelToLoadPath = SceneManager.GetSceneByBuildIndex(levelToLoad).path;
        levelToUnloadPath = SceneManager.GetSceneByBuildIndex(levelToUnload).path;

        StartCoroutine(LoadSceneRoutine());
    }

    public void LoadScenePath(string levelToUnloadPath, string levelToLoadPath, Vector3 playerPos)
    {
        loading = true;
        currentScenePath = levelToLoadPath;

        player.GetComponent<Player>().enabled = false;
        newPlayerPos = playerPos;

        this.levelToLoadPath = levelToLoadPath;
        this.levelToUnloadPath = levelToUnloadPath;

        StartCoroutine(LoadSceneRoutine());
    }

    public void LoadScenePath(string levelToUnloadPath, string levelToLoadPath)
    {
        loading = true;
        currentScenePath = levelToLoadPath;

        player.GetComponent<Player>().enabled = false;

        this.levelToLoadPath = levelToLoadPath;
        this.levelToUnloadPath = levelToUnloadPath;

        StartCoroutine(LoadSceneRoutine());
    }

    Vector3 newPlayerPos;
    public void LoadScene(int levelToUnload, int levelToLoad, Vector3 playerPos)
    {
        loading = true;
        currentScene = levelToLoad;
        player.GetComponent<Player>().enabled = false;
        newPlayerPos = playerPos;
        this.levelToLoad = levelToLoad;
        this.levelToUnload = levelToUnload;

        levelToLoadPath = SceneManager.GetSceneByBuildIndex(levelToLoad).path;
        levelToUnloadPath = SceneManager.GetSceneByBuildIndex(levelToUnload).path;

        StartCoroutine(LoadSceneRoutine());
    }

    IEnumerator LoadSceneRoutine()
    {
        anim.Play("Fade_Out");
        yield return new WaitForSecondsRealtime(1f);
        Debug.Log("Loading Scene: " + levelToLoadPath);
        SceneManager.LoadSceneAsync(levelToLoadPath, LoadSceneMode.Additive).completed += LoadScene_completed;

    }

    IEnumerator LoadSceneRoutineIndex()
    {
        anim.Play("Fade_Out");
        yield return new WaitForSecondsRealtime(1f);
        Debug.Log("Loading Scene: " + levelToLoad);
        SceneManager.LoadSceneAsync(levelToLoad, LoadSceneMode.Additive).completed += LoadScene_completedIndex;

    }
    //public void OnFadeComplete()
    //{
    //    SceneManager.LoadSceneAsync(levelToLoad, LoadSceneMode.Additive).completed += LoadScene_completed;
    //}

    private void LoadScene_completedIndex(AsyncOperation obj)
    {
        if (obj.isDone)
        {
            Debug.Log("Unloading Scene: " + levelToUnloadPath);

            SceneManager.UnloadSceneAsync(levelToUnloadPath).completed += UnloadScene_completedIndex;

            if (!isRespawning)
            {
                player.transform.position = newPlayerPos;
            }

            loading = false;
            isLoading = false;

            player.GetComponent<Player>().enabled = true;

        }
    }

    private void UnloadScene_completedIndex(AsyncOperation obj)
    {
        if (obj.isDone)
        {
            StartCoroutine(FadeIn());
            SceneManager.SetActiveScene(SceneManager.GetSceneByPath(levelToLoadPath));

            astarPath = FindObjectOfType<AstarPath>();
            if (astarPath != null)
            {
                astarPath.Scan();
            }
        }
    }

    private void LoadScene_completed(AsyncOperation obj)
    {
        if (obj.isDone)
        {
            Debug.Log("Unloading Scene: " + levelToUnloadPath);

            SceneManager.UnloadSceneAsync(levelToUnloadPath).completed += UnloadScene_completed;

            if (!isRespawning)
            {
                player.transform.position = newPlayerPos;
            }

            loading = false;
            isLoading = false;

            player.GetComponent<Player>().enabled = true;

        }
    }

    private void UnloadScene_completed(AsyncOperation obj)
    {
        if (obj.isDone)
        {
            StartCoroutine(FadeIn());
            SceneManager.SetActiveScene(SceneManager.GetSceneByPath(levelToLoadPath));

            astarPath = FindObjectOfType<AstarPath>();
            if (astarPath != null)
            {
                astarPath.Scan();
            }
        }
    }

    IEnumerator FadeIn()
    {
        yield return new WaitForSecondsRealtime(0.2f);
        anim.Play("Fade_in");
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

    private void OnDrawGizmos()
    {
        if (lastCheckpointPos != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(lastCheckpointPos, 2);
        }
    }
}
