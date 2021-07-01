using System;
using System.Collections;
using UnityEngine;

public class BoomerangLauncher : MonoBehaviour, ILauncher
{
    private GameManager gm;

    public GameObject boomerangPrefab;
    [SerializeField] private Transform firingPoint;
    Player_Input playerInput;
    public bool canFire = true;

    public LayerMask hittable;
    public GameObject hitEffect;
    public float rotateSpeed = 2000f;
    public float MoveSpeed = 1f;
    public float distance = 0.5f;

    public float boomerangHoverTime = 1f;

    public LayerMask damagable;

    [SerializeField] private int minDamage = 1;
    [SerializeField] private int maxDamage = 5;
    [SerializeField] private float damageMod = 1;

    private AttackProcessor attackProcessor;

    public int MinRangeDamage { get => minDamage; set => minDamage = value; }
    public int MaxRangeDamage { get => maxDamage; set => maxDamage = value; }
    public float RangedAttackMod { get => damageMod; set => damageMod = value; }

    public Boomerang boomerangReference;

    float fixedDeltaTime;

    public SpriteRenderer crosshair;

    private void Awake()
    {
        attackProcessor = new AttackProcessor();
        fixedDeltaTime = Time.fixedDeltaTime;
    }

    private void Start()
    {
        gm = GameManager.instance;
        playerInput = GameManager.instance.player.GetComponent<Player_Input>();
        gm.player.GetComponent<Player_Input>().OnFire += Launch;
    }

    private void Update()
    {
        Aim();

        if (playerInput.PS4_Controller == 1 || playerInput.Xbox_One_Controller == 1)
        {
            if (Input.GetAxisRaw("RHorizontal") == 0 && Input.GetAxisRaw("RVertical") == 0)
            {
                if (Input.GetAxisRaw("Horizontal") == 0 && Input.GetAxisRaw("Vertical") == 0)
                {
                    transform.rotation = Quaternion.Euler(0, 0, -90 * gm.player.transform.localScale.x);
                }
                else
                {
                    transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(Input.GetAxisRaw("Horizontal") * -1, Input.GetAxisRaw("Vertical")) * Mathf.Rad2Deg);
                }
            }
            else
            {
                transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(Input.GetAxisRaw("RHorizontal") * -1, Input.GetAxisRaw("RVertical")) * Mathf.Rad2Deg);
            }
        }
        else
        {
            var pos = Camera.main.WorldToScreenPoint(transform.position);
            var dir = Input.mousePosition - pos;
            var angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle - 90, Vector3.forward);
        }
    }

    bool isAiming;
    bool hasFired = false;

    public void Aim()
    {
        if(canFire && !isAiming && Input.GetButtonDown("Aim"))
        {
            playerInput.aiming = true;
            StartCoroutine(AimTimer());
            Time.timeScale = 0.5f;
            crosshair.enabled = true;
        }
        else if(canFire && Input.GetButtonUp("Aim"))
        {
            StopCoroutine(AimTimer());
            isAiming = false;
            playerInput.aiming = false;

            Launch();
            crosshair.enabled = false;
        }

        if (!isAiming)
        {
            Time.timeScale = 1f;
        }

        Time.fixedDeltaTime = fixedDeltaTime * Time.timeScale;
    }

    IEnumerator AimTimer()
    {
        isAiming = true;
        yield return new WaitForSecondsRealtime(0.5f);

        if (canFire)
        {
            Launch();
            hasFired = true;
            crosshair.enabled = false;
        }
        isAiming = false;
        playerInput.aiming = false;
    }

    public void Launch()
    {
        if (canFire)
        {
            hasFired = true;
            canFire = false;
            var Boomerang = Instantiate(boomerangPrefab, firingPoint.position, firingPoint.rotation);
            boomerangReference = Boomerang.GetComponent<Boomerang>();
            Boomerang.GetComponent<Boomerang>().OnRangedHit += RangedHit;
        }
    }

    public void RangedHit(Collider2D collider)
    {
        if (IsInLayerMask(collider.gameObject.layer, damagable) && collider.GetComponent<IDamagable>() != null)
        {
            attackProcessor.ProcessRanged(this, collider.GetComponent<IDamagable>());
        }
    }

    public static bool IsInLayerMask(int layer, LayerMask layermask)
    {
        return layermask == (layermask | (1 << layer));
    }

}
