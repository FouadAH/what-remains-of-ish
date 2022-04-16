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
    FMOD.Studio.Bus MasterBus;

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
        MasterBus = FMODUnity.RuntimeManager.GetBus("Bus:/");
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

    public void PlayAreaAmbiance()
    {
        areaAmbianceInstance = FMODUnity.RuntimeManager.CreateInstance(currentAmbiance);
        areaAmbianceInstance.start();
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

    public void SwitchLevelMusicEvent(string theme)
    {
        if (theme != currentTheme)
        {
            Debug.Log("Switching level theme music");

            StopAreaThemeWithFade();
            currentTheme = theme;
            areaThemeInstance = FMODUnity.RuntimeManager.CreateInstance(currentTheme);
            areaThemeInstance.start();
        }
    }

    public void SwitchLevelAmbianceEvent(string ambiance)
    {
        if (ambiance != currentAmbiance)
        {
            Debug.Log("Switching level ambiance");

            StopAreaAmbianceWithFade();
            currentAmbiance = ambiance;
            areaAmbianceInstance = FMODUnity.RuntimeManager.CreateInstance(currentAmbiance);
            areaAmbianceInstance.start();
        }
    }

    public void StopAllAudio()
    {
        StopAreaThemeWithFade();
        StopAreaAmbianceWithFade();
        StopSFXWithFade();
        MasterBus.stopAllEvents(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
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
