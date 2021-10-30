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

    public Transform heartBar;
    public GameObject heartPrefab;

    public GameObject healingPodPrefab;
    public Transform healingPodsBar;

    public List<HealingPod> healingFlasks = new List<HealingPod>();

    [Header("Dubug UI elements")]
    public bool debugMode;
    public TMP_Text velocityXDebug;
    public TMP_Text velocityYDebug;

    public GameObject debugTextPanel;
    public TMP_Text pickupDebugText;

    public GameObject debugTimer;
    public TMP_Text debugTimerText;


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

        RefrechHealth();
    }

    void Update()
    {
        currencyText.SetText(GameManager.instance.currency.ToString());
    }

    public void RefillFlask(float amount)
    {
        //float amountToFill = 0;
        //for (int i = 0; i < healingFlasks.Count; i++)
        //{
        //    if (i == 0)
        //    {
        //        amountToFill = amount;
        //    }

        //    if (healingFlasks[i].fillAmount != 100)
        //    {
        //        if (amountToFill + healingFlasks[i].fillAmount > 100)
        //        {
        //            amountToFill -= healingFlasks[i].fillAmount;
        //        }

        //        healingFlasks[i].Refill(amountToFill);
        //        Debug.Log(amountToFill);
        //    }
        //}

        foreach (HealingPod flask in healingFlasks)
        {
            if (flask.fillAmount < 100)
            {
                flask.Refill(amount);
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

        for (int i = 0; i < GameManager.instance.health; i++)
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
        for (int i = 0; i < amount; i++)
        {
            Instantiate(heartPrefab, heartBar);
        }

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
        yield return new WaitForSecondsRealtime(5f);
        pickupDebugText.SetText("");
        debugTextPanel.SetActive(false);
    }
}
