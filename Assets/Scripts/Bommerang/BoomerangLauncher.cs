using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;

using UnityEngine.Rendering.Universal;

public class BoomerangLauncher : MonoBehaviour, ILauncher
{
    private GameManager gm;

    public GameObject boomerangPrefab;
    public SpriteRenderer crosshair;
    public SpriteRenderer secondCrosshair;

    [SerializeField] private Transform firingPoint;
    
    public bool canFire = true;

    [Header("Layer Masks")]
    public LayerMask hittable;
    public LayerMask interactable;
    public LayerMask damagable;

    [Header("Movement values")]
    public float rotateSpeed = 2000f;
    public float MoveSpeed = 1f;
    public float boomerangHoverTime = 1f;
    public float boomerangAirTime = 0.38f;
    public float boomerangAirTimeBonus = 0f;
    public float accelerationTime = 0.05f;

    [Header("Aim assist values")]
    public float aimSnapTime = 0.02f;

    [Header("Damage values")]
    [SerializeField] private int minDamage = 1;
    [SerializeField] private int maxDamage = 5;
    [SerializeField] private float damageMod = 1;

    private AttackProcessor attackProcessor;
    public int MinRangeDamage { get => minDamage; set => minDamage = value; }
    public int MaxRangeDamage { get => maxDamage; set => maxDamage = value; }
    public float RangedAttackMod { get => damageMod; set => damageMod = value; }

    [Header("Effects")]
    public GameObject hitEffect;
    public float slowDownTime = 0.6f;
    public float cameraZoom = 15f;
    float fixedDeltaTime;

    CinemachineCameraOffset cameraOffset;
    private Volume volume;
    private ChromaticAberration chromaticAberration;

    [HideInInspector] public Boomerang boomerangReference;

    TimeStop timeStop;
    Player_Input playerInput;

    bool slowDown;
    bool isAiming;
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
        if (GameManager.instance.isPaused || DialogManager.instance.dialogueIsActive)
            return;

        FiringLogic();
        BoomerangAimingEffects();
        AimingLogic();
        Crosshair();
    }

    Vector2 referencePos;
    void Crosshair()
    {
        Vector2 targetPos;
        if (isAiming)
        {
            crosshair.enabled = true;
            secondCrosshair.enabled = true;
            RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.up, 18f, hittable);

            if (hit)
            {
                targetPos = hit.point;
                secondCrosshair.transform.position = Vector2.SmoothDamp(secondCrosshair.transform.position, targetPos, ref referencePos, 0.02f);
            }
            else if(playerInput.controllerConnected)
            {
                targetPos = new Vector3(0, 18f, 0);
                secondCrosshair.transform.localPosition = Vector2.SmoothDamp(secondCrosshair.transform.localPosition, targetPos, ref referencePos, 0.02f);
            }
            else
            {
                targetPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                secondCrosshair.transform.position = Vector2.SmoothDamp(secondCrosshair.transform.position, targetPos, ref referencePos, 0.02f);
                secondCrosshair.transform.localPosition = new Vector2(secondCrosshair.transform.localPosition.x, Mathf.Clamp(secondCrosshair.transform.localPosition.y, 0, 18f));
            }
        }
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
        secondCrosshair.enabled = false;
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
