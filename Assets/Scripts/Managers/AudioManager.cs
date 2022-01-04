using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;
    [FMODUnity.EventRef] public string currentTheme;

    public FMOD.Studio.EventInstance areaThemeInstance;
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
    }

    public void PlayAreaTheme()
    {
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

    public void StopAreaThemeWithFade()
    {
        areaThemeInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        currentTheme = null;
    }

    public void StopAreaThemeWithoutFade()
    {
        areaThemeInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        currentTheme = null;
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
