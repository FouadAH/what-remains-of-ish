using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealingPod : MonoBehaviour
{
    public float fillAmount = 0;
    public Image image;
    public Color fullColor = Color.white;
    public Color startColor = Color.grey;

    private void Update()
    {
        if (image.fillAmount != fillAmount)
        {
            image.fillAmount = Mathf.Lerp(image.fillAmount, fillAmount / 100, 0.1f);
        }

        if (fillAmount >= 100)
        {
            image.color = fullColor;
        }
        else
        {
            image.color = startColor;
        }
    }

    public void Refill(float amount)
    {
        fillAmount += amount;
        UpdateHealingPodFillAmount();
    }

    public void EmptyFlask()
    {
        fillAmount = 0;
        UpdateHealingPodFillAmount();
    }

    void UpdateHealingPodFillAmount()
    {
        for (int i = 0; i < UI_HUD.instance.healingFlasks.Count; i++)
        {
            UI_HUD.instance.playerData.playerHealingPodFillAmounts[i] = (int)UI_HUD.instance.healingFlasks[i].fillAmount;
        }
    }
}
