using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hurtbox : MonoBehaviour
{
    public Collider2D colliderObj;
    private AttackProcessor attackProcessor;
    public Color ColorOpen = Color.green;
    public Color ColorOpenWeakspot = Color.red;

    public Color ColorStunned = Color.yellow;
    private ColliderState _state = ColliderState.Open;
    IHittable hittable;
    public bool ignoreHitstop;
    public HurtboxType hurtboxType;

    public enum HurtboxType
    {
        Normal,
        Weakspot
    }

    private void Awake()
    {
        attackProcessor = new AttackProcessor();
        hittable = GetComponent<IHittable>();
        if(hittable == null)
        {
            hittable = GetComponentInParent<IHittable>();
        }
    }

    public void getHitBy(IAttacker attacker, float knockbackDirX, float knockbackDirY)
    {
        attackProcessor.ProcessMelee(attacker, hittable, knockbackDirX, knockbackDirY);
        
    }

    public void GetHitByNoKnockback(IAttacker attacker)
    {
        attackProcessor.ProcessMeleeNoKnockback(attacker, hittable);
    }

    public void getHitByRanged(ILauncher attacker, float knockbackDirX, float knockbackDirY, float stunDamageMod = 1)
    {
        attackProcessor.ProcessPlayerRangedAttack(attacker, hittable, knockbackDirX, knockbackDirY, stunDamageMod);
        
    }

    public void collisionDamage(int damageAmount, float knockbackDirX, float knockbackDirY)
    {
        attackProcessor.ProcessCollisionDamage(damageAmount, hittable, knockbackDirX, knockbackDirY);
    }

    private void OnDrawGizmos()
    {
        //if (colliderObj is BoxCollider2D)
        //{
        //    BoxCollider2D boxCollider2D = (BoxCollider2D)colliderObj;
        //    Rect rect = new Rect(colliderObj.transform.position, boxCollider2D.size);
        //    DrawRect(rect);
        //}

        //Transform parent = transform.parent;
        //Vector3 offset = new Vector3(colliderObj.offset.x, colliderObj.offset.y, 0);
        //Gizmos.matrix = Matrix4x4.TRS(colliderObj.transform.position, colliderObj.transform.rotation, colliderObj.transform.localScale);
        //Gizmos.DrawCube(offset, new Vector3(colliderObj.bounds.size.x, colliderObj.bounds.size.y, colliderObj.bounds.size.z));

        //Vector3 boxPos = colliderObj.transform.position;
        //Vector2 vector = new Vector2(boxPos.x + colliderObj.offset.x, boxPos.y + colliderObj.offset.y);
        //Gizmos.DrawCube(vector, colliderObj.bounds.size);
    }
    void DrawRect(Rect rect)
    {
        Gizmos.DrawWireCube(new Vector3(rect.center.x, rect.center.y, 0.01f), new Vector3(rect.size.x, rect.size.y, 0.01f));
    }
}