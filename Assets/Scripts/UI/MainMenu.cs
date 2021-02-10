using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Audio;

public class MainMenu : MonoBehaviour
{
    public static bool gameIsPaused = false;
    public GameObject mainMenu;
    public GameObject optionMenu;
    public GameObject videoOptions;
    public GameObject audioOptions;
    public EventSystem eventSystem;
    public AudioMixer audioMixer;
    public Transform lookat;

    public TMPro.TMP_Dropdown resolutionDropdown;

    Resolution[] resolutions;
    private int currentScene;

    private void Awake()
    {
        //GameManager.instance.cameraController.virtualCamera.transform.position = lookat.position;
    }
    public void Start()
    {
        GameManager.instance.cameraController.virtualCamera.Follow = lookat;

        resolutions = Screen.resolutions;

        resolutionDropdown.ClearOptions();

        int currentResolutionIndex = 0;

        List<string> options = new List<string>();
        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + "x" + resolutions[i].height;
            options.Add(option);

            if (resolutions[i].width == Screen.currentResolution.width && resolutions[i].height == Screen.currentResolution.height)
            {
                currentResolutionIndex = i;
            }
        }

        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();
    }

    public void StartGame()
    {
        StartCoroutine(NewGame());
    }

    public void LoadGame()
    {
        LoadData();
        StartCoroutine(Loading());
    }

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Quit");
    }

    public void Options()
    {
        mainMenu.SetActive(false);
        optionMenu.SetActive(true);
        eventSystem.SetSelectedGameObject(optionMenu.GetComponentInChildren<Button>().gameObject);
    }
    public void OptionsBack()
    {
        mainMenu.SetActive(true);
        optionMenu.SetActive(false);
        eventSystem.SetSelectedGameObject(mainMenu.GetComponentInChildren<Button>().gameObject);
    }
    public void VideoMenu()
    {
        videoOptions.SetActive(true);
        optionMenu.SetActive(false);
        eventSystem.SetSelectedGameObject(videoOptions.GetComponentInChildren<Button>().gameObject);
    }
    public void VideoMenuBack()
    {
        videoOptions.SetActive(false);
        optionMenu.SetActive(true);
        eventSystem.SetSelectedGameObject(optionMenu.GetComponentInChildren<Button>().gameObject);
    }
    public void AudioMenu()
    {
        audioOptions.SetActive(true);
        optionMenu.SetActive(false);
        eventSystem.SetSelectedGameObject(audioOptions.GetComponentInChildren<Button>().gameObject);
    }
    public void AudioMenuBack()
    {
        audioOptions.SetActive(false);
        optionMenu.SetActive(true);
        eventSystem.SetSelectedGameObject(optionMenu.GetComponentInChildren<Button>().gameObject);
    }

    public void SetMasterVolume(float volume)
    {
        audioMixer.SetFloat("volume", volume);
    }

    public void SetQuality(int qualityIndex)
    {
        QualitySettings.SetQualityLevel(qualityIndex);
    }

    public void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
    }
    public void SetResolution(int resolutionIndex)
    {
        Resolution resolution = resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
    }

    private IEnumerator NewGame()
    {
        GameManager.instance.loading = true;
        LoadDataNewGame();
        GameManager.instance.anim.SetTrigger("FadeD");
        SceneManager.LoadSceneAsync("AStar", LoadSceneMode.Additive);
        SceneManager.LoadSceneAsync("Base", LoadSceneMode.Additive).completed += Base_completed_initial;
        yield return null;
    }

    private void Base_completed_initial(AsyncOperation obj)
    {
        if (obj.isDone)
        {
            SceneManager.LoadSceneAsync("Level_1", LoadSceneMode.Additive).completed += MainMenu_completed;
        }
    }

    private void Base_completed(AsyncOperation obj)
    {
        if (obj.isDone)
        {
            SceneManager.LoadSceneAsync(currentScene, LoadSceneMode.Additive).completed += MainMenu_completed;
        }
    }

    private void MainMenu_completed(AsyncOperation obj)
    {
        if(obj.isDone)
        {
            AstarPath.active.Scan();
            SceneManager.SetActiveScene(SceneManager.GetSceneByBuildIndex(currentScene));
            GameManager.instance.loading = false;
            SceneManager.UnloadSceneAsync("MainMenu").completed += UnloadScene_completed;
        }
    }

    private void UnloadScene_completed(AsyncOperation obj)
    {
        if (obj.isDone)
        {
            GameManager.instance.anim.SetTrigger("FadeIn");
        }
    }


    private IEnumerator Loading()
    {
        GameManager.instance.loading = true;
        GameManager.instance.anim.SetTrigger("FadeD");
        SceneManager.LoadSceneAsync("AStar", LoadSceneMode.Additive);
        SceneManager.LoadSceneAsync("Base", LoadSceneMode.Additive).completed += Base_completed;
        yield return null;
    }
    

    /////////////////////////////////////////////////////////////////
    ///                         LOADING                           ///
    /////////////////////////////////////////////////////////////////                 
    public Vector3 playerPosition;
    public void LoadData()
    {
        PlayerData data = GameDataController.LoadData();

        GameManager.instance.health = data.health;
        GameManager.instance.maxHealth = data.maxHealth;
        GameManager.instance.currency = data.currency;

        Vector3 playerPosition;
        playerPosition.x = data.playerPosition[0];
        playerPosition.y = data.playerPosition[1];
        playerPosition.z = data.playerPosition[2];
        GameManager.instance.playerPosition = playerPosition;


        Vector3 dronePosition;
        dronePosition.x = data.dronePosition[0];
        dronePosition.y = data.dronePosition[1];
        dronePosition.z = data.dronePosition[2];
        GameManager.instance.dronePosition = dronePosition;

        Vector2 checkpointPosition;
        checkpointPosition.x = data.lastCheckpointPos[0];
        checkpointPosition.y = data.lastCheckpointPos[1];
        GameManager.instance.lastCheckpointPos = checkpointPosition;

        GameManager.instance.lastCheckpointLevelIndex = data.lastCheckpointLevelIndex;
        currentScene = data.currentScene;
    }

    public void LoadDataNewGame()
    {
        GameManager.instance.health = 100;
        GameManager.instance.maxHealth = 100;
        GameManager.instance.currency = 0;
        GameManager.instance.playerPosition = new Vector3(-12f, 105f);
        GameManager.instance.dronePosition = new Vector3(-12f, 105f);
        GameManager.instance.lastCheckpointPos = new Vector3(-12f, 105f);
        GameManager.instance.lastCheckpointLevelIndex = 3;
        GameManager.instance.currentScene = 3;
        currentScene = 3;
    }

}
