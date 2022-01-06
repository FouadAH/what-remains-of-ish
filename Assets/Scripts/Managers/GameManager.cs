using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

/// <summary>
/// Class responsible for ascynchronis scene loading and unloading, as well as holding, saving and loading player data 
/// </summary>
public class GameManager : MonoBehaviour
{
    public static GameManager instance { get; private set; }
    [HideInInspector] public GameObject player;
    [HideInInspector] public Transform playerCurrentPosition;

    [Header("Scene Loading Settings")]
    public Vector3 playerStartPosition;
    public Vector3 initialPlayerPosition;
    public string initialLevelPath;

    public Level currentLevel;
    public int currentSceneBuildIndex;
    public string currentScenePath;

    private string levelToUnloadPath;
    private string levelToLoadPath;

     public Vector2 lastCheckpointPos;
    [HideInInspector] public int lastCheckpointLevelIndex;
    [HideInInspector] public string lastCheckpointLevelPath;

    [HideInInspector] public Vector2 lastSavepointPos;
    [HideInInspector] public int lastSavepointLevelIndex;
    [HideInInspector] public string lastSavepointLevelPath;

    [Header("Player Settings")]
    public float health = 5;
    public float maxHealth = 5;

    public int healingPodAmount = 2;
    public int healingAmountPerPod = 2;

    int healthShardsNeeded = 3;
    public int healthShardAmount = 0;

    public int currency;
    public Camera playerCamera;

    [Header("Player Debug Settings")]
    public bool hasInfiniteLives = false;

    [HideInInspector] public CameraController cameraController;
    [HideInInspector] public Animator anim;
    AstarPath astarPath;

    [Header("Player Abilities")]
    public bool hasDashAbility = true;
    public bool hasWallJump = false;
    public bool hasTeleportAbility = false;
    public bool hasSprintAbility = false;

    [Header("Game State")]
    public bool isLoading = false;
    public bool isRespawning = false;
    public bool isPaused = false;

    Vector3 newPlayerPos;

    public GameEvent loadNewSceneEvent;
    public GameEvent loadDataEvent;
    public GameEvent saveDataEvent;


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

        lastSavepointLevelPath = initialLevelPath;
        lastCheckpointLevelPath = initialLevelPath;
        levelToLoadPath = initialLevelPath;

        lastCheckpointPos = initialPlayerPosition;
        lastSavepointPos = initialPlayerPosition;
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
            player.GetComponent<Player>().enabled = false;
            LoadScenePath(SceneManager.GetActiveScene().path, lastSavepointLevelPath);
        }
    }

    public void SoftRespawn()
    {
        Debug.Log("Soft Respawn");
        StartCoroutine(SoftRespawnRoutine());
    }

    private IEnumerator SoftRespawnRoutine()
    {
        anim.Play("Fade_Out");
        player.GetComponent<Player>().enabled = false;

        yield return new WaitForSecondsRealtime(1f);

        playerCamera.transform.position = player.transform.position;
        player.transform.position = lastCheckpointPos;

        yield return new WaitForSecondsRealtime(1f);

        anim.Play("Fade_in");

        player.GetComponent<Player>().enabled = true;
        player.GetComponentInChildren<BoomerangLauncher>().canFire = true;
    }

    public void LoadScenePath(string levelToUnloadPath, string levelToLoadPath, Vector3 playerPos)
    {
        isLoading = true;
        currentScenePath = levelToLoadPath;

        player.GetComponent<Player>().enabled = false;
        newPlayerPos = playerPos;

        this.levelToLoadPath = levelToLoadPath;
        this.levelToUnloadPath = levelToUnloadPath;

        StartCoroutine(LoadSceneRoutine());
    }

    public void LoadScenePath(string levelToUnloadPath, string levelToLoadPath)
    {
        isLoading = true;
        currentScenePath = levelToLoadPath;

        player.GetComponent<Player>().enabled = false;

        this.levelToLoadPath = levelToLoadPath;
        this.levelToUnloadPath = levelToUnloadPath;

        StartCoroutine(LoadSceneRoutine());
    }

    private IEnumerator LoadSceneRoutine()
    {
        SaveManager.instance.SaveSceneData();

        anim.Play("Fade_Out");
        yield return new WaitForSecondsRealtime(1f);
        SceneManager.LoadSceneAsync(levelToLoadPath, LoadSceneMode.Additive).completed += LoadScene_completed;
    }

    private void LoadScene_completed(AsyncOperation obj)
    {
        if (obj.isDone)
        {
            SceneManager.UnloadSceneAsync(levelToUnloadPath).completed += UnloadScene_completed;

            if (!isRespawning)
            {
                player.transform.position = newPlayerPos;
            }
            else
            {
                player.transform.position = lastSavepointPos;
                playerCamera.transform.position = player.transform.position;
                player.GetComponentInChildren<BoomerangLauncher>().canFire = true;
            }
            

            isLoading = false;
            isRespawning = false;

            StartCoroutine(FadeIn());

            player.GetComponent<Player>().enabled = true;
        }
    }

    private void UnloadScene_completed(AsyncOperation obj)
    {
        if (obj.isDone)
        {
            SceneManager.SetActiveScene(SceneManager.GetSceneByPath(levelToLoadPath));
            currentSceneBuildIndex = SceneManager.GetActiveScene().buildIndex;

            SaveManager.instance.SavePlayerData();

            loadNewSceneEvent.Raise();
            loadDataEvent.Raise();
                
            astarPath = FindObjectOfType<AstarPath>();
            if (astarPath != null)
            {
                astarPath.Scan();
            }
        }
    }

    private IEnumerator FadeIn()
    {
        yield return new WaitForSecondsRealtime(0.2f);
        anim.Play("Fade_in");
    }

    private void OnDrawGizmos()
    {
        if (lastCheckpointPos != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(lastCheckpointPos, 2);
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(initialPlayerPosition, 2);
        }
    }
}
