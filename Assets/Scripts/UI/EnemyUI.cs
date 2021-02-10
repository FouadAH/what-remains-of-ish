using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class EnemyUI : MonoBehaviour
{
    [SerializeField] private Slider healthSlider;
    [SerializeField] private Slider healthSliderGhost;
    private float previousHealthPercent;

    private float MaxHealth = 1f;
    private float Health = 1f;
    [SerializeField] private Canvas canvas;
    private Transform enemy;
    public float offset =1f;

    void Awake()
    {
        previousHealthPercent = 1;
        enemy = GetComponent<IEnemy>().transform;
        GetComponent<IEnemy>().OnHitEnemy += OnHitEnemy;
        canvas.enabled = false;
    }

    private void OnHitEnemy(float health, float maxHealth)
    {
        if (canvas.enabled == false) canvas.enabled = true;
        Health = health;
        MaxHealth= maxHealth;
    }

    private void LateUpdate()
    {
        canvas.transform.position = new Vector3(enemy.position.x , enemy.position.y + offset);
        canvas.transform.rotation = Quaternion.identity;
    }
    void Update()
    {
        healthSlider.value = CalculateHealthPercent();
        healthSliderGhost.value = Mathf.MoveTowards(previousHealthPercent, CalculateHealthPercent(), (1f / 5f) * Time.deltaTime);
        previousHealthPercent = healthSliderGhost.value;
    }

    private float CalculateHealthPercent()
    {
        return Health / MaxHealth;
    }


}
