using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : MonoBehaviour, IHitboxResponder
{
    public List<Hitbox> hitboxes;
    private Vector3 dir;

    public void AttackDefault()
    {
        dir = new Vector3(-transform.localScale.x, 0);
        CheckHitboxes();
    }

    public void AttackUp()
    {
        dir = new Vector3(0, -1);
        CheckHitboxes();
    }
    public void AttackDown()
    {
        dir = new Vector3(0, 1);
        CheckHitboxes();
    }

    private void CheckHitboxes()
    {
        foreach (var hitbox in hitboxes)
        {
            hitbox.useResponder(this);
            hitbox.startCheckingCollision();
        }
    }

    void IHitboxResponder.collisionedWith(Collider2D collider)
    {
        Hurtbox hurtbox = collider.GetComponent<Hurtbox>();
        hurtbox?.getHitBy(gameObject.GetComponent<IBaseStats>(), Mathf.RoundToInt(dir.x), Mathf.RoundToInt(dir.y));
    }
}
