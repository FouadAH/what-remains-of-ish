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
    public GameObject pauseMenu;
    public GameObject optionMenu;
    public GameObject controlsMenu;

    public GameObject videoOptions;
    public GameObject audioOptions;
    public GameObject gameOptions;

    public Toggle abilityToggle;
    public Toggle directionalAttackToggle;


    EventSystem eventSystem;
    FMOD.Studio.EventInstance SFXVolumeTestEvent;

    [FMODUnity.EventRef]
    public string SFXTestEvent;
    FMOD.Studio.VCA masterBus;
    FMOD.Studio.VCA musicBus;
    FMOD.Studio.VCA sfxBus;

    public TMPro.TMP_Dropdown resolutionDropdown;

    Resolution[] resolutions;
    Player_Input player_Input;

    public event Action OnPauseStart = delegate { };
    public event Action OnPauseEnd = delegate { };
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
            GameManager.instance.hasDashAbility = true;
            GameManager.instance.hasTeleportAbility = true;
            GameManager.instance.hasSprintAbility = true;
            GameManager.instance.hasWallJump = true;
        }
        else
        {
            GameManager.instance.hasDashAbility = false;
            GameManager.instance.hasTeleportAbility = false;
            GameManager.instance.hasSprintAbility = false;
            GameManager.instance.hasWallJump = false;
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
    public void Options()
    {
        pauseMenu.SetActive(false);
        optionMenu.SetActive(true);
        eventSystem.SetSelectedGameObject(optionMenu.GetComponentInChildren<Button>().gameObject);
    }
    public void OptionsBack()
    {
        pauseMenu.SetActive(true);
        optionMenu.SetActive(false);
        eventSystem.SetSelectedGameObject(pauseMenu.GetComponentInChildren<Button>().gameObject);
    }

    public void ControlsMenu()
    {
        pauseMenu.SetActive(false);
        controlsMenu.SetActive(true);
        eventSystem.SetSelectedGameObject(optionMenu.GetComponentInChildren<Button>().gameObject);
    }
    public void ControlsMenuBack()
    {
        pauseMenu.SetActive(true);
        controlsMenu.SetActive(false);
        eventSystem.SetSelectedGameObject(pauseMenu.GetComponentInChildren<Button>().gameObject);
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

    public void GameMenu()
    {
        gameOptions.SetActive(true);
        optionMenu.SetActive(false);
        eventSystem.SetSelectedGameObject(videoOptions.GetComponentInChildren<Button>().gameObject);
    }
    public void GameMenuBack()
    {
        gameOptions.SetActive(false);
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
            
            SceneManager.UnloadSceneAsync(GameManager.instance.currentSceneBuildIndex).completed += UnloadScene_completed;
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

    void Pause()
    {
        OnPauseStart();
        //GameManager.instance.player.GetComponent<Player_Input>().enabled = false;
        GameManager.instance.isPaused = true;
        Debug.Log(pauseMenu);
        pauseMenu.SetActive(true);
        eventSystem.SetSelectedGameObject(pauseMenu.GetComponentInChildren<Button>().gameObject);
        Time.timeScale = 0f;
    }

    float masterVolume = 1;
    float musicVolume = 1;
    float sfxVolume = 1;

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
