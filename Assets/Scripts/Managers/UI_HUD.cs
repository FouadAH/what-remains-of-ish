using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class UI_HUD : MonoBehaviour
{
    public TMPro.TMP_Text currencyText;
    public static UI_HUD instance;
    [SerializeField] private Animator anim;

    [Header("HUD UI elements")]

    public Transform heartBar;
    public GameObject heartPrefab;

    public GameObject healingPodPrefab;
    public Transform healingPodsBar;

    public List<HealingPod> healingFlasks = new List<HealingPod>();

    public Image TeleportCooldown;
    public Image ExplosionCooldown;
    public Image FreezeCooldown;

    [Header("Brooch Inventory")]
    public InventoryGrid broochInventoryGrid;
    public InventoryGrid broochEquipGrid;

    [Header("Tips UI elements")]

    public GameObject tipsPanel;
    public TMP_Text tipsText;


    [Header("Dubug UI elements")]
    public bool debugMode;
    public TMP_Text velocityXDebug;
    public TMP_Text velocityYDebug;

    public GameObject debugTextPanel;
    public TMP_Text pickupDebugText;

    public GameObject debugTimer;
    public TMP_Text debugTimerText;

    [Header("Variables")]
    public PlayerDataSO playerData;

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
    }

    private void Start()
    {
        velocityXDebug.gameObject.SetActive(debugMode);
        velocityYDebug.gameObject.SetActive(debugMode);
    }

    private void OnEnable()
    {
        GameManager.instance.player.GetComponent<Player>().OnHit += OnHit;
        GameManager.instance.player.GetComponent<Player>().OnHeal += OnHeal;
        InitHealingPods();
        RefrechHealth();
    }

    public void InitHealingPods()
    {
        healingFlasks.Clear();
        for (int i = 0; i < healingPodsBar.childCount; i++)
        {
            Destroy(healingPodsBar.GetChild(i).gameObject);
            
        }

        foreach (int fillAmount in playerData.playerHealingPodFillAmounts)
        {
            GameObject healingPodObject = Instantiate(healingPodPrefab, healingPodsBar);
            HealingPod healingPod = healingPodObject.GetComponent<HealingPod>();
            healingPod.fillAmount = fillAmount;
            healingFlasks.Add(healingPod);
        }
    }

    void Update()
    {
        currencyText.SetText(playerData.playerCurrency.Value.ToString());
    }

    float refillMod;
    float refillModNormal = 1f;
    float refillModExtra = 1.5f;

    public void RefillFlask(float amount)
    {
        foreach (HealingPod flask in healingFlasks)
        {
            if (flask.fillAmount < 100)
            {
                refillMod = (GameManager.instance.equippedBrooch_03) ? refillModExtra : refillModNormal;
                //Debug.Log("Reffiling flask: " + amount * refillMod);
                flask.Refill(amount * refillMod);
                break;
            }
        }
    }

    public void RefrechHealth()
    {
        for (int i = 0; i < heartBar.childCount; i++)
        {
            Destroy(heartBar.GetChild(i).gameObject);
        }

        for (int i = 0; i < playerData.playerHealth.Value; i++)
        {
            Instantiate(heartPrefab, heartBar);
        }
    }

    void OnHit(int amount)
    {
        anim.SetTrigger("OnHit");
        for (int i = 0; i < amount; i++)
        {
            if (heartBar.transform.childCount >0)
            {
                Transform heart = heartBar.transform.GetChild(heartBar.transform.childCount - 1);
                Destroy(heart.gameObject);
            }
        }
    }
    void OnHeal(int amount)
    {
        anim.SetTrigger("OnHeal");

        //Adding hearts to health bar 
        RefrechHealth();

        //Empty the first Flask
        healingFlasks[0].EmptyFlask();

        //Trickel down health
        for (int i = 1; i < healingFlasks.Count; i++)
        {
            if(healingFlasks[i].fillAmount > 0)
            {
                float newFillAmount = healingFlasks[i].fillAmount;
                healingFlasks[i-1].fillAmount = newFillAmount;
                healingFlasks[i].fillAmount = 0;
            }
        }

        for (int i = 0; i < healingFlasks.Count; i++)
        {
            playerData.playerHealingPodFillAmounts[i] = (int)healingFlasks[i].fillAmount;
        }
    }

    public void OnResetHP(float missingHealth)
    {
        anim.SetTrigger("OnHeal");

        //Adding hearts to health bar 
        for (int i = 0; i < missingHealth; i++)
        {
            Instantiate(heartPrefab, heartBar);
        }

       //Refill healing
        for (int i = 0; i < healingFlasks.Count; i++)
        {
            healingFlasks[i].fillAmount = 100;
        }

    }

    public void OnResetHealingFlasks()
    {
        //Refill healing
        for (int i = 0; i < healingFlasks.Count; i++)
        {
            healingFlasks[i].fillAmount = 100;
        }
    }

    public void Cooldown(PlayerAbility playerAbility, float cooldownDuration)
    {
        switch (playerAbility)
        {
            case PlayerAbility.BoomerangTeleport:
                StartCoroutine(CooldownRoutine(TeleportCooldown, cooldownDuration));
                break;
            case PlayerAbility.BoomerangExplosion:
                StartCoroutine(CooldownRoutine(ExplosionCooldown, cooldownDuration));
                break;
            case PlayerAbility.BoomerangFreeze:
                StartCoroutine(CooldownRoutine(FreezeCooldown, cooldownDuration));
                break;
            default:
                break;
        }
    }

    IEnumerator CooldownRoutine(Image image, float cooldownDuration)
    {
        image.fillAmount = 0;
        float cooldownTime = 0;
        while(cooldownDuration > cooldownTime)
        {
            cooldownTime += Time.deltaTime;
            image.fillAmount = (cooldownTime / cooldownDuration);
            yield return null;
        }
    }

    public void SetDebugText(string text)
    {
        StopCoroutine(DebugTextRoutine(text));
        StartCoroutine(DebugTextRoutine(text));
    }

    public void SetDebugTimerText(string text)
    {
        debugTextPanel.SetActive(true);
        pickupDebugText.SetText(text);
    }

    IEnumerator DebugTextRoutine(string text)
    {
        debugTextPanel.SetActive(true);
        pickupDebugText.SetText(text);
        yield return new WaitForSecondsRealtime(4f);
        pickupDebugText.SetText("");
        debugTextPanel.SetActive(false);
    }

    public void SetTipsText(string text)
    {
        StopCoroutine(TipsTextRoutine(text));
        StartCoroutine(TipsTextRoutine(text));
    }

    IEnumerator TipsTextRoutine(string text)
    {
        tipsPanel.SetActive(true);
        tipsText.SetText(text);
        yield return new WaitForSecondsRealtime(8f);
        tipsText.SetText("");
        tipsPanel.SetActive(false);
    }
}
