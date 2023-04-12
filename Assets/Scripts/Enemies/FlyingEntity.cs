using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingEntity : MonoBehaviour
{
    AIDestinationSetter destinationSetter;
    AIPath aIPath;

    [Header("Fly Settings")]
    public float flySpeed = 3f;
    public float radius = 10f;
    public float updateRate = 1;

    public GameObject TargetPoint;

    Vector2 startPosition;
    Vector2 Ntarget;

    int facingDirection;

    public void Start()
    {
        startPosition = transform.position;
        destinationSetter = GetComponent<AIDestinationSetter>();
        aIPath = GetComponent<AIPath>();
        aIPath.maxSpeed = flySpeed;

        StartCoroutine(UpdatePath());
    }


    IEnumerator UpdatePath()
    {
        while (true)
        {
            yield return new WaitForSeconds(1f / updateRate);
            UpdateTarget();
        }
    }

    private void UpdateTarget()
    {
        destinationSetter.target = TargetPoint.transform;
        Ntarget = (UnityEngine.Random.insideUnitCircle * radius) + startPosition;
        TargetPoint.transform.position = new Vector3(Ntarget.x, Ntarget.y);

        int directionX = (transform.position.x < TargetPoint.transform.position.x) ? 1 : -1;
        if (facingDirection != directionX)
        {
            Flip();
        }
    }

    public virtual void Flip()
    {
        facingDirection *= -1;
        transform.Rotate(0f, 180f, 0f);
    }
}
