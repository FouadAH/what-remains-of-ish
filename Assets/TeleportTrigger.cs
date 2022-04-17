using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportTrigger : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.GetComponent<Boomerang>()) 
        {
            GameObject player = GameManager.instance.player;
            player.GetComponent<PlayerTeleport>().TeleportAbility(player.transform);
            player.GetComponent<PlayerDash>().ResetDash();
        }
        
    }
}
