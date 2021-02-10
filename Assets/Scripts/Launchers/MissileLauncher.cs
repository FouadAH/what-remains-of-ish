using UnityEngine;

public class MissileLauncher : MonoBehaviour, ILauncher
{
    [SerializeField] private Rigidbody2D missilePrefab;
    [SerializeField] private Transform firingPoint;
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

    public void Launch(ProjectileLauncher projectileLauncher)
    {
        if (firingPoint != null)
        {
            var Missile = Instantiate(missilePrefab, firingPoint.position, firingPoint.rotation);
            Missile.GetComponent<Missile>().OnRangedHit += RangedHit;
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