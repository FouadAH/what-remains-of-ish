using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PauseMenu : MonoBehaviour
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
    Player_Input player_Input;

    public event Action OnPauseStart = delegate { };
    public event Action OnPauseEnd = delegate { };

    GameObject currentActiveMenu;

    void Start()
    {
        eventSystem = EventSystem.current;

        abilityToggle.onValueChanged.AddListener(delegate {
            ToggleAbilities(abilityToggle);
        });

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
            GameManager.instance.playerData.hasDashAbility = true;
            GameManager.instance.playerData.hasTeleportAbility = true;
            GameManager.instance.playerData.hasSprintAbility = true;
            GameManager.instance.playerData.hasWallJumpAbility = true;
            GameManager.instance.playerData.hasDoubleJumpAbility = true;
            GameManager.instance.playerData.hasBoomerangAbility = true;
            GameManager.instance.playerData.hasAirDashAbility = true;
        }
        else
        {
            GameManager.instance.playerData.hasAirDashAbility = false;
            GameManager.instance.playerData.hasTeleportAbility = false;
            GameManager.instance.playerData.hasSprintAbility = false;
            GameManager.instance.playerData.hasWallJumpAbility = false;
            GameManager.instance.playerData.hasDoubleJumpAbility = false;

        }
    }

    public void ToggleAimAssist(Toggle aimAssistToggle)
    {
        if (aimAssistToggle.isOn)
        {
            GameManager.instance.player.GetComponent<Player>().boomerangLauncher.aimAssistOn = true;
        }
        else
        {
            GameManager.instance.player.GetComponent<Player>().boomerangLauncher.aimAssistOn = false;
        }
    }

    public void ToggleDirectionalAttack(Toggle directionalAttackToggle)
    {
        if (directionalAttackToggle.isOn)
        {
            GameManager.instance.useDirectionalMouseAttack = true;
        }
        else
        {
            GameManager.instance.useDirectionalMouseAttack = false;
        }
    }

    void Pause()
    {
        OnPauseStart();
        GameManager.instance.isPaused = true;
        Debug.Log(pauseMenu);
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
        //Time.timeScale = 1f;
        //LoadMainMenu();
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
            
            SceneManager.UnloadSceneAsync(GameManager.instance.playerData.currentSceneBuildIndex.Value).completed += UnloadScene_completed;
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
