using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NeedleLauncher : MonoBehaviour, ILauncher
{
    [SerializeField] private Rigidbody2D bulletPrefab;
    [SerializeField] private Transform firingPoint;
    [SerializeField] private float firingForce;
    public LayerMask damagable;

    [SerializeField] private int minDamage = 1;
    [SerializeField] private int maxDamage = 5;
    [SerializeField] private float damageMod = 1;
    [SerializeField] private float hitKnockbackAmount = 15;

    private AttackProcessor attackProcessor;
    
    public int MinRangeDamage { get => minDamage; set => minDamage = value; }
    public int MaxRangeDamage { get => maxDamage; set => maxDamage = value; }
    public float RangedAttackMod { get => damageMod; set => damageMod = value; }
    public float HitKnockbackAmount { get => hitKnockbackAmount; set => hitKnockbackAmount = value; }

    private void Awake()
    {
        attackProcessor = new AttackProcessor();
    }

    public void Launch( )
    {
        if(firingPoint != null)
        {
            var bullet = Instantiate(bulletPrefab, firingPoint.position, firingPoint.rotation);
            bullet.GetComponent<Needle>().OnRangedHit += RangedHit;
            bullet.velocity = transform.up * firingForce;
        }
    }

    public void RangedHit(Collider2D collider, Vector2 pos)
    {
        if (IsInLayerMask(collider.gameObject.layer, damagable) && collider.GetComponent<IDamagable>() != null)
        {
            Vector2 direction = (pos - (Vector2)collider.transform.position).normalized;
            attackProcessor.ProcessRanged(this, collider.GetComponent<IDamagable>(), 1 * Mathf.Sign(direction.x), 1 * Mathf.Sign(direction.y));
        }
    }

    public static bool IsInLayerMask(int layer, LayerMask layermask)
    {
        return layermask == (layermask | (1 << layer));
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawRay(transform.position, transform.up*10);
    }
}
