using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "newJumpStateData", menuName = "Data/State Data/Jump State")]
public class D_JumpState : ScriptableObject
{
    public float jumpHeight = 20f;
    public float gravityScale = 4f;
}
