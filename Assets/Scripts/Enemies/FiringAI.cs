using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class FiringAI : IEnemy
{
    public float rotateSpeed;
    public Transform player;
    public Transform target;
    public float fireRate = 1f;
    public float nextFireTime;

    public event Action OnFire = delegate { };
    
    protected void RaiseOnFireEvent()
    {
        var eh = OnFire;
        if (eh != null)
            OnFire();
    }

    public void Rotation()
    {
        float AngleRad = Mathf.Atan2(target.transform.position.y - transform.position.y, target.transform.position.x - transform.position.x);
        float AngleDeg = (180 / Mathf.PI) * AngleRad;
        Quaternion q = Quaternion.AngleAxis(AngleDeg - 90, Vector3.forward);
        transform.rotation = Quaternion.Slerp(transform.rotation, q, Time.deltaTime * rotateSpeed);
    }

    public void RotationNoTarget()
    {
        transform.Rotate(Vector3.forward * (rotateSpeed * Time.deltaTime));
    }

    public bool CanFire()
    {
        return Time.time >= nextFireTime;
    }

    public override bool CanSeePlayer()
    {
        return Physics2D.Linecast(transform.position, player.transform.position, PlayerMask);
    }
    
}
