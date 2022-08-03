using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;

using UnityEngine.Rendering.Universal;

public class BoomerangLauncher : MonoBehaviour, ILauncher
{
    private GameManager gm;

    public GameObject boomerangPrefab;
    public SpriteRenderer crosshair;
    public SpriteRenderer secondCrosshair;
    public Texture2D cursorTexture;
    public CursorMode cursorMode = CursorMode.Auto;
    public Vector2 hotSpot = Vector2.zero;

    [SerializeField] private Transform firingPoint;
    
    public bool canFire = true;

    [Header("Layer Masks")]
    public LayerMask hittable;
    public LayerMask interactable;
    public LayerMask damagable;
    public LayerMask knockbackable;
    public LayerMask weakSpot;

    [Header("Movement values")]
    public float rotateSpeed = 2000f;
    public float MoveSpeed = 50f;
    public float returnMoveSpeed = 80f;

    public float throwForce = 100f;
    public float bounceForce = 100f;
    [Range(0,2)] public float falloutStrenght = 0.5f;

    public float boomerangHoverTime = 1f;
    public float boomerangAirTime = 0.38f;
    public float accelerationTime = 0.05f;

    public float gravityScale;
    public float angularDrag;
    public float linearDrag;
    public float stopForceImpulseSpeed;

    [Header("Aim assist values")]
    public float aimSnapTime = 0.02f;
    [Range(0,6)] public float aimAssistAmount = 3f;

    [Header("Damage values")]
    [SerializeField] private int minDamage = 1;
    [SerializeField] private int maxDamage = 5;
    [SerializeField] private float damageMod = 1;
    [SerializeField] private float hitKnockbackAmount = 15;

    private AttackProcessor attackProcessor;
    public int MinRangeDamage { get => minDamage; set => minDamage = value; }
    public int MaxRangeDamage { get => maxDamage; set => maxDamage = value; }
    public float RangedAttackMod { get => damageMod; set => damageMod = value; }
    public float HitKnockbackAmount { get => hitKnockbackAmount; set => hitKnockbackAmount = value; }


    [Header("Effects")]
    public GameObject hitEffect;
    public float slowDownTime = 0.6f;
    public float cameraZoom = 15f;
    float fixedDeltaTime;

    CameraController cameraController;
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
    float inputDeadZone = 0.3f;
    float secondaryCrosshairDistance = 16f;

    [Header("Events")]
    [FMODUnity.EventRef] public string boomerangSpinSFX;
    public UnityEvent OnBoomerangLaunched;
    public UnityEvent OnBoomerangHit;

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
        cameraController = Camera.main.GetComponent<CameraController>();
        cameraOffset = cameraController.virtualCamera.GetComponent<CinemachineCameraOffset>();

        volume = FindObjectOfType<Volume>();
        volume.profile.TryGet(out chromaticAberration);

        Cursor.visible = false;
        playerInput.OnQuickThrow += PlayerInput_OnQuickThrow;
    }

    private void PlayerInput_OnQuickThrow()
    {
        Launch();
    }

    private void Update()
    {
        if (!GameManager.instance.playerData.hasBoomerangAbility)
            return;

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
        if (playerInput.controllerConnected)
        {
            if (Cursor.visible)
            {
                Cursor.visible = false;
            }

            if (isAiming)
            {
                crosshair.enabled = true;
                secondCrosshair.enabled = true;
                RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.up, secondaryCrosshairDistance, hittable);

                if (hit)
                {
                    targetPos = hit.point;
                    secondCrosshair.transform.position = Vector2.SmoothDamp(secondCrosshair.transform.position, targetPos, ref referencePos, 0.02f);
                }
                else if (playerInput.controllerConnected)
                {
                    targetPos = new Vector3(0, secondaryCrosshairDistance, 0);
                    secondCrosshair.transform.localPosition = Vector2.SmoothDamp(secondCrosshair.transform.localPosition, targetPos, ref referencePos, 0.02f);
                }
                else
                {
                    targetPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    secondCrosshair.transform.position = Vector2.SmoothDamp(secondCrosshair.transform.position, targetPos, ref referencePos, 0.02f);
                    secondCrosshair.transform.localPosition = new Vector2(secondCrosshair.transform.localPosition.x, Mathf.Clamp(secondCrosshair.transform.localPosition.y, 0, secondaryCrosshairDistance));
                }
            }
        }
        else
        {
            if (!Cursor.visible)
            {
                Cursor.visible = true;
                Cursor.SetCursor(cursorTexture, hotSpot, cursorMode);
            }

            if (isAiming)
            {
                secondCrosshair.enabled = true;
                RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.up, secondaryCrosshairDistance, hittable);

                if (hit)
                {
                    targetPos = hit.point;
                    secondCrosshair.transform.position = Vector2.SmoothDamp(secondCrosshair.transform.position, targetPos, ref referencePos, 0.02f);
                    
                    if(IsInLayerMask( hit.collider.gameObject.layer, weakSpot))
                    {
                        secondCrosshair.color = Color.red;
                    }
                    else
                    {
                        secondCrosshair.color = Color.white;
                    }
                
                }
                else
                {
                    targetPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    secondCrosshair.transform.position = targetPos;
                    secondCrosshair.transform.localPosition = new Vector2(secondCrosshair.transform.localPosition.x, Mathf.Clamp(secondCrosshair.transform.localPosition.y, 0, secondaryCrosshairDistance));
                }
            }
            
            //targetPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            //crosshair.enabled = true;
            //crosshair.transform.position = Vector2.SmoothDamp(crosshair.transform.position, targetPos, ref referencePos, 0.02f); ;
            //crosshair.transform.localPosition = new Vector2(crosshair.transform.localPosition.x, crosshair.transform.localPosition.y);
        }
    }

    void CalculateAngle(Vector2 analoguStickInput)
    {
        analoguStickInput = analoguStickInput.normalized * ((analoguStickInput.magnitude - inputDeadZone) / (1 - inputDeadZone));
        float analogAngle = Mathf.Atan2(analoguStickInput.x * -1, analoguStickInput.y) * Mathf.Rad2Deg;
        digitalAngle = Mathf.SmoothDampAngle(digitalAngle, analogAngle, ref currentAngleVelocity, aimSnapTime);
        transform.rotation = Quaternion.Euler(0, 0, digitalAngle);
    }

    public Transform lookForEnemyWithThickRaycast(Vector2 startWorldPos, Vector2 direction, float visibilityThickness)
    {
        if (visibilityThickness == 0) return null; //aim assist disabled

        int[] castOrder = { 2, 1, 3, 0, 4 };
        int numberOfRays = castOrder.Length;
        const float minDistanceAway = 2.5f; //don't find results closer than this
        const float castDistance = 30f;
        const float flareOutAngle = 2f;

        Transform target = null;
        foreach (int i in castOrder)
        {
            Vector2 perpDirection = Vector2.Perpendicular(direction);
            float perpDistance = -visibilityThickness * 0.5f + i * visibilityThickness / (numberOfRays - 1);
            Vector2 startPos = perpDirection * perpDistance + startWorldPos;

            float angleOffset = -flareOutAngle * 0.5f + i * flareOutAngle / (numberOfRays - 1);
            Vector2 flaredDirection = direction.Rotate(angleOffset);

            RaycastHit2D hit = Physics2D.Raycast(startPos, flaredDirection, castDistance, LayerMask.GetMask("Enemy"));
            Debug.DrawRay(startPos, flaredDirection * castDistance, Color.yellow, Time.deltaTime);

            if (hit)
            {
                //make sure it's in range
                float distanceAwaySqr = ((Vector2)hit.transform.position - startWorldPos).sqrMagnitude;
                Debug.DrawRay(startPos, direction * castDistance, Color.red, Time.deltaTime);
                if (distanceAwaySqr > minDistanceAway * minDistanceAway)
                {
                    Debug.DrawRay(startPos, direction * castDistance, Color.red, Time.deltaTime);
                    target = hit.transform;
                    return target;
                }
            }
        }

        return target;
    }

    void AimingLogic()
    {
        if (playerInput.controllerConnected)
        {
            Vector2 rightStickInput = playerInput.rightStickInputRaw;

            if (rightStickInput.magnitude >= inputDeadZone)
            {
                CalculateAngle(rightStickInput);
            }
        }
        else
        {
            var pos = Camera.main.WorldToScreenPoint(transform.position);
            var dir = Input.mousePosition - pos;
            var angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle - 90, Vector3.forward);
        }

        //lookForEnemyWithThickRaycast(firingPoint.transform.position, (crosshair.transform.position - firingPoint.transform.position).normalized, aimAssistAmount);
    }
    void BoomerangAimingEffects()
    {
        if (timeStop.timeStopIsActive)
            return;

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
        if (canFire && playerInput.aiming && !isAiming)
        {
            //playerInput.aiming = true;
            aimTimer = StartCoroutine(AimTimer());
        }
        else if (isAiming && canFire && !playerInput.aiming)
        {
            if (aimTimer != null)
                StopCoroutine(aimTimer);

            Launch();
            slowDown = false;
            //playerInput.aiming = false;
        }
    }

    IEnumerator AimTimer()
    {
        slowDown = true;
        isAiming = true;
        yield return new WaitForSecondsRealtime(slowDownTime);
        slowDown = false;
        //Debug.Log(slowDown);
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
            boomerangReference.OnRangedHit += RangedHit;
            boomerangReference.Launch();
            //OnBoomerangLaunched.Invoke();

            FMODUnity.RuntimeManager.PlayOneShot(boomerangSpinSFX, transform.position);
        }
    }

    public void RangedHit(Collider2D collider, Vector2 pos)
    {
        Hurtbox hurtbox = collider.GetComponent<Hurtbox>();
        if (IsInLayerMask(collider.gameObject.layer, damagable) && hurtbox != null)
        {
            OnBoomerangHit.Invoke();
            Vector2 direction = (pos - (Vector2)collider.transform.position).normalized;
            if (hurtbox.hurtboxType == Hurtbox.HurtboxType.Weakspot)
            {
                Debug.Log("Hit weak spot");
                hurtbox.getHitByRanged(this, direction.x, direction.y, 5);
            }
            else
            {
                hurtbox.getHitByRanged(this, direction.x, direction.y, 1);
            }
        }
    }

    public static bool IsInLayerMask(int layer, LayerMask layermask)
    {
        return layermask == (layermask | (1 << layer));
    }

}
