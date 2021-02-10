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
    public GameObject drone;

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
    }

    public float health = 10000;
    public float maxHealth = 10000;
    public float gunHeat = 0;
    public float maxGunHeat = 100;
    public int currency;
    public Camera camera;
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
        loading = true;
        currentScene = levelToLoad;
        player.GetComponent<Player>().enabled = false;
        drone.GetComponent<DroneAI>().enabled = false;
        anim.SetTrigger("FadeOut");
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
            drone.GetComponent<DroneAI>().enabled = true;
            AstarPath.active.Scan();
            SceneManager.UnloadSceneAsync(levelToUnload).completed += UnloadScene_completed;
        }
    }

    private void UnloadScene_completed(AsyncOperation obj)
    {
        if (obj.isDone)
        {
            anim.SetTrigger("FadeIn");
            SceneManager.SetActiveScene(SceneManager.GetSceneByBuildIndex(levelToLoad));
            //AstarPath.active.Scan();
        }
    }

    /////////////////////////////////////////////////////////////////
    ///                 SAVING AND LOADING                       ///
    ///////////////////////////////////////////////////////////////                 

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

        Vector3 dronePosition;
        dronePosition.x = data.dronePosition[0];
        dronePosition.y = data.dronePosition[1];
        dronePosition.z = data.dronePosition[2];

        Vector2 checkpointPosition;
        checkpointPosition.x = data.lastCheckpointPos[0];
        checkpointPosition.y = data.lastCheckpointPos[1];
        lastCheckpointPos = checkpointPosition;
        
        lastCheckpointLevelIndex = data.lastCheckpointLevelIndex;

        currentScene = data.currentScene;
    }
}
