using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityPickup : MonoBehaviour
{
    public bool isBoomerang = false;
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
                GameManager.instance.playerData.hasDashAbility = true;
                UI_HUD.instance.SetDebugText("Picked Up Dash Ability");
            }
            else if (isTeleportPickup)
            {
                GameManager.instance.playerData.hasTeleportAbility = true;
                UI_HUD.instance.SetDebugText("Picked Up Teleport Ability");
            }
            else if (isWallJumpPickup)
            {
                GameManager.instance.playerData.hasWallJumpAbility = true;
                UI_HUD.instance.SetDebugText("Picked Up Wall Jump Ability");
            }
            else if (isBoomerang)
            {
                GameManager.instance.playerData.hasBoomerangAbility = true;
                UI_HUD.instance.SetDebugText("Picked Up Boomerang Ability");
            }

            Destroy(gameObject);
        }
    }
}
