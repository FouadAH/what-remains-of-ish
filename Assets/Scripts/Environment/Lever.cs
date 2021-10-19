using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lever : MonoBehaviour, IDamagable
{
    [SerializeField] private string m_ID = Guid.NewGuid().ToString();
    public string ID => m_ID;
    string key;

    [ContextMenu("Generate new ID")]
    private void RegenerateGUID() => m_ID = Guid.NewGuid().ToString();

    bool isOpen;
    
    Animator animator;
    public Door door;
    public float Health { get => 0; set => throw new System.NotImplementedException(); }
    public int MaxHealth { get => 0; set => throw new System.NotImplementedException(); }
    public int knockbackGiven { get => 0; set => throw new System.NotImplementedException(); }

    void Start()
    {
        animator = GetComponent<Animator>();
        key = "Lever_" + ID;
        isOpen = PlayerPrefs.GetInt(key) == 1 ? true : false;
        animator.SetBool("isOpen", isOpen);
    }

    public void ModifyHealth(int amount)
    {
        if (isOpen)
            return;

        Debug.Log("Open/Close");
        isOpen = true;
        animator.SetBool("isOpen", isOpen);
        PlayerPrefs.SetInt(key, 1);
        door.SetState(isOpen);
    }

    public void KnockbackOnDamage(int amount, float dirX, float dirY){}

}
