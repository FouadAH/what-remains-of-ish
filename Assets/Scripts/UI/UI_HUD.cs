using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class UI_HUD : MonoBehaviour
{
    public TMPro.TMP_Text currencyText;
    public static UI_HUD instance;
    [SerializeField] private Animator anim;

    public Transform heartBar;
    public GameObject heartPrefab;

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
        GameManager.instance.player.GetComponent<Player>().OnHit += OnHit;
        RefrechHealth();
    }

    private void OnEnable()
    {
        GameManager.instance.player.GetComponent<Player>().OnHit += OnHit;
        RefrechHealth();
    }

    private void OnDisable()
    {
        GameManager.instance.player.GetComponent<Player>().OnHit -= OnHit;
    }

    void Update()
    {
        currencyText.SetText(GameManager.instance.currency.ToString());
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
        anim.SetTrigger("Hit");
        for (int i = 0; i < amount; i++)
        {
            if (heartBar.transform.childCount >0)
            {
                Transform heart = heartBar.transform.GetChild(heartBar.transform.childCount - 1);
                Destroy(heart.gameObject);
            }
        }
    }
}
