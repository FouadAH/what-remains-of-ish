using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using static LevelManager;

/// <summary>
/// Class responsible for ascynchronis scene loading and unloading, as well as holding, saving and loading player data 
/// </summary>

public class GameManager : MonoBehaviour
{
    public static GameManager instance { get; private set; }
    [HideInInspector] public GameObject player;

    [Header("Scene Settings")]
    public PlayerConfig initalPlayerData;
    public Level currentLevel;

    [Header("Player Values")]
    public PlayerDataSO playerData;

    [Header("Brooches")]
    public bool equippedBrooch_01 = false; //Increase attack rate
    public bool equippedBrooch_02 = false; //Increase flask healing
    public bool equippedBrooch_03 = false; //Increase refill from enemies

    [Header("Game State")]
    public bool isLoading = false;
    public bool isRespawning = false;
    public bool isPaused = false;

    [Header("Other")]
    public bool isFirstTimeResting = true;
    public bool hasOpenedMap = false;
    public bool isInDebugMode = false;

    [Header("Game Events")]
    public GameEvent playerRespawn;
    public GameEvent loadNewLevelEvent;

    public GameEvent loadDataEvent;
    public GameEvent saveDataEvent;

    public GameEvent loadingStartEvent;
    public GameEvent loadingEndEvent;

    public GameEvent addHealingFlask;

    public GameEvent stopAllAudioEvent;
    public StringEvent switchMusicEvent;
    public StringEvent switchAmbianceEvent;

    public StringEvent debugTextEvent;

    Vector3 newPlayerPos;

    private string levelToUnloadPath;
    private string levelToLoadPath;

    readonly int healthShardsNeeded = 3;
    readonly int healingFlaskShardsNeeded = 3;

    bool emptyRef;

    AstarPath astarPath;
    Animator animator;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
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
        animator = gameObject.GetComponent<Animator>();

#if UNITY_EDITOR
        playerData.playerHealth.Value = initalPlayerData.health;
        playerData.playerMaxHealth.Value = initalPlayerData.maxHealth;
#endif

    }

    public ref bool GetBool(int broochID)
    {
        switch (broochID)
        {
            case 1:
                return ref equippedBrooch_01;
            case 2:
                return ref equippedBrooch_02;
            case 3:
                return ref equippedBrooch_03;
            default:
                return ref emptyRef;
        }
    }

    public void OnReceivedHealthShard()
    {
        playerData.playerHealthShardAmount.Value++;
        int remainder = (playerData.playerHealthShardAmount.Value % healthShardsNeeded);
        if (remainder == 0)
        {
            playerData.playerMaxHealth.Value++;
            float missingHealth = playerData.playerMaxHealth.Value - playerData.playerHealth.Value;
            player.GetComponent<Player>().RestoreHP((int)missingHealth);

            debugTextEvent.Raise("3 health shards collected. Max health increased by 1");
        }
        else
        {
            debugTextEvent.Raise("Picked Up Health Shard. Pick up " + (3 - remainder) + " more to increase your health");
        }
    }

    public void OnReceivedHealingFlaskShard()
    {
        playerData.playerHealingFlaskShards.Value++;
        int remainder = (playerData.playerHealingFlaskShards.Value % healingFlaskShardsNeeded);
        if (remainder == 0)
        {
            playerData.playerHealingPodAmount.Value++;
            playerData.playerHealingPodFillAmounts.Add(0);
            addHealingFlask.Raise();
        }
        else
        {
            debugTextEvent.Raise("Picked Up a Healing Pod Shard. Pick up " + (3 - remainder) + " more to increase the number of healing pods");
        }
    }

    public void OnReceivedArtifact()
    {
        debugTextEvent.Raise("Picked up an artifact!");
    }

    public void InitialSpawn()
    {
        player.transform.position = new Vector3(playerData.playerPosition.X, playerData.playerPosition.Y, 0);
    }

    public void Respawn()
    {
        Debug.Log("Hard Respawn");
        if (!isRespawning)
        {
            isRespawning = true;
            isLoading = true;
            player.GetComponent<Player>().enabled = false;
            LoadScenePath(SceneManager.GetActiveScene().path, playerData.lastSavepointLevelPath);
        }
    }

    public void SoftRespawn()
    {
        Debug.Log("Soft Respawn");
        StartCoroutine(SoftRespawnRoutine());
    }

    private IEnumerator SoftRespawnRoutine()
    {
        loadingStartEvent.Raise();
        player.GetComponent<Player>().enabled = false;

        yield return new WaitForSecondsRealtime(1f);

        playerRespawn.Raise();
        Camera.main.transform.position = player.transform.position;
        player.transform.position = new Vector2( playerData.lastCheckpointPos.X, playerData.lastCheckpointPos.Y );

        yield return new WaitForSecondsRealtime(1f);

        loadingEndEvent.Raise();

        player.GetComponent<Player>().enabled = true;
        player.GetComponentInChildren<BoomerangLauncher>().canFire = true;
    }

    public void LoadScenePath(string levelToUnloadPath, string levelToLoadPath, Vector3 playerPos)
    {
        isLoading = true;

        player.GetComponent<Player_Input>().DisablePlayerInput();
        newPlayerPos = playerPos;

        this.levelToLoadPath = levelToLoadPath;
        this.levelToUnloadPath = levelToUnloadPath;

        StartCoroutine(LoadSceneRoutine());
    }

    public void LoadScenePath(string levelToUnloadPath, string levelToLoadPath)
    {
        isLoading = true;

        player.GetComponent<Player>().enabled = false;

        this.levelToLoadPath = levelToLoadPath;
        this.levelToUnloadPath = levelToUnloadPath;

        StartCoroutine(LoadSceneRoutine());
    }

    private IEnumerator LoadSceneRoutine()
    {
        SaveManager.instance.SaveSceneData();

        loadingStartEvent.Raise();
        yield return new WaitForSeconds(1f);

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
                player.transform.position = new Vector2(playerData.lastSavepointPos.X, playerData.lastSavepointPos.Y);
                Camera.main.transform.position = player.transform.position;
                player.GetComponentInChildren<BoomerangLauncher>().canFire = true;
            }

            player.GetComponent<PlayerMovement>().movePlayerLoadingState = true;
            isRespawning = false;

            loadingEndEvent.Raise();

            SceneManager.SetActiveScene(SceneManager.GetSceneByPath(levelToLoadPath));
            playerData.currentSceneBuildIndex.Value = SceneManager.GetActiveScene().buildIndex;

            SaveManager.instance.SavePlayerData();

            loadNewLevelEvent.Raise();
            switchMusicEvent.Raise(currentLevel.theme);
            switchAmbianceEvent.Raise(currentLevel.ambiance);

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
        loadingStartEvent.Raise();
        stopAllAudioEvent.Raise();

        yield return new WaitForSeconds(1f);

        SaveManager.instance.SaveGame();
        loadingEndEvent.Raise();

        SceneManager.LoadScene(0);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(initalPlayerData.initialPlayerPosition, 2);

        if (playerData.lastCheckpointPos != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(new Vector3(playerData.lastCheckpointPos.X, playerData.lastCheckpointPos.Y, 0), 2);
        }
    }
}
