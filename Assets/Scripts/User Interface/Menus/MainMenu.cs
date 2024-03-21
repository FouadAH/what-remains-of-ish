using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Audio;

public class MainMenu : MonoBehaviour
{
    [Header("Menus")]

    public static bool gameIsPaused = false;
    public GameObject mainMenu;
    public GameObject loadView;
    public GameObject chapterSelect;
    public GameObject optionMenu;
    public GameObject videoOptions;
    public GameObject audioOptions;
    public GameObject gameOptions;

    [Header("Game Options")]

    public Toggle abilityToggle;
    public Toggle directionalAttackToggle;

    public EventSystem eventSystem;
    public Transform lookat;

    [Header("Video Options")]

    public TMPro.TMP_Dropdown resolutionDropdown;

    Resolution[] resolutions;
    CameraController cameraController;

    [Header("Audio Options")]

    [FMODUnity.EventRef]
    FMOD.Studio.EventInstance SFXVolumeTestEvent;

    [FMODUnity.EventRef] 
    public string SFXTestEvent;

    FMOD.Studio.VCA masterBus;
    FMOD.Studio.VCA musicBus;
    FMOD.Studio.VCA sfxBus;

    [Header("Data")]
    public PlayerDataSO playerData;
    public GlobalConfigSO globalConfig;

    public void Start()
    {
        lookat.GetComponent<Animator>().Play("camera");
        cameraController = Camera.main.GetComponent<CameraController>();
        cameraController.virtualCamera.transform.position = lookat.position;
        cameraController.virtualCamera.Follow = lookat;

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

        SFXVolumeTestEvent = FMODUnity.RuntimeManager.CreateInstance(SFXTestEvent);
        masterBus = FMODUnity.RuntimeManager.GetVCA("vca:/Master");
        musicBus = FMODUnity.RuntimeManager.GetVCA("vca:/Music");
        sfxBus = FMODUnity.RuntimeManager.GetVCA("vca:/SFX");

        abilityToggle.onValueChanged.AddListener(delegate {
            ToggleAbilities(abilityToggle);
        });

        directionalAttackToggle.onValueChanged.AddListener(delegate {
            ToggleDirectionalAttack(directionalAttackToggle);
        });
    }

    private void Update()
    {
        masterBus.setVolume(masterVolume);
        musicBus.setVolume(musicVolume);
        sfxBus.setVolume(sfxVolume);
    }

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Quit");
    }

    public void LoadViewOpen()
    {
        mainMenu.SetActive(false);
        loadView.SetActive(true);
        eventSystem.SetSelectedGameObject(loadView.GetComponentInChildren<Button>().gameObject);
    }
    public void LoadViewClose()
    {
        mainMenu.SetActive(true);
        loadView.SetActive(false);
        eventSystem.SetSelectedGameObject(mainMenu.GetComponentInChildren<Button>().gameObject);
    }

    public void ChapeterSelectOpen()
    {
        mainMenu.SetActive(false);
        chapterSelect.SetActive(true);
        eventSystem.SetSelectedGameObject(chapterSelect.GetComponentInChildren<Button>().gameObject);
    }
    public void ChapterSelectClose()
    {
        mainMenu.SetActive(true);
        chapterSelect.SetActive(false);
        eventSystem.SetSelectedGameObject(mainMenu.GetComponentInChildren<Button>().gameObject);
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

    public void GameMenu()
    {
        gameOptions.SetActive(true);
        optionMenu.SetActive(false);
        eventSystem.SetSelectedGameObject(gameOptions.GetComponentInChildren<Toggle>().gameObject);
    }
    public void GameMenuBack()
    {
        gameOptions.SetActive(false);
        optionMenu.SetActive(true);
        eventSystem.SetSelectedGameObject(optionMenu.GetComponentInChildren<Button>().gameObject);
    }

    public void VideoMenu()
    {
        videoOptions.SetActive(true);
        optionMenu.SetActive(false);
        eventSystem.SetSelectedGameObject(videoOptions.GetComponentInChildren<Toggle>().gameObject);
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
        eventSystem.SetSelectedGameObject(audioOptions.GetComponentInChildren<Slider>().gameObject);
    }
    public void AudioMenuBack()
    {
        audioOptions.SetActive(false);
        optionMenu.SetActive(true);
        eventSystem.SetSelectedGameObject(optionMenu.GetComponentInChildren<Button>().gameObject);
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

    public void ToggleAbilities(Toggle abilityToggle)
    {
        if (abilityToggle.isOn)
        {
            playerData.hasDashAbility = true;
            playerData.hasTeleportAbility = true;
            playerData.hasSprintAbility = true;
            playerData.hasWallJumpAbility = true;
            playerData.hasDoubleJumpAbility = true;

        }
        else
        {
            playerData.hasDashAbility = false;
            playerData.hasTeleportAbility = false;
            playerData.hasSprintAbility = false;
            playerData.hasWallJumpAbility = false;
            playerData.hasDoubleJumpAbility = false;

        }
    }

    public void ToggleDirectionalAttack(Toggle directionalAttackToggle)
    {
        globalConfig.gameSettings.UseDirectionalMouseAttacks= directionalAttackToggle.isOn;
    }
}
