﻿using System;
using UnityEngine;

/// <summary>
/// Class responsible for handling player input. 
/// </summary>
public class Player_Input : MonoBehaviour
{
    public Vector2 directionalInput;

    public bool jumping { get; set; }
    public bool attacking { get; set; }
    public bool aiming { get; set; }

    [SerializeField] private float attackRate = 0.1f;
    private float nextAttackTime;
    public bool dashing { get; set; }
    public bool firing { get; set; }
    [SerializeField] private float fireRate = 0.1f;
    private float nextFireTime;

    public event Action OnFire = delegate{};
    public event Action OnAim = delegate { };

    public event Action OnAttack = delegate { };
    public event Action OnJumpUp = delegate { };
    public event Action OnJumpDown = delegate { };
    public event Action OnDash = delegate { };
    public event Action OnBoomerangDash = delegate { };

    public event Action OnDetach = delegate { };
    public event Action OnAttach = delegate { };


    public bool controllerConnected = false;


    private void Update()
    {
        string[] names = Input.GetJoystickNames();
        controllerConnected = false;
        foreach (string item in names)
        {
            if (!item.Equals(""))
            {
                controllerConnected = true;
            }
        }

        if (!aiming)
        {
            directionalInput = new Vector2(Mathf.Round(Input.GetAxisRaw("Horizontal")), Mathf.Round(Input.GetAxisRaw("Vertical")));
        }

        if (Input.GetButtonDown("Jump"))
        {
            OnJumpDown();
        }
        if(Input.GetButtonUp("Jump"))
        {
            OnJumpUp();
        }

        if (Input.GetButtonDown("Dash"))
        {
            OnDash();
        }

        if (Input.GetButtonDown("Interact"))
        {
            Debug.Log("Teleport input");
            OnBoomerangDash();
        }

        attacking = Input.GetButtonDown("Attack");
        if (attacking && CanAttack())
        {
            nextAttackTime = Time.time + attackRate;
            OnAttack();
        }
    }

    private bool CanAttack()
    {
        return Time.time >= nextAttackTime;
    }

}
