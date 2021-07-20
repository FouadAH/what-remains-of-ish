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

    TimeStop timeStop;

    private void Awake()
    {
        attackProcessor = new AttackProcessor();
        fixedDeltaTime = Time.fixedDeltaTime;
    }

    private void Start()
    {
        gm = GameManager.instance;
        playerInput = GetComponentInParent<Player_Input>();
        timeStop = GetComponentInParent<TimeStop>();
    }

    float currentAngleVelocity;
    float digitalAngle = 0;
    float inputDeadZone = 0.19f;
    private void Update()
    {
        Aim();

        if (playerInput.controllerConnected)
        {
            Vector2 leftStickInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
            if (leftStickInput.x == 0 && leftStickInput.y == 0)
            {
                transform.rotation = Quaternion.Euler(0, 0, -90 * gm.player.transform.localScale.x);
            }
            else
            {

                if(leftStickInput.magnitude < inputDeadZone)
                {
                    leftStickInput = Vector2.zero;
                }
                else
                {
                    leftStickInput = leftStickInput.normalized * ((leftStickInput.magnitude - inputDeadZone) / (1 - inputDeadZone));
                }

                float analogAngle = Mathf.Atan2(leftStickInput.x * -1, leftStickInput.y) * Mathf.Rad2Deg;
                digitalAngle = Mathf.SmoothDampAngle(digitalAngle, analogAngle, ref currentAngleVelocity, 0.02f);
                transform.rotation = Quaternion.Euler(0, 0, digitalAngle);
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

    Coroutine aimTimer;

    public void Aim()
    {
        if(canFire && !isAiming && Input.GetButtonDown("Aim"))
        {
            playerInput.aiming = true;

            aimTimer = StartCoroutine(AimTimer());

            Time.timeScale = 0.3f;
            crosshair.enabled = true;
        }
        else if(isAiming && canFire && Input.GetButtonUp("Aim"))
        {
            if (aimTimer != null)
                StopCoroutine(aimTimer);

            Launch();
            isAiming = false;
            playerInput.aiming = false;
        }

        if (!isAiming && !timeStop.timeStopIsActive)
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
            Debug.Log("Launch AimTimer");
        }

        isAiming = false;
        playerInput.aiming = false;

        yield return null;
    }

    public void Launch()
    {
        crosshair.enabled = false;

        if (canFire)
        {
            hasFired = true;
            canFire = false;
            var Boomerang = Instantiate(boomerangPrefab, firingPoint.position, firingPoint.rotation);
            boomerangReference = Boomerang.GetComponent<Boomerang>();
            Boomerang.GetComponent<Boomerang>().OnRangedHit += RangedHit;
        }
    }

    public void RangedHit(Collider2D collider, Vector2 pos)
    {
        if (IsInLayerMask(collider.gameObject.layer, damagable) && collider.GetComponent<IDamagable>() != null)
        {
            //attackProcessor.ProcessRanged(this, collider.GetComponent<IDamagable>());

            Vector2 direction = (pos - (Vector2)collider.transform.position).normalized;
            attackProcessor.ProcessRanged(this, collider.GetComponent<IDamagable>(), Mathf.RoundToInt(direction.x), Mathf.RoundToInt(direction.y));
        }
    }

    public static bool IsInLayerMask(int layer, LayerMask layermask)
    {
        return layermask == (layermask | (1 << layer));
    }

}
