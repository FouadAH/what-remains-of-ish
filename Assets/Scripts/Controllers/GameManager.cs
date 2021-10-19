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
    public int healingAmount = 2;
    public HealingPod[] healingFlasks;

    int healthShardsNeeded = 3;
    public int healthShardAmount = 0;

    public int currency;
    public Camera playerCamera;

    [Header("Player Debug Settings")]
    public bool hasInfiniteLives = false;

    [Header("Checkpoints")]
    public Vector2 lastCheckpointPos;
    public int lastCheckpointLevelIndex;

    public Vector2 lastSavepointPos;
    public int lastSavepointLevelIndex;

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
            LoadScene(SceneManager.GetActiveScene().buildIndex, lastSavepointLevelIndex);
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

    Vector3 newPlayerPos;
    public void LoadScene(int levelToUnload, int levelToLoad, Vector3 playerPos)
    {
        loading = true;
        currentScene = levelToLoad;
        player.GetComponent<Player>().enabled = false;
        newPlayerPos = playerPos;
        this.levelToLoad = levelToLoad;
        this.levelToUnload = levelToUnload;

        StartCoroutine(LoadSceneRoutine());
    }

    IEnumerator LoadSceneRoutine()
    {
        anim.Play("Fade_Out");
        yield return new WaitForSecondsRealtime(1f);
        SceneManager.LoadSceneAsync(levelToLoadPath, LoadSceneMode.Additive).completed += LoadScene_completed;
        //SceneManager.LoadSceneAsync(levelToLoad, LoadSceneMode.Additive).completed += LoadScene_completed;

    }
    //public void OnFadeComplete()
    //{
    //    SceneManager.LoadSceneAsync(levelToLoad, LoadSceneMode.Additive).completed += LoadScene_completed;
    //}

    private void LoadScene_completed(AsyncOperation obj)
    {
        if (obj.isDone)
        {
            //SceneManager.UnloadSceneAsync(levelToUnload).completed += UnloadScene_completed;
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
            //SceneManager.SetActiveScene(SceneManager.GetSceneByBuildIndex(levelToLoad));
            SceneManager.SetActiveScene(SceneManager.GetSceneByPath(levelToLoadPath));

            astarPath = FindObjectOfType<AstarPath>();
            astarPath.Scan();
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
