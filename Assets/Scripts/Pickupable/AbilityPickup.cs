using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityPickup : MonoBehaviour
{
    public PlayerAbilities playerAbility;
    public PlayerDataSO playerData;
    public StringEvent debugText;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent<PlayerMovement>(out _))
        {
            switch (playerAbility)
            {
                case PlayerAbilities.Boomerang:
                    playerData.hasBoomerangAbility = true;
                    debugText.Raise("Unlocked Boomerang Ability");
                    break;

                case PlayerAbilities.Dash:
                    playerData.hasDashAbility = true;
                    debugText.Raise("Unlocked Air Dash Ability");
                    break;

                case PlayerAbilities.WallJump:
                    playerData.hasWallJumpAbility = true;
                    debugText.Raise("Unlocked Wall Jump Ability");
                    break;

                case PlayerAbilities.Teleport:
                    playerData.hasTeleportAbility = true;
                    debugText.Raise("Unlocked Teleport Ability");
                    break;

                default:
                    break;
            }

            Destroy(gameObject);
        }
    }
}

public enum PlayerAbilities
{
    Boomerang = 0,
    Dash = 1,
    WallJump = 2,
    Teleport = 3,
}
