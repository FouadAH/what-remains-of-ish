using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class responsible for setting the virtual cameras' bounds based on the current levels bounds
/// </summary>
public class SceneController : MonoBehaviour
{
    public PolygonCollider2D colCameraBounds;
    CameraController cameraController;
    void Start()
    {
        cameraController = Camera.main.GetComponent<CameraController>();
        cameraController.virtualCamera.PreviousStateIsValid = false;
        cameraController.confiner.m_BoundingShape2D = colCameraBounds;
        cameraController.confiner.InvalidatePathCache();
    }
    
}
