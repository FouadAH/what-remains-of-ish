using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchLever : MonoBehaviour, IHittable
{
    public bool isActive;
    Animator animator;

    public event Action OnTriggerLever = delegate { };

    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void ProcessHit(int amount, DamageType type)
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
            ProcessHit(0, DamageType.Melee);
        }
    }
}
