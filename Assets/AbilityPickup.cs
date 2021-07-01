using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityPickup : MonoBehaviour
{
    public bool isDashPickup = false;
    public bool isTeleportPickup = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerMovement playerMovement = collision.GetComponent<PlayerMovement>();
        if (playerMovement != null)
        {
            if (isDashPickup)
            {
                playerMovement.hasDashAbility = true;
            }
            else if (isTeleportPickup)
            {
                playerMovement.hasTeleportAbility = true;
            }

            Destroy(gameObject);
        }
    }
}
