using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface FiringAI 
{
    float fireRate { get; set; }
    float nextFireTime { get; set; }
    public event Action OnFire;
    public void RaiseOnFireEvent();
    public bool CanFire();
}
