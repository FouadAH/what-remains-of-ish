using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "newFlyStatData", menuName = "Data/State Data/Fly State")]
public class D_FlyState : ScriptableObject
{
    public float flySpeed = 3f;
    public float radius = 5f;
    public float updateRate = 0.3f;
}