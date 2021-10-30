using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;
    [FMODUnity.EventRef] public string areaTheme;
    FMOD.Studio.EventInstance areaThemeInstance;

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
        areaThemeInstance = FMODUnity.RuntimeManager.CreateInstance(areaTheme);
        areaThemeInstance.start();
    }

    public void FadeoutAreaTheme()
    {
        areaThemeInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
    }

    public void SwitchAreaTheme(string theme)
    {
        FadeoutAreaTheme();
        areaTheme = theme;
        PlayAreaTheme();
    }

    public void SetIntensity(float value)
    {
        areaThemeInstance.setParameterByName("Inensity", value);
    }

}
