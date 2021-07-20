using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Controller_2D))]

public class Player : MonoBehaviour, IBaseStats{

    Controller_2D controller;

    [HideInInspector]
    public Vector3 velocity;
   
    public LayerMask enemyMask;

    float iFrames = 0f;
    float iFrameTime = 0.5f;
    bool invinsible = false;
    
    Animator anim;

    [SerializeField] private float aggroRange;

    [SerializeField] private GameManager gm;
    
    public PlayerAnimations playerAnimations;
    [SerializeField] private PlayerMovementSettings playerSettings;

    public PlayerMovement playerMovement { get; private set; }

    private bool staggered;

    [Header("Player Values")]

    [SerializeField] private int minMeleeDamage;
    [SerializeField] private int maxMeleeDamage;
    [SerializeField] private float meleeAttackMod;

    [SerializeField] private float rangedAttackMod;
    [SerializeField] private int baseRangeDamage;

    [SerializeField] private int maxHealth;

    [SerializeField] private int hitKnockbackAmount;
    [SerializeField] private int damageKnockbackAmount;


    public float knockbackOnDamageTimer = 0.4f;
    public float knockbackOnHitTimer = 0.25f;



    public int MeleeDamage { get => minMeleeDamage; set => minMeleeDamage = value; }
    public int MaxMeleeDamage { get => maxMeleeDamage; set => maxMeleeDamage = value; }
    public float MeleeAttackMod { get => meleeAttackMod; set => meleeAttackMod = value; }

    public float RangedAttackMod { get => rangedAttackMod; set => rangedAttackMod = value; }
    public int BaseRangeDamage { get => baseRangeDamage; set => baseRangeDamage = value; }

    public int MaxHealth { get => maxHealth; set => maxHealth = value; }
    public float Health { get; set; }

    public int HitKnockbackAmount { get => hitKnockbackAmount; set => hitKnockbackAmount = value; }
    public int knockbackGiven { get => damageKnockbackAmount; set => damageKnockbackAmount= value; }

    public event Action<int> OnHit = delegate { };

    Camera mainCamera;
    Vector2 upperLeft;
    Vector2 lowerRight;

    public BoomerangLauncher boomerangLauncher;

    [Header("Effects")]
    public ParticleSystem dustParticles;

    TimeStop timeStop;

    void Awake()
    {
        GameManager.instance.player = gameObject;

        GameManager.instance.playerCamera = Camera.main;
        mainCamera = GameManager.instance.playerCamera;

        GameManager.instance.cameraController = Camera.main.GetComponent<CameraController>();
        GameManager.instance.cameraController.virtualCamera.Follow = transform;

        boomerangLauncher = GetComponentInChildren<BoomerangLauncher>();
    }

    void Start()
    {
        UI_HUD.instance.enabled = true;

        controller = GetComponent<Controller_2D>();
        gm = FindObjectOfType<GameManager>();
        anim = GetComponent<Animator>();
        timeStop = GetComponent<TimeStop>();
        playerAnimations = new PlayerAnimations(GetComponent<Animator>(), transform);
        playerMovement =  GetComponent<PlayerMovement>();
    }

    private void Update()
    {
        if (GameManager.instance.loading)
            return;

        OnDamage();
        Aggro();
        playerAnimations.Animate();

    }
    
    /// <summary>
    /// Method for handling enemy aggro
    /// </summary>
    public void Aggro()
    {
        Vector2 upperLeftScreen = new Vector2(0, Screen.height);
        Vector2 lowerRightScreen = new Vector2(Screen.width, 0);

        upperLeft = mainCamera.ScreenToWorldPoint(upperLeftScreen);
        lowerRight = mainCamera.ScreenToWorldPoint(lowerRightScreen);

        Collider2D[] enemiesToAggro = Physics2D.OverlapAreaAll(upperLeft, lowerRight, enemyMask);
        for (int i = 0; i < enemiesToAggro.Length; i++)
        {   
            bool hit = Physics2D.Linecast(transform.position, enemiesToAggro[i].transform.position, controller.collitionMask);
            //if(enemiesToAggro[i].GetComponent<IEnemy>() != null)
            //{
            //    if (enemiesToAggro[i].GetComponent<IEnemy>().CanSeePlayer() && !hit && !enemiesToAggro[i].GetComponent<IEnemy>().IsAggro)
            //    {
            //        enemiesToAggro[i].GetComponent<IEnemy>().Aggro();
            //    }
            //}
            if (enemiesToAggro[i].GetComponent<Entity>() != null)
            {
                if (enemiesToAggro[i].GetComponent<Entity>().CheckPlayerInMinAgroRange() && !hit && !enemiesToAggro[i].GetComponent<Entity>().IsAggro)
                {
                    enemiesToAggro[i].GetComponent<Entity>().Aggro();
                }
            }
        }
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
                anim.SetBool("invinsible", false);
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
            StartCoroutine(PlayerDeath());
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
        GetComponent<Player_Input>().enabled = false;
        
        anim.SetLayerWeight(0, 0f);
        anim.SetLayerWeight(1, 0f);
        anim.SetLayerWeight(2, 0f);
        anim.SetLayerWeight(3, 1f);
        anim.SetBool("isDead", true);

        yield return new WaitForSeconds(2f);
        GameManager.instance.Respawn();

        anim.SetBool("isDead", false);
        anim.SetLayerWeight(0, 1f);
        anim.SetLayerWeight(1, 1f);
        anim.SetLayerWeight(2, 1f);
        anim.SetLayerWeight(3, 0f);

        gm.health = gm.maxHealth;
        UI_HUD.instance.RefrechHealth();

        GetComponent<Player_Input>().enabled = true;
        playerMovement.isDead = false;
    }
    
    /// <summary>
    /// Method that respawns the player at the last checkpoint
    /// </summary>
    public void Respawn()
    {
        GameManager.instance.Respawn();
    }

    [Header("Time Stop")]
    public float changeTime = 0.05f;
    public float restoreSpeed = 10f;
    public float delay = 0.1f;

    /// <summary>
    /// Method reponsible for damaging the player
    /// </summary>
    /// <param name="amount"></param>
    public void ModifyHealth(int amount)
    {
        if (!invinsible && gm.health > 0 && !gm.isRespawning)
        {
            timeStop.StopTime(changeTime, restoreSpeed, delay);

            iFrames = iFrameTime;
            anim.SetTrigger("Hit");
            OnHit(amount);
            anim.SetBool("invinsible", true);
            invinsible = true;
            gm.health -= amount;
            CheckDeath();
        }
    }

    IEnumerator DisableInputTemp(float disableTime)
    {
        GetComponent<Player_Input>().enabled = false;
        yield return new WaitForSecondsRealtime(disableTime);
        GetComponent<Player_Input>().enabled = true;
    }

    public void KnockbackOnHit(int amount, float dirX, float dirY)
    {
        playerMovement.dirKnockback = new Vector3(dirX, dirY, 1);
        playerMovement.kockbackDistance = amount;

        StopCoroutine(KnockbackOnHitRoutine());
        StartCoroutine(KnockbackOnHitRoutine());
        //playerMovement.Knockback( new Vector3(dirX, dirY), amount);
    }

    public void KnockbackOnDamage(int amount, float dirX, float dirY)
    {
        playerMovement.dirKnockback = new Vector3(dirX, dirY, 1);
        playerMovement.kockbackDistance = amount;

        StopCoroutine(KnockbackRoutine());
        StartCoroutine(KnockbackRoutine());

        //playerMovement.Knockback(new Vector3(dirX, dirY), amount);
    }

    IEnumerator KnockbackRoutine()
    {
        //Debug.Log("KnockbackRoutine");
        playerMovement.isKnockedback = true;
        yield return new WaitForSecondsRealtime(knockbackOnDamageTimer);
        playerMovement.isKnockedback = false;
    }
    IEnumerator KnockbackOnHitRoutine()
    {
        //Debug.Log("KnockbackRoutine");
        playerMovement.isKnockedback = true;
        yield return new WaitForSecondsRealtime(knockbackOnHitTimer);
        playerMovement.isKnockedback = false;
    }
}
