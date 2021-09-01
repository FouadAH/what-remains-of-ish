using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;

using UnityEngine.Rendering.Universal;

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

    public float boomerangHoverTime = 1f;
    public float aimSnapTime = 0.02f;
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

    CinemachineCameraOffset cameraOffset;
    private Volume volume;
    private ChromaticAberration chromaticAberration;

    bool slowDown;
    bool isAiming;
    public float slowDownTime = 0.6f;
    public float cameraZoom = 15f;
    Coroutine aimTimer;

    float currentAngleVelocity;
    float digitalAngle = 0;
    float inputDeadZone = 0.19f;

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
        cameraOffset = GameManager.instance.cameraController.virtualCamera.GetComponent<CinemachineCameraOffset>();

        volume = FindObjectOfType<Volume>();
        volume.profile.TryGet(out chromaticAberration);
    }

    private void Update()
    {
        if (GameManager.instance.isPaused)
            return;

        FiringLogic();
        BoomerangAimingEffects();
        AimingLogic();
    }

    void AimingLogic()
    {
        if (playerInput.controllerConnected)
        {
            Vector2 leftStickInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
            if (leftStickInput.magnitude < inputDeadZone)
            {
                leftStickInput = Vector2.zero;
            }
            else
            {
                leftStickInput = leftStickInput.normalized * ((leftStickInput.magnitude - inputDeadZone) / (1 - inputDeadZone));
                float analogAngle = Mathf.Atan2(leftStickInput.x * -1, leftStickInput.y) * Mathf.Rad2Deg;
                digitalAngle = Mathf.SmoothDampAngle(digitalAngle, analogAngle, ref currentAngleVelocity, aimSnapTime);
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
    void BoomerangAimingEffects()
    {
        if (slowDown && !timeStop.timeStopIsActive)
        {
            Time.timeScale = 0.3f;
            cameraOffset.m_Offset.z = Mathf.Lerp(cameraOffset.m_Offset.z, cameraZoom, 0.05f);
            chromaticAberration.intensity.value = Mathf.Lerp(chromaticAberration.intensity.value, 0.6f, 0.05f);
        }
        else
        {
            Time.timeScale = 1f;
            if (cameraOffset.m_Offset.z != 0)
            {
                cameraOffset.m_Offset.z = Mathf.Lerp(cameraOffset.m_Offset.z, 0, 0.1f);
            }
            if (chromaticAberration.intensity.value != 0)
            {
                chromaticAberration.intensity.value = Mathf.Lerp(chromaticAberration.intensity.value, 0, 0.1f);
            }
        }

        Time.fixedDeltaTime = fixedDeltaTime * Time.timeScale;
    }
    void FiringLogic()
    {
        if (canFire && Input.GetButtonDown("Aim"))
        {
            playerInput.aiming = true;
            aimTimer = StartCoroutine(AimTimer());
            crosshair.enabled = true;
        }
        else if (isAiming && canFire && Input.GetButtonUp("Aim"))
        {
            if (aimTimer != null)
                StopCoroutine(aimTimer);

            Launch();
            slowDown = false;
            playerInput.aiming = false;
        }
    }

    IEnumerator AimTimer()
    {
        slowDown = true;
        isAiming = true;
        yield return new WaitForSecondsRealtime(slowDownTime);
        slowDown = false;

        playerInput.aiming = false;

        yield return null;
    }

    public void Launch()
    {
        crosshair.enabled = false;
        isAiming = false;

        if (canFire)
        {
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
            Vector2 direction = (pos - (Vector2)collider.transform.position).normalized;
            attackProcessor.ProcessRanged(this, collider.GetComponent<IDamagable>(), Mathf.RoundToInt(direction.x), Mathf.RoundToInt(direction.y));
        }
    }

    public static bool IsInLayerMask(int layer, LayerMask layermask)
    {
        return layermask == (layermask | (1 << layer));
    }

}
