using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class UI_HUD : MonoBehaviour
{
    [SerializeField] private Animator anim;

    [Header("HUD UI elements")]
    public TMP_Text currencyText;

    public Transform heartBar;
    public GameObject heartPrefab;

    public GameObject healingPodPrefab;
    public Transform healingPodsBar;

    public Image TeleportCooldown;
    public Image ExplosionCooldown;
    public Image FreezeCooldown;

    [Header("Tips UI elements")]
    public GameObject tipsPanel;
    public TMP_Text tipsText;

    [Header("Dubug UI elements")]
    public GameObject debugTextPanel;
    public TMP_Text debugText;

    [Header("Data")]
    public PlayerDataSO playerData;

    float previousCurrency = 0;

    List<HealingPod> healingFlasks = new();

    private void Start()
    {
        InitHealingPods();
        RefreshHealthUI();
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
        UpdateCurrencyUI();
        UpdateFlaskUI();
        UpdateHealthBar();
    }

    void UpdateCurrencyUI()
    {
        currencyText.SetText(playerData.playerCurrency.Value.ToString());
    }

    void UpdateFlaskUI()
    {
        for (int i = 0; i < healingFlasks.Count; i++)
        {
            healingFlasks[i].fillAmount = playerData.playerHealingPodFillAmounts[i];
        }
    }

    void UpdateHealthBar()
    {
        if (heartBar.childCount != playerData.playerHealth.Value)
        {
            RefreshHealthUI();
        }
    }

    IEnumerator CurrencyCounter()
    {
        float currencyDiff = playerData.playerCurrency.Value - previousCurrency;

        while (currencyDiff > 0)
        {
            yield return new WaitForSeconds(0.05f);
            currencyDiff = playerData.playerCurrency.Value - previousCurrency;
            previousCurrency++;
        }

        previousCurrency = playerData.playerCurrency.Value;
        currencyText.SetText(playerData.playerCurrency.Value.ToString());

        yield return new WaitForEndOfFrame();
    }

    void RefreshHealthUI()
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

    public void OnHit(int amount)
    {
        anim.SetTrigger("OnHit");
    }

    public void OnHeal(int amount)
    {
        anim.SetTrigger("OnHeal");
    }

    public void OnRest()
    {
        SetDebugText("Player health and flasks restored. Checkpoint set.");
    }

    public void OnBuyItem()
    {
        SetDebugText("Bought an item");
    }

    public void OnAddHealingFlask()
    {
        SetDebugText("Healing pods increased by 1");
        InitHealingPods();
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
        debugText.SetText(text);
    }

    IEnumerator DebugTextRoutine(string text)
    {
        debugTextPanel.SetActive(true);
        debugText.SetText(text);
        yield return new WaitForSecondsRealtime(4f);
        debugText.SetText("");
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
