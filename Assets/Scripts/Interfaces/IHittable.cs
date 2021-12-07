using System.Collections;
using UnityEngine;

public interface IHittable 
{
    public void ProcessHit();
    protected void ProcessHit(int hitAmount);
}
