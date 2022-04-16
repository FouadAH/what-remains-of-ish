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
    public GameObject loadView;
    public GameObject optionMenu;
    public GameObject videoOptions;
    public GameObject audioOptions;
    public GameObject gameOptions;

    public Toggle abilityToggle;
    public Toggle directionalAttackToggle;

    public EventSystem eventSystem;
    public AudioMixer audioMixer;
    public Transform lookat;

    public TMPro.TMP_Dropdown resolutionDropdown;

    Resolution[] resolutions;
    public AudioSource titleScreenTheme;
    public GameEvent loadInitialScene;
    CameraController cameraController;
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


        abilityToggle.onValueChanged.AddListener(delegate {
            ToggleAbilities(abilityToggle);
        });

        directionalAttackToggle.onValueChanged.AddListener(delegate {
            ToggleDirectionalAttack(directionalAttackToggle);
        });
    }

    public void StartGame()
    {
        GameManager.instance.GetComponent<Animator>().Play("Fade_Out");
        titleScreenTheme.mute = true;
        StartCoroutine(NewGame());
    }

    public void LoadEvent()
    {
        StartCoroutine(Loading());
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
        eventSystem.SetSelectedGameObject(videoOptions.GetComponentInChildren<Button>().gameObject);
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


    private IEnumerator NewGame()
    {
        GameManager.instance.isLoading = true;
        GameManager.instance.anim.Play("Fade_Out");
        SceneManager.UnloadSceneAsync("MainMenu").completed += MainMenuUnloadComplete;
        yield return null;
    }

    private void LoadBaseSceneComplete(AsyncOperation obj)
    {
        if (obj.isDone)
        {
            //SceneManager.UnloadSceneAsync(1);
            GameManager.instance.InitialSpawn();
            SceneManager.LoadSceneAsync(GameManager.instance.playerData.currentSceneBuildIndex.Value, LoadSceneMode.Additive).completed += LoadLevelComplete;
        }
    }

    private void LoadLevelComplete(AsyncOperation obj)
    {
        if(obj.isDone)
        {
            SceneManager.SetActiveScene(SceneManager.GetSceneByBuildIndex(GameManager.instance.playerData.currentSceneBuildIndex.Value));
            loadInitialScene.Raise();
            GameManager.instance.isLoading = false;
            GameManager.instance.anim.Play("Fade_in");
        }
    }

    private void MainMenuUnloadComplete(AsyncOperation obj)
    {
        if (obj.isDone)
        {
            SceneManager.LoadSceneAsync("PlayerScene", LoadSceneMode.Additive).completed += LoadBaseSceneComplete;
        }
    }


    private IEnumerator Loading()
    {
        GameManager.instance.isLoading = true;
        GameManager.instance.anim.Play("Fade_Out");
        //SceneManager.LoadSceneAsync("PlayerScene", LoadSceneMode.Additive).completed += LoadBaseSceneComplete;
        SceneManager.UnloadSceneAsync(1).completed += MainMenuUnloadComplete;

        yield return null;
    }
}
