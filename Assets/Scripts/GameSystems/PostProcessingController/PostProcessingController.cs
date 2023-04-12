using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class PostProcessingController : MonoBehaviour
{
    VolumeProfile profile;
    Volume globalRenderingVolume;

    private void Awake()
    {
        globalRenderingVolume = GetComponent<Volume>();
    }
    public void SetLightIntensity()
    {
        profile = GameManager.instance.currentLevel.volumeProfile;
        globalRenderingVolume.profile = profile;
    }
}
