using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SettingsMenu : MonoBehaviour
{
    [Header("Menus")]
    public GameObject pauseMenu;
    public GameObject optionMenu;
    public GameObject controlsMenu;

    public GameObject videoOptions;
    public GameObject audioOptions;
    public GameObject gameOptions;

    [Header("Game Options")]

    public Toggle abilityToggle;
    public Toggle directionalAttackToggle;

    [Header("Audio Options")]

    EventSystem eventSystem;
    FMOD.Studio.EventInstance SFXVolumeTestEvent;

    float masterVolume = 1;
    float musicVolume = 1;
    float sfxVolume = 1;

    [FMODUnity.EventRef]
    public string SFXTestEvent;

    FMOD.Studio.VCA masterBus;
    FMOD.Studio.VCA musicBus;
    FMOD.Studio.VCA sfxBus;

    [Header("Video Options")]
    public TMPro.TMP_Dropdown resolutionDropdown;
    Resolution[] resolutions;

    [Header("Data")]

    public PlayerDataSO playerData;
    public GlobalConfigSO globalConfig;

    public event Action OnPauseStart = delegate { };
    public event Action OnPauseEnd = delegate { };

    GameObject currentActiveMenu;

    void Start()
    {
        eventSystem = EventSystem.current;

        abilityToggle.onValueChanged.AddListener(delegate {
            ToggleAbilities(abilityToggle);
        });

        directionalAttackToggle.isOn = globalConfig.gameSettings.UseDirectionalMouseAttacks;
        directionalAttackToggle.onValueChanged.AddListener(delegate {
            ToggleDirectionalAttack(directionalAttackToggle);
        });

        masterBus = FMODUnity.RuntimeManager.GetVCA("vca:/Master");
        musicBus = FMODUnity.RuntimeManager.GetVCA("vca:/Music");
        sfxBus = FMODUnity.RuntimeManager.GetVCA("vca:/SFX");

        SFXVolumeTestEvent = FMODUnity.RuntimeManager.CreateInstance(SFXTestEvent);
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

    void Update()
    {
        masterBus.setVolume(masterVolume);
        musicBus.setVolume(musicVolume);
        sfxBus.setVolume(sfxVolume);
    }

    public void TogglePause()
    {
        if (ShopManager.instance.shopIsActive)
            return;

        if (GameManager.instance.isPaused)
        {
            Resume();
        }
        else
        {
            Pause();
        }
        FMODUnity.RuntimeManager.PlayOneShot("event:/SFX/UI/Pause Menu/Pause Button", GetComponent<Transform>().position);
    }

    public void ToggleAbilities(Toggle abilityToggle)
    {
        if (abilityToggle.isOn)
        {
            playerData.hasDashAbility = true;
            playerData.hasTeleportAbility = true;
            playerData.hasSprintAbility = true;
            playerData.hasWallJumpAbility = true;
            playerData.hasDoubleJumpAbility = true;
            playerData.hasBoomerangAbility = true;
            playerData.hasAirDashAbility = true;
        }
        else
        {
            playerData.hasAirDashAbility = false;
            playerData.hasTeleportAbility = false;
            playerData.hasSprintAbility = false;
            playerData.hasWallJumpAbility = false;
            playerData.hasDoubleJumpAbility = false;
        }
    }

    public void ToggleAimAssist(Toggle aimAssistToggle)
    {
        globalConfig.gameSettings.AimAssistOn = aimAssistToggle.isOn;
    }

    public void ToggleDirectionalAttack(Toggle directionalAttackToggle)
    {
        globalConfig.gameSettings.UseDirectionalMouseAttacks = directionalAttackToggle.isOn;
    }

    void Pause()
    {
        OnPauseStart();
        GameManager.instance.isPaused = true;
        pauseMenu.SetActive(true);
        eventSystem.SetSelectedGameObject(pauseMenu.GetComponentInChildren<Button>().gameObject);
        Time.timeScale = 0f;
    }

    public void Resume()
    {
        OnPauseEnd();
        pauseMenu.SetActive(false);
        videoOptions.SetActive(false);
        audioOptions.SetActive(false);
        optionMenu.SetActive(false);
        gameOptions.SetActive(false);
        controlsMenu.SetActive(false);

        eventSystem.SetSelectedGameObject(pauseMenu.GetComponentInChildren<Button>().gameObject);
        GameManager.instance.isPaused = false;

        Time.timeScale = 1f;
    }
    public void ControlsMenu()
    {
        if(currentActiveMenu != null)
        {
            currentActiveMenu.SetActive(false);
        }

        currentActiveMenu = controlsMenu;

        controlsMenu.SetActive(true);
    }
    public void VideoMenu()
    {
        if (currentActiveMenu != null)
        {
            currentActiveMenu.SetActive(false);
        }

        currentActiveMenu = videoOptions;

        videoOptions.SetActive(true);
        eventSystem.SetSelectedGameObject(videoOptions.GetComponentInChildren<Button>().gameObject);
    }
    public void GameMenu()
    {
        if (currentActiveMenu != null)
        {
            currentActiveMenu.SetActive(false);
        }

        currentActiveMenu = gameOptions;

        gameOptions.SetActive(true);
        eventSystem.SetSelectedGameObject(gameOptions.GetComponentInChildren<Toggle>().gameObject);
    }

    public void AudioMenu()
    {
        if (currentActiveMenu != null)
        {
            currentActiveMenu.SetActive(false);
        }

        currentActiveMenu = audioOptions;

        audioOptions.SetActive(true);
        eventSystem.SetSelectedGameObject(audioOptions.GetComponentInChildren<Button>().gameObject);
    }

    public void QuitGame()
    {
        SaveManager.instance.SaveGame();
        Application.Quit();
    }

    public void ReturnToMainMenu()
    {
        Time.timeScale = 1f;
        GameManager.instance.isPaused = false;

        if (DialogManager.instance.dialogueIsActive)
        {
            DialogManager.instance.EndDialogue();
        }

        StartCoroutine(GameManager.instance.LoadMainMenu());
    }

    public void SaveGame()
    {
        SaveManager.instance.SaveGame();
    }
    
    public void LoadMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadSceneAsync(2, LoadSceneMode.Additive).completed += LoadScene_completed;
    }

    private void LoadScene_completed(AsyncOperation obj)
    {
        if (obj.isDone)
        {
            
            SceneManager.UnloadSceneAsync(playerData.currentSceneBuildIndex.Value).completed += UnloadScene_completed;
            SceneManager.UnloadSceneAsync(1);
            SceneManager.UnloadSceneAsync("AStar");
        }
    }

    private void UnloadScene_completed(AsyncOperation obj)
    {
        if (obj.isDone)
        {
            SceneManager.SetActiveScene(SceneManager.GetSceneByBuildIndex(2));
        }
    }

    public void SetMasterVolume(float volume)
    {
        volume = Mathf.Pow(10.0f, volume / 20f);
        masterVolume = volume;
    }

    public void SetMusicVolume(float volume)
    {
        volume = Mathf.Pow(10.0f, volume / 20f);
        musicVolume = volume;
    }

    public void SetSFXVolume(float volume)
    {
        volume = Mathf.Pow(10.0f, volume / 20f);
        sfxVolume = volume;

        FMOD.Studio.PLAYBACK_STATE PbState;
        SFXVolumeTestEvent.getPlaybackState(out PbState);
        if (PbState != FMOD.Studio.PLAYBACK_STATE.PLAYING)
        {
            SFXVolumeTestEvent.start();
        }
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
}
