using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class EnemyUI : MonoBehaviour
{
    [SerializeField] private Slider healthSlider;
    [SerializeField] private Slider healthSliderGhost;

    [SerializeField] private Slider stunSlider;

    private float previousHealthPercent;

    private float MaxHealth = 1f;
    private float Health = 1f;
    private float StunResistance = 1f;
    private float CurrentStunResistance = 1f;

    [SerializeField] private Canvas canvas;
    public float offset =1f;

    Entity entity;
    void Awake()
    {
        entity = GetComponent<Entity>();
        previousHealthPercent = 1;
        GetComponent<Entity>().OnHitEnemy += OnHitEnemy;
    }

    private void OnHitEnemy(float health, float maxHealth, float currentStunResistance, float stunResistance)
    {
        if (canvas.enabled == false) 
            canvas.enabled = true;
        
        Health = health;
        MaxHealth = maxHealth;
        StunResistance = stunResistance;
        CurrentStunResistance = currentStunResistance;
    }

    void Update()
    {
        healthSlider.value = CalculateHealthPercent();
        stunSlider.value = CalculateStunPercent();

        healthSliderGhost.value = Mathf.MoveTowards(previousHealthPercent, CalculateHealthPercent(), 0.05f * Time.deltaTime);
        previousHealthPercent = healthSliderGhost.value;
    }

    private float CalculateHealthPercent()
    {
        return Health / MaxHealth;
    }

    private float CalculateStunPercent()
    {
        return (entity.entityData.stunResistance - entity.currentStunResistance) / entity.entityData.stunResistance;
    }

}
