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

    public void getHitBy(IBaseStats attacker, int knockbackDirX, int knockbackDirY)
    {
        if (!stunned)
        {
            StartCoroutine(StunTimer());
            attackProcessor.ProcessMelee(attacker, GetComponent<IDamagable>(), knockbackDirX, knockbackDirY);
        }
    }

    private IEnumerator StunTimer()
    {
        stunned = true;
        yield return new WaitForSeconds(stunTime);
        stunned = false;
    }

    //private void OnDrawGizmos()
    //{
    //    if (stunned)
    //    {
    //        Gizmos.color = ColorStunned;
    //    }
    //    else
    //    {
    //        Gizmos.color = ColorOpen;

    //    }
    //    Gizmos.matrix = Matrix4x4.TRS(new Vector3(collider.transform.position.x + collider.offset.x * transform.localScale.x, collider.transform.position.y + collider.offset.y * transform.localScale.y),
    //        collider.transform.rotation, collider.transform.localScale);
    //    Gizmos.DrawSphere(Vector3.zero, 1);
    //    //Gizmos.DrawCube(Vector3.zero, new Vector3(collider., collider.size.y, 0));
    //}
    
}