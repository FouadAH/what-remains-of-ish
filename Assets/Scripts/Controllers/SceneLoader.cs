using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

/// <summary>
/// Class responsible for loading in the main menu
/// </summary>
public class SceneLoader : MonoBehaviour
{
    public static SceneLoader instance;
    public bool loadDemo = false;

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

            if(loadDemo)
                SceneManager.LoadSceneAsync("DemoMainMenu", LoadSceneMode.Additive);
            else
                SceneManager.LoadSceneAsync("MainMenu", LoadSceneMode.Additive);
        }
    }
}
