using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Cinemachine;
using UnityEngine.Rendering;

using UnityEngine.Rendering.Universal;

[RequireComponent(typeof(Controller_2D))]

public class Player : MonoBehaviour, IAttacker {

    [HideInInspector] public Vector3 velocity;
    [HideInInspector] public float iFrames { get; private set; }

    public float damageIFrameTime = 1f;
    public bool invinsible = false;
    
    public PlayerDataSO playerData;
    public PlayerRuntimeDataSO PlayerRuntimeDataSO;

    public PlayerMovement playerMovement { get; private set; }

    [Header("Player Values")]

    [SerializeField] private int minMeleeDamage;
    [SerializeField] private int maxMeleeDamage;
    [SerializeField] private float meleeAttackMod;

    [SerializeField] private int hitKnockbackAmount;
    [SerializeField] private int damageKnockbackAmount;

    public float knockbackOnDamageTimer = 0.4f;
    public float knockbackOnHitTimer = 0.25f;

    public int MeleeDamage 
    { 
        get 
        {
            float damageModifier = 1;
            if (attackUp_LowHP.isEquipped)
            {
                if(playerData.playerHealth.Value <= 2)
                {
                    damageModifier = 1.5f;
                }
            }

            if (attackUp_MaxHP.isEquipped)
            {
                if(playerData.playerHealth.Value == playerData.playerMaxHealth.Value)
                {
                    damageModifier = 1.5f;
                }
            }

            if (damageBuffBrooche.isEquipped)
            {
                damageModifier += 0.5f;
            }

            return Mathf.CeilToInt(minMeleeDamage * damageModifier); 
        } 

        set => minMeleeDamage = value; 
    }

    public int MaxMeleeDamage { get => maxMeleeDamage; set => maxMeleeDamage = value; }
    public float MeleeAttackMod { get => meleeAttackMod; set => meleeAttackMod = value; }

    public int MaxHealth { get; set; }
    public float Health { get; set; }

    public int HitKnockbackAmount { get => hitKnockbackAmount; set => hitKnockbackAmount = value; }
    public int knockbackGiven { get => damageKnockbackAmount; set => damageKnockbackAmount= value; }

    public BoomerangLauncher boomerangLauncher;

    public float cameraLookOffsetValue = 9f;
    CinemachineCameraOffset cameraOffset;
    float cameraOffsetTarget = 0f;
    Vector2 lookVelocityTreshhold = new Vector2(0.2f, 0.2f);

    [Header("Effects")]
    public ParticleSystem dustParticles;
    public ParticleSystem damageParticle;
    public ParticleSystem healingParticles;
    public ParticleSystem knockbackParticles;

    [Header("Damage Time Stop")]
    public float changeTime = 0.05f;
    public float restoreSpeed = 10f;
    public float delay = 0.1f;

    [Header("Player SFX")]
    public int damageThreshold = 2;
    [FMODUnity.EventRef] public string playerBreathingSFX;
    public FMOD.Studio.EventInstance playerBreathingInstance;

    [Header("Player VFX")]
    private Volume volume;
    private Vignette vignette;
    public float lowHealthIntensity;
    float initalIntensity;

    [Header("Player Brooches")]
    public InventoryItemSO healingBrooche;
    public InventoryItemSO attackUp_LowHP;
    public InventoryItemSO attackUp_MaxHP;
    public InventoryItemSO iFrameUp;
    public InventoryItemSO flaskRefillOnDamageBrooche;
    public InventoryItemSO damageBuffBrooche;
    public InventoryItemSO kockbackDownBrooche;
    public InventoryItemSO coinDropRateUPBrooche;
    public InventoryItemSO stunTimeUpBrooche;

    [Header("Player Events")]
    public GameEvent PlayerDeathEvent;

    public IntegerGameEvent RefillEvent;

    public IntegerGameEvent PlayerHitEvent;
    public IntegerGameEvent PlayerHealEvent;

    [Header("Debug Settings")]
    public bool playerDebugMode;
    public bool hasInfiniteLives;

    public TrailRenderer playerPath;

    CameraController cameraController;
    TimeStop timeStop;
    ColouredFlash flashEffect;
    Animator anim;
    GameManager gm;
    Player_Input playerInput;

    void Awake()
    {
        if(GameManager.instance == null)
        {
            Debug.LogWarning("GameManager not loaded.");
            return;
        }

        GameManager.instance.player = gameObject;
        cameraController = Camera.main.GetComponent<CameraController>();
        cameraController.virtualCamera.Follow = transform;
        gm = GameManager.instance;

        boomerangLauncher = GetComponentInChildren<BoomerangLauncher>();
        AudioManager.instance.PlayAreaTheme();
    }

    void Start()
    {
        boomerangLauncher = GetComponentInChildren<BoomerangLauncher>();
        anim = GetComponent<Animator>();
        timeStop = TimeStop.instance;
        playerMovement =  GetComponent<PlayerMovement>();

        playerInput = GetComponent<Player_Input>();
        playerInput.OnHeal += Heal;
        playerInput.OnDebug += PlayerInput_OnDebug;

        flashEffect = GetComponentInChildren<ColouredFlash>();
        cameraController = Camera.main.GetComponent<CameraController>();
        cameraOffset = cameraController.virtualCamera.GetComponent<CinemachineCameraOffset>();

        volume = FindObjectOfType<Volume>();
        volume.profile.TryGet(out vignette);

        initalIntensity = vignette.intensity.value;

        if (GameManager.instance.isInDebugMode)
        {
            playerPath.emitting = true;
        }

#if UNITY_EDITOR
        if (playerDebugMode)
        {
            playerData.lastCheckpointPos.X = transform.position.x;
            playerData.lastCheckpointPos.Y = transform.position.y;

            playerData.lastCheckpointLevelPath = SceneManager.GetActiveScene().path;
            playerData.lastCheckpointLevelIndex.Value = SceneManager.GetActiveScene().buildIndex;

            playerData.lastSavepointPos.X = transform.position.x;
            playerData.lastSavepointPos.Y = transform.position.y;

            playerData.lastSavepointLevelPath = SceneManager.GetActiveScene().path;
            playerData.lastSavepointLevelIndex.Value = SceneManager.GetActiveScene().buildIndex;
        }
#endif

    }

    private void Update()
    {
        if (GameManager.instance.isLoading)
            return;

        Look();

#if UNITY_EDITOR
        if (playerDebugMode)
        {
            if (Input.GetKey(KeyCode.X))
            {
                Vector2 targetPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                transform.position = targetPos;
            }
        }
#endif

    }

    public void EmitRunParticle()
    {
        dustParticles.Play();
    }

    public Coroutine iFrameRoutine;
    public IEnumerator DamageIFrames(float iFrameTime)
    {
        invinsible = true;
        iFrames = iFrameTime;

        if (iFrameUp.isEquipped)
        {
            iFrames += 4;
        }

        while (iFrames > 0)
        {
            iFrames -= Time.deltaTime;
            yield return null;
        }
        invinsible = false;
        iFrameRoutine = null;
    }


    // not used 
    public void Knockback(Vector3 dir, Vector2 kockbackDistance)
    {
        velocity = Vector3.zero;
        velocity.x += dir.x * kockbackDistance.x;
        velocity.y += dir.y * kockbackDistance.y;
    }

    FMOD.Studio.PLAYBACK_STATE PLAYBACK_STATE;
    Coroutine lowHealthRoutine;
    /// <summary>
    /// Helper method that check if the player is dead or not 
    /// </summary>
    private void CheckDeath()
    {
        if (playerData.playerHealth.Value <= 0)
        {
            FMODUnity.RuntimeManager.PlayOneShot("event:/SFX/Player/Player Death", GetComponent<Transform>().position);
            playerBreathingInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            StartCoroutine(PlayerDeath());
        }
        else if(playerData.playerHealth.Value <= 1)
        {
            lowHealthRoutine = StartCoroutine(LowHealthVig());
            playerBreathingInstance.getPlaybackState(out PLAYBACK_STATE);

            if (PLAYBACK_STATE != FMOD.Studio.PLAYBACK_STATE.PLAYING)
            {
                playerBreathingInstance = FMODUnity.RuntimeManager.CreateInstance(playerBreathingSFX);
                playerBreathingInstance.start();
                playerBreathingInstance.setParameterByName("Health", 40);
            }
        }
        else if (playerData.playerHealth.Value <= 2)
        {
            AudioManager.instance.SetHealthParameter(20f);
        }
    }

    IEnumerator LowHealthVig()
    {
        while(vignette.intensity.value < lowHealthIntensity)
        {
            vignette.intensity.value = Mathf.Lerp(vignette.intensity.value, lowHealthIntensity, 0.1f);
            yield return null;
        }
        yield return null;
    }

    /// <summary>
    /// Coroutine that runs when the player dies, handles death animation and respawning
    /// </summary>
    /// <returns></returns>
    private IEnumerator PlayerDeath()
    {
        PlayerDeathEvent.Raise();

        playerMovement.isDead = true;
        playerInput.DisablePlayerInput();
        
        anim.SetLayerWeight(1, 0f);
        anim.SetLayerWeight(2, 0f);
        anim.SetLayerWeight(3, 0f);
        anim.SetBool("isDead", true);

        AudioManager.instance.StopAreaThemeWithFade();
        AudioManager.instance.StopAreaAmbianceWithFade();
        AudioManager.instance.StopSFXWithFade();

        if(boomerangLauncher.boomerangReference != null)
        {
            boomerangLauncher.boomerangReference.DestroyBoomerang();
        }

        yield return new WaitForSeconds(2f);

        if (lowHealthRoutine != null)
        {
            StopCoroutine(lowHealthRoutine);
            vignette.intensity.value = initalIntensity;
        }

        AudioManager.instance.PlayAreaTheme();
        GameManager.instance.Respawn();

        anim.SetBool("isDead", false);
        anim.SetLayerWeight(1, 1f);
        anim.SetLayerWeight(2, 1f);
        anim.SetLayerWeight(3, 1f);

        playerData.playerHealth.Value = playerData.playerMaxHealth.Value;

        playerInput.EnablePlayerInput();
        playerMovement.isDead = false;
    }
    
    /// <summary>
    /// Method that respawns the player at the last checkpoint
    /// </summary>
    public void Respawn()
    {
        GameManager.instance.Respawn();
    }

    public void Heal()
    {
        if (playerData.playerHealingPodAmount.Value > 0)
        {
            int healingAmount = playerData.playerHealingAmountPerPod.Value;
            int tempHealAmount = (healingBrooche.isEquipped) ? healingAmount + 1 : healingAmount;

            if (playerData.playerHealingPodFillAmounts[0] >= 100 && playerData.playerHealth.Value < playerData.playerMaxHealth.Value)
            {
                RestoreHP(tempHealAmount);
            }
        }

    }

    public void RestoreHP(int amount)
    {
        float previousHP = playerData.playerHealth.Value;
        playerData.playerHealth.Value = Mathf.Clamp(playerData.playerHealth.Value + amount, 0, playerData.playerMaxHealth.Value);

        int amountHealed = (int)(playerData.playerHealth.Value - previousHP);

        //OnHeal(amountHealed);
        PlayerHealEvent.Raise(amountHealed);

        flashEffect.Flash(Color.white);
        healingParticles.Play();
        FMODUnity.RuntimeManager.PlayOneShot("event:/SFX/Player/Healing", GetComponent<Transform>().position);

        if (playerData.playerHealth.Value > 2)
        {
            if (lowHealthRoutine != null)
            {
                StopCoroutine(lowHealthRoutine);
            }
            vignette.intensity.value = initalIntensity;

            AudioManager.instance.SetHealthParameter(100f);
            playerBreathingInstance.getPlaybackState(out PLAYBACK_STATE);

            if (PLAYBACK_STATE == FMOD.Studio.PLAYBACK_STATE.PLAYING)
            {
                playerBreathingInstance.release();
                playerBreathingInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            }
        }
    }

    Coroutine flashRoutine;
    /// <summary>
    /// Method reponsible for damaging the player
    /// </summary>
    /// <param name="amount"></param>
    /// <param name="type"></param>
    public void ProcessHit(int amount, DamageType type)
    {
        if (!invinsible && playerData.playerHealth.Value > 0 && !gm.isRespawning)
        {
            TakeDamage(amount);

            if(flaskRefillOnDamageBrooche.isEquipped)
            {
                RefillEvent.Raise(30);
            }
        }
    }

    public void ProcessForcedHit(int amount)
    {
        if (playerData.playerHealth.Value > 0 && !gm.isRespawning)
        {
            TakeDamage(amount);
        }
    }

    void TakeDamage(int amount)
    {
        CinemachineImpulseSource impulseListener = GetComponent<CinemachineImpulseSource>();
        impulseListener.GenerateImpulse();

        GetComponent<Rumbler>().RumblePulse(1, 5, 0.5f, 0.5f);

        timeStop.StopTime(changeTime, restoreSpeed, delay);
        damageParticle.Play();

        if (!hasInfiniteLives)
        {
            playerData.playerHealth.Value -= amount;
            PlayerHitEvent.Raise(amount);
        }

        CheckDeath();

        if (flashRoutine != null)
        {
            StopCoroutine(flashRoutine);
        }

        flashRoutine = StartCoroutine(flashEffect.FlashMultiple(Color.white, damageIFrameTime));

        iFrameRoutine = StartCoroutine(DamageIFrames(damageIFrameTime));

        invinsible = true;
        FMODUnity.RuntimeManager.PlayOneShot("event:/SFX/Player/Player Damage", GetComponent<Transform>().position);
    }

    void Look()
    {
        if((Mathf.Abs(playerMovement.Velocity.x) > lookVelocityTreshhold.x && Mathf.Abs(playerMovement.Velocity.y) > lookVelocityTreshhold.y))
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

    public void LookTowards(Vector2 cameraOffsetDir, float amount)
    {
        cameraOffset.m_Offset = Vector2.Lerp(cameraOffset.m_Offset, cameraOffsetDir * amount, 0.1f);
    }

    bool knockBackOnHit = false;
    public void KnockbackOnHit(int amount, float dirX, float dirY)
    {
        if (!playerMovement.isAirborne && kockbackDownBrooche.isEquipped)
        {
            return;
        }

        playerMovement.dirKnockback = new Vector3(dirX, dirY, 0);

        if (knockbackOnHitRoutine != null)
        {
            StopCoroutine(knockbackOnHitRoutine);
        }
        knockbackOnHitRoutine = StartCoroutine(KnockbackOnHitRoutine());
    }

    public void KnockbackOnDamage(int amount, float dirX, float dirY)
    {
        if (invinsible)
            return;
        
        dirY = Mathf.Clamp(dirY, -0.1f, 1);
        playerMovement.dirKnockback = new Vector3(dirX, dirY, 1);
        playerMovement.knockbackDistance = amount;
        if (knockbackOnDamageRoutine != null)
        {
            StopCoroutine(knockbackOnDamageRoutine);
        }
        knockbackOnDamageRoutine = StartCoroutine(KnockbackOnDamageRoutine());
    }

    float knockbackOnDamageTimer_Current;
    Coroutine knockbackOnDamageRoutine;
    IEnumerator KnockbackOnDamageRoutine()
    {
        yield return new WaitForFixedUpdate();

        knockbackOnDamageTimer_Current = 0;
        playerMovement.isKnockedback_Damage = true;
        knockbackParticles.Play();

        while (knockbackOnDamageTimer_Current < knockbackOnDamageTimer)
        {
            knockbackOnDamageTimer_Current += Time.smoothDeltaTime;
            yield return new WaitForFixedUpdate();
        }

        knockbackOnDamageTimer_Current = 0;
        playerMovement.isKnockedback_Damage = false;
    }

    float knockbackOnHitTimer_Current;
    Coroutine knockbackOnHitRoutine;

    IEnumerator KnockbackOnHitRoutine()
    {
        yield return new WaitForFixedUpdate();

        knockbackOnHitTimer_Current = 0;
        playerMovement.isKnockedback_Hit = true;

        while(knockbackOnHitTimer_Current < knockbackOnHitTimer)
        {
            knockbackOnHitTimer_Current += Time.smoothDeltaTime;
            yield return new WaitForFixedUpdate();
        }

        knockbackOnHitTimer_Current = 0;
        //yield return new WaitForSeconds(knockbackOnHitTimer);
        playerMovement.isKnockedback_Hit = false;
    }


    private void PlayerInput_OnDebug()
    {
        playerDebugMode = true;

        playerData.lastCheckpointPos.X = transform.position.x;
        playerData.lastCheckpointPos.Y = transform.position.y;

        playerData.lastCheckpointLevelPath = SceneManager.GetActiveScene().path;
        playerData.lastCheckpointLevelIndex.Value = SceneManager.GetActiveScene().buildIndex;

        playerData.lastSavepointPos.X = transform.position.x;
        playerData.lastSavepointPos.Y = transform.position.y;

        playerData.lastSavepointLevelPath = SceneManager.GetActiveScene().path;
        playerData.lastSavepointLevelIndex.Value = SceneManager.GetActiveScene().buildIndex;
    }

    public void OnBroocheEquiped()
    {
        if(coinDropRateUPBrooche.isEquipped)
        {
            PlayerRuntimeDataSO.entityCoinDropModidier = 1.5f;
        }

        if (stunTimeUpBrooche.isEquipped)
        {
            PlayerRuntimeDataSO.entityStunTimeModifier = 1.5f;
        }
    }

    public void OnBroocheUnequip()
    {
        if (!coinDropRateUPBrooche.isEquipped)
        {
            PlayerRuntimeDataSO.entityCoinDropModidier = 1f;
        }

        if (!stunTimeUpBrooche.isEquipped)
        {
            PlayerRuntimeDataSO.entityStunTimeModifier = 1.5f;
        }
    }

    private void OnParticleCollision(GameObject other)
    {
        Debug.Log("Particle collision");
    }

    private void OnDestroy()
    {
        playerBreathingInstance.getPlaybackState(out PLAYBACK_STATE);

        if (PLAYBACK_STATE == FMOD.Studio.PLAYBACK_STATE.PLAYING)
        {
            playerBreathingInstance.release();
            playerBreathingInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        }
    }

    public void ProcessStunDamage(int amount, float stunDamageMod = 1)
    {
        //throw new NotImplementedException();
    }

    
}
