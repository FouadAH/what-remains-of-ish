using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class UI_HUD : MonoBehaviour
{
    public Slider healthSlider;
    public Slider healthSliderGhost;
    public Slider gunHeatSlider;
    public TMPro.TMP_Text currencyText;
    public static UI_HUD instance;
    private float previousHealthPercent;
    [SerializeField] private Animator anim;

    void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
        }
        previousHealthPercent = 1;
        
    }

    private void Start()
    {
        GameManager.instance.player.GetComponent<Player>().OnHit += OnHit;

    }

    void Update()
    {
        gunHeatSlider.value = CalculateGunHeatPercent();
        currencyText.SetText(GameManager.instance.currency.ToString());
        healthSlider.value = CalculateHealthPercent();
        healthSliderGhost.value = Mathf.MoveTowards(previousHealthPercent, CalculateHealthPercent(), (1f / 5f) * Time.deltaTime);
        previousHealthPercent = healthSliderGhost.value;
    }

    private float CalculateHealthPercent()
    {
        return GameManager.instance.health / GameManager.instance.maxHealth;
    }

    private float CalculateGunHeatPercent()
    {
        return GameManager.instance.gunHeat / GameManager.instance.maxGunHeat;
    }

    void OnHit()
    {
        anim.SetTrigger("Hit");
    }
}
