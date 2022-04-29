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

    public Level currentLevel;
    public string currentScenePath;

    private string levelToUnloadPath;
    private string levelToLoadPath;

    [Header("Player Values")]

    int healthShardsNeeded = 3;
    int healingFlaskShardsNeeded = 3;

    public PlayerDataSO playerData;

    public Camera playerCamera;

    [Header("Player Debug Settings")]
    public bool hasInfiniteLives = false;

    [HideInInspector] public Animator anim;
    AstarPath astarPath;

    //[Header("Player Abilities")]
    //public bool hasBoomerang = false;
    //public bool hasDashAbility = true;
    //public bool hasWallJump = false;
    //public bool hasTeleportAbility = false;
    //public bool hasSprintAbility = false;
    //public bool hasDoubleJump = false;x

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

    [Header("Game Events")]
    public GameEvent loadNewLevelEvent;
    public GameEvent loadDataEvent;
    public GameEvent saveDataEvent;
    public GameEvent stopAllAudioEvent;
    public StringEvent switchMusicEvent;
    public StringEvent switchAmbianceEvent;

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

        playerData.lastSavepointLevelPath = initalPlayerData.initialLevelPath;
        playerData.lastCheckpointLevelPath = initalPlayerData.initialLevelPath;
        levelToLoadPath = initalPlayerData.initialLevelPath;

        playerData.lastCheckpointPos.X = initalPlayerData.initialPlayerPosition.x;
        playerData.lastCheckpointPos.Y = initalPlayerData.initialPlayerPosition.y;

        playerData.lastSavepointPos.X = initalPlayerData.initialPlayerPosition.x;
        playerData.lastSavepointPos.Y = initalPlayerData.initialPlayerPosition.y;
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
        playerData.playerHealthShardAmount.Value++;
        int remainder = (playerData.playerHealthShardAmount.Value % healthShardsNeeded);
        if (remainder == 0)
        {
            playerData.playerMaxHealth.Value++;
        }
        return remainder;
    }

    public int AddHealingFlaskShard()
    {
        playerData.playerHealingFlaskShards.Value++;
        int remainder = (playerData.playerHealingFlaskShards.Value % healingFlaskShardsNeeded);
        if (remainder == 0)
        {
            playerData.playerHealingPodAmount.Value++;
            playerData.playerHealingPodFillAmounts.Add(0);
        }
        return remainder;
    }

    public void UpdateHealingPodFillAmount()
    {
        for (int i = 0; i < UI_HUD.instance.healingFlasks.Count; i++)
        {
            playerData.playerHealingPodFillAmounts[i] = (int)UI_HUD.instance.healingFlasks[i].fillAmount;
        }
    }

    public void InitialSpawn()
    {
        playerCamera = Camera.main;
        player.transform.position = new Vector3(playerData.playerPosition.X, playerData.playerPosition.Y, 0);
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
            LoadScenePath(SceneManager.GetActiveScene().path, playerData.lastSavepointLevelPath);
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
        player.transform.position = new Vector2( playerData.lastCheckpointPos.X, playerData.lastCheckpointPos.Y );

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
        player.transform.position = new Vector2(playerData.lastSavepointPos.X, playerData.lastSavepointPos.Y);

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
                player.transform.position = new Vector2(playerData.lastSavepointPos.X, playerData.lastSavepointPos.Y);
                playerCamera.transform.position = player.transform.position;
                player.GetComponentInChildren<BoomerangLauncher>().canFire = true;
            }


            isLoading = false;
            isRespawning = false;

            StartCoroutine(FadeIn());

            player.GetComponent<Player>().enabled = true;

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
        anim.Play("Fade_Out");
        stopAllAudioEvent.Raise();

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

        if (playerData.lastCheckpointPos != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(new Vector3(playerData.lastCheckpointPos.X, playerData.lastCheckpointPos.Y, 0), 2);
        }
    }
}
