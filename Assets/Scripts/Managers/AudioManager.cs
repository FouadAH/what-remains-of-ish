using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;
    [FMODUnity.EventRef] public string currentTheme;
    [FMODUnity.EventRef] public string currentAmbiance;

    public FMOD.Studio.EventInstance areaThemeInstance;
    public FMOD.Studio.EventInstance areaAmbianceInstance;

    FMOD.Studio.Bus MusicBus;
    FMOD.Studio.Bus SFXBus;

    FMOD.Studio.EventDescription eventDescription;
    FMOD.Studio.PARAMETER_DESCRIPTION param;

    System.Guid eventID;

    [Range(0, 100)] public float playerHealth;

    /// <summary>
    /// Singelton, makes sure only one instance of this object exsists
    /// </summary>
    void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
        }
        DontDestroyOnLoad(this);
        MusicBus = FMODUnity.RuntimeManager.GetBus("Bus:/Music");
        SFXBus = FMODUnity.RuntimeManager.GetBus("Bus:/SFX");

    }

    public void PlayAreaTheme()
    {
        areaAmbianceInstance = FMODUnity.RuntimeManager.CreateInstance(currentAmbiance);
        areaAmbianceInstance.start();

        areaThemeInstance = FMODUnity.RuntimeManager.CreateInstance(currentTheme);
        areaThemeInstance.start();

        areaThemeInstance.getDescription(out eventDescription);
        eventDescription.getID(out eventID);
    }

    public void PlayAreaTheme(string newTheme)
    {
        if (newTheme == currentTheme)
            return;

        currentTheme = newTheme;

        areaThemeInstance = FMODUnity.RuntimeManager.CreateInstance(newTheme);
        areaThemeInstance.start();
        areaThemeInstance.getDescription(out eventDescription);

        eventDescription.getID(out eventID);

    }

    public void SwitchLevelMusic()
    {

        if (GameManager.instance.currentLevel.theme != currentTheme)
        {
            Debug.Log("Switching level theme music");

            StopAreaThemeWithFade();
            currentTheme = GameManager.instance.currentLevel.theme;
            areaThemeInstance = FMODUnity.RuntimeManager.CreateInstance(currentTheme);
            areaThemeInstance.start();
        }

        if (GameManager.instance.currentLevel.ambiance != currentAmbiance)
        {
            Debug.Log("Switching level ambiance");

            StopAreaAmbianceWithFade();
            currentAmbiance = GameManager.instance.currentLevel.ambiance;
            areaAmbianceInstance = FMODUnity.RuntimeManager.CreateInstance(currentAmbiance);
            areaAmbianceInstance.start();
        }
    }

    public void StopAreaThemeWithFade()
    {
        areaThemeInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        areaThemeInstance.release();
        MusicBus.stopAllEvents(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
    }

    public void StopAreaAmbianceWithFade()
    {
        areaAmbianceInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        areaAmbianceInstance.release();
    }


    public void StopSFXWithFade()
    {
        SFXBus.stopAllEvents(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
    }


    public void StopAreaThemeWithoutFade()
    {
        areaThemeInstance.release();
        areaThemeInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
    }

    public void SetIntensity(float value)
    {
        areaThemeInstance.setParameterByName("Intensity", value);
    }

    public void SetHealthParameter(float value)
    {
        areaThemeInstance.setParameterByName("Health", value);
    }

    private void OnValidate()
    {
        SetHealthParameter(playerHealth);
    }
}
