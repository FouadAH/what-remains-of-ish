﻿using System.Collections;
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

    public void Start()
    {
        lookat.GetComponent<Animator>().Play("camera");
        GameManager.instance.cameraController = Camera.main.GetComponent<CameraController>();
        GameManager.instance.cameraController.virtualCamera.transform.position = lookat.position;
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
        GameManager.instance.anim.Play("Fade_Out");
        SceneManager.LoadSceneAsync("AStar", LoadSceneMode.Additive);
        SceneManager.UnloadSceneAsync("MainMenu").completed += MainMenuUnloadComplete;
        yield return null;
    }

    private void Base_completed_initial(AsyncOperation obj)
    {
        if (obj.isDone)
        {
            SceneManager.LoadSceneAsync(currentScene, LoadSceneMode.Additive).completed += LoadLevelComplete;
        }
    }

    private void LoadBaseSceneComplete(AsyncOperation obj)
    {
        if (obj.isDone)
        {
            GameManager.instance.playerCamera = Camera.main;
            GameManager.instance.cameraController = Camera.main.GetComponent<CameraController>();

            GameManager.instance.player.transform.position = GameManager.instance.playerPosition;

            SceneManager.LoadSceneAsync(currentScene, LoadSceneMode.Additive).completed += LoadLevelComplete;
        }
    }

    private void LoadLevelComplete(AsyncOperation obj)
    {
        if(obj.isDone)
        {
            AstarPath.active.Scan();
            SceneManager.SetActiveScene(SceneManager.GetSceneByBuildIndex(currentScene));
            GameManager.instance.loading = false;
            GameManager.instance.anim.Play("Fade_in");
        }
    }

    private void MainMenuUnloadComplete(AsyncOperation obj)
    {
        if (obj.isDone)
        {
            SceneManager.LoadSceneAsync("Base", LoadSceneMode.Additive).completed += LoadBaseSceneComplete;
        }
    }


    private IEnumerator Loading()
    {
        GameManager.instance.loading = true;
        GameManager.instance.anim.SetTrigger("FadeD");
        SceneManager.LoadSceneAsync("AStar", LoadSceneMode.Additive);
        SceneManager.UnloadSceneAsync("MainMenu").completed += MainMenuUnloadComplete;
    
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

        Vector2 checkpointPosition;
        checkpointPosition.x = data.lastCheckpointPos[0];
        checkpointPosition.y = data.lastCheckpointPos[1];
        GameManager.instance.lastCheckpointPos = checkpointPosition;

        GameManager.instance.lastCheckpointLevelIndex = data.lastCheckpointLevelIndex;
        currentScene = data.currentScene;
    }

    public void LoadDataNewGame()
    {
        GameManager.instance.health = 5;
        GameManager.instance.maxHealth = 5;
        GameManager.instance.currency = 0;
        GameManager.instance.playerPosition = new Vector3(-40f, 5f);

        GameManager.instance.lastSavepointPos = new Vector3(-40f, 5f);
        GameManager.instance.lastSavepointLevelIndex = 4;

        GameManager.instance.lastCheckpointPos = new Vector3(-40f, 5f);
        GameManager.instance.lastCheckpointLevelIndex = 4;
        GameManager.instance.currentScene = 4;
        currentScene = 4;
    }

}
