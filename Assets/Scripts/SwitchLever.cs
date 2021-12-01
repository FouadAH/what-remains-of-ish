using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchLever : MonoBehaviour, IDamagable
{
    public bool isActive;
    Animator animator;

    public event Action OnTriggerLever = delegate { };

    public float Health { get => 0; set => throw new System.NotImplementedException(); }
    public int MaxHealth { get => 0; set => throw new System.NotImplementedException(); }
    public int knockbackGiven { get => 0; set => throw new System.NotImplementedException(); }

    int iFrames = 10;

    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void ModifyHealth(int amount)
    {
        SetActive(true);
        OnTriggerLever();
    }

    public void SetActive(bool activeState)
    {
        isActive = activeState;
        animator.SetBool("isOpen", activeState);
    }

    public void KnockbackOnDamage(int amount, float dirX, float dirY){}

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.GetComponent<Boomerang>() != null)
        {
            ModifyHealth(0);
        }
    }
}
