using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealingPod : MonoBehaviour
{
    public float fillAmount = 0;
    public Image image;

    private void Awake()
    {
        //image = GetComponentInChildren<Image>();
    }

    private void Update()
    {
        if(image.fillAmount != fillAmount)
            image.fillAmount = Mathf.Lerp(image.fillAmount, fillAmount/100, 0.1f);
    }

    public void Refill(float amount)
    {
        fillAmount += amount;
        GameManager.instance.UpdateHealingPodFillAmount();
    }

    public void EmptyFlask()
    {
        fillAmount = 0;
        GameManager.instance.UpdateHealingPodFillAmount();
    }

}
