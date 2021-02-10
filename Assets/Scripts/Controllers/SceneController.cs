using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class responsible for setting the virtual cameras' bounds based on the current levels bounds
/// </summary>
public class SceneController : MonoBehaviour
{
    public PolygonCollider2D colCameraBounds;

    void Start()
    {
        GameManager.instance.cameraController.virtualCamera.PreviousStateIsValid = false;
        GameManager.instance.cameraController.confiner.m_BoundingShape2D = colCameraBounds;
        GameManager.instance.cameraController.confiner.InvalidatePathCache();
    }
    
}
