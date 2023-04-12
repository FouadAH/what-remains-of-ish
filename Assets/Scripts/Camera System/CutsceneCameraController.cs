using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutsceneCameraController : MonoBehaviour
{
    public Color StartColor;
    public Color EndColor;

    public void ChangeBackgroundColor()
    {
        Camera.main.GetComponent<CameraController>().RenderCamera.backgroundColor = EndColor;
    }
}
