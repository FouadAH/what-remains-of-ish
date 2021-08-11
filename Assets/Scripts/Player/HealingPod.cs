using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealingPod : MonoBehaviour
{
    public float fillAmount = 0;
    [SerializeField] Image image;

    private void Update()
    {
        if(image.fillAmount != fillAmount)
            image.fillAmount = Mathf.Lerp(image.fillAmount, fillAmount/100, 0.1f);
    }

    public void InitFlask()
    {
        //image.fillAmount = fillAmount / 100;
    }

    public void Refill(float amount)
    {
        fillAmount += amount;
        //image.fillAmount = fillAmount/ 100;
    }

    public void EmptyFlask()
    {
        fillAmount = 0;
        //image.fillAmount = 0;
    }
    
}
