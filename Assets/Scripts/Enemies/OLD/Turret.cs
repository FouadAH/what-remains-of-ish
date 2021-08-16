using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret :  MonoBehaviour, FiringAI
{
    public bool targeting;

    public bool isDamagable = true;
    public bool isStatic = false;

    public float rotateSpeed;
    [HideInInspector] public Transform target;

    public float fireRate = 1f;
    public float nextFireTime;

    float FiringAI.fireRate { get => fireRate; set => value = fireRate; }
    float FiringAI.nextFireTime { get => nextFireTime; set => value = nextFireTime; }

    public event Action OnFire = delegate { };

    void Start()
    {
        target = GameManager.instance.player.transform;
    }

    void Update()
    {
        if (isStatic)
        {
            if (CanFire())
            {
                nextFireTime = Time.time + fireRate;
                RaiseOnFireEvent();
            }
            return;
        }

        if (targeting)
        {
            Rotation();
        }
        else
        {
            RotationNoTarget();
        }
        if (CanFire())
        {
            nextFireTime = Time.time + fireRate;
            RaiseOnFireEvent();
        }

    }

    public void RaiseOnFireEvent()
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


}
