using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHitBox : MonoBehaviour
{
    public Vector2 knockback;
    public float damageDealt;
    private AttackProcessor attackProcessor;

    private void Awake()
    {
        attackProcessor = new AttackProcessor();
    }

    private void OnTriggerEnter2D (Collider2D collision)
    {
        if (collision.GetComponent<IDamagable>() != null)
        {
            //attackProcessor.ProcessMelee(GetComponentInParent<IBaseStats>() ,collision.GetComponent<IDamagable>());
            // player.DealDamage(damageDealt, Vector3.Normalize(player.transform.position - transform.position));
        }
    }
}
