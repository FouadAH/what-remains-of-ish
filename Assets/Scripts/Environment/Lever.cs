using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Lever : MonoBehaviour, IDamagable
{
    [SerializeField] private string m_ID = Guid.NewGuid().ToString();
    public string ID => m_ID;
    string key;

    [ContextMenu("Generate new ID")]
    private void RegenerateGUID() => m_ID = Guid.NewGuid().ToString();

    bool isOpen;
    public bool toggleable = false;
    
    Animator animator;
    public float Health { get => 0; set => throw new System.NotImplementedException(); }
    public int MaxHealth { get => 0; set => throw new System.NotImplementedException(); }
    public int knockbackGiven { get => 0; set => throw new System.NotImplementedException(); }

    public event Action<bool> OnToggle = delegate { };
    public UnityEvent OnToggleLever;

    void Start()
    {
        animator = GetComponent<Animator>();
        key = "Lever_" + ID;
        isOpen = PlayerPrefs.GetInt(key) == 1 ? true : false;
        animator.SetBool("isOpen", isOpen);
    }

    public void ModifyHealth(int amount)
    {
        if (isOpen && !toggleable)
            return;

        isOpen = !isOpen;
        animator.SetBool("isOpen", isOpen);
        PlayerPrefs.SetInt(key, 1);

        OnToggle(isOpen);
        OnToggleLever.Invoke();
        FMODUnity.RuntimeManager.PlayOneShot("event:/SFX/Interactive Objects/Lever", GetComponent<Transform>().position);
    }

    public void KnockbackOnDamage(int amount, float dirX, float dirY){}

}
