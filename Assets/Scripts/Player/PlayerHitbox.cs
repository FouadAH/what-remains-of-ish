using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Player hitbox class, initializes an attack processor on start.  
/// </summary>
public class PlayerHitbox : MonoBehaviour
{
    Player player;
    private AttackProcessor attackProcessor;

    private void Start()
    {
        attackProcessor = new AttackProcessor();
        player = GetComponentInParent<Player>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<IDamagable>() != null)
        {
            //attackProcessor.ProcessMelee(player, collision.GetComponent<IDamagable>());
            //collision.GetComponent<BaseEnemy>().TakeDamage(Random.Range(player.minDamage, player.maxDamage + 1));
            //player.Knockback(player.transform.localScale*-1, player.swordKnockback);
        }
    }
}
