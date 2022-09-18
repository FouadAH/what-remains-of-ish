using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
/// <summary>
/// Class responsible for setting the virtual cameras' bounds based on the current levels bounds
/// </summary>
public class SceneController : MonoBehaviour
{
    public PolygonCollider2D colCameraBounds;
    CameraController cameraController;
    public Level level;
    public GameEvent sceneLoad;

    void Start()
    {
        cameraController = Camera.main.GetComponent<CameraController>();
        cameraController.virtualCamera.PreviousStateIsValid = false;
        cameraController.confiner.m_BoundingShape2D = colCameraBounds;
        cameraController.confiner.InvalidatePathCache();

        if(level != null)
        {
            GameManager.instance.currentLevel = level;
            sceneLoad.Raise();
        }

#if UNITY_EDITOR
        if(SceneManager.GetActiveScene().buildIndex != gameObject.scene.buildIndex)
        {
            Debug.LogError("Active scene not set to the current level. Please set active scene to level scene before pressing start.");
            Debug.Break();
            SceneManager.SetActiveScene(gameObject.scene);
        }
#endif
    }
    
}
