using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Player hitbox class, initializes an attack processor on start.  
/// </summary>
public class PlayerHitbox : MonoBehaviour
{
    Player player;
    private AttackProcessor attackProcessor;

    private void Start()
    {
        attackProcessor = new AttackProcessor();
        player = GetComponentInParent<Player>();
    }
}
