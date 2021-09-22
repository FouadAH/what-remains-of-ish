using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lever : MonoBehaviour, IDamagable
{
    bool isOpen;
    Animator animator;

    public Door door;

    public event Action OnTriggerLever = delegate { };
    
    public float Health { get => 0; set => throw new System.NotImplementedException(); }
    public int MaxHealth { get => 0; set => throw new System.NotImplementedException(); }
    public int knockbackGiven { get => 0; set => throw new System.NotImplementedException(); }

    int iFrames = 10;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void ModifyHealth(int amount)
    {
        Debug.Log("Open/Close");
        isOpen = !isOpen;
        animator.SetBool("isOpen", isOpen);

        door.SetState(isOpen);
    }

    public void KnockbackOnDamage(int amount, float dirX, float dirY)
    {
        
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.GetComponent<Boomerang>() != null)
        {
            ModifyHealth(0);
        }
    }
}
