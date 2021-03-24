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

    private void Awake()
    {
        attackProcessor = new AttackProcessor();
    }

    private void Start()
    {
        gm = GameManager.instance;
        playerInput = GameManager.instance.player.GetComponent<Player_Input>();
        gm.player.GetComponent<Player_Input>().OnFire += Launch;
    }

    private void Update()
    {
        if (playerInput.PS4_Controller == 1 || playerInput.Xbox_One_Controller == 1)
        {
            transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(Input.GetAxisRaw("Horizontal") * -1, Input.GetAxisRaw("Vertical")) * Mathf.Rad2Deg);
        }
        else
        {
            var pos = Camera.main.WorldToScreenPoint(transform.position);
            var dir = Input.mousePosition - pos;
            var angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle - 90, Vector3.forward);
        }
    }

    public void Launch()
    {
        if (canFire)
        {
            canFire = false;
            var Boomerang = Instantiate(boomerangPrefab, firingPoint.position, firingPoint.rotation);
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
