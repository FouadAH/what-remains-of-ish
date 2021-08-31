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
                UI_HUD.instance.SetPickupText("Picked Up Dash Ability");
            }
            else if (isTeleportPickup)
            {
                GameManager.instance.hasTeleportAbility = true;
                UI_HUD.instance.SetPickupText("Picked Up Teleport Ability");
            }
            else if (isWallJumpPickup)
            {
                GameManager.instance.hasWallJump = true;
                UI_HUD.instance.SetPickupText("Picked Up Wall Jump Ability");
            }

            Destroy(gameObject);
        }
    }
}
