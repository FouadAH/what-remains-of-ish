using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

/// <summary>
/// Class responsible for loading in the main menu
/// </summary>
public class GraphicsManager : MonoBehaviour
{
    public static GraphicsManager instance;

    public RenderPipelineAsset windowsURP;
    public RenderPipelineAsset macURP;

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

#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
            QualitySettings.renderPipeline = windowsURP;
#elif UNITY_STANDALONE_OSX || UNITY_EDITOR_OSX
            QualitySettings.renderPipeline = macURP;
#endif
        }

        DontDestroyOnLoad(this);
    }
}
