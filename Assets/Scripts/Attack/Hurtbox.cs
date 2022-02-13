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
    IHittable hittable;
    public bool ignoreHitstop;

    private void Awake()
    {
        attackProcessor = new AttackProcessor();
        hittable = GetComponent<IHittable>();
    }

    public void getHitBy(IAttacker attacker, float knockbackDirX, float knockbackDirY)
    {
        if (!stunned)
        {
            StartCoroutine(StunTimer());
            attackProcessor.ProcessMelee(attacker, hittable, knockbackDirX, knockbackDirY);
        }
    }

    public void collisionDamage(int damageAmount, float knockbackDirX, float knockbackDirY)
    {
        if (!stunned)
        {
            StartCoroutine(StunTimer());
            attackProcessor.ProcessCollisionDamage(damageAmount, hittable as IDamagable, knockbackDirX, knockbackDirY);
        }
    }

    int currentFrames;
    public int ignoreFrames = 2;
    private IEnumerator StunTimer()
    {
        stunned = true;
        currentFrames = 0;
        while(currentFrames <= ignoreFrames)
        {
            currentFrames++;
            yield return null;
        }
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