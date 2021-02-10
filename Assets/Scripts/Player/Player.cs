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
    public Vector2 swordKnockback;
    public Vector2 damageKnockback;

    float iFrames = 0f;
    float iFrameTime = 0.5f;
    bool invinsible = false;
    
    Animator anim;

    [SerializeField] private float aggroRange;

    [SerializeField] private GameManager gm;
    
    public PlayerAnimations playerAnimations;
    [SerializeField] private PlayerMovementSettings playerSettings;

    public PlayerMovement PlayerMovement { get; private set; }

    private bool staggered;

    [SerializeField] private int minMeleeDamage;
    [SerializeField] private int maxMeleeDamage;
    [SerializeField] private float meleeAttackMod;

    [SerializeField] private float rangedAttackMod;
    [SerializeField] private int baseRangeDamage;

    [SerializeField] private int maxHealth;

    [SerializeField] private int hitKnockbackAmount;
    [SerializeField] private int damageKnockbackAmount;

    public int MinMeleeDamage { get => minMeleeDamage; set => minMeleeDamage = value; }
    public int MaxMeleeDamage { get => maxMeleeDamage; set => maxMeleeDamage = value; }
    public float MeleeAttackMod { get => meleeAttackMod; set => meleeAttackMod = value; }

    public float RangedAttackMod { get => rangedAttackMod; set => rangedAttackMod = value; }
    public int BaseRangeDamage { get => baseRangeDamage; set => baseRangeDamage = value; }

    public int MaxHealth { get => maxHealth; set => maxHealth = value; }
    public float Health { get; set; }

    public int HitKnockbackAmount { get => hitKnockbackAmount; set => hitKnockbackAmount = value; }
    public int knockbackGiven { get => damageKnockbackAmount; set => damageKnockbackAmount= value; }

    public event Action OnHit = delegate { };

    public CameraController cameraController;
    public Camera camera;

    Vector2 upperLeft;
    Vector2 lowerRight;
    
    void Start()
    {
        GameManager.instance.player = gameObject;
        camera = GameManager.instance.camera;
        cameraController = GameManager.instance.cameraController;
        GameManager.instance.cameraController.virtualCamera.Follow = transform;

        transform.position = GameManager.instance.playerPosition;
        controller = GetComponent<Controller_2D>();
        gm = FindObjectOfType<GameManager>();
        anim = GetComponent<Animator>();
        controller = GetComponent<Controller_2D>();
        playerAnimations = new PlayerAnimations(GetComponent<Animator>(), transform);
        PlayerMovement = new PlayerMovement(transform, playerSettings);
    }

    private void FixedUpdate()
    {
        if (GameManager.instance.loading)
            return;
        PlayerMovement.Movement();
    }
    private void Update()
    {
        if (GameManager.instance.loading)
            return;
        OnDamage();
        Aggro();
        playerAnimations.Animate(PlayerMovement);
    }
    
    /// <summary>
    /// Method for handling enemy aggro
    /// </summary>
    public void Aggro()
    {
        Vector2 upperLeftScreen = new Vector2(0, Screen.height);
        Vector2 lowerRightScreen = new Vector2(Screen.width, 0);

        upperLeft = camera.ScreenToWorldPoint(upperLeftScreen);
        lowerRight = camera.ScreenToWorldPoint(lowerRightScreen);

        Collider2D[] enemiesToAggro = Physics2D.OverlapAreaAll(upperLeft, lowerRight, enemyMask);
        for (int i = 0; i < enemiesToAggro.Length; i++)
        {
            bool hit = Physics2D.Linecast(transform.position, enemiesToAggro[i].transform.position, controller.collitionMask);
            if (enemiesToAggro[i].GetComponent<IEnemy>().CanSeePlayer() && !hit && !enemiesToAggro[i].GetComponent<IEnemy>().IsAggro)  
            {
                enemiesToAggro[i].GetComponent<IEnemy>().Aggro();
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
        PlayerMovement.isDead = true;
        GetComponent<Player_Input>().enabled = false;

        anim.SetLayerWeight(0, 0f);
        anim.SetLayerWeight(1, 0f);
        anim.SetLayerWeight(2, 0f);
        anim.SetLayerWeight(3, 1f);
        anim.SetBool("isDead", true);

        yield return new WaitForSeconds(2f);
        Respawn();

        anim.SetBool("isDead", false);
        anim.SetLayerWeight(0, 1f);
        anim.SetLayerWeight(1, 1f);
        anim.SetLayerWeight(2, 1f);
        anim.SetLayerWeight(3, 0f);
        gm.health = gm.maxHealth;
        GetComponent<Player_Input>().enabled = true;
        PlayerMovement.isDead = false;
    }
    
    /// <summary>
    /// Method that respawns the player at the last checkpoint
    /// </summary>
    public void Respawn()
    {
        GameManager.instance.LoadScene(SceneManager.GetActiveScene().buildIndex, gm.lastCheckpointLevelIndex);
        transform.position = gm.lastCheckpointPos;
        GameManager.instance.drone.transform.position = transform.position;
    }

    /// <summary>
    /// Method reponsible for damaging the player
    /// </summary>
    /// <param name="amount"></param>
    public void ModifyHealth(int amount)
    {
        if (!invinsible)
        {
            iFrames = iFrameTime;
            anim.SetTrigger("Hit");
            OnHit();
            anim.SetBool("invinsible", true);
            invinsible = true;
            gm.health -= amount;
            CheckDeath();
        }
    }

    public void KnockbackOnHit(int amount, int dirX, int dirY)
    {
        PlayerMovement.Knockback( new Vector3(dirX, dirY), amount);
    }

    public void KnockbackOnDamage(int amount, int dirX, int dirY)
    {
        PlayerMovement.Knockback(new Vector3(dirX, dirY), amount);
    }
}
