using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityPickup : MonoBehaviour
{
    public bool isDashPickup = false;
    public bool isTeleportPickup = false;
    public bool isWallJumpPickup = false;


    private void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerMovement playerMovement = collision.GetComponent<PlayerMovement>();
        if (playerMovement != null)
        {
            if (isDashPickup)
            {
                GameManager.instance.hasDashAbility = true;
            }
            else if (isTeleportPickup)
            {
                GameManager.instance.hasTeleportAbility = true;
            }
            else if (isWallJumpPickup)
            {
                GameManager.instance.hasWallJump = true;
            }

            Destroy(gameObject);
        }
    }
}
