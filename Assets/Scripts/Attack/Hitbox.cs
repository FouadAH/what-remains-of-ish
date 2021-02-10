using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hitbox : MonoBehaviour
{
    public LayerMask mask;
    public bool useSphere = false;
    public Vector3 hitboxSize = Vector3.one;
    public float radius = 0.5f;
    public Color inactiveColor;
    public Color collisionOpenColor;
    public Color collidingColor;
    private IHitboxResponder _responder = null;

    private ColliderState _state;

    private void Update()
    {
        hitboxUpdate();
    }

    public void hitboxUpdate()
    {
        if (_state == ColliderState.Closed) { return; }
        Collider2D[] colliders = Physics2D.OverlapBoxAll(transform.position, hitboxSize, 0, mask);
        for (int i = 0; i < colliders.Length; i++)
        {
            Collider2D aCollider = colliders[i];
            _responder?.collisionedWith(aCollider);
        }
        _state = colliders.Length > 0 ? ColliderState.Colliding : ColliderState.Open;
        stopCheckingCollision();

    }

    public void useResponder(IHitboxResponder responder)
    {
        _responder = responder;
    }

    public void startCheckingCollision()
    {
        _state = ColliderState.Open;
    }

    public void stopCheckingCollision()
    {
        _state = ColliderState.Closed;
    }

    private void OnDrawGizmos()
    {
        checkGizmoColor();
        Gizmos.color = inactiveColor;
        Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, transform.localScale);
        Gizmos.DrawCube(Vector3.zero, new Vector3(hitboxSize.x, hitboxSize.y, 0)); // Because size is halfExtents
    }

    private void checkGizmoColor()
    {
        switch (_state)
        {
            case ColliderState.Closed:
                Gizmos.color = inactiveColor;
                break;
            case ColliderState.Open:
                Gizmos.color = collisionOpenColor;
                break;
            case ColliderState.Colliding:
                Gizmos.color = collidingColor;
                break;
        }
    }
}
