using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightController : MonoBehaviour
{
    public float intensity;
    UnityEngine.Rendering.Universal.Light2D lightComponent;

    private void Awake()
    {
        lightComponent = GetComponent<UnityEngine.Rendering.Universal.Light2D>();
    }
    public void SetLightIntensity()
    {
        intensity = GameManager.instance.currentLevel.globalLightIntensity;
        lightComponent.intensity = intensity;
    }
}
