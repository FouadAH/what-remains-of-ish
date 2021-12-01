using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hurtbox : MonoBehaviour
{
    public Collider2D colliderObj;
    private AttackProcessor attackProcessor;
    public Color ColorOpen;
    public Color ColorStunned;
    private ColliderState _state = ColliderState.Open;
    private bool stunned;
    [SerializeField] private float stunTime = 0.5f;

    private void Awake()
    {
        attackProcessor = new AttackProcessor();
    }

    public void getHitBy(IAttacker attacker, float knockbackDirX, float knockbackDirY)
    {
        if (!stunned)
        {
            StartCoroutine(StunTimer());
            attackProcessor.ProcessMelee(attacker, GetComponent<IDamagable>(), knockbackDirX, knockbackDirY);
        }
    }

    public void collisionDamage(int damageAmount, float knockbackDirX, float knockbackDirY)
    {
        if (!stunned)
        {
            StartCoroutine(StunTimer());
            attackProcessor.ProcessCollisionDamage(damageAmount, GetComponent<IDamagable>(), knockbackDirX, knockbackDirY);
        }
    }

    private IEnumerator StunTimer()
    {
        stunned = true;
        yield return new WaitForSeconds(stunTime);
        stunned = false;
    }

    private void OnDrawGizmos()
    {
        if (stunned)
        {
            Gizmos.color = ColorStunned;
        }
        else
        {
            Gizmos.color = ColorOpen;

        }

        Transform parent = transform.parent;
        //Vector3 offset = new Vector3(colliderObj.offset.x, colliderObj.offset.y, 0);
        //Gizmos.matrix = Matrix4x4.TRS(colliderObj.transform.position, colliderObj.transform.rotation, colliderObj.transform.localScale);
        //Gizmos.DrawCube(offset, new Vector3(colliderObj.bounds.size.x, colliderObj.bounds.size.y, colliderObj.bounds.size.z));

        //Vector3 boxPos = colliderObj.transform.position;
        //Vector2 vector = new Vector2(boxPos.x + colliderObj.offset.x, boxPos.y + colliderObj.offset.y);
        //Gizmos.DrawCube(vector, colliderObj.bounds.size);
    }

}