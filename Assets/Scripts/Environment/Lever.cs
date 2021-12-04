using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Lever : Savable, IDamagable
{
    [System.Serializable]
    public struct Data
    {
        public bool isOpen;
    }

    [SerializeField]
    private Data leverData;

    public bool toggleable = false;
    
    Animator animator;
    public float Health { get => 0; set => throw new System.NotImplementedException(); }
    public int MaxHealth { get => 0; set => throw new System.NotImplementedException(); }
    public int knockbackGiven { get => 0; set => throw new System.NotImplementedException(); }

    public event Action<bool> OnToggle = delegate { };
    public UnityEvent OnToggleLever;

    public override void Awake()
    {
        base.Awake();
        animator = GetComponent<Animator>();
    }

    public void ModifyHealth(int amount)
    {
        if (leverData.isOpen && !toggleable)
            return;

        leverData.isOpen = !leverData.isOpen;
        animator.SetBool("isOpen", leverData.isOpen);

        OnToggle(leverData.isOpen);
        OnToggleLever.Invoke();
    }

    public void KnockbackOnDamage(int amount, float dirX, float dirY){}

    public override string SaveData()
    {
        return JsonUtility.ToJson(leverData);
    }

    public override void LoadDefaultData()
    {
        leverData.isOpen = false;
        animator.SetBool("isOpen", leverData.isOpen);
    }

    public override void LoadData(string data, string version)
    {
        leverData = JsonUtility.FromJson<Data>(data);
        animator.SetBool("isOpen", leverData.isOpen);
    }
}
