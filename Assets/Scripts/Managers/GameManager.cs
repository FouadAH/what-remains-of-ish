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

    [Header("Scene Settings")]
    public PlayerConfig initalPlayerData;

    [HideInInspector] public Vector3 playerStartPosition;

    public Level currentLevel;
    public int currentSceneBuildIndex;
    public string currentScenePath;

    private string levelToUnloadPath;
    private string levelToLoadPath;

    [HideInInspector] public Vector2 lastCheckpointPos;
    [HideInInspector] public int lastCheckpointLevelIndex;
    [HideInInspector] public string lastCheckpointLevelPath;

    [HideInInspector] public Vector2 lastSavepointPos;
    [HideInInspector] public int lastSavepointLevelIndex;
    [HideInInspector] public string lastSavepointLevelPath;

    [Header("Player Values")]
    public float health = 5;
    public float maxHealth = 5;

    public int healingPodAmount = 2;
    public List<int> healingPodFillAmounts;
    public int healingAmountPerPod = 2;

    int healthShardsNeeded = 3;
    public int healthShardAmount = 0;

    int healingFlaskShardsNeeded = 3;
    public int healingFlaskShards = 0;

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
    public bool hasDoubleJump = false;

    [Header("Brooches")]

    [Header("Brooche 01")]
    public bool ownsBrooch_1 = false;
    //Increase attack rate
    public bool equippedBrooch_01 = false;

    [Header("Brooche 02")]
    public bool ownsBrooch_2 = false;
    //Increase flask healing
    public bool equippedBrooch_02 = false;

    [Header("Brooche 03")]
    //Increase refill from enemies
    public bool ownsBrooch_3 = false;
    public bool equippedBrooch_03 = false;

    [Header("Player Control Settings")]
    public bool useDirectionalMouseAttack = false;

    [Header("Game State")]
    public bool isLoading = false;
    public bool isRespawning = false;
    public bool isPaused = false;

    [Header("Other")]
    public bool isFirstTimeResting = true;
    public bool hasOpenedMap = false;

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

        lastSavepointLevelPath = initalPlayerData.initialLevelPath;
        lastCheckpointLevelPath = initalPlayerData.initialLevelPath;
        levelToLoadPath = initalPlayerData.initialLevelPath;

        lastCheckpointPos = initalPlayerData.initialPlayerPosition;
        lastSavepointPos = initalPlayerData.initialPlayerPosition;
    }

    bool emptyRef;
    public ref bool GetBool(string attribute)
    {
        switch (attribute)
        {
            case "equippedBrooch_1":
                return ref equippedBrooch_01;
            case "equippedBrooch_2":
                return ref equippedBrooch_02;
            case "equippedBrooch_3":
                return ref equippedBrooch_03;
            case null:
                return ref emptyRef;
        }

        return ref emptyRef;
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

    public int AddHealingFlaskShard()
    {
        healingFlaskShards++;
        int remainder = (healingFlaskShards % healingFlaskShardsNeeded);
        if (remainder == 0)
        {
            healingPodAmount++;
            healingPodFillAmounts.Add(0);
        }
        return remainder;
    }

    public void UpdateHealingPodFillAmount()
    {
        for (int i = 0; i < UI_HUD.instance.healingFlasks.Count; i++)
        {
            healingPodFillAmounts[i] = (int)UI_HUD.instance.healingFlasks[i].fillAmount;
        }
    }

    public void InitialSpawn()
    {
        playerCamera = Camera.main;
        cameraController = Camera.main.GetComponent<CameraController>();
        player.transform.position = playerStartPosition;
    }

    public void Respawn()
    {
        Debug.Log("Hard Respawn");
        if (!isRespawning)
        {
            UI_HUD.instance.enabled = false;
            isRespawning = true;
            isLoading = true;
            player.GetComponent<Player>().enabled = false;
            LoadScenePath(SceneManager.GetActiveScene().path, lastSavepointLevelPath);

            //if (SceneManager.GetActiveScene().path.Equals(lastSavepointLevelPath))
            //{
            //    StartCoroutine(HardRespawnSameLevel());
            //}
            //else
            //{
            //    LoadScenePath(SceneManager.GetActiveScene().path, lastSavepointLevelPath);
            //}
        }
    }

    public void SoftRespawn()
    {
        Debug.Log("Soft Respawn");
        UI_HUD.instance.enabled = false;
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
        UI_HUD.instance.enabled = true;
    }

    private IEnumerator HardRespawnSameLevel()
    {
        anim.Play("Fade_Out");
        player.GetComponent<Player>().enabled = false;

        yield return new WaitForSecondsRealtime(1f);

        playerCamera.transform.position = player.transform.position;
        player.transform.position = lastSavepointPos;

        yield return new WaitForSecondsRealtime(1f);

        anim.Play("Fade_in");

        player.GetComponent<Player>().enabled = true;
        player.GetComponentInChildren<BoomerangLauncher>().canFire = true;
        isRespawning = false;
        isLoading = false;
        UI_HUD.instance.enabled = true;
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
        if (astarPath != null)
        {
            astarPath.gameObject.SetActive(false);
            astarPath.enabled = false;
        }
        SceneManager.UnloadSceneAsync(levelToUnloadPath).completed += UnloadScene_completed;
    }

    private void UnloadScene_completed(AsyncOperation obj)
    {
        if (obj.isDone)
        {
            SceneManager.LoadSceneAsync(levelToLoadPath, LoadSceneMode.Additive).completed += LoadScene_completed;

        }
    }

    private void LoadScene_completed(AsyncOperation obj)
    {
        if (obj.isDone)
        {
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

    public IEnumerator LoadMainMenu()
    {
        anim.Play("Fade_Out");
        AudioManager.instance.StopAreaThemeWithFade();
        AudioManager.instance.StopAreaAmbianceWithFade();
        AudioManager.instance.StopSFXWithFade();
        AudioManager.instance.StopAllAudio();

        yield return new WaitForSeconds(1f);

        SaveManager.instance.SaveGame();
        anim.Play("Fade_in");
        
        SceneManager.LoadScene(0);
    }

    private void LoadMainMenuCompleted(AsyncOperation obj)
    {
    }

    private IEnumerator FadeIn()
    {
        yield return new WaitForSecondsRealtime(0.2f);
        anim.Play("Fade_in");
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(initalPlayerData.initialPlayerPosition, 2);

        if (lastCheckpointPos != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(lastCheckpointPos, 2);
        }
    }
}
