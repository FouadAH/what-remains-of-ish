﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Cinemachine;

[RequireComponent(typeof(Controller_2D))]

public class Player : MonoBehaviour, IAttacker{

    [HideInInspector] public Vector3 velocity;
    public LayerMask enemyMask;

    float iFrames = 0f;
    public float iFrameTime = 1f;
    bool invinsible = false;
    
    Animator anim;
    GameManager gm;
    [SerializeField] private PlayerMovementSettings playerSettings;

    public PlayerMovement playerMovement { get; private set; }
    Player_Input playerInput;

    private bool staggered;

    [Header("Player Values")]

    [SerializeField] private int minMeleeDamage;
    [SerializeField] private int maxMeleeDamage;
    [SerializeField] private float meleeAttackMod;

    [SerializeField] private int hitKnockbackAmount;
    [SerializeField] private int damageKnockbackAmount;

    public float knockbackOnDamageTimer = 0.4f;
    public float knockbackOnHitTimer = 0.25f;

    public int MeleeDamage { get => minMeleeDamage; set => minMeleeDamage = value; }
    public int MaxMeleeDamage { get => maxMeleeDamage; set => maxMeleeDamage = value; }
    public float MeleeAttackMod { get => meleeAttackMod; set => meleeAttackMod = value; }

    public int MaxHealth { get; set; }
    public float Health { get; set; }

    public int HitKnockbackAmount { get => hitKnockbackAmount; set => hitKnockbackAmount = value; }
    public int knockbackGiven { get => damageKnockbackAmount; set => damageKnockbackAmount= value; }

    public event Action<int> OnHit = delegate { };
    public event Action<int> OnHeal = delegate { };

    public BoomerangLauncher boomerangLauncher;

    public float cameraLookOffsetValue = 9f;
    CinemachineCameraOffset cameraOffset;
    float cameraOffsetTarget = 0f;
    Vector2 lookVelocityTreshhold = new Vector2(0.2f, 0.2f);

    [Header("Effects")]
    public ParticleSystem dustParticles;
    public ParticleSystem damageParticle;
    public ParticleSystem healingParticles;

    TimeStop timeStop;
    ColouredFlash flashEffect;

    [Header("Damage Time Stop")]
    public float changeTime = 0.05f;
    public float restoreSpeed = 10f;
    public float delay = 0.1f;

    [Header("Debug Settings")]
    public bool playerDebugMode;
    public TrailRenderer playerPath;

    void Awake()
    {
        if(GameManager.instance == null)
        {
            Debug.LogWarning("GameManager not loaded.");
            return;
        }

        GameManager.instance.player = gameObject;
        GameManager.instance.playerCurrentPosition = GetComponentInChildren<SpriteRenderer>().transform;
        GameManager.instance.playerCamera = Camera.main;
        GameManager.instance.cameraController = Camera.main.GetComponent<CameraController>();
        GameManager.instance.cameraController.virtualCamera.Follow = transform;
        gm = GameManager.instance;

        boomerangLauncher = GetComponentInChildren<BoomerangLauncher>();
        AudioManager.instance.PlayAreaTheme();
    }

    void Start()
    {
        //if (GameManager.instance == null)
        //{
        //    Debug.LogWarning("GameManager not loaded.");
        //}

        //if (gm == null)
        //{
        //    GameManager.instance.player = gameObject;
        //    GameManager.instance.playerCurrentPosition = GetComponentInChildren<SpriteRenderer>().transform;
        //    GameManager.instance.playerCamera = Camera.main;
        //    GameManager.instance.cameraController = Camera.main.GetComponent<CameraController>();
        //    GameManager.instance.cameraController.virtualCamera.Follow = transform;
        //    gm = GameManager.instance;
        //    AudioManager.instance.PlayAreaTheme();
        //}

        UI_HUD.instance.enabled = true;

        boomerangLauncher = GetComponentInChildren<BoomerangLauncher>();
        anim = GetComponent<Animator>();
        timeStop = GetComponent<TimeStop>();
        playerMovement =  GetComponent<PlayerMovement>();
        playerInput = GetComponent<Player_Input>();
        playerInput.OnHeal += Heal;
        flashEffect = GetComponentInChildren<ColouredFlash>();
        cameraOffset = gm.cameraController.virtualCamera.GetComponent<CinemachineCameraOffset>();

        if (playerDebugMode)
        {
            playerPath.emitting = true;
        }

#if UNITY_EDITOR
        //if (playerDebugMode)
        //{
        //    gm.lastCheckpointPos = transform.position;
        //    gm.lastCheckpointLevelPath = SceneManager.GetActiveScene().path;
        //    gm.lastCheckpointLevelIndex = SceneManager.GetActiveScene().buildIndex;

        //    gm.lastSavepointPos = transform.position;
        //    gm.lastSavepointLevelPath = SceneManager.GetActiveScene().path;
        //    gm.lastSavepointLevelIndex = SceneManager.GetActiveScene().buildIndex;
        //}
#endif

    }

    private void Update()
    {
        if (GameManager.instance.isLoading)
            return;

        OnDamage();
        Look();
    }

    public void EmitRunParticle()
    {
        dustParticles.Play();
    }

    /// <summary>
    /// Method that handles iframe logic when the player takes damage
    /// </summary>
    public void OnDamage()
    {
        if (invinsible)
        {
            if (iFrames > 0)
            {
                iFrames -= Time.deltaTime;

            }
            else
            {
                invinsible = false;
            }
        }
    }

    // not used 
    public void Knockback(Vector3 dir, Vector2 kockbackDistance)
    {
        velocity = Vector3.zero;
        velocity.x += dir.x * kockbackDistance.x;
        velocity.y += dir.y * kockbackDistance.y;
    }

    /// <summary>
    /// Helper method that check if the player is dead or not 
    /// </summary>
    private void CheckDeath()
    {
        if (gm.health <= 0)
        {
            FMODUnity.RuntimeManager.PlayOneShot("event:/SFX/Player/Player Death", GetComponent<Transform>().position);
            StartCoroutine(PlayerDeath());
        }
        else if(gm.health <=2)
        {
            AudioManager.instance.SetHealthParameter(20f);
        }
    }

    /// <summary>
    /// Coroutine that runs when the player dies, handles death animation and respawning
    /// </summary>
    /// <returns></returns>
    private IEnumerator PlayerDeath()
    {
        playerMovement.isDead = true;
        UI_HUD.instance.enabled = false;
        playerInput.enabled = false;
        
        anim.SetLayerWeight(0, 0f);
        anim.SetLayerWeight(1, 0f);
        anim.SetLayerWeight(2, 0f);
        anim.SetLayerWeight(3, 1f);
        anim.SetBool("isDead", true);

        AudioManager.instance.StopAreaThemeWithFade();
        AudioManager.instance.StopAreaAmbianceWithFade();
        AudioManager.instance.StopSFXWithFade();

        yield return new WaitForSeconds(2f);
        
        AudioManager.instance.PlayAreaTheme();
        GameManager.instance.Respawn();

        anim.SetBool("isDead", false);
        anim.SetLayerWeight(0, 1f);
        anim.SetLayerWeight(1, 1f);
        anim.SetLayerWeight(2, 1f);
        anim.SetLayerWeight(3, 0f);

        gm.health = gm.maxHealth;
        UI_HUD.instance.enabled = true;
        UI_HUD.instance.RefrechHealth();

        playerInput.enabled = true;
        playerMovement.isDead = false;
    }
    
    /// <summary>
    /// Method that respawns the player at the last checkpoint
    /// </summary>
    public void Respawn()
    {
        GameManager.instance.Respawn();
    }

    public void Heal(int amount)
    {
        HealingPod flask = UI_HUD.instance.healingFlasks[0];

        if (flask.fillAmount >= 100 && gm.health < gm.maxHealth)
        {
            flask.EmptyFlask();
            float previousHP = gm.health;
            gm.health = Mathf.Clamp(gm.health + amount, 0, gm.maxHealth);

            int amountHealed = (int)(gm.health - previousHP);
            OnHeal(amountHealed);
            flashEffect.Flash(Color.white);
            healingParticles.Play();
            FMODUnity.RuntimeManager.PlayOneShot("event:/SFX/Player/Healing", GetComponent<Transform>().position);
        }

        if (gm.health > 2)
        {
            AudioManager.instance.SetHealthParameter(100f);
        }
    }

    Coroutine flashRoutine;
    /// <summary>
    /// Method reponsible for damaging the player
    /// </summary>
    /// <param name="amount"></param>
    public void ProcessHit(int amount)
    {
        if (!invinsible && gm.health > 0 && !gm.isRespawning)
        {
            CinemachineImpulseSource impulseListener = GetComponent<CinemachineImpulseSource>();
            impulseListener.GenerateImpulse();
            timeStop.StopTime(changeTime, restoreSpeed, delay);
            damageParticle.Play();
            if (!GameManager.instance.hasInfiniteLives)
            {
                gm.health -= amount;
                OnHit(amount);
            }

            CheckDeath();

            iFrames = iFrameTime;

            if (flashRoutine != null)
            {
                StopCoroutine(flashRoutine);
            }

            flashRoutine = StartCoroutine(flashEffect.FlashMultiple(Color.white, iFrameTime));
            invinsible = true;
            FMODUnity.RuntimeManager.PlayOneShot("event:/SFX/Player/Player Damage", GetComponent<Transform>().position);
        }
    }

    void Look()
    {
        if(Mathf.Abs(playerMovement.Velocity.x) > lookVelocityTreshhold.x && Mathf.Abs(playerMovement.Velocity.y) > lookVelocityTreshhold.y)
        {
            cameraOffsetTarget = 0f;
        }
        else
        {
            if (playerInput.rightStickInput.y <= -0.5)
            {
                cameraOffsetTarget = -cameraLookOffsetValue;
            }
            else if (playerInput.rightStickInput.y >= 0.5)
            {
                cameraOffsetTarget = cameraLookOffsetValue;
            }
            else
            {
                cameraOffsetTarget = 0;
            }
        }

        cameraOffset.m_Offset.y = Mathf.Lerp(cameraOffset.m_Offset.y, cameraOffsetTarget, 0.1f);
    }

    //IEnumerator DisableInputTemp(float disableTime)
    //{
    //    playerInput.enabled = false;
    //    yield return new WaitForSecondsRealtime(disableTime);
    //    playerInput.enabled = true;
    //}

    public void KnockbackOnHit(int amount, float dirX, float dirY)
    {
        playerMovement.dirKnockback = new Vector3(dirX, dirY, 1);
        //playerMovement.knockbackDistance = amount;
        playerMovement.Knockback(playerMovement.dirKnockback, playerMovement.knockbackDistance);

        //StopCoroutine(KnockbackOnHitRoutine());
        //StartCoroutine(KnockbackOnHitRoutine());
    }

    public void KnockbackOnDamage(int amount, float dirX, float dirY)
    {
        if (invinsible)
            return;
        
        playerMovement.dirKnockback = new Vector3(dirX, dirY, 1);
        playerMovement.knockbackDistance = amount;
        //playerMovement.Knockback(playerMovement.dirKnockback, playerMovement.knockbackDistance);
        StopCoroutine(KnockbackOnDamageRoutine());
        StartCoroutine(KnockbackOnDamageRoutine());
    }

    IEnumerator KnockbackOnDamageRoutine()
    {
        playerMovement.isKnockedback = true;
        yield return new WaitForSeconds(knockbackOnDamageTimer);
        playerMovement.isKnockedback = false;
    }
    IEnumerator KnockbackOnHitRoutine()
    {
        playerMovement.isKnockedback = true;
        yield return new WaitForSeconds(knockbackOnHitTimer);
        playerMovement.isKnockedback = false;
    }

    private void OnParticleCollision(GameObject other)
    {
        Debug.Log("Particle collision");
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Collider2D collider = GetComponent<Collider2D>();
        Vector3 colliderCenter = collider.bounds.center;
        Vector3 colliderExtents = collider.bounds.extents;

        Vector3 startPos = new Vector3(colliderCenter.x, colliderCenter.y - colliderExtents.y, colliderCenter.z);
        Gizmos.DrawWireSphere(startPos, playerSettings.MaxJumpHeight);

        Gizmos.color = Color.red;
        startPos = new Vector3(colliderCenter.x, colliderCenter.y + colliderExtents.y, colliderCenter.z);
        Gizmos.DrawLine(startPos, new Vector3(startPos.x, startPos.y + playerSettings.MaxJumpHeight, startPos.z));

        Gizmos.color = Color.yellow;

        startPos = new Vector3(colliderCenter.x + colliderExtents.x, colliderCenter.y - colliderExtents.y, colliderCenter.z);
        Gizmos.DrawLine(startPos, new Vector3(startPos.x + playerSettings.MoveSpeed, startPos.y, startPos.z));

        startPos = new Vector3(colliderCenter.x - colliderExtents.x, colliderCenter.y - colliderExtents.y, colliderCenter.z);
        Gizmos.DrawLine(startPos, new Vector3(startPos.x - playerSettings.MoveSpeed, startPos.y, startPos.z));
    }
}
